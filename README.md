# Heroic

Browser-based bullet heaven prototype.

## Core Pitch

- Single-player only
- Top-down action game
- Living spellbook progression
- 8 magic schools
- Modular ability upgrades
- Movement is strategic, not mandatory

## Progression Rules

- Every level: choose one non-movement upgrade category
  - Attack Ability
  - Defense Ability
  - System
  - Boost
- Every other level: also choose one movement option
  - Select new movement skill
  - Boost existing movement skill
- Up to 3 movement skills equipped
  - Slot 1 = key 1
  - Slot 2 = key 2
  - Slot 3 = key 3

## Magic Schools

1. Arcane
2. Fire
3. Cold
4. Lightning
5. Earth
6. Mind
7. Blood
8. Poison

## Current Design Direction

- Arcane focuses on repeat casting and double-cast synergy
- Fire focuses on burst and burning areas
- Cold focuses on slows and freeze control
- Lightning focuses on fast chains and stun pressure
- Earth focuses on long-range terrain and crowd disruption
- Mind focuses on fear and confusion
- Blood focuses on sacrifice, bleed, and drain
- Poison focuses on contagious damage over time

## Movement Skills

- Blink
- Teleport
- Invisibility
- Stoneskin
- Lunge
- Whirlwind
- Tunnel

Movement is intentionally powerful and strategic so it competes with other upgrades.

## Obsidian Notes

Design memory is mirrored in `G:\my drive\heroic`.

## Unity Bootstrap

Unity is required to generate real scenes, prefabs, `.meta` files, and validate play mode.

Installed local editor:

- Unity Hub `3.19.4`
- Unity Editor `6000.5.3f1 (c2eb47b3a2a9)`
- WebGL Build Support

Recommended flow:

1. Sign into Unity Hub with the active Unity Personal license.
2. Open this folder in Unity `6000.5.3f1`.
3. Run `Heroic/Build 1.0 Prototype Content`.
4. Run `Heroic/Validate 1.0 Prototype`.
5. Open `Assets/_Heroic/Scenes/Game.unity`.
6. Press Play and follow `docs/smoke-test-checklist.md`.
7. For a browser build, run `Heroic/Build WebGL 1.0`.

Batch-mode flow after Unity license activation:

```powershell
.\scripts\run-unity-1.0.ps1
```

Use `-SkipWebGL` to generate and validate content without producing the browser build.

Before showing the game externally, also follow `docs/investor-demo-readiness.md`.
