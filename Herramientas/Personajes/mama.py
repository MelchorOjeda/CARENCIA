"""Mother of CARENCIA: average height (1.60 m), brown skin, wavy black hair, dark floral dress.

Two poses facing south-east like the dining room sprites, both with a subtle blink: seated on the
carved chair and standing. The upper body is modelled once, in the seated pose, and lifted for standing.
Usage: python -I Herramientas/Personajes/mama.py <output folder> [<Unity Assets/_Carencia folder>]
"""
import sys

import numpy as np

from cuerpo import SIDES, arm, shin, shoes
from iso_sdf import Scene, box, capsule, ellipsoid, project, rotate_z, smooth_union, subtract
from personaje import Animation, export
from silla_tallada import WOOD_RAMP, chair_distance
from unity import character_target

SKIN, HAIR, DRESS, SHOES, WOOD = range(5)
RAMPS = {
    SKIN: [(214, 160, 120), (186, 128, 92), (156, 100, 70), (122, 74, 52), (84, 48, 36)],
    HAIR: [(72, 64, 70), (46, 40, 46), (31, 27, 32), (21, 18, 22), (12, 10, 13)],
    DRESS: [(104, 132, 148), (78, 104, 122), (58, 82, 100), (42, 60, 76), (28, 40, 54)],
    SHOES: [(96, 70, 60), (72, 50, 44), (54, 37, 33), (38, 26, 24), (24, 16, 15)],
    WOOD: WOOD_RAMP,
}
SILHOUETTE = (26, 16, 20)
LIGHT = (0.35, 0.6, 0.9)

HEAD = np.array([-0.075, 0.0, 1.17])
# The head turns towards the camera so both eyes read, like the other dining room characters.
HEAD_TURN = np.radians(28)
LEG_Y = 0.08
HEM_HEIGHT = 0.28
# Seated knees and ankles as (x, z), shared by the skin and the dress so the skirt wraps the same knees.
KNEE = (0.31, 0.52)
ANKLE = (0.33, 0.07)
# Standing: the seated upper body moves forward over the feet and up by the length of the thighs.
STANDING_LIFT = np.array([0.075, 0.0, 0.29])
WAIST_HEIGHT = 0.95

BLINK = ("abiertos", "entrecerrados", "cerrados", "entrecerrados")
BLINK_DURATIONS_MS = (2600, 60, 90, 60)


SEATED_ORIGIN = (54, 163)
STANDING_ORIGIN = (36, 184)


def main(output_folder, unity_root=None):
    common = dict(subject="mama", orientation="frente_derecha", action="parpadear", character_layer="mama",
                  frames=BLINK, durations_ms=BLINK_DURATIONS_MS, ramps=RAMPS, light=LIGHT, silhouette=SILHOUETTE, paint_surface=paint_surface)
    export(Animation(pose="sentada", size=(112, 190), origin=SEATED_ORIGIN,
                     unity=character_target(unity_root, "Padres", "SPR_PJ_Madre_ParpadearSentada_SE_R",
                                            "ANIM_Madre_ParpadearSentada_SE"),
                     scene_for=lambda eyes: seated_scene, chair_material=WOOD,
                     paint_details=lambda rgba, eyes: paint_face(rgba, eyes, np.zeros(3), SEATED_ORIGIN),
                     **common), output_folder)
    export(Animation(pose="parada", size=(72, 196), origin=STANDING_ORIGIN,
                     unity=character_target(unity_root, "Padres", "SPR_PJ_Madre_Parpadear_SE_R",
                                            "ANIM_Madre_Parpadear_SE"),
                     scene_for=lambda eyes: standing_scene,
                     paint_details=lambda rgba, eyes: paint_face(rgba, eyes, STANDING_LIFT, STANDING_ORIGIN),
                     **common), output_folder)


def paint_surface(result, frame):
    paint_dress_flowers(result)
    paint_hem_shadow(result)


