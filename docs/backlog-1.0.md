# Heroic 1.0 Backlog

## 1.0 Target

Heroic 1.0 is a playable Unity 2D bullet heaven prototype with:

- one complete run loop
- all 5 Arcane skills implemented
- at least one complete movement set implemented
- XP, leveling, and upgrade drafting
- basic enemies, waves, and win/loss states
- enough UI, audio, and visual feedback to feel playable

## Current State

Already started:

- Unity folder scaffold under `Assets/_Heroic`
- runtime skeletons for run state, player, enemies, spells, movement, UI, and data
- ScriptableObject definition classes
- Magic Missile auto-targeting scaffold
- simple enemy spawning and chase behavior
- first prefab setup guide
- Arcane school design
- movement skill design

Not yet done:

- real Unity scenes
- actual prefabs
- ScriptableObject assets
- real upgrade draft flow
- all Arcane skill behavior
- movement skill behavior
- enemy variety
- run loop polish

## Milestone 0: Unity Project Hygiene

### H-001 Create Unity project metadata

Acceptance criteria:

- Unity opens `C:\Users\scott\Code\Heroic` without import errors
- `Assets/_Heroic` appears in the Project view
- Unity-generated `ProjectSettings`, `Packages`, and `.meta` files are committed

### H-002 Add Unity `.gitignore`

Acceptance criteria:

- `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`, and build outputs are ignored
- Unity-generated `.meta` files are tracked
- repo can be cloned and opened cleanly

### H-003 Create baseline scenes

Acceptance criteria:

- `MainMenu.unity`, `Game.unity`, and `Results.unity` exist as real Unity scenes
- `Game` scene contains a camera, player, managers, and spawn area
- scenes are added to Unity Build Settings

## Milestone 1: First Playable Combat Loop

### H-010 Build Player prefab

Acceptance criteria:

- player moves with keyboard input
- player uses `Rigidbody2D` and collider correctly
- player has health and can die
- player has `SpellCaster`, `MagicMissileCaster`, and `MovementCaster`

### H-011 Build basic enemy prefab

Acceptance criteria:

- enemy spawns into the scene
- enemy chases player
- enemy damages player on contact with a cooldown
- enemy can be killed by damage

### H-012 Build Magic Missile projectile prefab

Acceptance criteria:

- projectile launches toward nearest enemy
- projectile deals damage on trigger collision
- projectile despawns on hit or lifetime expiry
- projectile visuals are readable in motion

### H-013 Wire first Game scene loop

Acceptance criteria:

- entering Play Mode starts a playable combat loop
- enemies spawn around player
- player auto-fires Magic Missile
- enemies can kill player
- player can kill enemies

### H-014 Add camera follow

Acceptance criteria:

- camera follows player smoothly
- camera stays readable during movement
- camera does not jitter with physics movement

## Milestone 2: Core Run Systems

### H-020 Add XP pickups

Acceptance criteria:

- enemies drop XP on death
- player can collect XP
- XP pickup has visible feedback
- collected XP updates player XP state

### H-021 Add player level system

Acceptance criteria:

- player starts at level 1
- XP threshold triggers level-up
- level and XP can be displayed in UI
- level-up pauses or slows gameplay for draft selection

### H-022 Build upgrade draft flow

Acceptance criteria:

- level-up opens draft UI
- player chooses category for non-movement upgrade
- game presents 3 to 5 choices from that category
- selected upgrade applies immediately
- gameplay resumes after selection

### H-023 Add movement draft cadence

Acceptance criteria:

- every level gives one non-movement upgrade choice
- every other level also gives one movement choice
- movement choice can be new movement skill or movement boost
- equipped movement skills map to slots 1, 2, and 3

### H-024 Add run end states

Acceptance criteria:

- player death ends run
- survival timer is tracked
- results scene or results overlay displays run summary
- restart returns to a playable state

## Milestone 3: Data-Driven Ability Foundation

### H-030 Expand ScriptableObject skill model

Acceptance criteria:

- `SkillDefinition` can describe cast style, targeting, range, cooldown, damage, and effect type
- skill data can reference prefab behavior or runtime handler type
- upgrade paths can contain 5 tiers of values

