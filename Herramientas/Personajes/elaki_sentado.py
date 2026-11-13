"""Elaki sitting down on the carved chair and resting there, built from his own in-game sprites.

Everything comes from his idle animation (SPR_PJ_Elaki_Espera_<dir>_R) except the thighs:
  - head, torso and arms are cut at the hem of the tee (the hands below it are kept);
  - shins, socks and trainers are cut at knee height and stay planted on the floor;
  - the first frame of sitting down is his sprite untouched;
  - only the thighs are rendered, flat and in his trouser colours, because they turn 90 degrees to sit
    and his sprites have no such view. Their knees start where his legs are in the sprite.
Four diagonal views.
Usage: python -I Herramientas/Personajes/elaki_sentado.py <output folder> [<Unity Assets/_Carencia folder>]
"""
import sys
from pathlib import Path

import numpy as np
from PIL import Image

from cuerpo import SIDES, facing
from iso_sdf import PX_PER_M_GROUND, PX_PER_M_HEIGHT, Scene, capsule, ellipsoid, outline, project, render, rotate_z, \
    smooth_union
from personaje import sprite_name, write_sprites
from silla_tallada import WOOD_RAMP, chair_distance
from unity import character_target

PROJECT = Path(__file__).resolve().parents[2]
ELAKI_SPRITES = PROJECT / "Assets" / "_Carencia" / "Sprites" / "Personajes" / "Elaki"
SPRITE_FRAME = 315
# Column of his sprite frames under the middle of his body.
SPRITE_CENTRE_X = 157.5
IDLE_FRAME_COUNT = 4
IDLE_DURATION_MS = 120   # ANIM_Elaki_Espera_*.anim keys every 0.12 s

# Orientation name -> (direction of his sprite sheets, angle that turns the chair's +x onto it).
VIEWS = {
    "frente_derecha": ("SE", 0.0),
    "frente_izquierda": ("SO", np.radians(90)),
    "espalda_derecha": ("NE", np.radians(-90)),
    "espalda_izquierda": ("NO", np.radians(180)),
}

SHIRT_COLOURS = [(230, 230, 228), (180, 179, 181)]
SKIN_COLOURS = [(160, 127, 106), (159, 126, 105), (134, 106, 88), (123, 92, 77), (96, 70, 57), (61, 42, 34),
                (32, 25, 20)]
THIGHS, WOOD = range(2)
# His trousers are shaded flat: one lit colour, one shadow colour and a dark edge.
RAMPS = {
    THIGHS: [(34, 35, 30), (34, 35, 30), (26, 31, 24), (20, 21, 18), (20, 21, 18)],
    WOOD: WOOD_RAMP,
}
SILHOUETTE = (15, 18, 15)
LIGHT = (0.35, 0.6, 0.9)

# Chair frame, metres: x forward, z up. His feet stand here, in front of the seat.
FEET_X = 0.37
KNEE_HEIGHT = 0.36
HEM_ABOVE_HIP = 0.04
SEATED_HIP = (0.0, 0.51)
THIGH_RADII = (0.078, 0.07)
# The torso leans forward while he lowers himself, up to this many pixels at the top of the head.
MAX_LEAN_PX = 6

SIT_STEPS = (0.0, 0.2, 0.4, 0.6, 0.8, 1.0)
SIT_DURATIONS_MS = (300, 110, 110, 110, 110, 250)
WORK_SIZE = (240, 300)
WORK_ORIGIN = (120, 250)
MARGIN = 3


def main(output_folder, unity_root=None):
    for orientation, (direction, angle) in VIEWS.items():
        poses = [SpritePose(frame, angle) for frame in load_idle_frames(direction)]
        sitting = [compose(poses[0], angle, orientation, step) for step in SIT_STEPS]
        resting = [compose(pose, angle, orientation, 1.0) for pose in poses]
        (sitting, resting), pivot = crop_to_content(sitting, resting)
        write_sprites(resting, output_folder, still=sprite_name("elaki", "sentado", orientation),
                      animated=sprite_name("elaki", "sentado", orientation, "esperar"),
                      durations_ms=(IDLE_DURATION_MS,) * IDLE_FRAME_COUNT, tag="esperar", pivot_px=pivot,
                      unity=character_target(unity_root, "Elaki", f"SPR_PJ_Elaki_EsperaSentado_{direction}_R",
                                             f"ANIM_Elaki_EsperaSentado_{direction}", binding_path="Figura"))
        write_sprites(sitting, output_folder, still=None,
                      animated=sprite_name("elaki", None, orientation, "sentarse"),
                      durations_ms=SIT_DURATIONS_MS, tag="sentarse", pivot_px=pivot,
                      unity=character_target(unity_root, "Elaki", f"SPR_PJ_Elaki_Sentarse_{direction}_R",
                                             f"ANIM_Elaki_Sentarse_{direction}", binding_path="Figura",
                                             loop=False))


