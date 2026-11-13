"""Exports a character (or prop) as transparent sprites named like the rest of the project's assets.

Naming follows Desktop/Videojuego/Assets/Objetos/objeto_banera: <subject>_<state>_<description>_<orientation>.
For mama sitting and blinking, facing the camera to the right, `export` writes:
  mama_normal_sentada_frente_derecha.png / .aseprite                        the still pose, one frame
  mama_normal_animacion_parpadear_sentada_frente_derecha.aseprite           the whole animation
  mama_normal_animacion_parpadear_sentada_frente_derecha_hoja_sprites.png   its frames, left to right
  vista_previa/mama_normal_animacion_parpadear_sentada_frente_derecha.gif   preview of the animation

Every frame is rendered on its own, so an animation may move the geometry (a newspaper, breathing)
or only repaint details (a blink). When the character sits, the chair goes to its own Aseprite layer.
"""
import os
import subprocess
import tempfile
from dataclasses import dataclass
from pathlib import Path
from typing import Callable, Optional

import numpy as np
from PIL import Image

from iso_sdf import outline, render
from unity import UnityTarget, write_unity

ASEPRITE = os.environ.get("ASEPRITE", r"C:\Users\Zuri\Documents\aseprite\build\bin\aseprite.exe")
ASSEMBLE_SCRIPT = Path(__file__).resolve().parent.parent / "Aseprite" / "armar_capas.lua"
STATE = "normal"
STILL_DURATION_MS = 100
# Screen direction the subject faces, named like the other assets (silla_gamer_normal_espalda_derecha).
ORIENTATIONS = {
    "sureste": "frente_derecha",
    "suroeste": "frente_izquierda",
    "noreste": "espalda_derecha",
    "noroeste": "espalda_izquierda",
}


@dataclass(frozen=True)
class Animation:
    subject: str                    # e.g. mama, papa, elaki, silla_tallada
    pose: Optional[str]             # e.g. sentada, parado; None for props
    orientation: str                # a value of ORIENTATIONS, e.g. frente_derecha
    action: Optional[str]           # verb of the animation, e.g. parpadear; None for a still prop
    character_layer: str            # layer name in Aseprite, e.g. mama
    size: tuple                     # (width, height) in pixels
    origin: tuple                   # pixel where the world origin (feet or chair centre on the floor) is drawn
    frames: tuple                   # one value per frame, handed to the callbacks below; the first is the still pose
    durations_ms: tuple
    scene_for: Callable             # frame -> scene(points)
    ramps: dict
    light: tuple
    silhouette: tuple
    chair_material: Optional[int] = None
    paint_surface: Optional[Callable] = None   # (Render, frame), before outlining: prints, shadows
    paint_details: Optional[Callable] = None   # (rgba, frame), after outlining: faces, lettering
    unity: Optional[UnityTarget] = None        # also write the sheet, .meta and clip into the Unity project


def export(animation, folder):
    """Writes the still pose and, when there is an action, the animation of `animation` into `folder`."""
    frames = [render_layers(animation, frame) for frame in animation.frames]
    write_sprites(frames, folder, still=still_name(animation),
                  animated=animation_name(animation) if animation.action else None,
                  durations_ms=animation.durations_ms, tag=animation.action,
                  unity=animation.unity, pivot_px=animation.origin)


def still_name(animation):
    return sprite_name(animation.subject, animation.pose, animation.orientation)


def animation_name(animation):
    return sprite_name(animation.subject, animation.pose, animation.orientation, animation.action)


def sprite_name(subject, pose, orientation, action=None):
    """<subject>_normal[_animacion_<action>][_<pose>]_<orientation>, like the other project assets."""
    parts = [subject, STATE] + (["animacion", action] if action else []) + [pose, orientation]
    return "_".join(part for part in parts if part)


