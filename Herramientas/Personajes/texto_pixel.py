"""Tiny 3 x 5 pixel font for lettering painted on sprites (newspaper mastheads)."""
import numpy as np

GLYPHS = {
    "C": ("111", "100", "100", "100", "111"),
    "D": ("110", "101", "101", "101", "110"),
    "E": ("111", "100", "110", "100", "111"),
    "I": ("111", "010", "010", "010", "111"),
    "L": ("100", "100", "100", "100", "111"),
    "M": ("101", "111", "111", "101", "101"),
    "O": ("111", "101", "101", "101", "111"),
    "S": ("111", "100", "111", "001", "111"),
    "X": ("101", "101", "010", "101", "101"),
    " ": ("000", "000", "000", "000", "000"),
}
GLYPH_WIDTH = 3
GLYPH_HEIGHT = 5
ADVANCE = GLYPH_WIDTH + 1


def paint_text(text, column, row):
    """Boolean mask: True where print pixel (column, row) falls on an inked pixel of `text` drawn at (0, 0)."""
    column, row = np.asarray(column), np.asarray(row)
    glyph_index = column // ADVANCE
    x_in_glyph = column % ADVANCE
    inside = (row >= 0) & (row < GLYPH_HEIGHT) & (column >= 0) & (glyph_index < len(text)) & (x_in_glyph < GLYPH_WIDTH)
    lit = np.zeros(column.shape, dtype=bool)
    for index, character in enumerate(text):
        bitmap = np.array([[pixel == "1" for pixel in line] for line in GLYPHS[character]])
        here = inside & (glyph_index == index)
        lit[here] = bitmap[row[here], x_in_glyph[here]]
    return lit
