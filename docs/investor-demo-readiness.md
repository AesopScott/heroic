# Investor Demo Readiness

## Expert Question

If I were an expert in game design and marketing and about to show this to a potential investor, what would I make work before turning the game on?

## Answer

The demo must communicate the fantasy in under two minutes:

- you are a living spellbook
- Arcane magic starts simple and rapidly mutates
- movement is a strategic power choice
- enemies create pressure quickly
- the screen is readable
- the boss gives the run a clear promise of escalation

## What Was Added

- `InvestorShowcaseMode`
- faster level pacing for demo mode
- starting Arcane showcase loadout
- Blink, Lunge, and Teleport equipped from the start
- boss spawn pulled forward to two minutes
- visible `Heroic 1.0 Showcase` label
- procedural placeholder graphics
- hit flashes, death bursts, movement flashes, and Arcane area flashes
- `Heroic/Validate 1.0 Prototype` readiness command
- batch-mode 1.0 runner at `scripts/run-unity-1.0.ps1`
- generated Unity 6 scenes, prefabs, ScriptableObjects, and WebGL build
- clearer level-up draft buttons with category, name, and description
- included TextMesh Pro essentials so WebGL text renders without missing-font warnings
- persistent button listeners so Start Run, Restart, and Quit wiring survives scene serialization
- top-anchored health and XP HUD bars so core combat space stays readable
- WebGL smoke server for correct Unity gzip headers during local browser testing
- short WebGL browser smoke test proving menu, start flow, gameplay, movement input, and combat visuals
- in-game objective panel that explains the demo goal, boss countdown, and next draft XP progress
- polished results screen with stronger victory/defeat copy, run-time label, summary copy, and clearer buttons
- two-minute WebGL smoke test proving the showcase run reaches the polished victory screen at `02:00` with a clean current-port console
- forgiving XP pickup attraction so the first spellbook draft reliably appears during the early demo
- active WebGL smoke test proving a draft appears, a choice can be clicked, gameplay resumes at Level 2, and XP progress updates
- hidden F8/F9 prototype safety hotkeys for forcing defeat/victory during gameplay if QA or a live demo needs a quick end-state check
- WebGL smoke test proving F8 reaches the polished defeat screen with a clean current-port console
- results buttons load scenes directly so `Run It Back` starts a fresh Game run and `Main Menu` returns to the generated MainMenu scene
- WebGL smoke test proving both results buttons route correctly with a clean current-port console
- `scripts/pre-demo-check.ps1` builds, serves, verifies Unity WebGL headers, and prints the exact local demo URL plus controls
- `docs/investor-demo-runbook.md` captures the repeatable two-minute flow, talk track, and F8/F9 safety path
- drop-in background music hook for a Suno-generated `HeroicDemoLoop` track, with first-input retry for WebGL autoplay resilience
- `Shadows to Dawn.mp3` imported as the current `HeroicDemoLoop.mp3` music candidate
- `docs/music-brief-suno.md` captures the exact music prompt, direction, and Unity asset path
- current music build smoke passed: title loads, Start Run enters gameplay, movement input works, an early draft appears, and the current-port browser console is clean
- owned C# warning noise cleaned up for the 1.0 build log
- safety hotkeys now work even if the run is paused or a level-up draft is open, so demo recovery does not depend on exact gameplay state
- latest safety smoke confirms F9 from an open draft reaches `ARCANE WARDEN DEFEATED` with a clean current-port browser console
- custom branded WebGL shell replaces the default Unity footer, so the browser presentation reads as Heroic instead of a stock engine template
- latest branded-shell smoke confirms title/menu view, gameplay start, movement input, cooldown UI, and clean current-port browser console
- demo audio recovery controls added: `M` toggles music and `-` / `+` adjust master volume from menu, gameplay, or results
- latest audio-control smoke confirms the keys are visible on the menu, safe to press in menu/gameplay, and the current-port browser console stays clean
- visible pause overlay added so the advertised `Esc` key has clear feedback and recovery controls
- latest pause smoke confirms Esc pause/resume works visually and cleanly in the branded WebGL build
- camera shake impact feedback added for key beats so movement, damage, and boss moments read with more physicality
- latest camera feedback smoke confirms movement and result flow remain clean after the shake layer
- enemy silhouettes strengthened by role so basic, fast, tank, and boss enemies read more distinctly in the procedural art pass
- latest enemy-readability smoke confirms gameplay/draft/resume remain clean after the visual preset pass
- release source-control hygiene added so Unity scenes/prefabs stay reviewable and binary media stays binary
- release WebGL build passed through the full 1.0 pipeline
- release pre-demo check passed from `http://127.0.0.1:5211/`
- release browser smoke confirms the branded menu, early gameplay, movement cooldowns, XP counter/fill, level-up draft, draft selection/resume, pause overlay, F9 victory recovery, and current-port browser console are clean

## Pre-Demo Checklist

Do not show the build until all of these are true:

- Unity imports the project without script errors.
- `.\scripts\run-unity-1.0.ps1` completes successfully.
- `Heroic/Build 1.0 Prototype Content` completes.
- `Heroic/Validate 1.0 Prototype` passes.
- WebGL build exists at `Builds/WebGL`.
- WebGL build loads through `scripts/serve-webgl.py` or equivalent gzip-aware hosting.
- WebGL shell uses the branded Heroic frame with no default Unity footer.
- MainMenu Start Run enters the Game scene.
- Game scene enters Play Mode or browser gameplay.
- Player movement feels responsive.
- Magic Missile visibly targets and kills enemies.
- Arcane Blast and Warp Pulse visibly fire.
- Arcane Orbit is visible.
- Blink, Lunge, and Teleport are visible and understandable.
- Movement and impact feedback have visible punch without obscuring the HUD.
- XP pickup and level-up happen within the first minute.
- Draft UI appears and can be clicked.
- Draft selection returns to gameplay and visibly advances the run.
- Boss appears around the two-minute mark.
- Boss defeat triggers victory.
- Victory uses the polished `ARCANE WARDEN DEFEATED` results panel.
- Player death triggers defeat.
- F8 can force the polished defeat state during prototype QA/demo recovery.
- HUD is readable at a glance.
- Health and XP bars are not crossing the player or center playfield.
- Objective panel is readable and does not block core combat.
- The arena does not look blank.
- Browser console is clean for the current served build.
- Restart and Main Menu buttons have been checked in the current served build.
- Optional: if music is ready, `Assets/_Heroic/Resources/Audio/Music/HeroicDemoLoop.mp3` or `.wav` is present and audible.
- Audio recovery keys work: `M` mutes music and `-` / `+` adjust master volume.
- `Esc` shows a visible pause overlay and resumes gameplay.
- `.\scripts\pre-demo-check.ps1` passes immediately before the demo.
- The presenter has `docs/investor-demo-runbook.md` open or printed.

## Demo Flow

Use `docs/investor-demo-runbook.md` as the canonical flow.

## Do Not Overexplain

The investor should see:

- movement decisions
- spell growth
- readable pressure
- escalating chaos
- a clear path from prototype to product

The game should carry most of the explanation.
