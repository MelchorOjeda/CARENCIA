import os
import re
from PIL import Image

def scale_azulejos(dry_run=False):
    """
    Escala los azulejos de 64x32 a 48x24 (factor 0.75 exacto, 3/4)
    para encajar en celdas de 24 px por paso en el Grid de 48 PPU.
    """
    plantillas_dir = r"Assets\_Carencia\Sprites\Escenarios\Plantillas"
    files = [f for f in os.listdir(plantillas_dir) if f.startswith("azulejos_") and f.endswith(".png")]
    
    print(f"Encontrados {len(files)} archivos de azulejos.")
    for f in sorted(files):
        img_path = os.path.join(plantillas_dir, f)
        meta_path = img_path + ".meta"
        
        with Image.open(img_path) as im:
            orig_w, orig_h = im.size
            new_w = int(round(orig_w * 0.75))
            new_h = int(round(orig_h * 0.75))
            
            if dry_run:
                print(f"[DRY-RUN] {f}: {orig_w}x{orig_h} -> {new_w}x{new_h}")
                continue
            
            # Reescalado Pixel Art (Nearest Neighbor para preservar bordes nítidos)
            scaled_im = im.resize((new_w, new_h), Image.Resampling.NEAREST)
            scaled_im.save(img_path)
            print(f"Reescalado {f:45}: {orig_w}x{orig_h} -> {new_w}x{new_h}")
        
        # Actualizar .meta con las nuevas dimensiones y pivote
        if os.path.exists(meta_path) and not dry_run:
            with open(meta_path, 'r', encoding='utf-8') as mf:
                content = mf.read()
            
            # Recalcular pivote para azulejo de 50x32 (centro rombo en Y=20 px de 32 -> 0.625)
            piv_y = round(20.0 / new_h, 7) if "bloque" in f else (round(12.0 / new_h, 7) if new_h > 12 else 0.5)
            if "piso_8x8" in f or "sala_ejemplo" in f:
                piv_y = 0.5
            
            content = re.sub(r'spritePivot: \{x: [^,]+, y: [^}]+\}', f'spritePivot: {{x: 0.5, y: {piv_y}}}', content)
            content = re.sub(r'width: \d+', f'width: {new_w}', content)
            content = re.sub(r'height: \d+', f'height: {new_h}', content)
            
            with open(meta_path, 'w', encoding='utf-8') as mf:
                mf.write(content)

if __name__ == "__main__":
    import sys
    dry = "--dry-run" in sys.argv
    scale_azulejos(dry_run=dry)

