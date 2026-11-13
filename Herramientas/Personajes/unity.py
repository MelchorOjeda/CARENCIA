"""Writes sprite sheets with their Unity import settings (.meta) and sprite animation clips (.anim).

Import settings copy the project's character sheets (SPR_PJ_Elaki_Espera_SE_R): 96 PPU, Point filter,
no compression, Multiple sprites sliced left to right, custom pivot on the feet or the chair's floor centre.
Re-running keeps the GUIDs and sprite ids already written, so scenes and clips that use them stay linked.
"""
import random
import re
import uuid
from dataclasses import dataclass
from pathlib import Path
from typing import Optional

from PIL import Image

PIXELS_PER_UNIT = 96
CLIP_SAMPLE_RATE = 100   # every duration used is a multiple of 10 ms


@dataclass(frozen=True)
class UnityTarget:
    sprite: Path                 # sheet to write, e.g. .../SPR_PJ_Madre_Parpadear_SE_R.png
    clip: Optional[Path] = None  # animation clip, e.g. .../ANIM_Madre_Parpadear_SE.anim; None for a still prop
    binding_path: str = ""       # child holding the SpriteRenderer ("Figura" on PF_Elaki)
    loop: bool = True


def character_target(unity_root, folder, sprite, clip, binding_path="", loop=True):
    """Sheet in Sprites/Personajes/<folder> and clip in Animations/Personajes/<folder>; None without `unity_root`."""
    if unity_root is None:
        return None
    root = Path(unity_root)
    return UnityTarget(sprite=root / "Sprites" / "Personajes" / folder / f"{sprite}.png",
                       clip=root / "Animations" / "Personajes" / folder / f"{clip}.anim",
                       binding_path=binding_path, loop=loop)


def write_unity(target, frames, durations_ms, pivot_px):
    """Saves `frames` (PIL images of equal size) as one sheet with its .meta and, if asked, its clip."""
    _ensure_folder_meta(target.sprite.parent)
    sprite_ids = write_sheet(frames, target.sprite, pivot_px)
    if target.clip:
        _ensure_folder_meta(target.clip.parent)
        write_clip(target.clip, _guid_of(target.sprite), sprite_ids, durations_ms, target.loop, target.binding_path)


def write_sheet(frames, path, pivot_px):
    """Writes the sheet and its .meta; returns the internal id of every sprite, in frame order."""
    width, height = frames[0].size
    sheet = Image.new("RGBA", (width * len(frames), height), (0, 0, 0, 0))
    for index, frame in enumerate(frames):
        sheet.paste(frame, (index * width, 0))
    sheet.save(path)
    pivot = (pivot_px[0] / width, 1.0 - pivot_px[1] / height)
    meta = Path(f"{path}.meta")
    guid, previous_ids = _existing_ids(meta)
    names = [f"{path.stem}_{index:02d}" for index in range(len(frames))]
    ids = [previous_ids.get(name) or _new_internal_id() for name in names]
    meta.write_text(_texture_meta(guid, names, ids, (width, height), pivot), encoding="utf-8", newline="\n")
    return ids


def _existing_ids(meta):
    """GUID and {sprite name: internal id} of an existing .meta, or a fresh GUID when there is none."""
    if not meta.exists():
        return uuid.uuid4().hex, {}
    text = meta.read_text(encoding="utf-8")
    guid = re.search(r"^guid: (\w+)", text, re.MULTILINE).group(1)
    table = re.search(r"nameFileIdTable:\n((?:      .+\n)*)", text)
    pairs = re.findall(r"      (\S+): (-?\d+)", table.group(1)) if table else []
    return guid, {name: int(value) for name, value in pairs}


def _new_internal_id():
    return random.randint(-(2 ** 62), 2 ** 62)


def _guid_of(asset):
    return re.search(r"^guid: (\w+)", Path(f"{asset}.meta").read_text(encoding="utf-8"), re.MULTILINE).group(1)


def _ensure_folder_meta(folder):
    """Unity needs a .meta for every folder; a new one gets its own GUID."""
    meta = Path(f"{folder}.meta")
    if meta.exists():
        return
    folder.mkdir(parents=True, exist_ok=True)
    meta.write_text(FOLDER_META.format(guid=uuid.uuid4().hex), encoding="utf-8", newline="\n")


def write_clip(path, texture_guid, sprite_ids, durations_ms, loop, binding_path):
    """Sprite clip with one key per frame at its start time, plus a closing key that holds the last frame."""
    keys, time_ms = [], 0
    for sprite_id, duration in zip(sprite_ids, durations_ms):
        keys.append(CLIP_KEY.format(time=time_ms / 1000, id=sprite_id, guid=texture_guid))
        time_ms += duration
    closing_time = (time_ms - 1000 / CLIP_SAMPLE_RATE) / 1000
    keys.append(CLIP_KEY.format(time=closing_time, id=sprite_ids[-1], guid=texture_guid))
    clip = CLIP.format(name=path.stem, keys="".join(keys), path=binding_path, rate=CLIP_SAMPLE_RATE,
                       stop=time_ms / 1000, loop=int(loop))
    path.write_text(clip, encoding="utf-8", newline="\n")
    meta = Path(f"{path}.meta")
    if not meta.exists():
        meta.write_text(CLIP_META.format(guid=uuid.uuid4().hex), encoding="utf-8", newline="\n")


def _texture_meta(guid, names, ids, frame_size, pivot):
    width, height = frame_size
    sprites = "".join(SPRITE_ENTRY.format(name=name, x=index * width, width=width, height=height,
                                          pivot_x=round(pivot[0], 4), pivot_y=round(pivot[1], 4),
                                          sprite_id=uuid.uuid4().hex, internal_id=internal_id)
                      for index, (name, internal_id) in enumerate(zip(names, ids)))
    table = "".join(f"      {name}: {internal_id}\n" for name, internal_id in zip(names, ids))
    return TEXTURE_META.format(guid=guid, ppu=PIXELS_PER_UNIT, sprites=sprites, table=table)


FOLDER_META = """fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData:
  assetBundleName:
  assetBundleVariant:
"""

CLIP_META = """fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 7400000
  userData:
  assetBundleName:
  assetBundleVariant:
"""

CLIP_KEY = """    - time: {time}
      value: {{fileID: {id}, guid: {guid}, type: 3}}
"""

CLIP = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name}
  serializedVersion: 8
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - serializedVersion: 2
    curve:
{keys}    attribute: m_Sprite
    path: {path}
    classID: 212
    script: {{fileID: 0}}
    flags: 2
  m_SampleRate: {rate}
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings: []
    pptrCurveMapping: []
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: {stop}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: {loop}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""

SPRITE_ENTRY = """    - serializedVersion: 2
      name: {name}
      rect:
        serializedVersion: 2
        x: {x}
        y: 0
        width: {width}
        height: {height}
      alignment: 9
      pivot: {{x: {pivot_x}, y: {pivot_y}}}
      border: {{x: 0, y: 0, z: 0, w: 0}}
      customData:
      outline: []
      physicsShape: []
      tessellationDetail: 0
      bones: []
      spriteID: {sprite_id}
      internalID: {internal_id}
      vertices: []
      indices:
      edges: []
      weights: []
"""

TEXTURE_META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 2
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: {ppu}
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  spriteGeometrySubdivision: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 16384
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 4
    buildTarget: Standalone
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 4
    buildTarget: WebGL
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites:
{sprites}    outline: []
    customData:
    physicsShape: []
    bones: []
    spriteID:
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
    nameFileIdTable:
{table}  mipmapLimitGroupName:
  pSDRemoveMatte: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"""
