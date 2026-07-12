# Heroic Known Issues

## Blockers For 1.0 Prototype

- No known launch-blocking issues after the release validation and WebGL smoke on `2026-07-11`.
- The build is suitable for an internal/investor prototype demo if launched through `scripts/pre-demo-check.ps1` and presented with `docs/investor-demo-runbook.md`.

## High Priority

- Arcane visual feedback is procedural placeholder-only, but now passable for the 1.0 prototype demo.
- Movement cooldown UI is generated, validator-checked, and browser-smoked.
- Upgrade draft presenter is generated, validator-checked, and browser-smoked through selection/resume.
- ScriptableObject assets are generated and validated, but still deserve manual inspection before deeper content expansion.
- Enemy waves are generated and demo-safe, but still need longer balance testing beyond the investor prototype path.
- Editor bootstrapper and validator now run successfully through `scripts/run-unity-1.0.ps1`.
- Current WebGL smoke test passes: menu loads, Start Run enters gameplay, HUD bars are separated and top-anchored, XP displays `current/required` and starts empty at `0/5`, movement skill input works, and the current-port browser console is clean.
- Current two-minute WebGL smoke test passes: hands-off showcase run reaches the polished `ARCANE WARDEN DEFEATED` results screen at `02:00` with a clean current-port browser console.
- Current active draft-flow smoke test passes: forgiving XP pickup attraction opens a draft, clicking a choice resumes gameplay at Level 2, XP updates, and the current-port browser console is clean.
- Current defeat-flow smoke test passes: hidden F8 prototype safety hotkey reaches the polished `SPELLBOOK SHATTERED` results screen with a clean current-port browser console.
- Current results-button smoke test passes: `Run It Back` reloads gameplay, `Main Menu` returns to the title screen, and the current-port browser console is clean.
- Current pre-demo script smoke test passes: `scripts/pre-demo-check.ps1` builds, serves, validates headers, prints the demo URL, and the served build loads/starts with a clean current-port browser console.
- Current warning/safety smoke passes: owned C# warning noise is cleaned up, and F9 can force victory from an open level-up draft with a clean current-port browser console.
- Current branded-shell smoke passes: the WebGL page uses the custom Heroic frame, no default Unity footer, and gameplay starts cleanly from the branded shell.
- Current audio recovery wiring is validator-checked: `M` toggles music and `-` / `+` adjust master volume across menu, gameplay, and results.
- Current audio-control smoke passes: menu and gameplay tolerate `M`, `-`, and `+` with no current-port console errors.
- Current pause overlay wiring is validator-checked so `Esc` no longer pauses invisibly.
- Current pause smoke passes: Esc shows the pause panel, Esc resumes gameplay, and the current-port console stays clean.
- Camera shake impact feedback is wired and validator-checked for movement, player damage, boss spawn, and boss death.
- Current camera feedback smoke passes: movement/cooldowns and F9 victory remain clean after camera shake wiring.
- Enemy role silhouettes have been strengthened in procedural presets; current smoke confirms the runtime path remains clean.
- Current release smoke passes from `http://127.0.0.1:5211/?v=release-smoke`: branded menu, Start Run, movement slots, cooldowns, XP counter/fill, early draft, draft selection/resume, pause overlay, F9 victory, and current-port console all stayed clean.
- Source-control hygiene is in place: Unity Force Text serialization is enabled, `.gitattributes` marks Unity YAML assets as text with `unityyamlmerge`, binary media stays binary, and local git is configured to Unity's SmartMerge executable.

## Medium Priority

- Arcane upgrade tiers are wired to runtime fields, but values still need Unity play-mode tuning.
- Upgrade draft choices use a prototype runtime pool until ScriptableObject assets are authored.
- Source-level C# compile passes, Unity `6000.5.3f1` imports the project, content generation passes, validation passes, WebGL builds, and short browser smoke testing passes. This still does not replace longer balance testing.
- Procedural audio feedback exists, but volumes and tone shapes need play-mode tuning.
- World-space health bars and floating damage numbers exist, but size, readability, and density need play-mode tuning.
- Basic, fast, tank, and boss enemies now have stronger distinct runtime visual presets; final readability still needs longer in-editor wave testing.
- The objective panel explains demo goal, boss countdown, and next-draft XP progress; final HUD density should be judged in the human-guided rehearsal.

## Low Priority

- Imported music candidate is present as `Assets/_Heroic/Resources/Audio/Music/HeroicDemoLoop.mp3`; duration is about `2:19`, so it should not loop during the target two-minute demo. Browser smoke confirms no current-port console errors, and presenter audio recovery keys exist if the room mix needs adjustment.
- Impact feedback exists through enemy visual variation, hit flashes, death bursts, movement flashes, Arcane area flashes, camera shake, health bars, and floating damage numbers; still needs play-mode tuning.
- Camera follow exists as `CameraFollow2D` and is wired in the generated scene.
- Results presenter exists and is wired in the generated scene.
