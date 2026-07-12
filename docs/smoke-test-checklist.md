# Heroic Smoke Test Checklist

## Editor Import

- Unity `6000.5.3f1` or another Personal-license-compatible Editor is installed.
- Unity opens the project without script import errors.
- `Assets/_Heroic` is visible.
- `Heroic/Build 1.0 Prototype Content` can be run from the menu.
- `Heroic/Validate 1.0 Prototype` passes after content generation.
- Scenes can be opened.
- No required package is missing.

## Game Scene

- `Game` scene loads.
- Arena backdrop is visible.
- `Heroic 1.0 Showcase` label is visible in showcase mode.
- Player appears at spawn.
- Player sprite is visually distinct from enemies and pickups.
- Camera shows the player.
- Player moves with keyboard input.
- Player cannot rotate or fall due to physics.

## Combat

- Enemies spawn around the player.
- Enemies have readable placeholder sprites.
- Enemies chase the player.
- Enemies damage player on contact.
- Contact damage has a cooldown.
- Enemies flash when hit.
- Enemies create a burst on death.
- Magic Missile auto-targets nearest enemy.
- Magic Missile projectiles move correctly.
- Magic Missile trails or motion are visually readable.
- Magic Missile damages enemies.
- Enemies die at zero health.

## XP And Leveling

- Enemies drop XP pickups.
- XP pickups are easy to see against the arena.
- XP pickups can be collected.
- Player XP increases.
- Player levels up when threshold is reached.
- Level-up opens an upgrade draft.
- Gameplay pauses while draft is open.
- Gameplay resumes after a draft choice is selected.
- Even-numbered levels include movement draft eligibility.

## Arcane Skills

- Magic Missile can be active from run start.
- Arcane Blast can be enabled by skill id.
- Warp Pulse can be enabled by skill id.
- Spell Echo support can repeat a provided cast action.
- Arcane Orbit spawns orbiting damage objects.
- Arcane Double Cast can repeat Arcane casts.

## Movement

- Key `1` activates slot 1.
- Key `2` activates slot 2.
- Key `3` activates slot 3.
- Blink repositions the player.
- Blink has departure and arrival feedback.
- Lunge moves the player forward and can damage enemies.
- Lunge has visible travel feedback.
- Teleport repositions farther than Blink.
- Teleport has departure and arrival feedback.
- Each movement slot has its own cooldown.

## Run End

- Player reaches zero health.
- Death can transition to results or end state.
- Restart works.
- Quit to menu works.

## Waves And Boss

- Active wave data can spawn enemy definitions.
- Enemy definitions apply health, move speed, contact damage, and XP value.
- Boss spawns at configured run time.
- Boss is clearly visually distinct from normal enemies.
- Boss pulse pattern damages player.
- Boss surge pattern changes pressure.
- Boss death triggers victory.

## 1.0 Gate

- A full run can be started.
- A full run can be completed.
- A full run can be lost.
- No console errors appear during normal play.
- Investor demo checklist in `docs/investor-demo-readiness.md` passes before showing externally.
