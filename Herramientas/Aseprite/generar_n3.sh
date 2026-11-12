#!/usr/bin/env bash
# Genera el arte de carne del Nivel 3 (Dimensión 3) a partir del arte de la casa real.
# Uso (desde la raíz del proyecto, con Unity cerrado o sin importar): bash Herramientas/Aseprite/generar_n3.sh
# Cada PNG nuevo copia el .meta del original (mismo pivote, PPU y recorte) con un GUID nuevo.
set -euo pipefail

ASEPRITE="${ASEPRITE:-/c/Users/Melch/aseprite/build/bin/aseprite.exe}"
LUA="Herramientas/Aseprite/carne.lua"
REAL="Assets/_Carencia/Sprites/Escenarios/Realidad"
OBJ="Assets/_Carencia/Sprites/Objetos"
ESC_D3="Assets/_Carencia/Sprites/Escenarios/Dimension3"
OBJ_D3="Assets/_Carencia/Sprites/Objetos/Dimension3"
mkdir -p "$ESC_D3" "$OBJ_D3"

guid() { python -I -c "import uuid; print(uuid.uuid4().hex)"; }

# Copia el .meta del original con GUID nuevo; renombra los sprites internos si es una hoja
meta() {
  local origen="$1" destino="$2"
  local viejo nuevo
  viejo="$(basename "${origen%.png}")"
  nuevo="$(basename "${destino%.png}")"
  # Si ya existe, conserva su GUID: las escenas generadas lo referencian
  local id
  if [ -f "$destino.meta" ]; then id="$(sed -n 's/^guid: //p' "$destino.meta")"; else id="$(guid)"; fi
  sed -e "s/^guid: .*/guid: $id/" -e "s/${viejo}_/${nuevo}_/g" "$origen.meta" > "$destino.meta"
}

gen() {
  "$ASEPRITE" -b "$@" --script "$LUA" > /dev/null
}

# Genera a partir de un original y copia su .meta
derivar() {
  local modo="$1" origen="$2" destino="$3"
  shift 3
  gen --script-param modo="$modo" --script-param entrada="$origen" --script-param salida="$destino" "$@"
  meta "$origen" "$destino"
  echo "  $destino"
}

echo "Paredes y pisos"
for lado in Izq Der; do
  derivar pared "$REAL/SPR_ESC_ParedPasillo${lado}_R.png" "$ESC_D3/SPR_ESC_ParedPasillo${lado}A_D3.png" --script-param semilla=11 --script-param ojos=1
  derivar pared "$REAL/SPR_ESC_ParedPasillo${lado}_R.png" "$ESC_D3/SPR_ESC_ParedPasillo${lado}B_D3.png" --script-param semilla=12
  derivar pared "$REAL/SPR_ESC_ParedPasillo${lado}_R.png" "$ESC_D3/SPR_ESC_ParedPasillo${lado}C_D3.png" --script-param semilla=13 --script-param raices=3
  derivar pared "$REAL/SPR_ESC_ParedPasillo${lado}_R.png" "$ESC_D3/SPR_ESC_ParedPasillo${lado}D_D3.png" --script-param semilla=14 --script-param poros=5 --script-param raices=1
  derivar pared "$REAL/SPR_ESC_ParedBano${lado}_R.png" "$ESC_D3/SPR_ESC_ParedBano${lado}A_D3.png" --script-param semilla=21 --script-param ojos=1
  derivar pared "$REAL/SPR_ESC_ParedBano${lado}_R.png" "$ESC_D3/SPR_ESC_ParedBano${lado}B_D3.png" --script-param semilla=22
  derivar pared "$REAL/SPR_ESC_ParedBano${lado}_R.png" "$ESC_D3/SPR_ESC_ParedBano${lado}C_D3.png" --script-param semilla=23 --script-param raices=3
  derivar mueble "$REAL/SPR_ESC_MuroBajo${lado}_R.png" "$ESC_D3/SPR_ESC_MuroBajo${lado}_D3.png" --script-param semilla=31 --script-param poros=0.6 --script-param radio=2 --script-param gotas=0.25
done
for v in A B C D; do
  s=$(( $(printf '%d' "'$v") ))
  derivar piso "$REAL/SPR_ESC_LosaBeige_R.png" "$ESC_D3/SPR_ESC_Piso${v}_D3.png" --script-param semilla=$s --script-param poros=$(( s % 3 )) --script-param venas=$(( s % 2 + 1 ))
done
# Cuarto de Elaki (paredes angostas de 24 px)
for lado in Izq Der; do
  derivar pared "$REAL/SPR_ESC_Pared${lado}_R.png" "$ESC_D3/SPR_ESC_Pared${lado}A_D3.png" --script-param semilla=27 --script-param raices=1 --script-param poros=1
  derivar pared "$REAL/SPR_ESC_Pared${lado}_R.png" "$ESC_D3/SPR_ESC_Pared${lado}B_D3.png" --script-param semilla=28 --script-param raices=2
