# PickupRotate

Adds a pickup that **rotates or flips the screen**: 90 degrees left or right, half
turn, full turn, horizontal or vertical mirror. Each effect is toggled on its own in
the settings; the pickup draws one at random among those enabled.

A mod for **FortRise 5** (>= 5.3.3). The FortRise 4 version (`tf-mod-fortrise-pickup-rotate`) is no longer maintained: fixes and new features only land in this repository.

## Installation

1. Install FortRise 5 and start the game through `FortRise.exe`.
2. Copy `release/pickuprotate` (or the shipped folder) into `<TowerFall>/FortRise/Mods/`.

Settings are under **Options > Mods > PickupRotate**.
Data and log files live in `<TowerFall>/FortRise/Saves/PickupRotate/` and `<TowerFall>/FortRise/Logs/`.

## Usage

Tick the **Rotate** variant on the versus variants screen, or turn on the
"Pickup activated even when variant is not selected" setting.

Press the **left upper shoulder** (Alt2) on that variant to open the mod's
settings right there, without leaving the variants screen. Whatever you change is
written to disk when the window closes.

> All my mods declare the same `Header` (`EBE1 MODS`), so their variants are
> grouped into a **single column** of the variants screen instead of one column
> per mod.

## Settings

| Setting | Purpose |
|---------|---------|
| Pickup activated even when variant is not selected | spawn the pickup even when the variant is unticked |
| Periodicity | `Normal` (the game draws it) or `Test` (forced into the chests, for trying it out) |
| Treasure rate | how often it turns up, from `0.001` (as rare as the Chaos Orb) to `20.000`. The labels name the game items sharing that rate, which says more than a number |
| RotateRight90 / RotateLeft90 / Rotate180 / Rotate360 / FlipY / FlipX | which effects the pickup may draw from |
| Effect Time | how long an effect lasts |
| Effect 360 Time | how long a full turn lasts |

## How the pickup reaches the chests

Registering a pickup only makes it *exist*: FortRise widens the treasure tables,
but the new entry stays at zero and the pickup never drops. What puts it in the
draw is an `ITowerHook`, whose `VersusTowerTreasurePatch` runs inside the
`TreasureSpawner` constructor - once per match, after the variants are set.

The weight the game draws on is `units x Chance`. `Chance` is declared when the
pickup is registered and is fixed at **0.001** here - the lowest rate in the game,
the one the Chaos Orb uses. The setting counts those units, which is how a rate
below 1 is reachable at all: the API only adds integers.

The upside over writing into `TreasureRates` directly: the game's weighted draw
still applies afterwards, so variant exclusions, the tower's item set and arrow
shuffle are all respected, and several chests can hold it.

## Build / deployment

| Script | Purpose |
|--------|---------|
| `script/release.bat` | build, then assemble into `release/` |
| `script/deploy.bat` | copy `release/` into the TowerFall `Mods` folder |
| `script/release_deploy.bat` | both, one after the other |

Paths (game folder, module name) are set in `script/config.bat`.
