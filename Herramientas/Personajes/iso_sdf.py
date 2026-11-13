"""Isometric pixel-art renderer for CARENCIA characters, based on signed distance fields (SDF).

A character is modelled in metres as SDF primitives, ray-marched with the game's 2:1 projection,
shaded in flat colour ramps and outlined like hand-made pixel art. Faces and small details are
painted afterwards on the returned image.

Scale of the dining room (Docs/MEDIDAS.md): 96 PPU, one 0.5 m cell is 96 x 48 px, 1 m of height is 108 px.
World axes: x points to the screen south-east, y to the south-west, z up.
"""
from dataclasses import dataclass

import numpy as np

PX_PER_M_GROUND = 96.0
PX_PER_M_HEIGHT = 108.0
# Direction from the scene towards the camera: moving along it keeps the projected pixel fixed.
TOWARDS_CAMERA = np.array([1.0, 1.0, 2 * (PX_PER_M_GROUND / 2) / PX_PER_M_HEIGHT])
MAX_MARCH_STEPS = 220
HIT_DISTANCE = 0.0008
NORMAL_EPSILON = 0.003
OUTLINE_DEPTH_GAP = 0.05


@dataclass
class Render:
    """Result of a render: RGBA image plus per-pixel buffers used by later painting passes."""
    rgba: np.ndarray       # (H, W, 4) uint8
    material: np.ndarray   # (H, W) int, -1 where empty
    tone: np.ndarray       # (H, W) int, index into the material ramp
    depth: np.ndarray      # (H, W) float, larger is nearer to the camera
    world: np.ndarray      # (H, W, 3) hit point in metres


def project(point, origin):
    """Screen pixel (u, v) of a world point, with the world origin drawn at `origin`."""
    x, y, z = point
    u = origin[0] + PX_PER_M_GROUND * (x - y)
    v = origin[1] + (PX_PER_M_GROUND / 2) * (x + y) - PX_PER_M_HEIGHT * z
    return u, v


def render(scene, ramps, light, size, origin):
    """Ray-march `scene` into a `size` = (width, height) image and shade it with `ramps` per material."""
    width, height = size
    starts = _ray_starts(width, height, origin)
    hit, depth, points = _march(scene, starts)
    distances, material = scene(points)
    normals = _normals(scene, points)
    tone = _tones(normals, light)
    return _compose(width, height, hit, depth, points, material, tone, ramps)


def _ray_starts(width, height, origin):
    """World point on the floor plane under each pixel centre, pushed towards the camera."""
    u, v = np.meshgrid(np.arange(width) + 0.5 - origin[0], np.arange(height) + 0.5 - origin[1])
    x_minus_y = u.ravel() / PX_PER_M_GROUND
    x_plus_y = v.ravel() / (PX_PER_M_GROUND / 2)
    floor = np.stack([(x_plus_y + x_minus_y) / 2, (x_plus_y - x_minus_y) / 2, np.zeros_like(u.ravel())], axis=1)
    return floor + 2.4 * TOWARDS_CAMERA


def _march(scene, starts):
    direction = -TOWARDS_CAMERA / np.linalg.norm(TOWARDS_CAMERA)
    travelled = np.zeros(len(starts))
    hit = np.zeros(len(starts), dtype=bool)
    active = np.ones(len(starts), dtype=bool)
    max_travel = 2.6 * np.linalg.norm(TOWARDS_CAMERA)
    for _ in range(MAX_MARCH_STEPS):
        if not active.any():
            break
        points = starts[active] + travelled[active, None] * direction
        distances, _ = scene(points)
        indices = np.flatnonzero(active)
        reached = distances < HIT_DISTANCE
        hit[indices[reached]] = True
        travelled[indices] += np.maximum(distances, HIT_DISTANCE) * 0.7
        active[indices[reached]] = False
        active &= travelled < max_travel
    points = starts + travelled[:, None] * direction
    return hit, -travelled, points


def _normals(scene, points):
    gradient = np.zeros_like(points)
    for axis in range(3):
        offset = np.zeros(3)
        offset[axis] = NORMAL_EPSILON
        gradient[:, axis] = scene(points + offset)[0] - scene(points - offset)[0]
    return gradient / (np.linalg.norm(gradient, axis=1, keepdims=True) + 1e-9)


def _tones(normals, light):
    """Half-Lambert light quantised into five tones: 0 highlight ... 4 deep shadow."""
    light = np.asarray(light, dtype=float) / np.linalg.norm(light)
    half_lambert = 0.5 + 0.5 * normals @ light
    return 4 - np.digitize(half_lambert, [0.36, 0.52, 0.72, 0.93])


