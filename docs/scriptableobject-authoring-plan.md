# ScriptableObject Authoring Plan

## Purpose

This plan turns the current design into Unity assets.

Author these assets in order so the first playable loop can grow into the living spellbook without a big rewrite.

## Asset Folders

Use these folders:

- `Assets/_Heroic/ScriptableObjects/Schools`
- `Assets/_Heroic/ScriptableObjects/Skills/Arcane`
- `Assets/_Heroic/ScriptableObjects/Skills/Fire`
- `Assets/_Heroic/ScriptableObjects/Movement`
- `Assets/_Heroic/ScriptableObjects/Upgrades`
- `Assets/_Heroic/ScriptableObjects/Enemies`
- `Assets/_Heroic/ScriptableObjects/Waves`

## Databases

Create these database assets last, after the individual content assets exist:

- `SchoolDatabase.asset`
- `SkillDatabase.asset`
- `MovementDatabase.asset`

## Schools

### `School_Arcane.asset`

Type: `MagicSchoolDefinition`

- id: `arcane`
- schoolName: `Arcane`
- role: `repeat casting, consistency, double-cast synergy`
- baseDamage: `Low`
- castStyle: `Instant`
- areaShape: `Limited`
- range: `Long`
- cooldown: `Short`
- proc1: `Double Cast`
- proc2: `None`
- valueProposition: `Arcane rewards frequent casting, repeat effects, and safe long-range play.`

### `School_Fire.asset`

Type: `MagicSchoolDefinition`

- id: `fire`
- schoolName: `Fire`
- role: `burst damage, area denial, burn pressure`
- baseDamage: `Very High`
- castStyle: `Standard, 1 sec`
- areaShape: `Many`
- range: `Standard`
- cooldown: `Standard`
- proc1: `Burn`
- proc2: `Burning Area`
- valueProposition: `Fire rewards immediate impact, crowd clearing, and battlefield denial.`

## Arcane Skills

### `Skill_Arcane_MagicMissile.asset`

Type: `SkillDefinition`

- id: `arcane_magic_missile`
- skillName: `Magic Missile`
- role: `starter attack, reliable single-target pressure`
- baseDescription: `A basic precision projectile that auto-fires at the nearest enemy.`
- upgradePaths:
  - `Upgrade_Arcane_MagicMissile_SplitShot.asset`
  - `Upgrade_Arcane_MagicMissile_SeekingShot.asset`
  - `Upgrade_Arcane_MagicMissile_ArcanePierce.asset`

### `Skill_Arcane_ArcaneBlast.asset`

Type: `SkillDefinition`

- id: `arcane_arcane_blast`
- skillName: `Arcane Blast`
- role: `direct burst damage`
- baseDescription: `A focused burst of spell force in a small zone.`
- upgradePaths:
  - `Upgrade_Arcane_ArcaneBlast_Power.asset`
  - `Upgrade_Arcane_ArcaneBlast_Reach.asset`
  - `Upgrade_Arcane_ArcaneBlast_Scatter.asset`

### `Skill_Arcane_WarpPulse.asset`

Type: `SkillDefinition`

- id: `arcane_warp_pulse`
- skillName: `Warp Pulse`
- role: `control, spacing, disruption`
- baseDescription: `A distortion wave that manipulates enemy position.`
- upgradePaths:
  - `Upgrade_Arcane_WarpPulse_Push.asset`
  - `Upgrade_Arcane_WarpPulse_Pull.asset`
  - `Upgrade_Arcane_WarpPulse_SlowWarp.asset`

### `Skill_Arcane_SpellEcho.asset`

Type: `SkillDefinition`

- id: `arcane_spell_echo`
- skillName: `Spell Echo`
- role: `repeat-cast enhancer`
- baseDescription: `A spell repeats itself after a short delay.`
- upgradePaths:
  - `Upgrade_Arcane_SpellEcho_Repeat.asset`
  - `Upgrade_Arcane_SpellEcho_Amplify.asset`
  - `Upgrade_Arcane_SpellEcho_ChainEcho.asset`

### `Skill_Arcane_ArcaneOrbit.asset`

Type: `SkillDefinition`

- id: `arcane_arcane_orbit`
- skillName: `Arcane Orbit`
- role: `defensive offense, constant pressure`
- baseDescription: `Floating projectiles circle the player and strike enemies on contact.`
- upgradePaths:
  - `Upgrade_Arcane_ArcaneOrbit_MoreOrbs.asset`
  - `Upgrade_Arcane_ArcaneOrbit_FasterOrbs.asset`
  - `Upgrade_Arcane_ArcaneOrbit_LargerOrbs.asset`

## Fire Skills

### `Skill_Fire_Fireball.asset`

Type: `SkillDefinition`

- id: `fire_fireball`
- skillName: `Fireball`
- role: `core burst attack`
- baseDescription: `A direct explosive projectile that hits hard and burns.`
- upgradePaths:
  - `Upgrade_Fire_Fireball_Impact.asset`
  - `Upgrade_Fire_Fireball_Explosion.asset`
  - `Upgrade_Fire_Fireball_Burn.asset`

