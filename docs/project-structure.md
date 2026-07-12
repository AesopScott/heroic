# Heroic Project Structure

## Root Layout

```text
Heroic/
  Assets/
    _Heroic/
      Art/
      Audio/
      Prefabs/
      Scenes/
      Scripts/
      ScriptableObjects/
      UI/
  docs/
  README.md
  Heroic.code-workspace
```

## Script Layout

```text
Scripts/
  Core/
  Combat/
  Data/
  Enemies/
  Player/
  Spells/
  Systems/
  UI/
  Utilities/
```

## ScriptableObject Layout

```text
ScriptableObjects/
  Schools/
  Skills/
  Movement/
  Upgrades/
  Enemies/
  Waves/
```

## Scene Layout

```text
Scenes/
  MainMenu
  Game
  Results
```

## Notes

- Keep game logic data-driven
- Keep the Unity assets under `Assets/_Heroic`
- Keep design docs in `docs/`