### H-031 Add upgrade tier model

Acceptance criteria:

- each upgrade path supports Basic, Advanced, Expert, Master, and Grandmaster tiers
- tiers can change numeric values such as damage, cooldown, count, radius, range, or proc chance
- active tier state is stored in the run build

### H-032 Build living spellbook runtime state

Acceptance criteria:

- run build tracks learned skills
- run build tracks skill upgrade tiers
- run build tracks equipped movement skills
- run build can answer what choices are eligible at level-up

### H-033 Build ability execution router

Acceptance criteria:

- `SpellCaster` can execute learned skills from data
- skills can auto-cast on cooldown
- skills can apply upgrades from current run state
- new Arcane skills can plug into the router without rewriting player code

## Milestone 4: Arcane Skill Implementation

### H-040 Implement Magic Missile

Acceptance criteria:

- auto-targets nearest enemy
- supports damage, cooldown, range, projectile count, homing, and pierce
- supports Double Cast proc
- upgrade paths work: Split Shot, Seeking Shot, Arcane Pierce

### H-041 Implement Arcane Blast

Acceptance criteria:

- creates a targeted impact zone
- supports damage, radius, range, and cooldown
- upgrade paths work: Power, Reach, Scatter
- Double Cast can repeat the blast

### H-042 Implement Warp Pulse

Acceptance criteria:

- emits a pulse that affects nearby enemies
- supports push, pull, and slow variants
- enemies respond visibly to displacement or slow
- upgrade paths work: Push, Pull, Slow Warp

### H-043 Implement Spell Echo

Acceptance criteria:

- repeats eligible Arcane casts after a delay
- supports repeat count, echo damage, and chain behavior
- upgrade paths work: Repeat, Amplify, Chain Echo
- avoids infinite echo loops

### H-044 Implement Arcane Orbit

Acceptance criteria:

- creates orbiting projectiles around player
- orbitals damage enemies on contact
- supports orb count, speed, size, and duration
- upgrade paths work: More Orbs, Faster Orbs, Larger Orbs

### H-045 Implement Arcane Double Cast system

Acceptance criteria:

- Arcane skills can trigger Double Cast
- Double Cast chance and power can be tuned
- repeated casts are readable and do not break cooldown logic
- UI or feedback shows when Double Cast triggers

## Milestone 5: Movement Set Implementation

### H-050 Build movement slot system

Acceptance criteria:

- player can equip up to 3 movement skills
- keys `1`, `2`, and `3` activate movement slots
- each movement skill has independent cooldown
- UI shows slot cooldowns

### H-051 Implement Blink

Acceptance criteria:

- short instant reposition
- cannot place player inside invalid collision
- supports offensive and defensive upgrade hooks
- feels responsive under pressure

### H-052 Implement Lunge

Acceptance criteria:

- committed ground-based forward burst
- can damage or push enemies in path when upgraded
- does not bypass terrain like Flight
- has clear recovery/cooldown

### H-053 Implement Teleport

Acceptance criteria:

- long-range reposition with longer cooldown
- target placement is constrained to valid ground
- supports range and cooldown upgrades
- gives strong mobility without offensive default behavior

### H-054 Implement movement boosts

Acceptance criteria:

- movement skills have at least 3 boost paths each
- each boost path supports 5 tiers
- boosts can modify cooldown, range, damage, defense, or utility
- draft system can offer movement boosts every other level

### H-055 Add movement feedback

Acceptance criteria:

- movement activation has distinct visual feedback
- cooldown failure has clear feedback
- movement slot UI updates after use
- player can tell which movement skill fired

## Milestone 6: Enemies And Waves

### H-060 Add enemy definitions

Acceptance criteria:

- enemy stats come from `EnemyDefinition`
- at least 3 enemy types exist: chaser, fast weak enemy, tank enemy
- enemy types are visually distinguishable

### H-061 Build wave definitions

Acceptance criteria:

- waves can spawn different enemy compositions over time
- spawn rate scales during the run
- wave data can be tuned without code edits

### H-062 Add elite enemy