def seated_scene(points):
    scene = Scene(points)
    scene.add(WOOD, chair_distance(points))
    scene.add(SKIN, np.minimum(upper_skin(points), seated_limbs(points)))
    scene.add(DRESS, smooth_union(bodice_and_sleeves(points), seated_skirt(points), 0.05))
    scene.add(SHOES, shoes(points, ANKLE[0], LEG_Y))
    scene.add(HAIR, hair(points))
    return scene.result()


def standing_scene(points):
    upper = points - STANDING_LIFT
    scene = Scene(points)
    scene.add(SKIN, np.minimum(upper_skin(upper), standing_limbs(points)))
    scene.add(DRESS, smooth_union(bodice_and_sleeves(upper), a_line_skirt(points), 0.04))
    scene.add(SHOES, shoes(points, 0.005, LEG_Y))
    scene.add(HAIR, hair(upper))
    return scene.result()


# ---------------------------------------------------------------- upper body, modelled in the seated pose

def upper_skin(points):
    neck = capsule(points, (-0.085, 0, 0.94), HEAD + (-0.005, 0, -0.08), 0.042)
    chest = ellipsoid(points, (-0.08, 0, 0.88), (0.105, 0.16, 0.12))
    return np.minimum(_head(head_local(points)), smooth_union(neck, chest, 0.03))


def head_local(points):
    """Points in the head frame: origin at the head centre, face looking along +x."""
    return rotate_z(points - HEAD, -HEAD_TURN)


def head_to_world(local_point):
    return HEAD + rotate_z(np.asarray(local_point, float), HEAD_TURN)


def _head(local):
    skull = ellipsoid(local, (0, 0, 0), (0.098, 0.09, 0.118))
    jaw = ellipsoid(local, (0.024, 0, -0.058), (0.074, 0.068, 0.068))
    nose = capsule(local, (0.092, 0, 0.0), (0.104, 0, -0.028), 0.008, 0.012)
    return smooth_union(smooth_union(skull, jaw, 0.03), nose, 0.01)


def bodice_and_sleeves(points):
    neckline = 0.958 + 2.2 * points[:, 1] ** 2 + 0.05 * (points[:, 0] < -0.1)
    bodice = smooth_union(ellipsoid(points, (-0.08, 0, 0.88), (0.115, 0.17, 0.13)),
                          ellipsoid(points, (-0.075, 0, 0.66), (0.1, 0.145, 0.13)), 0.05)
    bodice = np.maximum(bodice, points[:, 2] - neckline)
    sleeves = [capsule(points, (-0.09, 0.172 * side, 0.95), (-0.06, 0.188 * side, 0.83), 0.056, 0.05)
               for side in SIDES]
    return np.minimum.reduce([bodice] + sleeves)


def hair(points):
    local = head_local(points)
    cap = ellipsoid(local, (-0.018, 0, 0.02), (0.108, 0.104, 0.126))
    parting = box(local, (0.04, 0, 0.16), (0.09, 0.006, 0.012), 0.004)
    face_window = box(local, (0.08, 0, -0.05), (0.06, 0.078, 0.11), 0.03)
    bangs = np.minimum.reduce([capsule(local, (0.1, 0.014 * side, 0.098), (0.07, 0.09 * side, 0.01), 0.016, 0.022)
                               for side in SIDES])
    hanging = np.minimum.reduce([
        box(points, (-0.15, 0, 0.98), (0.055, 0.14, 0.2), 0.045),
        capsule(local, (-0.02, 0.1, -0.02), (0.0, 0.15, -0.4), 0.04, 0.042),
        capsule(local, (-0.02, -0.1, -0.02), (0.0, -0.16, -0.38), 0.04, 0.04),
        capsule(local, (-0.08, 0.06, 0.0), (-0.11, 0.1, -0.42), 0.05, 0.045),
    ])
    hanging = hanging + _waves(points)
    head_hair = smooth_union(subtract(subtract(cap, face_window), parting), bangs, 0.02)
    return smooth_union(head_hair, hanging, 0.04)


def _waves(points):
    x, y, z = points[:, 0], points[:, 1], points[:, 2]
    return 0.012 * np.sin(42 * z + 18 * y) + 0.006 * np.sin(29 * z - 36 * x)