def _compose(width, height, hit, depth, points, material, tone, ramps):
    material = np.where(hit, material, -1).reshape(height, width)
    tone = tone.reshape(height, width)
    rgba = np.zeros((height, width, 4), dtype=np.uint8)
    for material_id, ramp in ramps.items():
        mask = material == material_id
        rgba[mask, :3] = np.asarray(ramp)[tone[mask]]
        rgba[mask, 3] = 255
    return Render(rgba, material, tone, depth.reshape(height, width), points.reshape(height, width, 3))


def outline(result, ramps, silhouette_color):
    """Silhouette pixels take `silhouette_color`; pixels behind a nearer surface take their darkest tone."""
    occupied = result.material >= 0
    rgba = result.rgba
    for d_row, d_col in ((1, 0), (-1, 0), (0, 1), (0, -1)):
        neighbour_occupied = _shifted(occupied, d_row, d_col, False)
        neighbour_depth = _shifted(result.depth, d_row, d_col, -np.inf)
        on_silhouette = occupied & ~neighbour_occupied
        behind_edge = occupied & neighbour_occupied & (neighbour_depth - result.depth > OUTLINE_DEPTH_GAP)
        rgba[on_silhouette, :3] = silhouette_color
        for material_id, ramp in ramps.items():
            mask = behind_edge & ~on_silhouette & (result.material == material_id)
            rgba[mask, :3] = ramp[-1]
    return result


def _shifted(array, d_row, d_col, fill):
    """Value of each pixel's neighbour at (row + d_row, col + d_col)."""
    out = np.full_like(array, fill)
    rows, cols = array.shape[:2]
    out[max(0, -d_row):rows - max(0, d_row), max(0, -d_col):cols - max(0, d_col)] = \
        array[max(0, d_row):rows - max(0, -d_row), max(0, d_col):cols - max(0, -d_col)]
    return out


# ---------------------------------------------------------------- SDF primitives (points: (N, 3) arrays)

def sphere(points, centre, radius):
    return np.linalg.norm(points - centre, axis=1) - radius


def ellipsoid(points, centre, radii):
    scaled = (points - centre) / radii
    k0 = np.linalg.norm(scaled, axis=1)
    k1 = np.linalg.norm(scaled / radii, axis=1)
    return k0 * (k0 - 1.0) / (k1 + 1e-9)


def capsule(points, start, end, radius_start, radius_end=None):
    """Segment from `start` to `end` whose radius goes linearly from `radius_start` to `radius_end`."""
    radius_end = radius_start if radius_end is None else radius_end
    start, end = np.asarray(start, float), np.asarray(end, float)
    axis = end - start
    along = np.clip((points - start) @ axis / (axis @ axis), 0.0, 1.0)
    closest = start + along[:, None] * axis
    return np.linalg.norm(points - closest, axis=1) - (radius_start + (radius_end - radius_start) * along)


def box(points, centre, half_size, rounding=0.0):
    """Axis-aligned box; each half size may be a scalar or a per-point array (tapered shapes)."""
    half = np.stack(np.broadcast_arrays(*[np.asarray(h, float) for h in half_size]), axis=-1)
    q = np.abs(points - centre) - (half - rounding)
    outside = np.linalg.norm(np.maximum(q, 0.0), axis=1)
    inside = np.minimum(q.max(axis=1), 0.0)
    return outside + inside - rounding


def smooth_union(a, b, k):
    h = np.clip(0.5 + 0.5 * (b - a) / k, 0.0, 1.0)
    return b + (a - b) * h - k * h * (1.0 - h)


def subtract(base, cut):
    return np.maximum(base, -cut)


def rotate_z(points, angle):
    """Points rotated by `angle` radians around the z axis (counter-clockwise seen from above)."""
    cos, sin = np.cos(angle), np.sin(angle)
    x, y = points[..., 0], points[..., 1]
    return np.stack([cos * x - sin * y, sin * x + cos * y, points[..., 2]], axis=-1)


class Scene:
    """Collects one SDF per material and returns the nearest distance and its material id."""

    def __init__(self, points):
        self.points = points
        self.distance = np.full(len(points), np.inf)
        self.material = np.full(len(points), -1)

    def add(self, material_id, distances):
        nearer = distances < self.distance
        self.distance = np.where(nearer, distances, self.distance)
        self.material = np.where(nearer, material_id, self.material)

    def result(self):
        return self.distance, self.material