def load_idle_frames(direction):
    sheet = np.array(Image.open(ELAKI_SPRITES / f"SPR_PJ_Elaki_Espera_{direction}_R.png").convert("RGBA"))
    return [sheet[:, index * SPRITE_FRAME:(index + 1) * SPRITE_FRAME] for index in range(IDLE_FRAME_COUNT)]


# ---------------------------------------------------------------- his sprite, split in pieces

class SpritePose:
    """One frame of his idle animation, split into the pieces that are reused when he sits."""

    def __init__(self, frame, angle):
        opaque = frame[..., 3] > 0
        rows = np.arange(frame.shape[0])[:, None]
        self.whole = frame
        self.sole_row = int(np.flatnonzero(opaque.any(axis=1)).max())
        self.hem_row = int(np.flatnonzero((opaque & _has_colour(frame, SHIRT_COLOURS)).any(axis=1)).max())
        self.knee_row = self.sole_row - int(round(KNEE_HEIGHT * PX_PER_M_HEIGHT))
        is_skin = opaque & _has_colour(frame, SKIN_COLOURS)
        self.upper_body = _keep(frame, opaque & ((rows <= self.hem_row) | is_skin))
        self.lower_legs = _keep(frame, opaque & (rows >= self.knee_row) & ~is_skin)
        self.hem_height = (self.sole_row - self.hem_row) / PX_PER_M_HEIGHT
        self.knees = [self._knee_in_chair_frame(column, angle) for column in self._knee_columns()]

    def _knee_columns(self):
        """Middle column of each leg at knee height; legs drawn together are split in two halves."""
        columns = np.flatnonzero(self.lower_legs[self.knee_row, :, 3] > 0)
        runs = np.split(columns, np.flatnonzero(np.diff(columns) > 1) + 1)
        if len(runs) >= 2:
            return [runs[0].mean(), runs[-1].mean()]
        middle = (columns[0] + columns[-1]) / 2
        return [(columns[0] + middle) / 2, (middle + columns[-1]) / 2]

    def _knee_in_chair_frame(self, column, angle):
        """World point at knee height that projects onto `column` of the knee row, in the chair frame."""
        offset_u = column - SPRITE_CENTRE_X
        # At knee height the row offset cancels the height, so the knee only moves sideways on screen.
        sideways = offset_u / (2 * PX_PER_M_GROUND)
        world = np.array([FEET_X * np.cos(angle) + sideways, FEET_X * np.sin(angle) - sideways, KNEE_HEIGHT])
        return rotate_z(world, -angle)


def _has_colour(frame, colours):
    rgb = frame[..., :3].astype(int)
    return np.any([np.all(rgb == colour, axis=-1) for colour in colours], axis=0)


def _keep(frame, mask):
    return np.where(mask[..., None], frame, 0).astype(np.uint8)


# ---------------------------------------------------------------- one frame

def compose(pose, angle, orientation, step):
    """Chair and Elaki for one moment of sitting down: `step` 0 is standing, 1 is seated."""
    hip = _hip_at(pose, step)
    result = render(facing(lambda points: thighs_and_chair(points, hip, pose.knees, step > 0), angle),
                    RAMPS, LIGHT, WORK_SIZE, WORK_ORIGIN)
    outline(result, RAMPS, SILHOUETTE)
    is_chair = (result.material == WOOD)[..., None]
    chair = np.where(is_chair, result.rgba, 0).astype(np.uint8)
    feet = project(rotate_z(np.array([FEET_X, 0.0, 0.0]), angle), WORK_ORIGIN)
    if step == 0:
        elaki = _paste(pose.whole, pose.sole_row, feet, lean_px=0, hem_row=pose.hem_row)
    else:
        thighs = np.where(is_chair, 0, result.rgba).astype(np.uint8)
        hem = project(rotate_z(np.array([hip[0], 0.0, hip[1] + HEM_ABOVE_HIP]), angle), WORK_ORIGIN)
        lower_legs = _paste(pose.lower_legs, pose.sole_row, feet, lean_px=0, hem_row=pose.hem_row)
        upper_body = _paste(pose.upper_body, pose.hem_row, hem, _lean_px(step, angle), pose.hem_row)
        elaki = _stack(_stack(lower_legs, thighs), upper_body)
    # From behind, the backrest stands between the camera and his back.
    if orientation.startswith("espalda"):
        return {"elaki": elaki, "silla": chair}
    return {"silla": chair, "elaki": elaki}