# ---------------------------------------------------------------- seated pose

def seated_limbs(points):
    parts = []
    for side in SIDES:
        parts.append(arm(points, shoulder=(-0.09, 0.172 * side, 0.95), elbow=(-0.035, 0.2 * side, 0.73),
                         wrist=(0.14, 0.055 * side, 0.635), hand=(0.185, 0.03 * side, 0.625)))
        parts.append(shin(points, KNEE, ANKLE, LEG_Y * side))
    return np.minimum.reduce(parts)


def seated_skirt(points):
    hips = ellipsoid(points, (-0.07, 0, 0.55), (0.14, 0.18, 0.1))
    skirt = smooth_union(_skirt_over_thighs(points), hips, 0.05)
    return smooth_union(skirt, _skirt_over_shins(points), 0.03)


def _skirt_over_thighs(points):
    thighs = [capsule(points, (-0.04, LEG_Y * side, 0.545), (KNEE[0], LEG_Y * side, 0.535), 0.082, 0.07)
              for side in SIDES]
    return smooth_union(thighs[0], thighs[1], 0.06)


def _skirt_over_shins(points):
    """Fabric falls from each knee as a loose tube, sags between the legs and ends in a soft hem."""
    tubes = [capsule(points, (KNEE[0] + 0.005, LEG_Y * side, KNEE[1] + 0.01),
                     (KNEE[0] + 0.02, LEG_Y * 1.25 * side, HEM_HEIGHT - 0.02), 0.074, 0.07)
             for side in SIDES]
    fabric = smooth_union(tubes[0], tubes[1], 0.045)
    return np.maximum(fabric, _hem(points) - points[:, 2])


def _hem(points):
    return HEM_HEIGHT + 0.012 * np.sin(55 * points[:, 1] + 1.3)


# ---------------------------------------------------------------- standing pose

def standing_limbs(points):
    parts = []
    for side in SIDES:
        parts.append(arm(points, shoulder=(-0.015, 0.172 * side, 1.24), elbow=(0.0, 0.205 * side, 1.0),
                         wrist=(0.105, 0.075 * side, 0.865), hand=(0.145, 0.035 * side, 0.845)))
        parts.append(shin(points, (0.0, 0.5), (0.005, 0.07), 0.07 * side))
    return np.minimum.reduce(parts)


def a_line_skirt(points):
    """Skirt that widens from the waist to the hem, with soft vertical folds that grow towards the hem."""
    x, y, z = points[:, 0], points[:, 1], points[:, 2]
    flare = np.clip((WAIST_HEIGHT - z) / (WAIST_HEIGHT - HEM_HEIGHT), 0.0, 1.0)
    folds = 1.0 + 0.03 * flare * np.sin(6 * np.arctan2(y, x))
    radius_x = (0.115 + 0.075 * flare) * folds
    radius_y = (0.15 + 0.07 * flare) * folds
    radial = np.hypot((x - 0.012 * flare) / radius_x, y / radius_y)
    side_wall = (radial - 1.0) * np.minimum(radius_x, radius_y)
    return np.maximum.reduce([side_wall, _hem(points) - z, z - WAIST_HEIGHT - 0.04])


# ---------------------------------------------------------------- painted details

FLOWERS = [((214, 168, 80), (150, 112, 56)), ((196, 112, 112), (134, 72, 78)), ((226, 214, 190), (150, 142, 132))]
FLOWER_SPACING = 0.04
FLOWER_RADIUS = 0.0105
FLOWER_DENSITY = 0.45


def paint_dress_flowers(result):
    """Small flowers scattered in world space, so the print stays put on the body between frames."""
    is_dress = result.material == DRESS
    world = result.world[is_dress]
    cells = np.floor(world / FLOWER_SPACING)
    seed = _hash(cells)
    jitter = np.stack([_hash(cells + k) for k in (1, 2, 3)], axis=-1) * 0.6 + 0.2
    centres = (cells + jitter) * FLOWER_SPACING
    near_centre = (np.linalg.norm(world - centres, axis=1) < FLOWER_RADIUS) & (_hash(cells + 7) < FLOWER_DENSITY)
    tones = result.tone[is_dress]
    colours = result.rgba[is_dress]
    for index, (lit, shaded) in enumerate(FLOWERS):
        chosen = near_centre & (np.floor(seed * len(FLOWERS)) == index)
        colours[chosen & (tones <= 2), :3] = lit
        colours[chosen & (tones > 2), :3] = shaded
    result.rgba[is_dress] = colours