### `Skill_Fire_FlameWave.asset`

Type: `SkillDefinition`

- id: `fire_flame_wave`
- skillName: `Flame Wave`
- role: `area sweep and crowd clear`
- baseDescription: `A sweeping wave of fire that travels outward and burns everything it touches.`
- upgradePaths:
  - `Upgrade_Fire_FlameWave_WiderWave.asset`
  - `Upgrade_Fire_FlameWave_LongerWave.asset`
  - `Upgrade_Fire_FlameWave_HotterWave.asset`

### `Skill_Fire_EmberRain.asset`

Type: `SkillDefinition`

- id: `fire_ember_rain`
- skillName: `Ember Rain`
- role: `delayed burst and zone pressure`
- baseDescription: `A delayed barrage of falling fire strikes.`
- upgradePaths:
  - `Upgrade_Fire_EmberRain_MoreMeteors.asset`
  - `Upgrade_Fire_EmberRain_FasterRain.asset`
  - `Upgrade_Fire_EmberRain_Firestorm.asset`

### `Skill_Fire_Ignition.asset`

Type: `SkillDefinition`

- id: `fire_ignition`
- skillName: `Ignition`
- role: `burn amplifier`
- baseDescription: `A spreading burn effect that makes enemies dangerous to stand near.`
- upgradePaths:
  - `Upgrade_Fire_Ignition_Spread.asset`
  - `Upgrade_Fire_Ignition_Intensify.asset`
  - `Upgrade_Fire_Ignition_Detonate.asset`

### `Skill_Fire_CinderWall.asset`

Type: `SkillDefinition`

- id: `fire_cinder_wall`
- skillName: `Cinder Wall`
- role: `area denial and choke control`
- baseDescription: `A barrier of fire that shapes the battlefield.`
- upgradePaths:
  - `Upgrade_Fire_CinderWall_LongerWall.asset`
  - `Upgrade_Fire_CinderWall_HotterWall.asset`
  - `Upgrade_Fire_CinderWall_MovingWall.asset`

## First Movement Assets

Create all seven movement skills, but only wire two or three into the first prototype draft pool.

### `Movement_Blink.asset`

Type: `MovementSkillDefinition`

- id: `blink`
- skillName: `Blink`
- role: `short offensive and defensive reposition`
- cooldown: `Short`
- range: `Short`
- description: `A short instant reposition with offensive and defensive upgrade paths.`

### `Movement_Teleport.asset`

Type: `MovementSkillDefinition`

- id: `teleport`
- skillName: `Teleport`
- role: `long-range pure mobility`
- cooldown: `Long`
- range: `Long`
- description: `A longer-range repositioning tool with flexible placement.`

### `Movement_Invisibility.asset`

Type: `MovementSkillDefinition`

- id: `invisibility`
- skillName: `Invisibility`
- role: `untargetability and aggro break`
- cooldown: `Standard`
- range: `Self`
- description: `Enemies stop tracking the player while invisible.`

### `Movement_Stoneskin.asset`

Type: `MovementSkillDefinition`

- id: `stoneskin`
- skillName: `Stoneskin`
- role: `slow movement, high offense and defense`
- cooldown: `Standard`
- range: `Self`
- description: `The player moves slowly but gains strong offensive and defensive value.`

### `Movement_Lunge.asset`

Type: `MovementSkillDefinition`

- id: `lunge`
- skillName: `Lunge`
- role: `ground-based aggressive burst movement`
- cooldown: `Short`
- range: `Medium`
- description: `A committed forward burst that rewards pressure and positioning.`

### `Movement_Whirlwind.asset`

Type: `MovementSkillDefinition`

- id: `whirlwind`
- skillName: `Whirlwind`
- role: `offensive movement through mobs`
- cooldown: `Standard`
- range: `Medium`
- description: `A damaging elemental pass through enemy groups.`

### `Movement_Tunnel.asset`

Type: `MovementSkillDefinition`

- id: `tunnel`
- skillName: `Tunnel`
- role: `underground untargetability with surfacing bonuses`
- cooldown: `Standard`
- range: `Medium`
- description: `The player travels underground while enemies keep following the path.`

## First Draft Pool

For the first playable upgrade draft, use:

- `Skill_Arcane_MagicMissile.asset`
- `Skill_Arcane_ArcaneBlast.asset`
- `Skill_Fire_Fireball.asset`
- `Skill_Fire_FlameWave.asset`
- `Movement_Blink.asset`
- `Movement_Lunge.asset`
- `Movement_Teleport.asset`

## Authoring Order

1. Create the Arcane and Fire school assets.
2. Create Magic Missile, Arcane Blast, Fireball, and Flame Wave.
3. Create Blink, Lunge, and Teleport.
4. Create the first database assets.
5. Wire those assets into the prototype managers.
6. Add the remaining skills after the first loop feels good.
