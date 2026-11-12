-- Genera la versión de carne (Dimensión 3) de un sprite de la casa.
-- Uso: aseprite -b --script-param modo=<pared|piso|mueble|oscurecer|ojo|corazon|charco|venas> \
--        --script-param entrada=<png> --script-param salida=<png> [--script-param semilla=N ...] --script carne.lua
-- Modos:
--   pared     tira de pared de 48 px: textura continua de una tira a la siguiente (periódica a lo ancho)
--   piso      losa isométrica de 96x48: continua en las dos direcciones de la rejilla
--   mueble    recolorea un mueble como carne, con poros, venas y, opcional, una pantalla con estática
--   oscurecer oscurece un dibujo "agobiante" y le agrega venas y manchas
--   ojo       hoja de 4 cuadros de un ojo incrustado en la pared: abierto, entrecerrado, cerrado y llorando
--   corazon   hoja de 4 cuadros del corazón deformado latiendo
--   charco    charco de sangre isométrico
--   venas     raíces/venas colgantes para las esquinas de las paredes

local P = app.params
local pc = app.pixelColor

local function num(nombre, defecto)
  local v = P[nombre]
  if v == nil or v == "" then return defecto end
  return tonumber(v)
end

local semilla = num("semilla", 1)
math.randomseed(semilla * 7919 + 13)

-- ---------------------------------------------------------------- Paleta
-- Carne oscura, del negro al tono de grasa
local CARNE = {
  {13, 3, 3}, {29, 6, 6}, {46, 10, 11}, {66, 16, 15}, {89, 22, 20}, {115, 30, 27},
  {142, 42, 36}, {168, 58, 47}, {192, 85, 67}, {212, 124, 100}, {226, 165, 142},
}
local VENA_OSCURA = {38, 6, 18}
local VENA_MEDIA = {72, 14, 36}
local VENA_LUZ = {118, 36, 62}
local HUECO = {9, 2, 2}
local ARO = {216, 182, 164}
local ARO_SOMBRA = {150, 104, 92}

local brillo = num("brillo", 1.0)

local BAYER = {
  {0, 8, 2, 10}, {12, 4, 14, 6}, {3, 11, 1, 9}, {15, 7, 13, 5},
}

local function rgba(c, a) return pc.rgba(c[1], c[2], c[3], a or 255) end

