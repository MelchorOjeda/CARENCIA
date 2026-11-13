"""Dark carved wooden chair (Referencias-Imagenes/silla_referencia.jpg) as SDF, in local metres.

The seat is centred on the origin, the chair faces +x and its back stands at -x.
Run on its own to export the empty chair:
  python -I Herramientas/Personajes/silla_tallada.py <output folder> [<Unity Assets/_Carencia folder>]
"""
import sys
from pathlib import Path

import numpy as np

from cuerpo import facing
from iso_sdf import Scene, box, capsule, subtract
from personaje import ORIENTATIONS, Animation, export
from unity import UnityTarget

SEAT_HEIGHT = 0.46
HALF_WIDTH = 0.23
LEG = 0.024
POST_X = -HALF_WIDTH + LEG
BACK_TOP = 1.02

WOOD_RAMP = [(122, 84, 60), (96, 64, 44), (74, 48, 32), (54, 34, 23), (34, 21, 15)]


def chair_distance(points):
    seat = box(points, (0, 0, SEAT_HEIGHT - 0.02), (HALF_WIDTH, HALF_WIDTH, 0.022), 0.006)
    apron = box(points, (0, 0, SEAT_HEIGHT - 0.07), (HALF_WIDTH - 0.012, HALF_WIDTH - 0.012, 0.03))
    return np.minimum.reduce([seat, apron, _legs(points), _stretchers(points), _backrest(points)])


def _legs(points):
    front = [box(points, (HALF_WIDTH - LEG, side, SEAT_HEIGHT / 2), (LEG, LEG, SEAT_HEIGHT / 2))
             for side in (-HALF_WIDTH + LEG, HALF_WIDTH - LEG)]
    back = [box(points, (POST_X, side, BACK_TOP / 2), (LEG, LEG, BACK_TOP / 2))
            for side in (-HALF_WIDTH + LEG, HALF_WIDTH - LEG)]
    finials = [capsule(points, (POST_X, side, BACK_TOP), (POST_X, side, BACK_TOP + 0.05), 0.022, 0.004)
               for side in (-HALF_WIDTH + LEG, HALF_WIDTH - LEG)]
    return np.minimum.reduce(front + back + finials)


def _stretchers(points):
    inner = HALF_WIDTH - LEG
    sides = [box(points, (0, side, 0.12), (inner, 0.012, 0.014)) for side in (-inner, inner)]
    front = box(points, (inner, 0, 0.16), (0.012, inner, 0.014))
    return np.minimum.reduce(sides + [front])


def _backrest(points):
    """Gabled top rail, a lower rail and a pierced panel with a cross pattee, like the reference."""
    inner = HALF_WIDTH - 2 * LEG
    crest_peak = BACK_TOP + 0.035 * (1.0 - np.abs(points[:, 1]) / inner)
    top_rail = np.maximum(box(points, (POST_X, 0, 0.95), (0.016, inner, 0.08)), points[:, 2] - crest_peak)
    lower_rail = box(points, (POST_X, 0, 0.58), (0.016, inner, 0.025))
    panel = box(points, (POST_X, 0, 0.75), (0.01, inner, 0.15))
    openings = subtract(box(points, (POST_X, 0, 0.75), (0.03, inner - 0.025, 0.125)), _cross(points))
    return np.minimum.reduce([top_rail, lower_rail, subtract(panel, openings)])


def _cross(points):
    """Cross pattee: arms that widen towards their ends."""
    y, z = points[:, 1], points[:, 2] - 0.76
    vertical = box(points, (POST_X, 0, 0.76), (0.04, 0.026 + 0.18 * np.abs(z), 0.13))
    horizontal = box(points, (POST_X, 0, 0.76), (0.04, 0.12, 0.026 + 0.18 * np.abs(y)))
    return np.minimum(vertical, horizontal)


VIEWS = {"noroeste": np.radians(180), "noreste": np.radians(-90), "sureste": 0.0, "suroeste": np.radians(90)}
# Direction codes of the project sprites (SPR_PJ_Elaki_Espera_SE_R).
DIRECTION_CODES = {"noroeste": "NO", "noreste": "NE", "sureste": "SE", "suroeste": "SO"}


def main(output_folder, unity_root=None):
    """Exports the empty chair facing each diagonal, as single-frame sprites."""
    def chair_scene(points):
        scene = Scene(points)
        scene.add(0, chair_distance(points))
        return scene.result()

    for view, angle in VIEWS.items():
        export(Animation(subject="silla_tallada", pose=None, orientation=ORIENTATIONS[view], action=None,
                         character_layer="silla",
                         size=(112, 168), origin=(56, 142), frames=(None,), durations_ms=(100,),
                         scene_for=lambda frame, angle=angle: facing(chair_scene, angle),
                         ramps={0: WOOD_RAMP}, light=(0.35, 0.6, 0.9), silhouette=(26, 16, 20),
                         unity=_unity_target(unity_root, view)), output_folder)


def _unity_target(unity_root, view):
    if unity_root is None:
        return None
    folder = Path(unity_root) / "Sprites" / "Objetos" / "Comedor"
    return UnityTarget(sprite=folder / f"SPR_OBJ_SillaTallada_{DIRECTION_CODES[view]}_R.png")


if __name__ == "__main__":
    main(*sys.argv[1:3])
