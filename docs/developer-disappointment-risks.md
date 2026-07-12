# Developer Disappointment Risks

## Question

If anything would upset the developer for this 1.0 build, what would it be?

## Highest-Risk Answers

### 1. It does not open or compile in Unity

Prevention:

- Added Unity project hygiene files.
- Added `Heroic/Build 1.0 Prototype Content`.
- Added `Heroic/Validate 1.0 Prototype`.

Remaining gate:

- Must be opened in Unity 2022.3 LTS or newer and validated in-editor.

### 2. The screen looks blank, cheap, or confusing

Prevention:

- Added procedural sprites for player, enemies, boss, projectiles, orbitals, and XP.
- Added arena backdrop.
- Added trails and area flashes.
- Added hit flashes and death bursts.

Remaining gate:

- Must inspect the generated Game scene in Play Mode and confirm readability.

### 3. Upgrades feel fake

Prevention:

- Added prototype draft pool.
- Added five-tier Arcane upgrade application.
- Wired Magic Missile, Arcane Blast, Warp Pulse, Spell Echo, and Arcane Orbit upgrades into runtime values.

Remaining gate:

- Tune values in Play Mode.

### 4. Movement feels like a gimmick

Prevention:

- Added Blink, Lunge, and Teleport as real slot-based movement skills.
- Added movement visuals and cooldown tracking.

Remaining gate:

- Tune cooldowns, ranges, and collision feel in Play Mode.

### 5. There is no clear path to prove 1.0 is ready

Prevention:

- Added smoke test checklist.
- Added known issues doc.
- Added 1.0 progress doc.
- Added editor readiness validator.

Remaining gate:

- Run the validator and complete the smoke checklist without blockers.

## 1.0 Refusal Criteria

Do not call the build 1.0 if any of these are true:

- Unity import has script errors.
- Game scene cannot be generated.
- Game scene cannot enter Play Mode.
- Player cannot move.
- Magic Missile cannot kill enemies.
- Enemies cannot damage or kill the player.
- XP and level-up do not work.
- Upgrade draft does not appear.
- Blink, Lunge, and Teleport do not activate from keys `1`, `2`, and `3`.
- Boss does not spawn.
- Boss death does not trigger victory.
- Player death does not trigger defeat.
- The game screen is visually unclear.
- The HUD/draft/results UI is unreadable.