def paint_hem_shadow(result):
    """The skirt shades the top of the shins, so the legs read as coming out from under it."""
    z = result.world[..., 2]
    under_hem = (result.material == SKIN) & (z > HEM_HEIGHT - 0.045) & (z < HEM_HEIGHT + 0.01)
    darker = np.minimum(result.tone[under_hem] + 1, 4)
    result.rgba[under_hem, :3] = np.asarray(RAMPS[SKIN])[darker]


def _hash(cells):
    """Deterministic pseudo-random value in [0, 1) per integer cell."""
    value = np.sin(cells @ np.array([127.1, 311.7, 74.7])) * 43758.5453
    return value - np.floor(value)


LASH = (30, 18, 18)
IRIS = (58, 34, 26)
EYE_WHITE = (214, 196, 182)
LID = RAMPS[SKIN][3]
LIPS = (150, 78, 70)
LIPS_LINE = (104, 52, 48)
BROW = RAMPS[HAIR][3]
BLUSH = (178, 104, 86)


def paint_face(rgba, eyes, upper_body_lift, origin):
    def pixel(local_point):
        u, v = project(head_to_world(local_point) + upper_body_lift, origin)
        return int(round(u)), int(round(v))

    _paint_eye(rgba, pixel((0.086, 0.044, 0.012)), width=3, eyes=eyes, white_on_left=True)
    _paint_eye(rgba, pixel((0.09, -0.022, 0.012)), width=2, eyes=eyes, white_on_left=False)
    _paint_brow(rgba, pixel((0.088, 0.046, 0.042)), width=3)
    _paint_brow(rgba, pixel((0.092, -0.022, 0.042)), width=2)
    _paint_mouth(rgba, pixel((0.09, 0.006, -0.07)))
    _put(rgba, pixel((0.074, 0.058, -0.03)), BLUSH)


def _paint_eye(rgba, anchor, width, eyes, white_on_left):
    """Eye drawn from its top-left pixel: a lash row over two iris rows; the lid lowers as it closes."""
    x, y = anchor
    columns = range(x, x + width)
    outer = x - 1 if white_on_left else x + width
    if eyes == "abiertos":
        _paint_rows(rgba, columns, y, [LASH, IRIS, IRIS])
        _put(rgba, (outer, y), LASH)
        _put(rgba, (x if white_on_left else x + width - 1, y + 1), EYE_WHITE)
    elif eyes == "entrecerrados":
        _paint_rows(rgba, columns, y, [LID, LASH, IRIS])
        _put(rgba, (outer, y + 1), LASH)
    else:
        _paint_rows(rgba, columns, y, [LID, LID, LASH])
        _put(rgba, (outer, y + 1), LASH)


def _paint_rows(rgba, columns, top, colours):
    for row, colour in enumerate(colours):
        for column in columns:
            _put(rgba, (column, top + row), colour)


def _paint_brow(rgba, anchor, width):
    x, y = anchor
    for column in range(x, x + width):
        _put(rgba, (column, y - 1 if column == x + 1 else y), BROW)


def _paint_mouth(rgba, anchor):
    x, y = anchor
    _put(rgba, (x - 1, y), LIPS_LINE)
    _put(rgba, (x, y), LIPS_LINE)
    _put(rgba, (x, y + 1), LIPS)
    _put(rgba, (x - 1, y + 1), LIPS)


def _put(rgba, pixel, colour):
    x, y = pixel
    rgba[y, x, :3] = colour
    rgba[y, x, 3] = 255


if __name__ == "__main__":
    main(*sys.argv[1:3])
