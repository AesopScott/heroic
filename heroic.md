# Heroic Orchestration

## Session Scope

- Worktree: `C:\Users\scott\Code\Heroic`
- Session name: `Heroic Orchestration`
- Scope: backlog routing, assignment, dependency tracking, and status synthesis only.
- Do not create game content.
- Do not make code changes.
- Do not do Git work.
- Do not run Unity regeneration.
- Do not run WebGL builds.
- Do not update preview servers.
- Do not handle image/art/assets unless explicitly redirected here.
- `Heroic Main` owns Git, Unity, WebGL, previews, art, and final integration.

## Worktree Routing

- `C:\Users\scott\Code\Heroic-skills`: skills, abilities, spell school mechanics, progression tasks.
- `C:\Users\scott\Code\Heroic\heroic-build`: UI and game logic implementation tasks.
- `C:\Users\scott\Code\Heroic`: orchestration plus `Heroic Main` ownership for Git, art, Unity/WebGL, previews, and integration.

## Mailbox Protocol

- Orchestrator writes assignments under each worktree's `## Inbox`.
- Worker sessions write results under `## Outbox`.
- A local watcher script at `scripts/watch-heroic-mailboxes.ps1` reads all `heroic.md` mailboxes every 5 minutes and appends a heartbeat result.
- Entries should be newest-last and include timestamp, target session, status, and concise result.

## Watched Mailboxes

- `C:\Users\scott\Code\Heroic\heroic.md`
- `C:\Users\scott\Code\Heroic-skills\heroic.md`
- `C:\Users\scott\Code\Heroic\heroic-build\heroic.md`

## Inbox

- 2026-07-12: Orchestrator session established. Awaiting backlog items to route to `heroic-skills`, `heroic-build`, and `Heroic Main`.
- 2026-07-12: Introduction request from `Heroic Orchestration`: I own backlog routing, assignment, dependency tracking, and status synthesis only. Please respond under `## Outbox` with your channel/session name, what you believe you own, what you believe `Heroic Orchestration` owns, and any mismatch you see in the ownership model. Current expectation: `Heroic Main` owns Git, Unity, WebGL, previews, art/assets, UI polish, and final integration.
- 2026-07-12: Session command from Scott: auto-compact every 5 prompts. Before/after compaction, preserve current ownership boundaries, active task state, blockers, pending handoffs, and any required MD communication file updates.
- 2026-07-12T11:55:53-06:00: Session command from Scott: all future communications in MD communication files must include a full date and time stamp. Use local timestamp format like `YYYY-MM-DDTHH:mm:ss-06:00` unless another exact format is explicitly requested.

## Outbox

### Introduction

- 2026-07-12T11:58:00-06:00: Session name `Heroic Orchestration`; I own backlog routing, assignment, dependency tracking, and status synthesis only. `Heroic Main` owns Git, Unity, WebGL, previews, art/assets, UI polish, and final integration. Mismatch noted: I should not be treated as the owner of git pushes or build execution.

### Status

- 2026-07-12T11:58:00-06:00: Created orchestration mailbox and routing contract.
- 2026-07-12T11:58:00-06:00: Recent updates captured for Obsidian: Crash I now uses `Assets/mobs/Crash I.png` through a new `CrashSpriteAnimator`, the Crash prefab was rewired to that sprite path, the prototype builder now preserves the same setup, and the old Crash Level I procedural triangle visual was removed.
- 2026-07-12T11:58:00-06:00: Recent design/docs context captured for Obsidian: Cold and Lightning tree notes live in `docs/cold-tree.md`, `docs/lightning-tree.md`, and `docs/magic-schools-master.md`, so the vault reflects the current school taxonomy alongside the runtime enemy/art changes.
- 2026-07-12T11:58:00-06:00: Orchestration heartbeat support added at `scripts/watch-heroic-mailboxes.ps1` to re-read mailbox notes on a 5-minute cadence and append timestamped heartbeats.
- 2026-07-12T11:58:00-06:00: Update pass completed; current worktree still has pending Crash/art/orchestration edits and no new WebGL/Unity build has been run in this turn.
- 2026-07-12T12:32:00-06:00: Crash ladder expanded in Unity: Crash I through Crash V are now defined, Crash IV and Crash V use the new texture-backed path, and Wall I now has its own prefab and definition.
- 2026-07-12T12:32:00-06:00: Unity/WebGL regeneration completed successfully after the new mob wiring; local browser host remains `http://127.0.0.1:5239/`.
- 2026-07-12T13:05:00-06:00: Shooter naming was renamed to Thrower across the game content, including `Enemy_Thrower`, `Enemy_Thrower_01`, `ThrowerLevel1`, and Thrower I art at `Assets/mobs/thrower I.png`.
- 2026-07-12T13:05:00-06:00: Unity/WebGL build completed successfully after the Thrower rename; local browser host remains `http://127.0.0.1:5239/`.
- 2026-07-12T13:18:00-06:00: Player visuals now switch from Player I at level 1 to Player II at level 2+, and the player tint follows the selected school color with Boost choices treated as Arcane.
- 2026-07-12T13:18:00-06:00: Unity/WebGL rebuild completed successfully after the player visual update; local browser host remains `http://127.0.0.1:5239/`.
- 2026-07-12T13:28:00-06:00: Player robe tint now accumulates and blends every gathered school or boost color instead of replacing the previous color, so the robe evolves with the full build.
- 2026-07-12T13:28:00-06:00: Unity/WebGL rebuild completed successfully after the robe blend update; local browser host remains `http://127.0.0.1:5239/`.
- 2026-07-12T13:38:00-06:00: Player level 6 now uses `Assets/mobs/Player VI.png` while preserving the accumulated robe tint, so the art can add the beard without changing the color mixing behavior.
- 2026-07-12T13:38:00-06:00: Unity/WebGL rebuild completed successfully after the level 6 player art update; local browser host remains `http://127.0.0.1:5239/`.
- 2026-07-12T13:48:00-06:00: Player visuals now cycle the 4-frame player sheets instead of stacking all art at once, Crash sprites face the player horizontally, and the mob/player PNGs were converted to transparent cutouts from their white backgrounds.
- 2026-07-12T13:48:00-06:00: Unity/WebGL rebuild completed successfully after the sprite-cycle and cutout update; local browser host remains `http://127.0.0.1:5239/`.
