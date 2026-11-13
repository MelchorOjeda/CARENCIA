"""Body parts shared by every character, as SDF in metres (x forward, y to the character's right, z up)."""
import numpy as np

from iso_sdf import capsule, ellipsoid, rotate_z, smooth_union, sphere

SIDES = (-1, 1)


def arm(points, shoulder, elbow, wrist, hand, thickness=1.0):
    """Upper arm, forearm and a flat hand; `thickness` scales the radii (1.0 is the mother)."""
    upper = capsule(points, shoulder, elbow, 0.044 * thickness, 0.036 * thickness)
    forearm = capsule(points, elbow, wrist, 0.034 * thickness, 0.026 * thickness)
    palm = ellipsoid(points, hand, (0.05, 0.034, 0.022 * thickness))
    return smooth_union(smooth_union(upper, forearm, 0.015), palm, 0.012)


def shin(points, knee, ankle, y, thickness=1.0):
    """Lower leg from `knee` to `ankle`, both given as (x, z), with a calf bulge."""
    bone = capsule(points, (knee[0], y, knee[1] - 0.02), (ankle[0], y, ankle[1]), 0.044 * thickness, 0.03 * thickness)
    calf = ellipsoid(points, (knee[0] - 0.012, y, (knee[1] + ankle[1]) * 0.5 + 0.05),
                     (0.044 * thickness, 0.042 * thickness, 0.09))
    return smooth_union(bone, calf, 0.03)


def shoes(points, ankle_x, foot_y, length=0.1, size=1.0):
    """A pair of closed shoes with the heel under `ankle_x`, pointing forward."""
    return np.minimum.reduce([
        smooth_union(capsule(points, (ankle_x - 0.02, foot_y * side, 0.035 * size),
                             (ankle_x - 0.02 + length, foot_y * side, 0.025 * size), 0.034 * size, 0.03 * size),
                     sphere(points, (ankle_x - 0.01, foot_y * side, 0.06 * size), 0.034 * size), 0.02)
        for side in SIDES])


def facing(scene, angle):
    """The same scene turned by `angle` radians around the vertical axis through the world origin."""
    def turned(points):
        return scene(rotate_z(points, -angle))
    return turned
