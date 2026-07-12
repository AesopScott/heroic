# Heroic Graphics Plan

## 1.0 Graphics Target

The 1.0 build does not need final art, but it must be readable and playable.

Passable means:

- player is visually distinct
- enemies are visually distinct by role
- boss is clearly special
- Magic Missile and Arcane Orbit are visible in motion
- XP pickups are visible
- Arcane Blast, Warp Pulse, Blink, Lunge, and Teleport have visible feedback
- cooldown and draft UI are readable

## Current Placeholder System

Runtime placeholder graphics live in `Assets/_Heroic/Scripts/Visuals`.

Components:

- `ProceduralSpriteFactory`
- `AutoSpriteVisual`
- `AutoTrailVisual`
- `VisualPresetApplier`
- `TemporaryVisualEffect`

## Prefab Presets

Use `VisualPresetApplier` on early prefabs:

- Player: `Player`
- Basic enemy: `BasicEnemy`
- fast enemy: `FastEnemy`
- tank enemy: `TankEnemy`
- boss: `Boss`
- Magic Missile projectile: `MagicMissile`
- Arcane Orbit orb: `ArcaneOrb`
- XP pickup: `ExperiencePickup`

## Runtime Effects

The following effects are already generated in code:

- Arcane Blast area flash
- Warp Pulse area flash
- Blink departure and arrival flash
- Teleport departure and arrival flash
- Lunge trail flashes

## Later Art Pass

When final art starts, replace procedural sprites with:

- player sprite or animated mage
- enemy silhouettes by role
- Arcane projectile sprites
- XP gem sprites
- boss sprite
- school-specific VFX sheets
