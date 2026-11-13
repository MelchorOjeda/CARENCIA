"""Father of CARENCIA: average height (1.75 m), brown skin, short black hair, always behind his newspaper.

Two poses facing south-east like the dining room sprites: seated on the carved chair and standing.
The open newspaper hides his face; the animation spreads it out a little, as if straightening the page.
The upper body (head, torso, arms, newspaper) is modelled once, in the seated pose, and lifted for standing.
Usage: python -I Herramientas/Personajes/papa.py <output folder> [<Unity Assets/_Carencia folder>]
"""
import sys

import numpy as np

from cuerpo import SIDES, arm, shoes
from iso_sdf import Scene, box, capsule, ellipsoid, smooth_union
from personaje import Animation, export
from silla_tallada import WOOD_RAMP, chair_distance
from texto_pixel import paint_text
from unity import character_target

SKIN, HAIR, SHIRT, PANTS, SHOES, PAPER, BELT, WOOD = range(8)
RAMPS = {
    SKIN: [(206, 150, 108), (178, 120, 84), (148, 94, 64), (116, 70, 48), (80, 46, 34)],
    HAIR: [(64, 58, 64), (42, 37, 42), (29, 25, 30), (20, 17, 21), (12, 10, 13)],
    SHIRT: [(206, 160, 158), (182, 134, 134), (156, 108, 112), (124, 82, 90), (86, 56, 64)],
    PANTS: [(112, 86, 76), (90, 66, 58), (70, 50, 44), (52, 36, 32), (34, 23, 21)],
    SHOES: [(84, 64, 58), (62, 46, 42), (46, 33, 31), (32, 23, 22), (20, 14, 14)],
    PAPER: [(242, 234, 214), (228, 216, 190), (208, 192, 164), (176, 158, 132), (128, 112, 92)],
    BELT: [(110, 76, 52), (88, 58, 40), (66, 42, 30), (48, 30, 22), (30, 19, 14)],
    WOOD: WOOD_RAMP,
}
SILHOUETTE = (26, 16, 20)
LIGHT = (0.35, 0.6, 0.9)

HEAD = np.array([-0.06, 0.0, 1.24])
HIP_Y = 0.1
# Seated knees and ankles as (x, z).
KNEE = (0.33, 0.54)
ANKLE = (0.35, 0.08)
# Standing: the seated upper body moves over the feet and up by the length of the thighs.
STANDING_LIFT = np.array([0.06, 0.0, 0.36])

# Open newspaper held in front of the face, its fold (spine) nearest to the camera.
PAPER_SPINE_X = 0.17
PAPER_EDGE_X = 0.1
PAPER_HALF_WIDTH = 0.34
PAPER_CENTRE_Z = 1.19
PAPER_HALF_HEIGHT = 0.22
PAPER_THICKNESS = 0.003

# Spread of the newspaper per frame: 0 is the resting V, 1 is the flattest, slightly raised.
# It opens quickly, holds, and eases back in two steps.
SPREAD = (0.0, 0.45, 1.0, 0.8, 0.4)
SPREAD_DURATIONS_MS = (1800, 90, 160, 110, 90)
SEATED_ORIGIN = (62, 166)
STANDING_ORIGIN = (52, 206)


def main(output_folder, unity_root=None):
    common = dict(subject="papa", orientation="frente_derecha", action="extender_periodico", character_layer="papa",
                  frames=SPREAD, durations_ms=SPREAD_DURATIONS_MS, ramps=RAMPS, light=LIGHT, silhouette=SILHOUETTE)
    export(Animation(pose="sentado", size=(128, 196), origin=SEATED_ORIGIN,
                     unity=character_target(unity_root, "Padres", "SPR_PJ_Padre_PeriodicoSentado_SE_R",
                                            "ANIM_Padre_PeriodicoSentado_SE"),
                     scene_for=lambda spread: lambda points: seated_scene(points, spread),
                     chair_material=WOOD,
                     paint_surface=lambda result, spread: paint_newspaper(result, spread, np.zeros(3)),
                     **common), output_folder)
    export(Animation(pose="parado", size=(112, 222), origin=STANDING_ORIGIN,
                     unity=character_target(unity_root, "Padres", "SPR_PJ_Padre_Periodico_SE_R",
                                            "ANIM_Padre_Periodico_SE"),
                     scene_for=lambda spread: lambda points: standing_scene(points, spread),
                     paint_surface=lambda result, spread: paint_newspaper(result, spread, STANDING_LIFT),
                     **common), output_folder)