def render_layers(animation, frame):
    """One rendered frame as {layer name: RGBA array}: the chair (when there is one) under the character."""
    result = render(animation.scene_for(frame), animation.ramps, animation.light, animation.size, animation.origin)
    if animation.paint_surface:
        animation.paint_surface(result, frame)
    outline(result, animation.ramps, animation.silhouette)
    is_chair = result.material == animation.chair_material
    layers = {}
    if animation.chair_material is not None:
        layers["silla"] = np.where(is_chair[..., None], result.rgba, 0).astype(np.uint8)
    character = np.where(is_chair[..., None], 0, result.rgba).astype(np.uint8)
    if animation.paint_details:
        animation.paint_details(character, frame)
    layers[animation.character_layer] = character
    return layers


def write_sprites(frames, folder, still, animated, durations_ms, tag, unity=None, pivot_px=None):
    """Writes the sprite named `still` from the first frame and, when `animated` is given, the whole animation.

    `frames` is a list of {layer name: RGBA array}, every frame with the same layers in the same order.
    With `unity`, the frames also go to the Unity project, pivoted on `pivot_px` (feet or chair floor centre).
    """
    folder = Path(folder)
    folder.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory() as layers_folder:
        layers = _save_layers(frames, Path(layers_folder))
        flattened = _composite_frames(layers, len(frames))
        if still:
            _assemble_aseprite(layers, (STILL_DURATION_MS,), "", folder / f"{still}.aseprite")
            flattened[0].save(folder / f"{still}.png")
        if animated:
            _assemble_aseprite(layers, durations_ms, tag, folder / f"{animated}.aseprite")
            _save_sheet(flattened, folder / f"{animated}_hoja_sprites.png")
            _save_preview(flattened, durations_ms, folder / "vista_previa" / f"{animated}.gif")
        if unity:
            write_unity(unity, flattened, durations_ms, pivot_px)


def _save_layers(frames, layers_folder):
    """Saves every layer of every frame and returns {layer name: path pattern with {i}}, bottom layer first."""
    layers = {}
    for index, frame in enumerate(frames):
        for name, rgba in frame.items():
            _save(rgba, layers_folder / f"{name}_{index}.png")
            layers[name] = str(layers_folder / (name + "_{i}.png"))
    return layers


def _save(rgba, path):
    Image.fromarray(np.asarray(rgba, dtype=np.uint8)).save(path)


def _composite_frames(layers, frame_count):
    """Each frame with all its layers flattened, bottom layer first."""
    frames = []
    for index in range(frame_count):
        stacked = None
        for pattern in layers.values():
            image = Image.open(pattern.replace("{i}", str(index))).convert("RGBA")
            stacked = image if stacked is None else Image.alpha_composite(stacked, image)
        frames.append(stacked)
    return frames


def _assemble_aseprite(layers, durations_ms, tag, destination):
    """One Aseprite frame per duration, taken from the first frames of `layers`."""
    capas = ",".join(f"{name}={pattern}" for name, pattern in layers.items())
    subprocess.run([ASEPRITE, "-b",
                    "--script-param", f"salida={destination}",
                    "--script-param", f"capas={capas}",
                    "--script-param", "duraciones=" + ",".join(str(ms) for ms in durations_ms),
                    "--script-param", f"etiqueta={tag}",
                    "--script", str(ASSEMBLE_SCRIPT)],
                   check=True, capture_output=True)


def _save_sheet(frames, destination):
    width, height = frames[0].size
    sheet = Image.new("RGBA", (width * len(frames), height), (0, 0, 0, 0))
    for index, frame in enumerate(frames):
        sheet.paste(frame, (index * width, 0))
    sheet.save(destination)


def _save_preview(frames, durations_ms, destination):
    """Looping GIF with a transparent background, at the sprite's own size."""
    destination.parent.mkdir(parents=True, exist_ok=True)
    frames[0].save(destination, save_all=True, append_images=frames[1:], duration=list(durations_ms),
                   loop=0, disposal=2)