-- Color de la paleta para un valor de 0 a 1, con tramado ordenado para que se lea como pixel art
local function carne(t, x, y)
  t = math.max(0, math.min(0.999, t * brillo))
  local pos = t * (#CARNE - 1)
  local i = math.floor(pos)
  local f = pos - i
  local umbral = (BAYER[(y % 4) + 1][(x % 4) + 1] + 0.5) / 16
  if f > umbral then i = i + 1 end
  return CARNE[math.min(#CARNE, i + 1)]
end

-- ---------------------------------------------------------------- Ruido
local function hash(ix, iy, s)
  local h = ix * 374761393 + iy * 668265263 + (s or semilla) * 982451653
  h = (h ~ (h >> 13)) * 1274126177
  h = h ~ (h >> 16)
  return (h & 0xffff) / 65535
end

local function suave(t) return t * t * (3 - 2 * t) end

-- Ruido de valor periódico: se repite cada px celdas en x y cada py en y (0 = sin repetir)
local function ruido(x, y, px, py, s)
  local x0, y0 = math.floor(x), math.floor(y)
  local fx, fy = suave(x - x0), suave(y - y0)
  local function h(ix, iy)
    if px > 0 then ix = ix % px end
    if py > 0 then iy = iy % py end
    return hash(ix, iy, s)
  end
  local a = h(x0, y0) + (h(x0 + 1, y0) - h(x0, y0)) * fx
  local b = h(x0, y0 + 1) + (h(x0 + 1, y0 + 1) - h(x0, y0 + 1)) * fx
  return a + (b - a) * fy
end

-- Varias octavas; u y v en "vueltas" (0..1 se repite si periodico), escala = celdas por vuelta
local function fbm(u, v, escalaU, escalaV, periodicoU, periodicoV, octavas, s)
  local suma, amp, total = 0, 1, 0
  local eu, ev = escalaU, escalaV
  for o = 1, octavas do
    suma = suma + amp * ruido(u * eu, v * ev, periodicoU and eu or 0, periodicoV and ev or 0, (s or semilla) + o * 31)
    total = total + amp
    amp = amp * 0.5
    eu, ev = eu * 2, ev * 2
  end
  return suma / total
end

-- ---------------------------------------------------------------- Utilidades de imagen
local function cargar(ruta)
  local img = Image{ fromFile = ruta }
  if img.colorMode ~= ColorMode.RGB then
    local copia = Image(img.width, img.height, ColorMode.RGB)
    copia:drawImage(img, Point(0, 0))
    img = copia
  end
  return img
end

local function px(img, x, y)
  if x < 0 or y < 0 or x >= img.width or y >= img.height then return 0, 0, 0, 0 end
  local v = img:getPixel(x, y)
  return pc.rgbaR(v), pc.rgbaG(v), pc.rgbaB(v), pc.rgbaA(v)
end

local function opaco(img, x, y)
  local _, _, _, a = px(img, x, y)
  return a > 127
end

local function poner(img, x, y, c, a)
  if x < 0 or y < 0 or x >= img.width or y >= img.height then return end
  img:drawPixel(x, y, rgba(c, a))
end

local function lum(r, g, b) return (0.299 * r + 0.587 * g + 0.114 * b) / 255 end

-- Mezcla un color sobre el píxel que ya está (para manchas y brillos)
local function mezclar(img, x, y, c, k)
  local r, g, b, a = px(img, x, y)
  if a == 0 then return end
  poner(img, x, y, {math.floor(r + (c[1] - r) * k), math.floor(g + (c[2] - g) * k), math.floor(b + (c[3] - b) * k)}, a)
end

-- Distancia al borde transparente más cercano (hasta 'max'), para no dibujar poros en la silueta
local function lejosDelBorde(img, x, y, r)
  for dy = -r, r do
    for dx = -r, r do
      if not opaco(img, x + dx, y + dy) then return false end
    end
  end
  return true
end

-- ---------------------------------------------------------------- Detalles
-- Poro: agujero oscuro con aro de piel pálida, como los de los muebles de la referencia
local function poro(img, cx, cy, r, mascara)
  for dy = -r - 1, r + 1 do
    for dx = -r - 1, r + 1 do
      local x, y = cx + dx, cy + dy
      if mascara == nil or mascara(x, y) then
        local d = math.sqrt(dx * dx + dy * dy * 1.6)
        local grueso = (r >= 3) and 1.6 or 1.0
        if d <= r - grueso then
          -- fondo del agujero: negro, con un rojo profundo en el borde de abajo
          poner(img, x, y, (dy > 0 and d > r - grueso - 1.2) and {58, 8, 10} or HUECO)
        elseif d <= r + 0.3 then
          poner(img, x, y, (dx + dy > 0) and ARO_SOMBRA or ARO)
        end
      end
    end
  end
end

-- Vena: camino al azar, con borde oscuro y un brillo de un píxel
local function vena(img, x, y, largo, dirX, dirY, grosor, dentro, envolver)
  local ang = math.atan(dirY, dirX)
  for i = 1, largo do
    ang = ang + (math.random() - 0.5) * 0.7
    x = x + math.cos(ang)
    y = y + math.sin(ang)
    local ix, iy = math.floor(x + 0.5), math.floor(y + 0.5)
    if envolver then ix = envolver(ix) end
    if dentro(ix, iy) then
      poner(img, ix, iy, VENA_OSCURA)
      if grosor > 1 and dentro(ix + 1, iy) then poner(img, ix + 1, iy, VENA_MEDIA) end
      if dentro(ix, iy - 1) and math.random() < 0.6 then mezclar(img, ix, iy - 1, VENA_LUZ, 0.55) end
    end
    -- Ramas
    if math.random() < 0.04 and largo > 12 then
      vena(img, x, y, math.floor(largo * 0.4), math.cos(ang + 0.9), math.sin(ang + 0.9), 1, dentro, envolver)
    end
  end
end

-- Ojo incrustado: párpados de carne, esclerótica con capilares, iris y pupila
local function ojo(img, cx, cy, ancho, alto, apertura, mirada, lagrima)
  local rx, ry = ancho / 2, alto / 2
  -- párpado: anillo de carne hinchada
  for dy = -math.ceil(ry) - 2, math.ceil(ry) + 2 do
    for dx = -math.ceil(rx) - 2, math.ceil(rx) + 2 do
      local d = (dx / (rx + 1.6)) ^ 2 + (dy / (ry + 1.6)) ^ 2
      if d <= 1 then
        local t = 0.5 + 0.35 * (1 - d) - dy * 0.03
        poner(img, cx + dx, cy + dy, carne(t, cx + dx, cy + dy))
      end
    end
  end
  local abiertoRy = ry * apertura
  if abiertoRy < 0.6 then
    -- cerrado: una línea oscura con pestañas
    for dx = -math.floor(rx), math.floor(rx) do
      poner(img, cx + dx, cy, CARNE[1])
      if dx % 3 == 0 then poner(img, cx + dx, cy + 1, CARNE[2]) end
    end
    return
  end
  for dy = -math.ceil(abiertoRy), math.ceil(abiertoRy) do
    for dx = -math.ceil(rx), math.ceil(rx) do
      local d = (dx / rx) ^ 2 + (dy / abiertoRy) ^ 2
      if d <= 1 then
        local sombra = (dy < -abiertoRy * 0.4) and 0.78 or 1
        local c = {math.floor(214 * sombra), math.floor(200 * sombra), math.floor(186 * sombra)}
        if d > 0.75 then c = {176, 120, 110} end
        poner(img, cx + dx, cy + dy, c)
      end
    end
  end
  -- capilares
  for k = 1, 4 do
    local lado = (k % 2 == 0) and 1 or -1
    local x, y = cx + lado * math.floor(rx), cy + math.random(-1, 1)
    for i = 1, math.floor(rx * 0.6) do
      x = x - lado
      if math.random() < 0.4 then y = y + math.random(-1, 1) end
      local d = ((x - cx) / rx) ^ 2 + ((y - cy) / abiertoRy) ^ 2
      if d <= 1 then poner(img, x, y, {178, 40, 40}) end
    end
  end
  -- iris y pupila (la mirada mueve el iris)
  local ix = cx + math.floor(mirada * rx * 0.45 + 0.5)
  local ri = math.max(1.5, math.min(rx * 0.42, abiertoRy + 0.6))
  for dy = -math.ceil(ri), math.ceil(ri) do
    for dx = -math.ceil(ri), math.ceil(ri) do
      local d = math.sqrt(dx * dx + dy * dy)
      local enOjo = ((ix + dx - cx) / rx) ^ 2 + (dy / abiertoRy) ^ 2 <= 1
      if d <= ri and enOjo then
        local c = (d < ri * 0.45) and {6, 2, 2} or ((d > ri - 0.8) and {34, 46, 30} or {64, 86, 52})
        poner(img, ix + dx, cy + dy, c)
      end
    end
  end
  poner(img, ix - 1, cy - 1, {240, 236, 226})
  if lagrima then
    for i = 1, math.floor(ry * 2 + 4) do
      local x = ix + ((i % 5 == 0) and 1 or 0)
      poner(img, x, cy + math.ceil(abiertoRy) + i, (i % 2 == 0) and {150, 20, 22} or {110, 12, 16})
    end
  end
end

-- ---------------------------------------------------------------- Figuras de carne viva (bocas, apéndices, criaturas)
-- Carne cruda, rosada, como la de un pollo desplumado
local CRUDA = {
  {48, 16, 20}, {84, 34, 38}, {124, 58, 58}, {164, 86, 82}, {196, 116, 106},
  {218, 146, 132}, {234, 176, 160}, {246, 206, 192}, {252, 230, 220},
}
local PEZ = { {24, 30, 42}, {48, 60, 78}, {84, 98, 118}, {124, 138, 156}, {168, 178, 192}, {208, 214, 222}, {236, 238, 240} }

local function rampa(pal, t, x, y)
  t = math.max(0, math.min(0.999, t))
  local pos = t * (#pal - 1)
  local i = math.floor(pos)
  local f = pos - i
  local umbral = (BAYER[(y % 4) + 1][(x % 4) + 1] + 0.5) / 16
  if f > umbral then i = i + 1 end
  return pal[math.min(#pal, i + 1)]
end

-- Recorre los píxeles de una elipse; f recibe (x, y, nx, ny, d) con d de 0 (centro) a 1 (borde)
local function elipse(cx, cy, rx, ry, f)
  for y = math.floor(cy - ry - 1), math.ceil(cy + ry + 1) do
    for x = math.floor(cx - rx - 1), math.ceil(cx + rx + 1) do
      local nx, ny = (x + 0.5 - cx) / rx, (y + 0.5 - cy) / ry
      local d = math.sqrt(nx * nx + ny * ny)
      if d <= 1 then f(x, y, nx, ny, d) end
    end
  end
end

-- Volumen redondo con luz de arriba a la izquierda
local function bulto(img, pal, cx, cy, rx, ry, base, rugoso, sem)
  elipse(cx, cy, rx, ry, function(x, y, nx, ny, d)
    local t = base + (1 - d) * 0.25 - nx * 0.18 - ny * 0.28
    if rugoso then t = t + (fbm(x / 20, y / 20, 3, 3, false, false, 2, sem or 1300) - 0.5) * rugoso end
    poner(img, x, y, rampa(pal, t, x, y))
  end)
end

-- Tubo (cuello, brazo, pierna, tentáculo) por una lista de puntos con grosores
local function tubo(img, pal, puntos, grosores, base)
  for i = 1, #puntos - 1 do
    local a, b = puntos[i], puntos[i + 1]
    local largo = math.max(1, math.sqrt((b[1] - a[1]) ^ 2 + (b[2] - a[2]) ^ 2))
    for s = 0, largo do
      local k = s / largo
      local x = a[1] + (b[1] - a[1]) * k
      local y = a[2] + (b[2] - a[2]) * k
      local r = grosores[i] + (grosores[i + 1] - grosores[i]) * k
      elipse(x, y, r, r, function(px, py, nx, ny, d)
        poner(img, px, py, rampa(pal, base + (1 - d) * 0.2 - ny * 0.3 - nx * 0.1, px, py))
      end)
    end
  end
end

-- Contorno oscuro de un píxel alrededor de lo opaco, dentro de una franja del lienzo
local function contornear(img, x0, x1, color)
  local copia = img:clone()
  for y = 0, img.height - 1 do
    for x = x0, x1 do
      if not opaco(copia, x, y) and (opaco(copia, x - 1, y) or opaco(copia, x + 1, y) or opaco(copia, x, y - 1) or opaco(copia, x, y + 1)) then
        if x >= x0 and x <= x1 then poner(img, x, y, color) end
      end
    end
  end
end

local function guardarCuadros(cuadros)
  for f, img in ipairs(cuadros) do
    img:saveAs((P.salida:gsub("%.png$", "_" .. (f - 1) .. ".png")))
  end
end

-- ---------------------------------------------------------------- Modos
local modo = P.modo

if modo == "pared" then
  -- Tira de pared: cada columna tiene su borde de arriba (la tira está inclinada 2:1).
  -- u = columna (se repite cada ancho), v = píxeles desde el borde de arriba de esa columna.
  local img = cargar(P.entrada)
  local W, H = img.width, img.height
  local orig = img:clone()
  local arriba, abajo = {}, {}
  for x = 0, W - 1 do
    arriba[x], abajo[x] = nil, nil
    for y = 0, H - 1 do
      if opaco(orig, x, y) then
        if arriba[x] == nil then arriba[x] = y end
        abajo[x] = y
      end
    end
  end
  local tapa = num("tapa", 6)
  local altoMax = 0
  for x = 0, W - 1 do if arriba[x] then altoMax = math.max(altoMax, abajo[x] - arriba[x]) end end

  for x = 0, W - 1 do
    if arriba[x] then
      for y = arriba[x], abajo[x] do
        local _, _, _, a = px(orig, x, y)
        if a > 0 then
          local u0 = x / W
          local v0 = (y - arriba[x]) / altoMax
          -- Pliegues orgánicos: el ruido se deforma con otro ruido (sigue siendo continuo a lo ancho)
          local u = u0 + (fbm(u0, v0, 3, 4, true, false, 2, 151) - 0.5) * 0.35
          local v = v0 + (fbm(u0, v0, 3, 4, true, false, 2, 252) - 0.5) * 0.18
          local fibra = fbm(u, v, 4, 6, true, false, 3, 101)
          local grumo = fbm(u, v, 2, 8, true, false, 3, 202)
          local surco = math.abs(fbm(u, v, 3, 7, true, false, 3, 303) - 0.5) * 2
          local t = 0.30 + (fibra - 0.5) * 0.5 + (grumo - 0.5) * 0.4
          -- grietas oscuras entre los pliegues y lomos claros
          if surco < 0.07 then t = t - 0.22 elseif surco < 0.13 then t = t - 0.1 end
          if surco > 0.9 then t = t + 0.1 end
          -- más oscuro abajo y en lo alto, donde la pared se pierde en la sombra
          t = t - math.max(0, v - 0.55) * 0.55 - math.max(0, 0.12 - v) * 0.8
          -- la tapa de arriba: un borde de carne más claro
          if y - arriba[x] < tapa then t = 0.42 + (fibra - 0.5) * 0.3 end
          if y - arriba[x] == tapa then t = 0.12 end
          poner(img, x, y, carne(t, x, y), a)
        end
      end
    end
  end

  local function envolver(ix) return ix % W end
  local function dentro(ix, iy)
    return ix >= 0 and ix < W and arriba[ix] ~= nil and iy > arriba[ix] + tapa and iy <= abajo[ix] - 2
  end
  -- Venas sobre la superficie (u, v): se dibujan en coordenadas de la superficie y se pasan a píxeles
  local nVenas = num("venas", 3)
  for k = 1, nVenas do
    local u0 = math.random(0, W - 1)
    local v0 = math.random(0, math.floor(altoMax * 0.3))
    local largo = math.random(60, 160)
    local ang = math.pi / 2 + (math.random() - 0.5) * 0.8
    local u, v = u0, v0
    for i = 1, largo do
      ang = ang + (math.random() - 0.5) * 0.5
      ang = math.max(0.4, math.min(math.pi - 0.4, ang))
      u = u + math.cos(ang) * 0.8
      v = v + math.sin(ang)
      local ix = math.floor(u + 0.5) % W
      local iy = arriba[ix] and (arriba[ix] + math.floor(v + 0.5)) or -1
      if dentro(ix, iy) then
        poner(img, ix, iy, VENA_OSCURA)
        local ix2 = (ix + 1) % W
        if arriba[ix2] and dentro(ix2, arriba[ix2] + math.floor(v + 0.5)) and (i % 3 ~= 0) then
          poner(img, ix2, arriba[ix2] + math.floor(v + 0.5), VENA_MEDIA)
        end
        local ix0 = (ix - 1 + W) % W
        if arriba[ix0] and dentro(ix0, arriba[ix0] + math.floor(v + 0.5)) and math.random() < 0.5 then
          mezclar(img, ix0, arriba[ix0] + math.floor(v + 0.5), VENA_LUZ, 0.5)
        end
      end
    end
  end

  -- Raíces gruesas en relieve, del techo hacia abajo: borde de luz a la izquierda y sombra a la derecha.
  -- Se trazan sobre la superficie y dan la vuelta por los lados, así siguen en la tira de junto.
  for k = 1, num("raices", 2) do
    local u, v = math.random(0, W - 1), -4
    local grosor = math.random(3, 5)
    local ang = math.pi / 2 + (math.random() - 0.5) * 0.5
    local largo = math.random(math.floor(altoMax * 0.5), math.floor(altoMax * 1.1))
    for i = 1, largo do
      ang = ang + (math.random() - 0.5) * 0.3
      ang = math.max(1.0, math.min(math.pi - 1.0, ang))
      u = u + math.cos(ang) * 0.9
      v = v + math.sin(ang)
      local g = grosor - math.floor(i / largo * (grosor - 1))
      for o = 0, g - 1 do
        local ix = math.floor(u + 0.5 + o) % W
        if arriba[ix] then
          local iy = arriba[ix] + math.floor(v + 0.5)
          if dentro(ix, iy) then
            local c = (o == 0) and {176, 74, 60} or ((o == g - 1) and {18, 3, 4} or ((o == 1) and {132, 44, 38} or {96, 26, 26}))
            poner(img, ix, iy, c)
            -- sombra que proyecta la raíz sobre la pared
            if o == g - 1 then
              local sx = (ix + 1) % W
              if arriba[sx] and dentro(sx, arriba[sx] + math.floor(v + 0.5)) then mezclar(img, sx, arriba[sx] + math.floor(v + 0.5), {10, 2, 2}, 0.6) end
            end
          end
        end
      end
      if math.random() < 0.008 then
        -- nudo: un abultamiento con un poro
        local ix = math.floor(u + 0.5 + g / 2) % W
        if arriba[ix] and ix > 6 and ix < W - 7 then poro(img, ix, arriba[ix] + math.floor(v), 2, dentro) end
      end
    end
  end

  -- Poros y ojos lejos de los lados de la tira, para que las variantes se puedan mezclar sin cortes
  local function dentroCentro(ix, iy) return ix >= 5 and ix <= W - 6 and dentro(ix, iy) end
  for k = 1, num("poros", 2) do
    local x = math.random(9, W - 10)
    if arriba[x] then
      local y = arriba[x] + math.random(math.floor(altoMax * 0.2), math.floor(altoMax * 0.85))
      poro(img, x, y, math.random(2, 4), dentroCentro)
    end
  end
  for k = 1, num("ojos", 0) do
    local x = math.floor(W / 2) + math.random(-3, 3)
    if arriba[x] then
      local y = arriba[x] + math.random(math.floor(altoMax * 0.22), math.floor(altoMax * 0.5))
      local ancho = math.random(17, 21)
      ojo(img, x, y, ancho, math.floor(ancho * 0.5), 1, (math.random() - 0.5) * 2, false)
    end
  end
  -- brillo húmedo: algunos píxeles claros sueltos
  for k = 1, 14 do
    local x = math.random(1, W - 2)
    if arriba[x] then
      local y = arriba[x] + math.random(tapa + 2, math.floor(altoMax * 0.7))
      if dentro(x, y) then mezclar(img, x, y, {236, 170, 150}, 0.55) end
    end
  end
  img:saveAs(P.salida)

elseif modo == "piso" then
  -- Losa isométrica 96x48: se pasa a coordenadas de la rejilla (a, b) en 0..1, donde el ruido se repite
  local img = cargar(P.entrada)
  local W, H = img.width, img.height
  local tono = num("tono", 0)
  for y = 0, H - 1 do
    for x = 0, W - 1 do
      local _, _, _, a = px(img, x, y)
      if a > 0 then
        local dx = (x + 0.5 - W / 2) / (W / 2)
        local dy = (y + 0.5 - H / 2) / (H / 2)
        local A = (dx + dy) / 2 + 0.5
        local B = (dx - dy) / 2 + 0.5
        local pliegue = fbm(A, B, 5, 5, true, true, 3, 404 + semilla)
        local grumo = fbm(A, B, 3, 3, true, true, 2, 505 + semilla)
        local surco = math.abs(fbm(A, B, 4, 4, true, true, 2, 606 + semilla) - 0.5) * 2
        local t = 0.2 + tono + (pliegue - 0.5) * 0.3 + (grumo - 0.5) * 0.16
        if surco < 0.08 then t = t - 0.12 end
        if surco > 0.93 then t = t + 0.1 end
        poner(img, x, y, carne(t, x, y), a)
      end
    end
  end
  -- Venas cortas en el centro de la losa y brillos húmedos
  local function dentro(ix, iy)
    local dx = math.abs(ix + 0.5 - W / 2) / (W / 2)
    local dy = math.abs(iy + 0.5 - H / 2) / (H / 2)
    return dx + dy < 0.78
  end
  for k = 1, num("venas", 1) do
    vena(img, W / 2 + math.random(-14, 14), H / 2 + math.random(-6, 6), math.random(10, 24), math.random() - 0.5, math.random() - 0.5, 1, dentro)
  end
  for k = 1, num("poros", 0) do
    local x, y = math.floor(W / 2 + math.random(-20, 20)), math.floor(H / 2 + math.random(-7, 7))
    if dentro(x, y) then poro(img, x, y, math.random(1, 2), dentro) end
  end
  for k = 1, 10 do
    local x, y = math.floor(W / 2 + math.random(-30, 30)), math.floor(H / 2 + math.random(-12, 12))
    if dentro(x, y) then mezclar(img, x, y, {230, 160, 140}, 0.5) end
  end
  img:saveAs(P.salida)

elseif modo == "mueble" or modo == "oscurecer" then
  local img = cargar(P.entrada)
  local W, H = img.width, img.height
  local orig = img:clone()
  local escala = num("escala", 1)
  local claro = num("claro", 0.0)

  -- Pantalla: polígono opcional (x1,y1,x2,y2,...) que se llena de estática fría, sin ningún ojo
  local poli = nil
  if P.pantalla and P.pantalla ~= "" then
    poli = {}
    for n in string.gmatch(P.pantalla, "[^,]+") do poli[#poli + 1] = tonumber(n) end
  end
  local function enPoli(x, y)
    if poli == nil then return false end
    local dentro = false
    local j = #poli - 1
    for i = 1, #poli, 2 do
      local xi, yi, xj, yj = poli[i], poli[i + 1], poli[j], poli[j + 1]
      if ((yi > y) ~= (yj > y)) and (x < (xj - xi) * (y - yi) / (yj - yi) + xi) then dentro = not dentro end
      j = i
    end
    return dentro
  end

  for y = 0, H - 1 do
    for x = 0, W - 1 do
      local r, g, b, a = px(orig, x, y)
      if a > 0 then
        local L = lum(r, g, b)
        if enPoli(x + 0.5, y + 0.5) then
          -- estática: grises azulados, con líneas de barrido
          local s = hash(x, y, num("estatica", semilla) + 77)
          local fila = (y % 3 == 0) and 0.75 or 1
          local v = (0.25 + s * 0.7) * fila
          poner(img, x, y, {math.floor(150 * v), math.floor(160 * v), math.floor(176 * v)}, a)
        elseif modo == "oscurecer" then
          -- Conserva el dibujo, lo baja de tono y lo tiñe hacia la carne
          local m = fbm(x / W, y / H, 5 * escala, 5 * escala, false, false, 3, 707)
          local k = num("tono", 0.55) + (m - 0.5) * 0.25
          local nr = math.floor(r * k * 1.05 + 10)
          local ng = math.floor(g * k * 0.62)
          local nb = math.floor(b * k * 0.6)
          poner(img, x, y, {math.min(255, nr), ng, nb}, a)
        else
          local marmol = fbm(x / W, y / H, 7 * escala * (W / 200), 7 * escala * (H / 200), false, false, 3, 808)
          local grasa = math.abs(fbm(x / W, y / H, 4 * escala * (W / 200), 9 * escala * (H / 200), false, false, 2, 909) - 0.5) * 2
          local t = 0.12 + claro + L * 0.62 + (marmol - 0.5) * 0.34
          if grasa < 0.025 and L > 0.25 then t = 0.6 end
          if grasa >= 0.025 and grasa < 0.05 and L > 0.25 then t = t + 0.07 end
          if L < 0.13 then t = 0.0 end
          poner(img, x, y, carne(t, x, y), a)
        end
      end
    end
  end

  local function dentro(ix, iy) return opaco(orig, ix, iy) and not enPoli(ix + 0.5, iy + 0.5) end
  local area = 0
  for y = 0, H - 1 do for x = 0, W - 1 do if opaco(orig, x, y) then area = area + 1 end end end

  -- Poros: más en los muebles grandes
  local nPoros = math.floor(area / 1000 * num("poros", 1.2))
  local intentos = 0
  while nPoros > 0 and intentos < 4000 do
    intentos = intentos + 1
    local x, y = math.random(2, W - 3), math.random(2, H - 3)
    local r = math.random(math.min(2, num("radio", 3)), num("radio", 3))
    if lejosDelBorde(orig, x, y, r + 2) and not enPoli(x, y) then
      local _, _, _, _ = px(orig, x, y)
      local R, G, B = px(orig, x, y)
      if lum(R, G, B) > 0.15 then
        poro(img, x, y, r, dentro)
        nPoros = nPoros - 1
      end
    end
  end

  local nVenas = math.floor(area / 1800 * num("venas", 1))
  for k = 1, nVenas do
    local x, y
    for i = 1, 50 do
      x, y = math.random(0, W - 1), math.random(0, H - 1)
      if dentro(x, y) then break end
    end
    vena(img, x, y, math.random(8, 30), math.random() - 0.5, math.random() * 0.8 + 0.2, (math.random() < 0.3) and 2 or 1, dentro)
  end

  for k = 1, num("ojos", 0) do
    for i = 1, 400 do
      local x, y = math.random(8, W - 9), math.random(6, H - 7)
      if lejosDelBorde(orig, x, y, 7) and not enPoli(x, y) then
        local ancho = math.random(8, 12)
        ojo(img, x, y, ancho, math.floor(ancho * 0.55), 1, (math.random() - 0.5) * 2, false)
        break
      end
    end
  end

  -- Escurrimientos: gotas de un píxel bajo los bordes de abajo, dentro del lienzo
  for x = 0, W - 1 do
    if math.random() < num("gotas", 0.05) then
      for y = H - 1, 0, -1 do
        if opaco(orig, x, y) then
          local largo = math.random(2, 7)
          for i = 1, largo do
            if y + i < H and not opaco(orig, x, y + i) then
              poner(img, x, y + i, (i == largo) and {120, 18, 20} or {70, 10, 12})
            end
          end
          break
        end
      end
    end
  end
  img:saveAs(P.salida)

elseif modo == "ojo" then
  -- 4 cuadros, cada uno en su archivo (salida_0..3): abierto, entrecerrado, cerrado, abierto mirando al baño y llorando
  local w, h = num("ancho", 34), num("alto", 34)
  local cx, cy = math.floor(w / 2), math.floor(h / 2) - 4
  -- 4 y 5: llorando mirando a la izquierda y a la derecha (cada ojo voltea hacia el baño)
  local aperturas = {1, 0.45, 0, 1, 1}
  local miradas = {0, 0, 0, -1, 1}
  for f = 0, 4 do
    local img = Image(w, h, ColorMode.RGB)
    math.randomseed(semilla * 7919 + 13)
    ojo(img, cx, cy, w - 10, math.floor((w - 10) * 0.52), aperturas[f + 1], miradas[f + 1], f >= 3)
    img:saveAs((P.salida:gsub("%.png$", "_" .. f .. ".png")))
  end

elseif modo == "corazon" then
  -- Corazón deformado latiendo: 4 cuadros (reposo, contrae, late, vuelve)
  local w, h = 40, 44
  local escalas = {1.0, 0.9, 1.12, 1.04}
  for f = 0, 3 do
    local img = Image(w * 4, h, ColorMode.RGB)
    local s = escalas[f + 1]
    local cx, cy = f * w + 20, 22
    for y = 0, h - 1 do
      for x = 0, w - 1 do
        local dx = (x - 20) / (13 * s)
        local dy = (y - 22) / (15 * s)
        -- forma de corazón anatómico: dos lóbulos y punta torcida
        local lobulo = math.min((dx + 0.35) ^ 2 + (dy + 0.25) ^ 2, (dx - 0.4) ^ 2 * 1.3 + (dy + 0.35) ^ 2)
        local cuerpo = (dx - dy * 0.25) ^ 2 * 1.4 + (dy - 0.1) ^ 2 * 0.9
        if lobulo < 0.42 or cuerpo < 0.62 then
          local t = 0.55 - dy * 0.25 + (fbm(x / w, y / h, 6, 6, false, false, 2, 1111) - 0.5) * 0.4
          if f == 2 then t = t + 0.1 end
          poner(img, f * w + x, y, carne(t, x, y))
        end
      end
    end
    -- arterias que salen por arriba
    for k = 0, 2 do
      local ax = cx - 6 + k * 6
      for i = 0, 8 do
        local x = ax + math.floor(math.sin(i * 0.7 + k) * 1.5)
        poner(img, x, 22 - math.floor(15 * s) - i + 6, (k == 1) and {70, 30, 90} or {150, 34, 36})
        poner(img, x + 1, 22 - math.floor(15 * s) - i + 6, (k == 1) and {50, 18, 70} or {110, 22, 26})
      end
    end
    -- venas sobre el músculo
    math.randomseed(semilla * 7919 + 13)
    local function dentro(ix, iy) return ix >= f * w and ix < (f + 1) * w and opaco(img, ix, iy) end
    for k = 1, 4 do vena(img, cx + math.random(-6, 6), 22 + math.random(-6, 4), 14, math.random() - 0.5, 1, 1, dentro) end
    -- contorno oscuro
    local copia = img:clone()
    for y = 0, h - 1 do
      for x = f * w, (f + 1) * w - 1 do
        if opaco(copia, x, y) and (not opaco(copia, x - 1, y) or not opaco(copia, x + 1, y) or not opaco(copia, x, y - 1) or not opaco(copia, x, y + 1)) then
          poner(img, x, y, CARNE[1])
        end
      end
    end
    local cuadro = Image(w, h, ColorMode.RGB)
    cuadro:drawImage(img, Point(-f * w, 0))
    cuadro:saveAs((P.salida:gsub("%.png$", "_" .. f .. ".png")))
  end

elseif modo == "charco" then
  local w, h = num("ancho", 96), num("alto", 40)
  local img = Image(w, h, ColorMode.RGB)
  for y = 0, h - 1 do
    for x = 0, w - 1 do
      local dx = (x + 0.5 - w / 2) / (w / 2)
      local dy = (y + 0.5 - h / 2) / (h / 2)
      local borde = fbm(x / w, y / h, 5, 5, false, false, 2, 1212)
      if dx * dx + dy * dy < 0.55 + (borde - 0.5) * 0.7 then
        local t = (fbm(x / w, y / h, 8, 8, false, false, 2, 1313))
        local c = {math.floor(60 + t * 40), 6, math.floor(8 + t * 6)}
        if (x + y * 3) % 23 == 0 and dy < 0 then c = {190, 90, 90} end
        poner(img, x, y, c, 235)
      end
    end
  end
  img:saveAs(P.salida)

elseif modo == "venas" then
  -- Raíces que bajan del techo por la pared, sobre fondo transparente
  local w, h = num("ancho", 48), num("alto", 200)
  local img = Image(w, h, ColorMode.RGB)
  local function dentro(ix, iy) return ix >= 0 and iy >= 0 and ix < w and iy < h end
  for k = 1, num("cantidad", 5) do
    local x = math.random(4, w - 5)
    local function trazo(x, y, largo, grosor)
      local ang = math.pi / 2 + (math.random() - 0.5) * 0.6
      for i = 1, largo do
        ang = ang + (math.random() - 0.5) * 0.35
        x = x + math.cos(ang) * 0.7
        y = y + math.sin(ang)
        local ix, iy = math.floor(x + 0.5), math.floor(y + 0.5)
        for g = 0, grosor - 1 do
          if dentro(ix + g, iy) then poner(img, ix + g, iy, (g == 0) and {54, 10, 16} or ((g == grosor - 1) and {128, 40, 44} or {86, 18, 26})) end
        end
        if math.random() < 0.05 and largo > 20 then trazo(x, y, math.floor(largo * 0.5), math.max(1, grosor - 1)) end
      end
    end
    trazo(x, 0, math.random(math.floor(h * 0.4), h - 4), math.random(2, 4))
  end
  img:saveAs(P.salida)
elseif modo == "boca" then
  -- Boca humana en el piso, vista desde arriba en isométrico: labios gruesos con arco, encías, dos filas
  -- de dientes y lengua. 4 cuadros: cerrada, entreabierta, abierta, mordida
  local w, h = 60, 38
  local cx, cy = 30, 19
  local rx = 25
  local LABIO = { {54, 8, 14}, {98, 18, 28}, {142, 30, 42}, {178, 52, 62}, {208, 88, 96}, {232, 140, 144} }
  local aperturas = {0, 0.45, 1, 0}
  local cuadros = {}
  for f = 1, 4 do
    local img = Image(w, h, ColorMode.RGB)
    -- piel fruncida alrededor, que se funde con el piso
    elipse(cx, cy, rx + 5, 14, function(x, y, nx, ny, d)
      local arruga = math.abs(math.sin(math.atan(ny, nx) * 11 + d * 5))
      poner(img, x, y, carne(0.14 + arruga * 0.12 - ny * 0.06, x, y), math.floor(255 * math.min(1, (1 - d) * 3)))
    end)
    local ap = aperturas[f]
    for x = cx - rx, cx + rx do
      local nx = (x - cx) / rx
      local ancho = math.sqrt(math.max(0, 1 - nx * nx))
      -- labio de arriba con arco de cupido (dos bultos y una muesca al centro)
      local arco = 1 + 0.35 * math.exp(-((math.abs(nx) - 0.28) / 0.16) ^ 2) - 0.25 * math.exp(-(nx / 0.08) ^ 2)
      local arriba = cy - math.floor(7 * ancho ^ 0.7 * arco + 0.5)
      local abajo = cy + math.floor(8 * ancho ^ 0.8 + 0.5)
      local hueco = math.floor(6.5 * ap * ancho ^ 0.9 + 0.5)
      local bordeSup = cy - hueco
      local bordeInf = cy + hueco
      for y = arriba, abajo do
        if y < bordeSup then
          -- labio de arriba: más oscuro, con brillo sobre el borde
          local t = (y - arriba) / math.max(1, bordeSup - arriba)
          poner(img, x, y, rampa(LABIO, 0.35 + t * 0.35 - math.abs(nx) * 0.15, x, y))
        elseif y > bordeInf then
          -- labio de abajo: grueso, con el brillo húmedo en medio
          local t = (y - bordeInf) / math.max(1, abajo - bordeInf)
          local brillo = (math.abs(t - 0.35) < 0.12 and math.abs(nx) < 0.45) and 0.3 or 0
          poner(img, x, y, rampa(LABIO, 0.6 - t * 0.35 + brillo - math.abs(nx) * 0.15, x, y))
        elseif hueco > 0 then
          -- dentro: garganta oscura, encías y lengua al fondo
          local prof = 1 - math.abs(y - cy) / math.max(1, hueco)
          poner(img, x, y, (prof > 0.55) and {16, 3, 5} or {64, 12, 18})
        end
      end
      -- contorno oscuro de los labios
      poner(img, x, arriba - 1, {40, 6, 10})
      poner(img, x, abajo + 1, {40, 6, 10})
      if hueco > 0 then
        -- encías y dientes: arriba cuelgan, abajo se levantan
        local largo = math.max(1, math.floor(hueco * 0.55))
        local diente = ((x - cx) % 3 ~= 0) and math.abs(nx) < 0.85
        poner(img, x, bordeSup, {196, 96, 104})
        poner(img, x, bordeInf, {186, 86, 96})
        if diente then
          for i = 1, largo do
            poner(img, x, bordeSup + i, (i == largo) and {196, 184, 150} or {240, 234, 214})
            poner(img, x, bordeInf - i, (i == largo) and {180, 168, 136} or {230, 222, 200})
          end
        end
      else
        -- cerrada: la línea entre los labios
        poner(img, x, cy, {30, 4, 8})
      end
    end
    if ap >= 1 then
      -- lengua asomando al fondo
      elipse(cx + 3, cy + 2, 8, 2.5, function(x, y, nx, ny, d) poner(img, x, y, rampa(LABIO, 0.5 - ny * 0.2, x, y)) end)
    end
    if f == 4 then
      -- mordida: los dientes se clavan por fuera y salpica sangre
      for k = -4, 4 do
        poner(img, cx + k * 4, cy - 1, {240, 232, 210}); poner(img, cx + k * 4 + 1, cy + 1, {232, 222, 200})
      end
      for k = 1, 12 do
        local ang = k * 0.52
        poner(img, cx + math.floor(math.cos(ang) * (rx + 2 + k % 3)), cy + math.floor(math.sin(ang) * (10 + k % 2)), {160, 12, 18})
      end
    end
    cuadros[f] = img
  end
  guardarCuadros(cuadros)

elseif modo == "apendice" then
  -- Apéndice que sale de la pared hacia abajo a la izquierda (2:1). 4 cuadros: dentro, asoma, a medias, fuera.
  -- La base (el agujero en la pared) queda en (118, 22) del lienzo de 128x84.
  local w, h = 128, 84
  local bx, by = 118, 22
  -- 8 cuadros: dentro, asoma, sale (4 pasos), fuera ondulando (2 cuadros)
  local extension = {0.0, 0.12, 0.3, 0.55, 0.8, 1.0, 1.0, 1.0}
  local fase = {0, 0, 0.4, 0.8, 1.2, 1.6, 2.6, 3.6}
  local cuadros = {}
  for f = 1, 8 do
    local img = Image(w, h, ColorMode.RGB)
    -- agujero desgarrado en la pared
    elipse(bx, by, 9, 11, function(x, y, nx, ny, d)
      poner(img, x, y, (d > 0.72) and carne(0.42 - ny * 0.2, x, y) or {16, 3, 4})
    end)
    local e = extension[f]
    if e > 0 then
      local tx, ty = bx - 108 * e, by + 54 * e + 2
      local mx, my = (bx + tx) / 2, (by + ty) / 2 + 10 * e
      local puntos, grosores = {}, {}
      local pasos = 14
      for i = 0, pasos do
        local t = i / pasos
        local x = (1 - t) ^ 2 * bx + 2 * (1 - t) * t * mx + t * t * tx
        local y = (1 - t) ^ 2 * by + 2 * (1 - t) * t * my + t * t * ty
        -- ondula como látigo: la onda viaja hacia la punta y es más fuerte al final
        y = y + math.sin(t * 8 - fase[f] * 2) * 3.2 * e * t
        x = x + math.cos(t * 6 - fase[f]) * 1.5 * e * t
        puntos[#puntos + 1] = {x, y}
        grosores[#grosores + 1] = 10 * (1 - t) ^ 0.8 + 2
      end
      tubo(img, CRUDA, puntos, grosores, 0.4)
      for i = 2, #puntos do
        local p, r = puntos[i], grosores[i]
        local px0, py0 = math.floor(p[1]), math.floor(p[2])
        -- anillos oscuros, como de lombriz
        if i % 2 == 0 then
          for k = -math.floor(r), math.floor(r) do mezclar(img, px0 + 1, py0 + k, {60, 16, 22}, 0.55) end
        end
        -- brillo húmedo por arriba y ventosas pálidas por abajo
        poner(img, px0, math.floor(py0 - r * 0.6), {252, 214, 200})
        if i % 2 == 1 and r > 2.5 then
          elipse(px0, py0 + r * 0.55, 1.2, 0.9, function(x, y) poner(img, x, y, {236, 186, 170}) end)
          poner(img, px0, math.floor(py0 + r * 0.55), {90, 24, 30})
        end
      end
      -- punta abierta con dientitos
      local p = puntos[#puntos]
      elipse(p[1] - 1, p[2], 3.2, 2.4, function(x, y, nx, ny, d) poner(img, x, y, (d < 0.6) and {18, 3, 5} or {150, 46, 52}) end)
      for k = -1, 1 do poner(img, math.floor(p[1]) - 2 + k * 2, math.floor(p[2]) - 1, {240, 232, 210}) end
      -- borde desgarrado del agujero, encima de donde sale
      for k = 0, 16 do
        local ang = k / 16 * math.pi * 2
        local x = bx + math.floor(math.cos(ang) * 9 + 0.5)
        local y = by + math.floor(math.sin(ang) * 11 + 0.5)
        if math.cos(ang) > -0.2 then poner(img, x, y, (k % 3 == 0) and {196, 90, 92} or {120, 30, 36}) end
      end
    else
      -- dentro: solo un bulto que late en el agujero
      elipse(bx - 1, by + 1, 5, 6, function(x, y, nx, ny, d) poner(img, x, y, rampa(CRUDA, 0.4 - ny * 0.3, x, y)) end)
    end
    contornear(img, 0, w - 1, {30, 6, 8})
    cuadros[f] = img
  end
  guardarCuadros(cuadros)

elseif modo == "criatura" then
  -- Carnes vivas que salen del refrigerador. tipo: pollo | pez | feto | rana. 2 cuadros al arrastrarse.
  local tipo = P.tipo
  local w, h = 56, 48
  local cuadros = {}
  for f = 1, 2 do
    local img = Image(w, h, ColorMode.RGB)
    local s = (f == 2) and 1 or 0
    if tipo == "pollo" then
      -- Ave cruda desplumada, hecha bola, con el cuello largo y una pata colgando
      bulto(img, CRUDA, 30, 31 + s, 19 + s, 12 - s, 0.38, 0.35, 1301)
      -- pliegues de la piel
      for k = 0, 5 do
        for x = 16, 44 do
          local y = math.floor(28 + k * 2.4 + math.sin(x * 0.35 + k) * 1.4)
          if opaco(img, x, y) and (x + k) % 3 ~= 0 then mezclar(img, x, y, {120, 54, 54}, 0.55) end
        end
      end
      tubo(img, CRUDA, {{22, 24}, {16, 16}, {14, 9 - s}}, {5, 4, 3.2}, 0.5)
      bulto(img, CRUDA, 12, 7 - s, 5, 4, 0.55, nil)
      for i = 0, 3 do poner(img, 6 - i, 7 - s + (i > 1 and 1 or 0), {232, 190, 140}) end
      poner(img, 11, 6 - s, {40, 10, 12})
      tubo(img, CRUDA, {{38, 38}, {44 + s * 2, 43}, {47 + s * 2, 46}}, {3.5, 2.6, 2}, 0.45)
      for i = -1, 1 do poner(img, 47 + s * 2 + i * 2, 47, {200, 140, 120}) end
    elseif tipo == "pez" then
      -- Pez con la panza abierta, llena de cuerpos pálidos enredados
      elipse(28, 22, 24, 11, function(x, y, nx, ny, d)
        if ny < 0.1 then
          local t = 0.55 - ny * 0.3 + (1 - d) * 0.2
          poner(img, x, y, rampa(PEZ, t, x, y))
          if hash(x, y, 1404) > 0.93 then poner(img, x, y, {24, 22, 30}) end
        else
          poner(img, x, y, rampa(CRUDA, 0.35 + (fbm(x / 30, y / 30, 4, 4, false, false, 2, 1405) - 0.5) * 0.6, x, y))
        end
      end)
      -- cuerpitos y brazos enredados en la panza
      for k = 0, 5 do
        local x0 = 10 + k * 7 + s
        tubo(img, CRUDA, {{x0, 24}, {x0 + 3, 30 + (k % 2) * 2}, {x0 + 1 + s, 35}}, {2, 1.7, 1.3}, 0.62)
        poner(img, x0 + 1 + s, 36, {244, 200, 186})
      end
      -- aleta y cola
      for i = 0, 9 do
        for j = -i, i do
          poner(img, 52 + i, 22 + j - s, rampa(PEZ, 0.25 + (i % 3) * 0.06, 52 + i, 22 + j))
        end
      end
      -- ojo naranja enorme y boca con algo saliendo
      elipse(9, 17, 4, 4, function(x, y, nx, ny, d) poner(img, x, y, (d < 0.45) and {10, 8, 6} or ((d < 0.85) and {214, 128, 34} or {60, 40, 20})) end)
      poner(img, 8, 16, {250, 240, 220})
      tubo(img, CRUDA, {{4, 26}, {1, 31 + s}}, {2, 1.5}, 0.55)
    elseif tipo == "feto" then
      -- Cabeza enorme y pálida, un ojo hundido y la boca abierta con dientes; cuerpo encogido
      bulto(img, CRUDA, 26, 33 - s, 11, 10, 0.45, 0.3, 1501)
      tubo(img, CRUDA, {{19, 33}, {14, 37 - s}, {12, 34}}, {2.4, 2, 1.6}, 0.5)
      tubo(img, CRUDA, {{30, 40}, {36, 45}, {40, 45}}, {3, 2.6, 2}, 0.48)
      bulto(img, CRUDA, 24, 16 - s, 14, 13, 0.55, 0.15, 1502)
      -- venas en el cráneo
      vena(img, 20, 8 - s, 10, 1, 0.4, 1, function(x, y) return opaco(img, x, y) end)
      elipse(30, 15 - s, 3.5, 3, function(x, y, nx, ny, d) poner(img, x, y, (d < 0.5) and {16, 6, 6} or {96, 40, 46}) end)
      elipse(20, 13 - s, 2.5, 2, function(x, y, nx, ny, d) poner(img, x, y, {90, 40, 44}) end)
      local abre = (f == 1) and 4 or 2
      elipse(30, 23 - s, 6, abre, function(x, y, nx, ny, d) poner(img, x, y, (d < 0.8) and {26, 4, 6} or {120, 40, 44}) end)
      for k = -2, 2 do
        poner(img, 30 + k * 2, 23 - s - abre + 1, {236, 226, 196})
        poner(img, 30 + k * 2 + 1, 23 - s + abre - 1, {220, 208, 176})
      end
    elseif tipo == "rana" then
      -- Cuerpo pálido de rana boca arriba, con brazos de dedos largos y muslos gordos
      tubo(img, CRUDA, {{18, 24}, {10, 18}, {8 - s, 8}}, {3.2, 2.6, 2}, 0.5)
      tubo(img, CRUDA, {{38, 24}, {46, 18}, {48 + s, 8}}, {3.2, 2.6, 2}, 0.5)
      for i = -1, 2 do
        poner(img, 8 - s + i * 2, 5, {214, 120, 110}); poner(img, 8 - s + i * 2, 6, {230, 160, 146})
        poner(img, 48 + s - i * 2, 5, {214, 120, 110}); poner(img, 48 + s - i * 2, 6, {230, 160, 146})
      end
      bulto(img, CRUDA, 16 - s, 38, 9, 7, 0.45, 0.2, 1601)
      bulto(img, CRUDA, 40 + s, 38, 9, 7, 0.45, 0.2, 1602)
      bulto(img, CRUDA, 28, 26, 11, 14, 0.55, 0.2, 1603)
      bulto(img, CRUDA, 28, 12, 8, 6, 0.6, nil)
      for _, ex in ipairs({24, 32}) do
        elipse(ex, 11, 2, 2, function(x, y, nx, ny, d) poner(img, x, y, (d < 0.6) and {70, 6, 10} or {160, 40, 40}) end)
        poner(img, ex - 1, 10, {250, 230, 220})
      end
      vena(img, 28, 18, 14, 0, 1, 1, function(x, y) return opaco(img, x, y) end)
    end
    contornear(img, 0, w - 1, {28, 8, 10})
    cuadros[f] = img
  end
  guardarCuadros(cuadros)

elseif modo == "manos" then
  -- Brazos pálidos que salen de debajo de la cama (por la izquierda) hacia Elaki. 4 cuadros, cada vez más fuera.
  local w, h = 104, 60
  local alcance = {0.25, 0.5, 0.75, 1.0}
  local cuadros = {}
  for f = 1, 4 do
    local img = Image(w, h, ColorMode.RGB)
    -- sombra bajo la cama
    elipse(4, 30, 14, 26, function(x, y, nx, ny, d) poner(img, x, y, {8, 2, 3}, math.floor(220 * (1 - d))) end)
    local brazos = { {6, 18, -0.15}, {6, 32, 0.05}, {6, 44, 0.22} }
    for i, b in ipairs(brazos) do
      local a = alcance[f] * (0.85 + i * 0.07)
      local tx = b[1] + 86 * a
      local ty = b[2] + 30 * a * (0.6 + b[3])
      local mx, my = (b[1] + tx) / 2, (b[2] + ty) / 2 - 6
      local puntos = {{b[1], b[2]}, {mx, my}, {tx, ty}}
      tubo(img, CRUDA, puntos, {3, 2.6, 2.2}, 0.62)
      -- mano: palma y dedos largos abiertos
      elipse(tx + 2, ty, 3, 2.4, function(x, y, nx, ny, d) poner(img, x, y, rampa(CRUDA, 0.6 - ny * 0.2, x, y)) end)
      for k = -2, 1 do
        local dx, dy = 1, k * 0.55
        for s2 = 1, 7 do
          poner(img, math.floor(tx + 3 + s2 * dx), math.floor(ty + s2 * dy), (s2 == 7) and {150, 60, 60} or {238, 196, 182})
        end
      end
    end
    contornear(img, 0, w - 1, {30, 8, 10})
    cuadros[f] = img
  end
  guardarCuadros(cuadros)

elseif modo == "ojogigante" then
  -- Ojo enorme para la ventana del pasillo. 3 cuadros (cerrado, entreabierto, abierto) y la pupila aparte.
  local w, h = 84, 50
  local cx, cy, rx, ry = 42, 25, 38, 20
  local cuadros = {}
  for f = 1, 3 do
    local img = Image(w, h, ColorMode.RGB)
    -- párpados: carne hinchada con venas, forma de almendra
    for y = 0, h - 1 do
      for x = 0, w - 1 do
        local nx = (x + 0.5 - cx) / (rx + 4)
        local borde = (1 - nx * nx) * (ry + 5)
        local dy = y + 0.5 - cy
        if math.abs(nx) <= 1 and math.abs(dy) <= borde then
          poner(img, x, y, carne(0.36 + (fbm(x / w, y / h, 6, 4, false, false, 2, 1701) - 0.5) * 0.4 - dy / borde * 0.15, x, y))
        end
      end
    end
    for k = 1, 5 do vena(img, math.random(8, w - 8), math.random(4, 10), math.random(10, 22), math.random() - 0.5, 0.6, 1, function(x, y) return opaco(img, x, y) end) end
    local apertura = ({0, 0.45, 1})[f]
    if apertura == 0 then
      for x = cx - rx + 4, cx + rx - 4 do
        local nx = (x - cx) / rx
        local y = cy + math.floor(nx * nx * 3)
        poner(img, x, y, {20, 4, 6}); poner(img, x, y + 1, {120, 40, 40})
        if x % 3 == 0 then poner(img, x, y + 2, {16, 4, 4}); poner(img, x - 1, y + 3, {16, 4, 4}) end
      end
    else
      for y = 0, h - 1 do
        for x = 0, w - 1 do
          local nx = (x + 0.5 - cx) / rx
          local alto = (1 - nx * nx) * ry
          local dy = y + 0.5 - cy
          -- el párpado de arriba baja cuando está entreabierto
          local tope = -alto + (1 - apertura) * alto * 1.1
          if math.abs(nx) < 1 and dy <= alto * 0.92 and dy >= tope then
            local orilla = math.min(dy - tope, alto * 0.92 - dy)
            local c = (orilla < 1.5) and {110, 26, 28} or {226 - math.floor(math.abs(nx) * 40), 210 - math.floor(math.abs(nx) * 60), 186 - math.floor(math.abs(nx) * 60)}
            if dy < tope + 4 then c = {math.floor(c[1] * 0.75), math.floor(c[2] * 0.7), math.floor(c[3] * 0.7)} end
            poner(img, x, y, c)
          end
        end
      end
      -- capilares que serpentean desde las orillas hacia el centro
      local function esclera(x, y) local _, g = px(img, x, y); return g > 120 end
      for k = 1, 18 do
        local lado = (k % 2 == 0) and 1 or -1
        local x, y = cx + lado * (rx - 5), cy + math.random(-7, 7)
        local ang = (lado > 0) and math.pi or 0
        ang = ang + (math.random() - 0.5) * 0.8
        for i = 1, math.random(10, 22) do
          ang = ang + (math.random() - 0.5) * 0.9
          x = x + math.cos(ang)
          y = y + math.sin(ang) * 0.7
          local ix, iy = math.floor(x + 0.5), math.floor(y + 0.5)
          if esclera(ix, iy) then poner(img, ix, iy, (i < 6) and {150, 18, 22} or {196, 52, 50}) end
          if math.random() < 0.08 then
            local bx2, by2, ba = x, y, ang + 1
            for j = 1, 6 do
              bx2 = bx2 + math.cos(ba); by2 = by2 + math.sin(ba) * 0.7
              if esclera(math.floor(bx2), math.floor(by2)) then poner(img, math.floor(bx2), math.floor(by2), {204, 70, 64}) end
            end
          end
        end
      end
      -- sombra húmeda bajo el párpado de arriba
      -- pestañas largas arriba
      for x = cx - rx + 6, cx + rx - 6, 3 do
        local nx = (x - cx) / rx
        local y = cy - math.floor((1 - nx * nx) * ry) + math.floor((1 - apertura) * (1 - nx * nx) * ry * 1.1) - 1
        for i = 0, 3 do poner(img, x - math.floor(nx * i), y - i, {12, 4, 4}) end
      end
    end
    contornear(img, 0, w - 1, {14, 3, 4})
    cuadros[f] = img
  end
  guardarCuadros(cuadros)
  -- pupila: iris ámbar enfermo con una pupila negra rasgada
  local pup = Image(22, 22, ColorMode.RGB)
  elipse(11, 11, 10, 10, function(x, y, nx, ny, d)
    local ang = math.atan(ny, nx)
    local c = (d > 0.88) and {40, 22, 8} or {math.floor(168 + math.sin(ang * 11) * 30 - d * 60), math.floor(118 + math.sin(ang * 11) * 20 - d * 50), 24}
    if math.abs(nx) < 0.22 and math.abs(ny) < 0.85 then c = {6, 2, 2} end
    poner(pup, x, y, c)
  end)
  poner(pup, 7, 6, {255, 246, 220}); poner(pup, 8, 6, {255, 246, 220}); poner(pup, 7, 7, {255, 246, 220})
  pup:saveAs((P.salida:gsub("%.png$", "_pupila.png")))

else
  error("Modo desconocido: " .. tostring(modo))
end
