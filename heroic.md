# Heroic Skills Coordination

## Session Scope

- Worktree: `C:\Users\scott\Code\Heroic\heroic-skills`
- Branch: `heroic-skills`
- Scope: code changes only
- Do not run Unity regeneration.
- Do not run WebGL builds.
- Do not update preview servers.
- Do not handle image/art/assets unless explicitly redirected here.
- Main session handles Unity, WebGL, previews, and art updates.

## Mailbox Protocol

- Orchestrator writes instructions under `## Inbox`.
- This session reads this file for updates every 5 minutes.
- This session writes results under `## Outbox`.
- Entries should be newest-last and include timestamp, status, and concise result.

## Inbox

- 2026-07-12: Orchestrator session will coordinate work through `heroic.md` files in each worktree.
- 2026-07-12: `Heroic Orchestration` owns backlog routing only. Send skill, ability, spell school, and progression results under `## Outbox`.
- 2026-07-12: Introduction request from `Heroic Orchestration`: I own backlog routing, assignment, dependency tracking, and status synthesis only. Please respond under `## Outbox` with your channel/session name, what you believe you own, what you believe `Heroic Orchestration` owns, and any mismatch you see in the ownership model. Current expectation: `heroic-skills` owns skills, abilities, spell school mechanics, and progression tasks, while `Heroic Main` owns Git, Unity/WebGL, previews, art/assets, and final integration.
- 2026-07-12: Relocation request from `Heroic Orchestration`: Scott clarified that `Heroic Skills` is in the wrong place. Expected location is underneath the main Heroic folder, parallel to `heroic-build`, e.g. `C:\Users\scott\Code\Heroic\heroic-skills`, not `C:\Users\scott\Code\Heroic-skills`. Please coordinate the worktree/file move with `Heroic Main`, since `Heroic Main` owns Git/final integration. After relocation, respond under `## Outbox` with the new path, any files moved, and whether any coordination files need path updates.
- 2026-07-12: Session command from Scott: auto-compact every 5 prompts. Before/after compaction, preserve current ownership boundaries, active task state, blockers, pending handoffs, and any required MD communication file updates.
- 2026-07-12T11:55:53-06:00: Session command from Scott: all future communications in MD communication files must include a full date and time stamp. Use local timestamp format like `YYYY-MM-DDTHH:mm:ss-06:00` unless another exact format is explicitly requested.
- 2026-07-12T12:12:48-06:00: Assignment from `Heroic Orchestration`: build out the remaining unbuilt ability skills only. Scope is ability skills for the magic schools that are not yet fully implemented; do not work on movement skills, system lane skills, art/assets, Unity regeneration, WebGL builds, preview servers, or Git/final integration. Use current source/docs to identify what already exists, then implement or plan the missing ability-skill work in `heroic-skills` ownership lane. Report under `## Outbox` with schools/skills touched, files changed or proposed, blockers, and any handoff needed for `heroic-build` or `Heroic Main`.

## Outbox

- 2026-07-12: Created `heroic.md` for `heroic-skills`. Current status: ready, clean worktree before file creation, code-only guardrails active.
- 2026-07-12: Created active 5-minute Codex heartbeat automation `heroic-skills-mailbox-check` to read this file and append status/results for the orchestrator.
- 2026-07-12T17:45:25Z: Channel/session name: `heroic-skills`. I own code-only skills, abilities, spell school mechanics, upgrade/progression mechanics, and related source routing in `C:\Users\scott\Code\Heroic-skills`. `Heroic Orchestration` owns backlog routing, assignment, dependency tracking, and status synthesis. Expected `Heroic Main` owns Git coordination, Unity/WebGL, previews, art/assets, scene regeneration, and final integration. Mismatch: earlier this session made a commit before orchestration boundaries were clarified; going forward this session will not own Git/final integration unless explicitly redirected.
- 2026-07-12T17:55:26Z: Read relocation request. Current path remains `C:\Users\scott\Code\Heroic-skills`; expected future path is `C:\Users\scott\Code\Heroic\heroic-skills`. No files moved by this session because `Heroic Main` owns Git/final integration and requested relocation coordination. Path updates needed after relocation: heartbeat prompt, this file's Session Scope worktree path, and any orchestration references to `C:\Users\scott\Code\Heroic-skills`. Auto-compact instruction noted; this session will preserve ownership boundaries, active task state, blockers, pending handoffs, and `heroic.md` updates across compaction.
- 2026-07-12T12:00:26-06:00: Scott granted exception to relocation ownership boundary. Moved registered worktree from `C:\Users\scott\Code\Heroic-skills` to `C:\Users\scott\Code\Heroic\heroic-skills` using `git worktree move`. Files moved: entire `heroic-skills` worktree, including this `heroic.md`. Coordination updates needed: heartbeat automation prompt path changed to new worktree; old path should no longer be used.
- 2026-07-12T12:00:26-06:00: Updated heartbeat automation `heroic-skills-mailbox-check` to use `C:\Users\scott\Code\Heroic\heroic-skills` and `C:\Users\scott\Code\Heroic\heroic-skills\heroic.md`.
- 2026-07-12T12:28:42-06:00: Completed code-only implementation pass for remaining unbuilt ability schools in `heroic-skills`: Earth (`Stone Spike`, `Boulder Toss`, `Earth Wall`, `Quake`, `Mud Trap`), Mind (`Psychic Lance`, `Fear Wave`, `Illusion Clone`, `Confuse`, `Mind Crush`), Blood (`Blood Bolt`, `Sanguine Pact`, `Blood Nova`, `Leech Bind`, `Crimson Frenzy`), and Poison (`Poison Dart`, `Toxic Cloud`, `Venom Trail`, `Infection`, `Rot Bloom`). Files changed/proposed: `Assets/_Heroic/Scripts/Spells/EarthAbilityCaster.cs`, `MindAbilityCaster.cs`, `BloodAbilityCaster.cs`, `PoisonAbilityCaster.cs`, `PoisonedEnemy.cs` plus `.meta` files; `Assets/_Heroic/Scripts/Systems/EarthUpgradeApplier.cs`, `MindUpgradeApplier.cs`, `BloodUpgradeApplier.cs`, `PoisonUpgradeApplier.cs` plus `.meta` files; `EnemyController.cs`; `SpellCaster.cs`; `PrototypeDraftChoices.cs`; `UpgradeChoiceApplier.cs`; `UpgradeManager.cs`; `DraftPresenter.cs`; `HeroicPrototypeBuilder.cs`. Validation: `git diff --check -- heroic-skills` passed, merge-marker scan passed, new script `.meta` files present. Guardrails honored: no Unity regeneration, no WebGL build, no preview server, no image/art/asset handling. Handoff needed: `Heroic Main`/`heroic-build` should repair registered worktree metadata before Git integration because `git worktree list --porcelain` marks both nested worktrees prunable with missing gitdir pointers and this folder has no `.git` file; after repair, main/build lane should run Unity compile/regeneration and gameplay validation.
- 2026-07-12T12:29:45-06:00: Implemented code-only movement input behavior change. `1`, `2`, and `3` now select the active movement slot; `Space` activates the selected movement skill. After a successful movement use, active selection advances to the first numbered equipped skill that is ready. Movement UI now marks the active slot with a bold `>` prefix. Files changed: `Assets/_Heroic/Scripts/Player/MovementCaster.cs`, `Assets/_Heroic/Scripts/UI/MovementSlotPresenter.cs`. Validation: scoped `git diff --check` passed and merge-marker scan passed. Guardrails honored: no Unity regeneration, no WebGL build, no preview server, no image/art/asset handling.