def _hip_at(pose, step):
    """Hip (x, z) in the chair frame: from over his feet, back and down onto the seat along an eased path."""
    standing = (FEET_X, pose.hem_height - HEM_ABOVE_HIP)
    eased = step * step * (3 - 2 * step)
    return (standing[0] + (SEATED_HIP[0] - standing[0]) * eased,
            standing[1] + (SEATED_HIP[1] - standing[1]) * eased)


def _lean_px(step, angle):
    """Horizontal shift of the head while sitting down, towards where he faces on screen."""
    forward_u = np.cos(angle) - np.sin(angle)
    return np.sin(np.pi * step) * MAX_LEAN_PX * np.sign(forward_u)


def _paste(pixels, anchor_row, anchor, lean_px, hem_row):
    """Draws sprite `pixels` so (SPRITE_CENTRE_X, anchor_row) lands on `anchor`; rows above the hem lean."""
    u, v = anchor
    canvas = np.zeros((WORK_SIZE[1], WORK_SIZE[0], 4), dtype=np.uint8)
    rows_with_pixels = np.flatnonzero((pixels[..., 3] > 0).any(axis=1))
    if len(rows_with_pixels) == 0:
        return canvas
    top = int(rows_with_pixels.min())
    for row in rows_with_pixels:
        lean = int(round(lean_px * max(hem_row - row, 0) / max(hem_row - top, 1)))
        _paste_row(canvas, pixels[row], int(round(v)) + row - anchor_row, int(round(u - SPRITE_CENTRE_X)) + lean)
    return canvas


def _paste_row(canvas, row_pixels, target_row, offset):
    if not 0 <= target_row < canvas.shape[0]:
        return
    columns = np.flatnonzero(row_pixels[:, 3] > 0)
    targets = columns + offset
    inside = (targets >= 0) & (targets < canvas.shape[1])
    canvas[target_row, targets[inside]] = row_pixels[columns[inside]]


def _stack(bottom, top):
    covered = top[..., 3:4] > 0
    return np.where(covered, top, bottom).astype(np.uint8)


# ---------------------------------------------------------------- thighs and chair

def thighs_and_chair(points, hip, knees, with_thighs):
    scene = Scene(points)
    scene.add(WOOD, chair_distance(points))
    if with_thighs:
        scene.add(THIGHS, thighs(points, hip, knees))
    return scene.result()


def thighs(points, hip, knees):
    """Both thighs, from the hips to the knees of his sprite, joined by the seat of the trousers."""
    hip_x, hip_z = hip
    legs = [capsule(points, (hip_x, knee[1], hip_z), knee + (0.0, 0.0, 0.02), *THIGH_RADII) for knee in knees]
    seat = ellipsoid(points, (hip_x - 0.01, 0, hip_z), (0.11, 0.15, 0.08))
    return smooth_union(np.minimum.reduce(legs), seat, 0.04)


# ---------------------------------------------------------------- framing

def crop_to_content(*animations):
    """Crops every frame of every animation to the same box around all their pixels, plus a margin.

    Returns the cropped animations and where the chair's floor centre (the pivot) ends up in them.
    """
    boxes = [Image.fromarray(layer).getbbox()
             for animation in animations for frame in animation for layer in frame.values()]
    boxes = [box for box in boxes if box]
    left = max(min(box[0] for box in boxes) - MARGIN, 0)
    upper = max(min(box[1] for box in boxes) - MARGIN, 0)
    right = min(max(box[2] for box in boxes) + MARGIN, WORK_SIZE[0])
    lower = min(max(box[3] for box in boxes) + MARGIN, WORK_SIZE[1])
    cropped = tuple([{name: layer[upper:lower, left:right] for name, layer in frame.items()} for frame in animation]
                    for animation in animations)
    return cropped, (WORK_ORIGIN[0] - left, WORK_ORIGIN[1] - upper)


if __name__ == "__main__":
    main(*sys.argv[1:3])
