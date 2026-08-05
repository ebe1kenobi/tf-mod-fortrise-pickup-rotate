#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Genere les images du mod Rotate : icone du variant et sprite du pickup.

L'effet fait pivoter ou retourner l'ecran (90 gauche/droite, 180, 360, flips),
d'ou le motif : une fleche circulaire.

Deux formats, deux regles de contour :

  ../ModFile/Content/Atlas/atlas.png        14x14, AVEC contour
      Icone du variant. Reste lue via atlas.xml a la region (0,0,14,14),
      inchangee : seule l'image est remplacee.

  ../ModFile/Content/Atlas/rotatepickup.png 16x16, SANS contour
      Sprite de l'objet a ramasser (taille du collider). Le jeu ajoute lui-meme
      le contour via graphic.DrawOutline() dans RotatePickup.Render().

    python make_icons.py

Dependance : Pillow  ->  pip install Pillow
"""

import os
from PIL import Image

HERE = os.path.dirname(os.path.abspath(__file__))
ATLAS = os.path.join(HERE, "..", "ModFile", "Content", "Atlas")

OUTLINE = (26, 18, 34, 255)
DEEP    = (150, 78, 20, 255)    # bord de l'orbe
BODY    = (232, 136, 40, 255)   # corps
GLOW    = (255, 206, 120, 255)  # reflet
MARK    = (255, 250, 235, 255)  # fleche circulaire


def add_outline(img, color):
    """Contour par dilatation, comme les icones du jeu."""
    w, h = img.size
    px = img.load()
    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    op = out.load()
    for y in range(h):
        for x in range(w):
            if px[x, y][3]:
                continue
            if any(0 <= x + dx < w and 0 <= y + dy < h and px[x + dx, y + dy][3]
                   for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1))):
                op[x, y] = color
    out.alpha_composite(img)
    return out



# Fleche circulaire dessinee a la main : a cette taille une construction
# geometrique donne un anneau illisible, la tete de fleche se noyant dans la
# couronne. '#' = trait, '>' = tete de fleche.
# Fleche circulaire dessinee a la main. A cette taille, une construction
# geometrique donne un anneau illisible et la tete de fleche s'y noie.
#
# Pas de disque de fond ici, contrairement au pickup Gravity : il ne resterait
# qu'une dizaine de pixels utiles au centre, trop peu pour lire une rotation.
# La fleche occupe donc toute l'image.
#
#   '#' trait   '>' tete de fleche
ARROW_16 = [
    "................",
    "......####......",
    "....########....",
    "...###....###...",
    "..###.....>>>>>>",
    "..##.......>>>>.",
    ".###........>>..",
    ".###.........>..",
    ".###............",
    ".###............",
    "..##............",
    "..###......###..",
    "...###....###...",
    "....########....",
    "......####......",
    "................",
]

ARROW_14 = [
    "..............",
    ".....####.....",
    "...########...",
    "..###....##...",
    "..##....>>>>>>",
    ".###.....>>>>.",
    ".###......>>..",
    ".###.......>..",
    ".###..........",
    "..##..........",
    "..###....###..",
    "...########...",
    ".....####.....",
    "..............",
]


def stamp(px, size, art, colour, head_colour):
    for y, row in enumerate(art):
        for x, ch in enumerate(row):
            if x >= size or y >= size:
                continue
            if ch == "#":
                px[x, y] = colour
            elif ch == ">":
                px[x, y] = head_colour


def make_variant():
    """14x14 : fleche circulaire, contour inclus (aucun DrawOutline cote jeu)."""
    size = 14
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    stamp(img.load(), size, ARROW_14, BODY, GLOW)
    return add_outline(img, OUTLINE)


def make_pickup():
    """16x16 : meme motif en plus grand, SANS contour (le jeu l'ajoute)."""
    size = 16
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    stamp(img.load(), size, ARROW_16, BODY, GLOW)
    return img


os.makedirs(ATLAS, exist_ok=True)

variant = make_variant()
variant.save(os.path.join(ATLAS, "atlas.png"))
variant.resize((196, 196), Image.NEAREST).save(os.path.join(HERE, "preview_variant.png"))

pickup = make_pickup()
pickup.save(os.path.join(ATLAS, "rotatepickup.png"))
pickup.resize((224, 224), Image.NEAREST).save(os.path.join(HERE, "preview_pickup.png"))

print(f"atlas.png        {variant.size}  {len({p for p in variant.getdata() if p[3]})} couleurs")
print(f"rotatepickup.png {pickup.size}  {len({p for p in pickup.getdata() if p[3]})} couleurs")
