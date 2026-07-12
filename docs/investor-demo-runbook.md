# Heroic Investor Demo Runbook

## Pre-Demo Command

Run this from `C:\Users\scott\Code\Heroic`:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\pre-demo-check.ps1
```

The script builds the Unity WebGL version, starts a local gzip-aware server, verifies Unity WebGL headers, and prints the URL.

The WebGL page now uses the branded Heroic shell. If the default white Unity footer appears, rebuild before showing the demo.

For a quick server-only check after a known-good build:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\pre-demo-check.ps1 -SkipBuild
```

## Optional Music Drop-In

If a Suno track is ready before the demo, place it here before running the pre-demo command:

`Assets/_Heroic/Resources/Audio/Music/HeroicDemoLoop.wav`

The build will auto-load it. If the file is missing, the demo stays silent except for procedural SFX.

Current candidate: `Shadows to Dawn.mp3` has been copied into the project as `Assets/_Heroic/Resources/Audio/Music/HeroicDemoLoop.mp3`.

## Show, Don't Explain

Target the first two minutes:

- "You are a living spellbook."
- "Magic Missile starts simple and mutates through draft choices."
- "Movement skills are strategic picks, not just dodges."
- "The Warden is the two-minute promise of escalation."

## Exact Flow

1. Open the URL printed by `pre-demo-check.ps1`.
2. Click `Start Run`.
3. Move immediately with `WASD` or arrow keys.
4. Press `1`, `2`, and `3` once to show Blink, Lunge, and Teleport.
5. Let XP pull into the player and take the first draft choice.
6. Say: "This is the living spellbook leveling itself mid-run."
7. Keep moving until the boss/result sequence.
8. Use the polished result screen as the closing beat.

## Safety Keys

- `F8`: force polished defeat result.
- `F9`: force polished victory result.
- `M`: mute/unmute background music if the room mix is wrong.
- `-` / `+`: lower/raise master volume.
- `Esc`: pause/resume with a visible pause overlay.

Use these only if the live run stalls, the room needs a quick end-state, or you need to demonstrate both result panels quickly. They work during active play, pause, and level-up draft screens.

## Do Not Do

- Do not apologize for placeholder graphics. Say "procedural art pass for 1.0 prototype" if asked.
- Do not overexplain systems before the first draft appears.
- Do not hunt for XP manually; the pickup magnet is intentionally forgiving for demo reliability.
- Do not keep playing after the result screen unless the viewer asks.

## Success Criteria

- The title screen loads.
- Start enters gameplay.
- HUD is readable.
- XP pulls in and opens a draft.
- Draft selection resumes gameplay.
- Movement skills are visible.
- The result screen is polished.
- Browser console stays clean for the current served build.