done
derivar piso "$REAL/SPR_ESC_LosaBanoClara_R.png" "$ESC_D3/SPR_ESC_PisoBanoClaro_D3.png" --script-param semilla=41 --script-param tono=0.05
derivar piso "$REAL/SPR_ESC_LosaBanoOscura_R.png" "$ESC_D3/SPR_ESC_PisoBanoOscuro_D3.png" --script-param semilla=42 --script-param tono=-0.06

echo "Muebles"
mueble() {
  local carpeta="$1" nombre="$2"
  shift 2
  derivar mueble "$OBJ/$carpeta/SPR_OBJ_${nombre}_R.png" "$OBJ_D3/SPR_OBJ_${nombre}_D3.png" "$@"
}
mueble Comedor BarraCocina --script-param semilla=51 --script-param radio=4 --script-param poros=1.2 --script-param gotas=0.12
# La computadora: la pantalla es estática fría, sin ningún ojo
mueble Comedor EscritorioComputadora --script-param semilla=52 --script-param radio=4 --script-param poros=1.6 --script-param pantalla=59,66,125,33,125,80,59,113
mueble Comedor MesaCentro --script-param semilla=53 --script-param radio=3 --script-param poros=1.4
mueble Comedor MesaComedor --script-param semilla=54 --script-param radio=4 --script-param poros=1.4 --script-param ojos=1
mueble Comedor Refrigerador --script-param semilla=55 --script-param radio=4 --script-param poros=1.0 --script-param gotas=0.15
mueble Comedor SillaComedorAtras --script-param semilla=56 --script-param radio=2 --script-param poros=1.2
mueble Comedor SillaComedorFrente --script-param semilla=57 --script-param radio=2 --script-param poros=1.2
mueble Comedor SillaOficina --script-param semilla=58 --script-param radio=2 --script-param poros=0.8
mueble Comedor Sillon --script-param semilla=59 --script-param radio=4 --script-param poros=1.6 --script-param claro=0.05
mueble Comedor Sofa --script-param semilla=60 --script-param radio=4 --script-param poros=1.8 --script-param claro=0.05
mueble Comedor Tapete --script-param semilla=61 --script-param radio=2 --script-param poros=0.8 --script-param claro=-0.04
mueble Comedor Television --script-param semilla=62 --script-param pantalla=6,82,147,10,147,90,6,162
# Cuadros extra de la estática (mismo mueble, solo cambia el ruido de la pantalla)
for k in 1 2; do
  derivar mueble "$OBJ/Comedor/SPR_OBJ_EscritorioComputadora_R.png" "$OBJ_D3/SPR_OBJ_EscritorioComputadora_D3_$k.png" --script-param semilla=52 --script-param radio=4 --script-param poros=1.6 --script-param pantalla=59,66,125,33,125,80,59,113 --script-param estatica=$((52 + k * 100))
  derivar mueble "$OBJ/Comedor/SPR_OBJ_Television_R.png" "$OBJ_D3/SPR_OBJ_Television_D3_$k.png" --script-param semilla=62 --script-param pantalla=6,82,147,10,147,90,6,162 --script-param estatica=$((62 + k * 100))
done
mueble Comedor PuertaEntrada --script-param semilla=63 --script-param radio=3 --script-param poros=1.0
mueble Pasillo ArmarioBulto --script-param semilla=71 --script-param radio=4 --script-param poros=1.2 --script-param ojos=1
mueble Pasillo MesaPasillo --script-param semilla=72 --script-param radio=3 --script-param poros=1.2
mueble Pasillo PuertaLisa --script-param semilla=73 --script-param radio=3 --script-param poros=1.0
mueble Pasillo PuertaNoPasar --script-param semilla=74 --script-param radio=3 --script-param poros=0.8
mueble Pasillo VentanaPasillo --script-param semilla=75 --script-param radio=2 --script-param poros=0.6
mueble Pasillo FotoBoda --script-param semilla=76 --script-param radio=2 --script-param poros=0.5
mueble Pasillo FotoGrupal --script-param semilla=77 --script-param radio=2 --script-param poros=0.5
mueble Pasillo CalendarioOctubre --script-param semilla=79 --script-param radio=2 --script-param poros=0.4
mueble Pasillo AltarPerro --script-param semilla=78 --script-param radio=2 --script-param poros=0.8
mueble Bano LavaboBano --script-param semilla=81 --script-param radio=3 --script-param poros=1.2
mueble Bano TazaBano --script-param semilla=82 --script-param radio=3 --script-param poros=1.2
mueble Bano EspejoBano --script-param semilla=83 --script-param radio=2 --script-param poros=0.6
mueble Bano ToallaManoBano --script-param semilla=85 --script-param radio=2 --script-param poros=0.6
mueble Bano TapeteBano --script-param semilla=84 --script-param radio=2 --script-param poros=0.8
mueble Puertas PuertaBano --script-param semilla=91 --script-param radio=2 --script-param poros=1.0
mueble Puertas PuertaElaki --script-param semilla=92 --script-param radio=2 --script-param poros=1.0
mueble Puertas JambaPuerta --script-param semilla=93 --script-param radio=1 --script-param poros=0.4
mueble Puertas UmbralPuerta --script-param semilla=94 --script-param radio=2 --script-param poros=0.6