Acceptance criteria:

- elite enemy appears periodically
- elite has more HP and a distinct behavior or modifier
- killing elite gives a meaningful XP or upgrade reward

### H-063 Add 1.0 boss encounter

Acceptance criteria:

- boss appears near end of run
- boss has at least 2 attack or movement patterns
- defeating boss completes the run
- losing to boss ends the run normally

## Milestone 7: UI And UX

### H-070 Build HUD

Acceptance criteria:

- health is visible
- XP and level are visible
- run timer is visible
- movement slots and cooldowns are visible

### H-071 Build upgrade draft UI

Acceptance criteria:

- category choice is clear
- 3 to 5 draft choices are readable
- selected choice previews what it changes
- keyboard/controller-friendly navigation is possible later

### H-072 Build pause menu

Acceptance criteria:

- player can pause
- pause stops gameplay
- player can resume, restart, or quit to menu

### H-073 Build results UI

Acceptance criteria:

- results show survival time, level reached, enemies killed, skills learned, and movement loadout
- restart button works
- quit to menu works

## Milestone 8: Visuals, Audio, And Feel

### H-080 Add placeholder visual style

Acceptance criteria:

- player, enemies, projectiles, XP, and areas are visually distinct
- Arcane effects have a consistent visual identity
- movement effects are readable

### H-081 Add impact feedback

Acceptance criteria:

- enemy hit flashes or reacts
- enemy death has feedback
- XP pickup has feedback
- level-up has feedback

### H-082 Add audio placeholders

Acceptance criteria:

- Magic Missile has cast/hit sound
- movement skills have activation sounds
- XP pickup and level-up have sounds
- player damage and death have sounds

### H-083 Add screenshake and tuning controls

Acceptance criteria:

- impact shake is subtle and optional
- shake can be tuned or disabled
- effects do not obscure gameplay clarity

## Milestone 9: Balance And Progression

### H-090 Tune first 20 levels

Acceptance criteria:

- level pacing feels steady
- player reaches several upgrade decisions in a short test run
- movement choices every other level feel meaningful
- Arcane upgrades produce obvious power growth

### H-091 Tune first run length

Acceptance criteria:

- 1.0 run length target is chosen
- enemy density ramps over the run
- boss timing matches the run length
- run can be completed by a competent player

### H-092 Tune Arcane upgrade values

Acceptance criteria:

- all Arcane skill paths have 5 tiers
- no Arcane skill is useless at tier 1
- Grandmaster tier feels meaningfully stronger
- Double Cast feels powerful without making every other mechanic irrelevant

### H-093 Tune movement upgrade values

Acceptance criteria:

- Blink, Lunge, and Teleport are all worth taking
- movement builds feel strategic
- movement does not consume the entire progression economy
- cooldowns support tactical use

## Milestone 10: 1.0 Release Readiness

### H-100 Add smoke test checklist

Acceptance criteria:

- checklist covers scene load, player movement, enemy spawn, Magic Missile, XP, level-up, movement skills, boss, win, loss, restart
- checklist lives in repo docs

### H-101 Add known issues doc

Acceptance criteria:

- known issues are tracked in repo
- each issue has severity and workaround if applicable
- 1.0 blockers are clearly marked

### H-102 Build 1.0 playable package

Acceptance criteria:

- game can be launched from Unity build
- run can be completed
- no console errors during normal run
- build includes all required scenes and assets

## Suggested 1.0 Scope Lock

Include:

- Arcane school only as complete magic school
- Blink, Lunge, and Teleport as first movement set
- one arena
- 3 regular enemies
- 1 elite
- 1 boss
- 20-level progression target

Defer:

- full Fire implementation
- all remaining magic schools
- meta-progression
- save/load
- procedural arenas
- controller support
- web build optimization
- Steam/platform packaging

## Recommended Build Order

1. Unity project hygiene
2. first playable combat loop
3. XP and level-up
4. upgrade draft
5. movement slots
6. Magic Missile upgrades
7. remaining Arcane skills
8. wave ramp
9. boss
10. UI and feedback polish
11. balance pass
12. 1.0 build
