# Playable Loop Prefabs

## Goal

Create the smallest Unity setup that proves the game loop:

- player moves
- enemies spawn
- enemies chase player
- Magic Missile auto-targets nearest enemy
- projectile damages enemies

## Game Scene Objects

Create these objects in the `Game` scene.

### `GameManagers`

Components:

- `RunManager`
- `RunBootstrapper`
- `RunEndWatcher`
- `PauseInputHandler`
- `GameStateManager`
- `EnemySpawner`
- `EnemyDirector`
- `BossSpawner`
- `UpgradeManager`
- `UpgradeChoiceApplier`
- `UIManager`

Inspector wiring:

- `EnemySpawner.enemyPrefab`: assign `Enemy_Basic`
- `EnemySpawner.playerTarget`: assign the `Player` transform
- `EnemySpawner.spawnRadius`: start at `8`
- `EnemySpawner.spawnInterval`: start at `2`
- `BossSpawner.spawnAtSeconds`: start at `600` for a 10-minute test run

### `Main Camera`

Components:

- `Camera`
- `CameraFollow2D`

Recommended settings:

- `Camera.orthographic`: enabled
- `Camera.orthographicSize`: `7`
- `CameraFollow2D.target`: assign the `Player` transform

### `Player`

Components:

- `Rigidbody2D`
- `CircleCollider2D`
- `VisualPresetApplier`
- `PlayerController`
- `PlayerHealth`
- `PlayerExperience`
- `SpellCaster`
- `ArcaneDoubleCast`
- `MagicMissileCaster`
- `ArcaneBlastCaster`
- `WarpPulseCaster`
- `SpellEchoCaster`
- `ArcaneOrbitCaster`
- `MovementCaster`

Recommended settings:

- `Rigidbody2D.gravityScale`: `0`
- `Rigidbody2D.freezeRotation`: enabled
- `CircleCollider2D.isTrigger`: disabled
- `VisualPresetApplier.preset`: `Player`
- `PlayerController.moveSpeed`: `6`
- `PlayerHealth.maxHealth`: `100`
- `PlayerExperience.baseExperienceToLevel`: `10`
- `MagicMissileCaster.castInterval`: `0.75`
- `MagicMissileCaster.projectileSpeed`: `12`
- `MagicMissileCaster.damage`: `10`
- `MagicMissileCaster.projectilePrefab`: assign `Projectile_MagicMissile`

Optional child:

- `FirePoint`
- place at local position `0.35, 0, 0`
- assign to `MagicMissileCaster.firePoint`

### `Enemy_Basic`

Create as a prefab under `Assets/_Heroic/Prefabs/Enemies`.

Components:

- `Rigidbody2D`
- `CircleCollider2D`
- `VisualPresetApplier`
- `Damageable`
- `EnemyController`
- `ExperienceDropper`
- `BossController` only for boss prefab variants

Recommended settings:

- `Rigidbody2D.gravityScale`: `0`
- `Rigidbody2D.freezeRotation`: enabled
- `CircleCollider2D.isTrigger`: disabled
- `Damageable.health`: `30`
- `EnemyController.moveSpeed`: `2`
- `EnemyController.contactDamage`: `10`
- `EnemyController.contactRange`: `0.5`
- `ExperienceDropper.experienceValue`: `1`
- `VisualPresetApplier.preset`: `BasicEnemy`

### `XP_Pickup`

Create as a prefab under `Assets/_Heroic/Prefabs/Pickups`.

Components:

- `CircleCollider2D`
- `VisualPresetApplier`
- `ExperiencePickup`

Recommended settings:

- `CircleCollider2D.isTrigger`: enabled
- `ExperiencePickup.experienceValue`: `1`
- `ExperiencePickup.magnetRange`: `7`
- `ExperiencePickup.magnetSpeed`: `11`
- `VisualPresetApplier.preset`: `ExperiencePickup`

The larger pickup attraction range is intentional for 1.0 demo reliability: early XP should visibly pull into the living spellbook so the first draft appears without awkward pixel-hunting.

### `HUD`

Create a canvas with:

- health slider
- XP slider
- level text
- timer text
- health text
- three movement slot presenters

Components:

- `HudPresenter`
- `MovementSlotPresenter` for each movement slot

### `Draft UI`

Create a canvas panel with:

- header text
- 3 to 5 choice buttons
- text label under each choice button

Components:

- `DraftPresenter`

### `Results UI`

Create a canvas panel with:

- result text
- survival time text
- restart button
- quit to menu button

Components:

- `ResultsPresenter`

### `Projectile_MagicMissile`

Create as a prefab under `Assets/_Heroic/Prefabs/Projectiles`.

Components:

- `CircleCollider2D`
- `VisualPresetApplier`
- `Projectile`
- `ProjectileHit`

Recommended settings:

- `CircleCollider2D.isTrigger`: enabled
- `Projectile.speed`: `12`
- `ProjectileHit.damage`: `10`
- `ProjectileHit.lifetime`: `5`
- `VisualPresetApplier.preset`: `MagicMissile`

## Camera

For the first pass:

- use an orthographic camera
- set `orthographicSize` to `7`
- position at `0, 0, -10`

Camera follow can come after the first loop works.

## First Test

Press Play in the `Game` scene.

Expected result:

- enemies spawn around the player
- enemies chase the player
- player auto-fires Magic Missile
- missiles destroy enemies after enough hits
