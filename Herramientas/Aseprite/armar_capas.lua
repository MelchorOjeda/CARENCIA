-- Arma un .aseprite con capas y cuadros a partir de PNG del mismo tamaño.
-- Uso: aseprite -b --script-param salida=<archivo.aseprite> --script-param capas=<nombre=patron,...>
--        --script-param duraciones=<ms,ms,...> [--script-param etiqueta=<nombre>] --script armar_capas.lua
-- Cada patron es una ruta PNG; "{i}" se cambia por el número de cuadro (desde 0). Un patron sin "{i}"
-- repite la misma imagen en todos los cuadros. Las capas se apilan en el orden dado: la primera queda abajo.

local P = app.params

local function split(text, separator)
  local parts = {}
  for part in string.gmatch(text, "([^" .. separator .. "]+)") do table.insert(parts, part) end
  return parts
end

local durations = split(P.duraciones, ",")
local layers = split(P.capas, ",")

local function image_for(pattern, frame_index)
  return Image{ fromFile = string.gsub(pattern, "{i}", tostring(frame_index - 1)) }
end

local first_pattern = split(layers[1], "=")[2]
local first = image_for(first_pattern, 1)
local sprite = Sprite(first.width, first.height, ColorMode.RGB)

for _ = 2, #durations do sprite:newEmptyFrame() end
for index, duration in ipairs(durations) do sprite.frames[index].duration = tonumber(duration) / 1000 end

for layer_index, entry in ipairs(layers) do
  local name_and_pattern = split(entry, "=")
  local layer = layer_index == 1 and sprite.layers[1] or sprite:newLayer()
  layer.name = name_and_pattern[1]
  for frame_index = 1, #durations do
    sprite:newCel(layer, frame_index, image_for(name_and_pattern[2], frame_index), Point(0, 0))
  end
end

if P.etiqueta ~= nil and P.etiqueta ~= "" then
  local tag = sprite:newTag(1, #durations)
  tag.name = P.etiqueta
end
sprite:saveAs(P.salida)
