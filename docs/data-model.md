# Heroic Data Model

## Goal

Store the game as data assets first, then have runtime systems read from those assets.

## ScriptableObject Types

- `MagicSchoolDefinition`
- `SkillDefinition`
- `UpgradePathDefinition`
- `MovementSkillDefinition`
- `EnemyDefinition`
- `WaveDefinition`
- `SchoolDatabase`
- `SkillDatabase`
- `MovementDatabase`

## What They Represent

- School definitions hold the identity and value proposition for each school.
- Skill definitions hold the 5 skills per school.
- Upgrade path definitions hold the 3 path choices per skill.
- Movement skill definitions hold movement skill metadata.
- Enemy definitions hold enemy stats and behavior tags.
- Wave definitions hold spawn composition.

## Design Rule

If something is going to be tuned often, it should be represented as data first.