# Cuarto de Elaki deforme
mueble Muebles CamaDesordenada --script-param semilla=121 --script-param radio=3 --script-param poros=1.6 --script-param ojos=1 --script-param gotas=0.2
mueble Escritorio Escritorio --script-param semilla=122 --script-param radio=2 --script-param poros=1.2
mueble Escritorio SillaEscritorio --script-param semilla=123 --script-param radio=2 --script-param poros=1
mueble Escritorio Laptop --script-param semilla=124 --script-param radio=1 --script-param poros=0.5
mueble Escritorio Poster --script-param semilla=125 --script-param radio=1 --script-param poros=0.6
for n in LibroParado LibrosApilados Lapicero Hoja; do mueble Escritorio $n --script-param semilla=126 --script-param radio=1 --script-param poros=0.3; done
# La membrana que sella la puerta de la cocina hasta que se abre el refrigerador
derivar mueble "$OBJ/Comedor/SPR_OBJ_PuertaEntrada_R.png" "$OBJ_D3/SPR_OBJ_MembranaPuerta_D3.png" --script-param semilla=131 --script-param claro=0.12 --script-param venas=5 --script-param poros=2.2 --script-param radio=3 --script-param ojos=1 --script-param gotas=0.3

echo "Adornos (se importan desde el constructor)"
gen --script-param modo=ojo --script-param salida="$OBJ_D3/SPR_OBJ_OjoPared_D3.png" --script-param semilla=101
gen --script-param modo=corazon --script-param salida="$OBJ_D3/SPR_OBJ_CorazonDeformado_D3.png" --script-param semilla=102
gen --script-param modo=charco --script-param salida="$OBJ_D3/SPR_OBJ_CharcoSangre_D3.png" --script-param semilla=103
gen --script-param modo=venas --script-param salida="$OBJ_D3/SPR_OBJ_RaicesTecho_D3.png" --script-param semilla=104 --script-param ancho=64 --script-param alto=220 --script-param cantidad=6

gen --script-param modo=boca --script-param salida="$OBJ_D3/SPR_OBJ_BocaPiso_D3.png" --script-param semilla=141
gen --script-param modo=apendice --script-param salida="$OBJ_D3/SPR_OBJ_ApendicePared_D3.png" --script-param semilla=142
gen --script-param modo=manos --script-param salida="$OBJ_D3/SPR_OBJ_ManosCama_D3.png" --script-param semilla=143
gen --script-param modo=ojogigante --script-param salida="$OBJ_D3/SPR_OBJ_OjoGigante_D3.png" --script-param semilla=144
for tipo in pollo pez feto rana; do
  gen --script-param modo=criatura --script-param tipo=$tipo --script-param salida="$OBJ_D3/SPR_ENE_Carne_${tipo}_D3.png" --script-param semilla=145
done

echo "Dibujos agobiantes, más oscuros"
oscuro() {
  local ruta="$1"
  shift
  local origen="$OBJ/$ruta.png"
  local destino="$OBJ/${ruta/_agobiante/_agobiante_oscuro}.png"
  derivar oscurecer "$origen" "$destino" "$@"
}
oscuro Cocina/Refrigerador/refrigerador_agobiante_abierto_frente_recto --script-param semilla=111 --script-param venas=1.2 --script-param poros=0.25
oscuro Pasillo/Fotos/foto_familia_sofa_agobiante_frente_recto --script-param semilla=112 --script-param venas=1 --script-param poros=0.2 --script-param tono=0.6
oscuro Pasillo/Fotos/foto_cara_embarrada_agobiante_frente_recto --script-param semilla=113 --script-param venas=1 --script-param poros=0.2 --script-param tono=0.6
oscuro Escritorio/Calendario/calendario_pared_agobiante_frente_derecha --script-param semilla=114 --script-param venas=0.8 --script-param poros=0.2 --script-param tono=0.6
oscuro General/Ventana/ventana_tapiada_agobiante_frente_izquierda --script-param semilla=115 --script-param venas=1 --script-param poros=0.3
oscuro Bano/Banera/banera_agobiante_cortina_cerrada_frente_derecha --script-param semilla=116 --script-param venas=0.8 --script-param poros=0.2 --script-param tono=0.62
oscuro Bano/Banera/banera_agobiante_animacion_abrir_cortina_frente_derecha_hoja_sprites --script-param semilla=117 --script-param venas=0.6 --script-param poros=0.1 --script-param tono=0.62
oscuro Bano/Toallas/toallas_perchero_agobiante_frente_izquierda --script-param semilla=118 --script-param venas=0.8 --script-param poros=0.3 --script-param tono=0.6
echo "Listo"
