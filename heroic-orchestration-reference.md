# Heroic Orchestration Reference

Date: 2026-07-12
Owner: Heroic Orchestration

## Purpose

This file is the main-session working reference for backlog routing. Durable product memory lives in `G:\My Drive\heroic\soul.md`.

## File Ownership

Heroic Orchestration owns only:

- `G:\My Drive\heroic\soul.md`
- `C:\Users\scott\Code\Heroic\heroic-orchestration-reference.md`

Other Heroic sessions should not treat these as their editable ownership surface unless Scott explicitly redirects them.

## Project Soul

Heroic is a browser-playable bullet heaven where the player is a living spellbook, surviving pressure by mutating spells, choosing movement as power, and turning a simple run into escalating magical authorship.

## Routing Rules

- `Heroic Orchestration`: backlog routing, assignment, dependency tracking, status synthesis.
- `heroic-skills`: skills, abilities, spell school mechanics, movement designs, progression details, balance intent. Expected worktree location is under main Heroic, parallel to `heroic-build`: `C:\Users\scott\Code\Heroic\heroic-skills`.
- `heroic-build`: UI and game logic implementation tasks.
- `Heroic Main`: Git, Unity, WebGL, previews, art/assets, UI polish, final integration.

## Current Playable Truth

- 1.0 internal/investor prototype has no known launch blocker in reviewed notes.
- Arcane is the baseline school.
- Fire is partly playable through Fire Bolt, Flame Wave, and Burning Ground.
- Blink, Lunge, Teleport, Whirlwind, and Cloud Walk exist in current references.
- Territory Casting is the first standalone system lane choice.
- Loot drops include health restore, XP boost, speed boost, and invulnerability.
- Current enemies include Crash I-V, Wall I, Thrower I, and Arcane Warden; crash and thrower notes should reference the actual art filenames in Obsidian.
- Remaining product work centers on balance, school breadth, defense lane, burn/status depth, visual polish, and longer run tuning.

## Coordination Status

- 2026-07-12T11:58:07-06:00: Scott reported `heroic-build` automatic polling was previously not working and is now working.

## Backlog Bias

- Protect the playable demo loop before adding breadth.
- Add skills only when they preserve distinct school identity.
- Keep movement as a meaningful build lane, not utility filler.
- Favor data-driven implementation paths.
- Make build identity visible in UI/art.

## Source Memory

Primary durable file created this turn:

- `G:\My Drive\heroic\soul.md`

Important reviewed sources:

- `G:\My Drive\heroic\Heroic - Session Notes.md`
- `G:\My Drive\heroic\reference\abilities-current.md`
- `G:\My Drive\heroic\reference\movement-skills-current.md`
- `G:\My Drive\heroic\reference\spell-systems-current.md`
- `G:\My Drive\heroic\reference\enemies-current.md`
- `G:\My Drive\heroic\reference\loot-drops-current.md`
- `C:\Users\scott\Code\Heroic\docs\1.0-progress.md`
- `C:\Users\scott\Code\Heroic\docs\known-issues.md`
- `C:\Users\scott\Code\Heroic\docs\investor-demo-readiness.md`
- `C:\Users\scott\Code\Heroic\heroic.md`
- `C:\Users\scott\Code\Heroic\heroic-build\heroic.md`
- `C:\Users\scott\Code\Heroic-skills\heroic.md`

## Note

No Git, Unity regeneration, WebGL build, preview update, code change, or asset handling was performed for this evaluation.


## Current Art Truth

- 2026-07-12T13:40:14-06:00: Player and mob visuals now use explicit cropped frame assets instead of sheet slicing. Player level 1/2/6 use four frame exports; Crash and Thrower/Wall use two frame exports; crash shields still face the player horizontally.
- 2026-07-12T13:40:14-06:00: Player and mob art were rebuilt with clean single-frame sprite exports; later build sessions must use a fresh port rather than reusing prior WebGL sessions.
- 2026-07-12T14:16:09-06:00: Movement skill UI now uses square icon tiles instead of rectangular text windows, with cooldown tint handled by icon graying instead of an overlay.
- 2026-07-12T14:16:09-06:00: Current local WebGL session for this rebuild is http://127.0.0.1:5244/ and responded with HTTP 200.