def seated_scene(points, spread):
    scene = Scene(points)
    scene.add(WOOD, chair_distance(points))
    _add_upper_body(scene, points, spread)
    scene.add(PANTS, seated_legs(points))
    scene.add(SHOES, shoes(points, ANKLE[0], 0.12, length=0.12, size=1.1))
    return scene.result()


def standing_scene(points, spread):
    scene = Scene(points)
    _add_upper_body(scene, points - STANDING_LIFT, spread)
    scene.add(PANTS, standing_legs(points))
    scene.add(SHOES, shoes(points, 0.0, HIP_Y, length=0.12, size=1.1))
    return scene.result()


def _add_upper_body(scene, points, spread):
    scene.add(SKIN, np.minimum(head_and_neck(points), hands(points, spread)))
    scene.add(HAIR, hair(points))
    scene.add(SHIRT, shirt(points, spread))
    scene.add(BELT, belt(points))
    scene.add(PAPER, newspaper(points, spread))


# ---------------------------------------------------------------- upper body, modelled in the seated pose

def head_and_neck(points):
    skull = ellipsoid(points, HEAD, (0.1, 0.092, 0.12))
    jaw = ellipsoid(points, HEAD + (0.025, 0, -0.06), (0.078, 0.074, 0.07))
    ears = [ellipsoid(points, HEAD + (-0.005, 0.092 * side, -0.01), (0.018, 0.012, 0.03)) for side in SIDES]
    neck = capsule(points, (-0.07, 0, 1.02), HEAD + (-0.01, 0, -0.08), 0.05)
    return np.minimum.reduce([smooth_union(skull, jaw, 0.03), neck] + ears)


def hair(points):
    """Short hair: a cap over the skull that leaves the face and ears free."""
    cap = ellipsoid(points, HEAD + (-0.012, 0, 0.022), (0.106, 0.098, 0.118))
    face = box(points, HEAD + (0.08, 0, -0.06), (0.06, 0.1, 0.09), 0.03)
    nape = HEAD[2] - 0.05 - points[:, 2]
    return np.maximum.reduce([cap, -face, nape])


def shirt(points, spread):
    chest = ellipsoid(points, (-0.07, 0, 0.93), (0.13, 0.2, 0.15))
    belly = ellipsoid(points, (-0.06, 0, 0.7), (0.122, 0.165, 0.14))
    collar = capsule(points, (-0.06, -0.05, 1.03), (-0.06, 0.05, 1.03), 0.055)
    torso = smooth_union(smooth_union(chest, belly, 0.06), collar, 0.02)
    sleeves = []
    for side in SIDES:
        shoulder, elbow, wrist, _ = _arm_joints(side, spread)
        sleeves.append(arm(points, shoulder, elbow, wrist, wrist, thickness=1.35))
    return np.minimum.reduce([torso] + sleeves)


def belt(points):
    return np.maximum(ellipsoid(points, (-0.05, 0, 0.6), (0.14, 0.175, 0.1)), np.abs(points[:, 2] - 0.6) - 0.018)


def hands(points, spread):
    return np.minimum.reduce([ellipsoid(points, _arm_joints(side, spread)[3], (0.03, 0.026, 0.048))
                              for side in SIDES])


def _arm_joints(side, spread):
    """Shoulder, elbow, wrist and hand centre; the hands grip the lower outer corners of the paper."""
    edge_x, half_width, centre_z = _paper_shape(spread)
    shoulder = (-0.08, 0.2 * side, 1.02)
    elbow = (-0.03, 0.25 * side, 0.83)
    wrist = (edge_x - 0.03, (half_width - 0.02) * side, centre_z - 0.12)
    hand = (edge_x + 0.012, (half_width - 0.025) * side, centre_z - 0.1)
    return shoulder, elbow, wrist, hand


def newspaper(points, spread):
    """Two thin pages folded in a V, each one a box turned around the vertical spine."""
    edge_x, half_width, centre_z = _paper_shape(spread)
    pages = []
    for side in SIDES:
        direction = np.array([edge_x - PAPER_SPINE_X, half_width * side])
        length = np.linalg.norm(direction)
        direction /= length
        relative = points[:, :2] - (PAPER_SPINE_X, 0.0)
        along = relative @ direction
        across = relative @ np.array([-direction[1], direction[0]])
        local = np.stack([along - length / 2, across, points[:, 2] - centre_z], axis=1)
        pages.append(box(local, (0, 0, 0), (length / 2, PAPER_THICKNESS, PAPER_HALF_HEIGHT)))
    return np.minimum.reduce(pages)


