"""Sintetiza la música de suspenso del forcejeo bajo la cama (Nivel 3).

Uso (desde la raíz del proyecto): python -I Herramientas/Audio/suspenso.py
Escribe Assets/_Carencia/Audio/Musica/MUS_N3_Suspenso.wav: 24 s en bucle, 44.1 kHz, mono.
Dron grave disonante, un latido que se acelera, cuerdas agudas en trémolo y golpes metálicos.
Todo dura un número entero de ciclos del bucle o se funde al final con el principio, así no se oye el corte.
"""
import math
import random
import struct
import wave
from pathlib import Path

TASA = 44100
SEGUNDOS = 24.0
N = int(TASA * SEGUNDOS)
random.seed(31)

muestras = [0.0] * N


def seno(f, t):
    return math.sin(2 * math.pi * f * t)


# Dron: la grave con una segunda menor encima, que respira lento
for i in range(N):
    t = i / TASA
    respira = 0.6 + 0.4 * math.sin(2 * math.pi * t / SEGUNDOS * 2)
    v = 0.0
    v += 0.32 * seno(55.0, t) + 0.22 * seno(55.0 * 1.003, t)
    v += 0.16 * seno(58.27, t) * (0.5 + 0.5 * math.sin(2 * math.pi * t / SEGUNDOS * 3))
    v += 0.08 * seno(110.0 * 1.5, t)
    muestras[i] += v * respira * 0.55

# Latido: dos golpes graves (lub-dub) que se aceleran a lo largo del bucle y vuelven a empezar despacio
t = 0.4
while t < SEGUNDOS - 0.6:
    avance = t / SEGUNDOS
    periodo = 1.05 - 0.45 * avance
    for desfase, fuerza in ((0.0, 1.0), (0.22, 0.7)):
        inicio = int((t + desfase) * TASA)
        for k in range(int(0.18 * TASA)):
            if inicio + k >= N:
                break
            s = k / TASA
            env = math.exp(-s * 26)
            f = 48 + 30 * math.exp(-s * 30)
            muestras[inicio + k] += 0.75 * fuerza * env * math.sin(2 * math.pi * f * s)
    t += periodo

# Cuerdas agudas en trémolo, un cluster que entra y sale
for i in range(N):
    t = i / TASA
    sube = max(0.0, math.sin(2 * math.pi * t / SEGUNDOS)) ** 2
    tremolo = 0.5 + 0.5 * math.sin(2 * math.pi * 11 * t)
    v = seno(880.0, t) + seno(932.33, t) * 0.8 + seno(1244.5, t) * 0.4
    muestras[i] += 0.035 * v * tremolo * sube

# Golpes metálicos sueltos, como algo que raspa bajo la cama
for _ in range(9):
    inicio = int(random.uniform(1, SEGUNDOS - 1.5) * TASA)
    base = random.uniform(300, 700)
    for k in range(int(1.2 * TASA)):
        s = k / TASA
        env = math.exp(-s * 4.5)
        v = sum(math.sin(2 * math.pi * base * r * s) / (j + 1) for j, r in enumerate((1.0, 2.76, 5.40, 8.93)))
        muestras[inicio + k] += 0.06 * env * v

# Fundido del final con el principio para que el bucle no tenga corte
cruce = int(0.6 * TASA)
for k in range(cruce):
    a = k / cruce
    muestras[k] = muestras[k] * a + muestras[N - cruce + k] * (1 - a)
muestras = muestras[: N - cruce]

pico = max(abs(m) for m in muestras)
destino = Path("Assets/_Carencia/Audio/Musica/MUS_N3_Suspenso.wav")
with wave.open(str(destino), "wb") as w:
    w.setnchannels(1)
    w.setsampwidth(2)
    w.setframerate(TASA)
    w.writeframes(b"".join(struct.pack("<h", int(m / pico * 0.85 * 32767)) for m in muestras))
print("Escrito", destino, round(len(muestras) / TASA, 2), "s")