def _paper_shape(spread):
    """Edge depth, half width and centre height of the open newspaper for a spread in [0, 1]."""
    return (PAPER_EDGE_X + 0.04 * spread, PAPER_HALF_WIDTH + 0.012 * spread, PAPER_CENTRE_Z + 0.01 * spread)


# ---------------------------------------------------------------- legs

def seated_legs(points):
    hips = ellipsoid(points, (-0.06, 0, 0.55), (0.15, 0.2, 0.1))
    legs = [hips]
    for side in SIDES:
        y = HIP_Y * side
        thigh = capsule(points, (-0.05, y, 0.56), (KNEE[0], y * 1.1, KNEE[1]), 0.088, 0.07)
        lower = capsule(points, (KNEE[0], y * 1.1, KNEE[1]), (ANKLE[0], y * 1.2, ANKLE[1]), 0.062, 0.05)
        legs.append(smooth_union(thigh, lower, 0.02))
    return smooth_union(np.minimum.reduce(legs[1:]), hips, 0.05)


def standing_legs(points):
    hips = ellipsoid(points, (0.0, 0, 0.88), (0.125, 0.17, 0.1))
    legs = []
    for side in SIDES:
        y = HIP_Y * side
        thigh = capsule(points, (0.0, y, 0.9), (0.012, y, 0.5), 0.085, 0.064)
        lower = capsule(points, (0.012, y, 0.5), (0.0, y, 0.08), 0.06, 0.05)
        legs.append(smooth_union(thigh, lower, 0.02))
    return smooth_union(np.minimum.reduce(legs), hips, 0.05)


# ---------------------------------------------------------------- newspaper print

INK = (46, 38, 38)
PRINT_GREY = (138, 124, 110)
PHOTO = (104, 96, 92)
PHOTO_LIGHT = (156, 146, 136)
# Paper position mapped to print, close to one screen pixel each: 38 print pixels across a page.
PRINT_PIXEL = (0.0088, 1 / 108)


def paint_newspaper(result, spread, lift):
    """Front page on the right-hand half (screen left), inner pages on the other one, lined with text."""
    is_paper = (result.material == PAPER) & (result.tone <= 3)
    world = result.world[is_paper] - lift
    _, half_width, centre_z = _paper_shape(spread)
    column = np.floor((half_width - np.abs(world[:, 1])) / PRINT_PIXEL[0]).astype(int)
    row = np.floor((centre_z + PAPER_HALF_HEIGHT - world[:, 2]) / PRINT_PIXEL[1]).astype(int)
    front_page = world[:, 1] > 0
    colours = result.rgba[is_paper]
    for mask, layout in ((front_page, _front_page), (~front_page, _inner_page)):
        layout(colours, mask, column, row)
    result.rgba[is_paper] = colours


def _front_page(colours, mask, column, row):
    _paint_mask(colours, mask & paint_text("EL SOL", column - 9, row - 2), INK)
    _paint_mask(colours, mask & paint_text("DE MEXICO", column - 3, row - 9), INK)
    _paint_mask(colours, mask & (row == 15) & (column >= 2) & (column <= 37), INK)
    photo = mask & (row >= 18) & (row <= 29) & (column >= 3) & (column <= 14)
    _paint_mask(colours, photo, PHOTO)
    _paint_mask(colours, photo & ((column - 8) ** 2 + (row - 22) ** 2 <= 5), PHOTO_LIGHT)
    _paint_mask(colours, mask & _text_lines(column, row, first_row=18, columns=((17, 37),)), PRINT_GREY)
    _paint_mask(colours, mask & _text_lines(column, row, first_row=32, columns=((2, 17), (20, 37))), PRINT_GREY)


def _inner_page(colours, mask, column, row):
    _paint_mask(colours, mask & (row >= 3) & (row <= 5) & (column >= 4) & (column <= 34) & (column % 5 != 0), INK)
    _paint_mask(colours, mask & _text_lines(column, row, first_row=9, columns=((2, 17), (20, 37))), PRINT_GREY)


def _text_lines(column, row, first_row, columns):
    """Rows of grey dashes every other print row, with ragged word gaps, inside the given column ranges."""
    in_column = np.zeros_like(column, dtype=bool)
    for start, end in columns:
        in_column |= (column >= start) & (column <= end)
    word_gap = ((column * 7 + row * 3) % 11) == 0
    return in_column & (row >= first_row) & (row <= 46) & ((row - first_row) % 2 == 0) & ~word_gap


def _paint_mask(colours, mask, colour):
    colours[mask, :3] = colour


if __name__ == "__main__":
    main(*sys.argv[1:3])
