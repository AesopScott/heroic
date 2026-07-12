using Heroic.Combat;
using Heroic.Core;
using Heroic.Data;
using Heroic.Enemies;
using Heroic.Audio;
using Heroic.Player;
using Heroic.Spells;
using Heroic.Systems;
using Heroic.UI;
using Heroic.Visuals;
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heroic.Editor
{
    public static class HeroicReadinessValidator
    {
        private const string Root = "Assets/_Heroic";

        public readonly struct ValidationSummary
        {
            public ValidationSummary(int errors, int warnings)
            {
                Errors = errors;
                Warnings = warnings;
            }

            public int Errors { get; }
            public int Warnings { get; }
            public bool Passed => Errors == 0;
        }

        [MenuItem("Heroic/Validate 1.0 Prototype")]
        public static void ValidatePrototype()
        {
            LogValidationSummary(ValidatePrototypeContent());
        }

        public static ValidationSummary ValidatePrototypeContent()
        {
            int errors = 0;
            int warnings = 0;

            ValidateAssetExists(Root + "/Scenes/Game.unity", "Game scene", ref errors);
            ValidateAssetExists(Root + "/Scenes/MainMenu.unity", "MainMenu scene", ref errors);
            ValidateAssetExists(Root + "/Scenes/Results.unity", "Results scene", ref errors);
            ValidatePrefab<EnemyController>(Root + "/Prefabs/Enemies/Enemy_Crash.prefab", "Crash enemy prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Enemies/Enemy_Crash.prefab", "Crash enemy prefab", ref errors, typeof(Damageable), typeof(VisualPresetApplier), typeof(HitFlashVisual), typeof(DeathBurstVisual), typeof(WorldHealthBar), typeof(DamageNumberEmitter), typeof(AudioSource), typeof(ProceduralAudioFeedback));
            ValidatePrefab<EnemyController>(Root + "/Prefabs/Enemies/Enemy_Thrower.prefab", "Thrower enemy prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Enemies/Enemy_Thrower.prefab", "Thrower enemy prefab", ref errors, typeof(Damageable), typeof(VisualPresetApplier), typeof(HitFlashVisual), typeof(DeathBurstVisual), typeof(WorldHealthBar), typeof(DamageNumberEmitter), typeof(AudioSource), typeof(ProceduralAudioFeedback));
            ValidatePrefab<BossController>(Root + "/Prefabs/Enemies/Enemy_Boss_ArcaneWarden.prefab", "boss prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Enemies/Enemy_Boss_ArcaneWarden.prefab", "boss prefab", ref errors, typeof(Damageable), typeof(VisualPresetApplier), typeof(HitFlashVisual), typeof(DeathBurstVisual), typeof(WorldHealthBar), typeof(DamageNumberEmitter), typeof(AudioSource), typeof(ProceduralAudioFeedback));
            ValidatePrefab<Projectile>(Root + "/Prefabs/Projectiles/Projectile_MagicMissile.prefab", "Magic Missile projectile prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Projectiles/Projectile_MagicMissile.prefab", "Magic Missile projectile prefab", ref errors, typeof(ProjectileHit), typeof(VisualPresetApplier));
            ValidatePrefab<EnemyProjectile>(Root + "/Prefabs/Projectiles/Projectile_EnemyMissile.prefab", "enemy missile projectile prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Projectiles/Projectile_EnemyMissile.prefab", "enemy missile projectile prefab", ref errors, typeof(VisualPresetApplier));
            ValidatePrefab<ExperiencePickup>(Root + "/Prefabs/Pickups/XP_Pickup.prefab", "XP pickup prefab", ref errors);
            ValidatePrefabComponents(Root + "/Prefabs/Pickups/XP_Pickup.prefab", "XP pickup prefab", ref errors, typeof(VisualPresetApplier), typeof(AudioSource), typeof(ProceduralAudioFeedback));
            ValidateFloatFieldMinimum<ExperiencePickup>(Root + "/Prefabs/Pickups/XP_Pickup.prefab", "magnetRange", 6f, "XP pickup magnet range", ref errors);
            ValidateAssetExists(Root + "/Resources/Audio/Music/HeroicDemoLoop.mp3", "current demo music candidate", ref errors);
            ValidateAssetExists(Root + "/ScriptableObjects/Waves/Wave_001.asset", "first wave asset", ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Crash_01.asset", "Crash I definition", VisualPresetApplier.Preset.CrashLevel1, ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Crash_02.asset", "Crash II definition", VisualPresetApplier.Preset.CrashLevel2, ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Crash_03.asset", "Crash III definition", VisualPresetApplier.Preset.CrashLevel3, ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Crash_04.asset", "Crash IV definition", VisualPresetApplier.Preset.CrashLevel4, ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Thrower_01.asset", "Thrower I definition", VisualPresetApplier.Preset.ThrowerLevel1, ref errors);
            ValidateAssetExists(Root + "/ScriptableObjects/Enemies/Enemy_Boss_ArcaneWarden.asset", "boss definition asset", ref errors);
            ValidateEnemyDefinition(Root + "/ScriptableObjects/Enemies/Enemy_Boss_ArcaneWarden.asset", "boss definition asset", VisualPresetApplier.Preset.Boss, ref errors);
            ValidateBuildSettings(ref errors);

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Scenes/Game.unity") != null)
            {
                ValidateGameScene(ref errors, ref warnings);
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Scenes/MainMenu.unity") != null)
            {
                ValidateMainMenuScene(ref errors);
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Scenes/Results.unity") != null)
            {
                ValidateResultsScene(ref errors);
            }

            return new ValidationSummary(errors, warnings);
        }

        private static void LogValidationSummary(ValidationSummary summary)
        {
            if (summary.Passed)
            {
                Debug.Log($"Heroic 1.0 validation passed with {summary.Warnings} warning(s). Run Play Mode smoke tests next.");
            }
            else
            {
                Debug.LogError($"Heroic 1.0 validation failed with {summary.Errors} error(s) and {summary.Warnings} warning(s). Run Heroic/Build 1.0 Prototype Content, then validate again.");
            }
        }

        private static void ValidateGameScene(ref int errors, ref int warnings)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(Root + "/Scenes/Game.unity");

            RequireSceneObject<RunManager>("RunManager", ref errors);
            RequireSceneObject<RunBootstrapper>("RunBootstrapper", ref errors);
            RequireSceneObject<RunEndWatcher>("RunEndWatcher", ref errors);
            RequireSceneObject<DemoSafetyHotkeys>("DemoSafetyHotkeys", ref errors);
            RequireSceneObject<DemoAudioControls>("DemoAudioControls", ref errors);
            RequireSceneObject<EnemySpawner>("EnemySpawner", ref errors);
            RequireSceneObject<BossSpawner>("BossSpawner", ref errors);
            RequireSceneObject<UpgradeManager>("UpgradeManager", ref errors);
            RequireSceneObject<UpgradeChoiceApplier>("UpgradeChoiceApplier", ref errors);
            RequireSceneObject<ArcaneUpgradeApplier>("ArcaneUpgradeApplier", ref errors);
            RequireSceneObject<UIManager>("UIManager", ref errors);
            RequireSceneObject<CameraFollow2D>("CameraFollow2D", ref errors);
            RequireSceneObject<CameraShakeFeedback>("CameraShakeFeedback", ref errors);
            RequireSceneObject<ArenaBackdrop>("ArenaBackdrop", ref errors);
            RequireSceneObject<PlayerController>("PlayerController", ref errors);
            RequireSceneObject<PlayerHealth>("PlayerHealth", ref errors);
            RequireSceneObject<PlayerExperience>("PlayerExperience", ref errors);
            RequireSceneObject<PlayerTemporaryBuffs>("PlayerTemporaryBuffs", ref errors);
            RequireSceneObject<SpellCaster>("SpellCaster", ref errors);
            RequireSceneObject<MagicMissileCaster>("MagicMissileCaster", ref errors);
            RequireSceneObject<ArcaneBlastCaster>("ArcaneBlastCaster", ref warnings, true);
            RequireSceneObject<WarpPulseCaster>("WarpPulseCaster", ref warnings, true);
            RequireSceneObject<MovementCaster>("MovementCaster", ref errors);
            RequireSceneObject<WorldHealthBar>("WorldHealthBar", ref errors);
            RequireSceneObject<DamageNumberEmitter>("DamageNumberEmitter", ref errors);
            RequireSceneObject<HudPresenter>("HudPresenter", ref errors);
            RequireSceneObject<CharacterStatsPanel>("CharacterStatsPanel", ref errors);
            RequireSceneObject<ObjectivePresenter>("ObjectivePresenter", ref errors);
            RequireSceneObject<DraftPresenter>("DraftPresenter", ref errors);
            RequireSceneObject<ResultsPresenter>("ResultsPresenter", ref errors);
            RequireSceneObject<ProceduralAudioFeedback>("ProceduralAudioFeedback", ref errors);
            RequireSceneObject<BackgroundMusicPlayer>("BackgroundMusicPlayer", ref errors);
            RequireTextFonts("Game scene text", ref errors);
            BackgroundMusicPlayer gameMusic = FindSceneComponent<BackgroundMusicPlayer>();
            RequireSerializedString(gameMusic, "resourcesClipPath", "game music Resources path", ref errors);

            CameraShakeFeedback cameraShake = FindSceneComponent<CameraShakeFeedback>();
            RequireSerializedObjectReference(cameraShake, "playerHealth", "camera shake player health", ref errors);
            RequireSerializedObjectReference(cameraShake, "movementCaster", "camera shake movement caster", ref errors);
            RequireSerializedObjectReference(cameraShake, "bossSpawner", "camera shake boss spawner", ref errors);

            HudPresenter hud = FindSceneComponent<HudPresenter>();
            RequireSerializedObjectReference(hud, "healthSlider", "HUD health slider", ref errors);
            RequireSerializedObjectReference(hud, "experienceSlider", "HUD experience slider", ref errors);
            RequireSerializedObjectReference(hud, "healthFillImage", "HUD health fill image", ref errors);
            RequireSerializedObjectReference(hud, "experienceFillImage", "HUD experience fill image", ref errors);
            RequireSerializedObjectReference(hud, "healthFillRect", "HUD health fill rect", ref errors);
            RequireSerializedObjectReference(hud, "experienceFillRect", "HUD experience fill rect", ref errors);
            RequireSerializedObjectReference(hud, "levelText", "HUD level text", ref errors);
            RequireSerializedObjectReference(hud, "timerText", "HUD timer text", ref errors);
            RequireSerializedObjectReference(hud, "experienceText", "HUD experience text", ref errors);
            RequireTopAnchoredRect("HealthBar", "HUD health bar", ref errors);
            RequireTopAnchoredRect("ExperienceBar", "HUD experience bar", ref errors);

            CharacterStatsPanel characterStats = FindSceneComponent<CharacterStatsPanel>();
            RequireSerializedObjectReference(characterStats, "playerHealth", "character stats player health", ref errors);
            RequireSerializedObjectReference(characterStats, "playerExperience", "character stats player experience", ref errors);
            RequireSerializedObjectReference(characterStats, "buildState", "character stats build state", ref errors);
            RequireSerializedObjectReference(characterStats, "movementCaster", "character stats movement caster", ref errors);
            RequireSerializedObjectReference(characterStats, "temporaryBuffs", "character stats temporary buffs", ref errors);
            RequireSerializedObjectReference(characterStats, "spellStats", "character stats spell stats", ref errors);
            RequireSerializedObjectReference(characterStats, "territoryCasting", "character stats territory casting", ref errors);
            RequireSerializedObjectReference(characterStats, "skillListText", "character stats skill list", ref errors);
            RequireSerializedObjectReference(characterStats, "bonusListText", "character stats bonus list", ref errors);

            UIManager uiManager = FindSceneComponent<UIManager>();
            RequireSerializedObjectReference(uiManager, "gameUiRoot", "UI manager game root", ref errors);
            RequireSerializedObjectReference(uiManager, "levelUpDraftRoot", "UI manager draft root", ref errors);
            RequireSerializedObjectReference(uiManager, "pauseRoot", "UI manager pause root", ref errors);
            RequireSerializedObjectReference(uiManager, "resultsRoot", "UI manager results root", ref errors);
            RequireImageAlpha("Pause", 0.5f, "pause backdrop", ref errors);

            ObjectivePresenter objective = FindSceneComponent<ObjectivePresenter>();
            RequireSerializedObjectReference(objective, "goalText", "objective goal text", ref errors);
            RequireSerializedObjectReference(objective, "bossText", "objective boss text", ref errors);
            RequireSerializedObjectReference(objective, "upgradeText", "objective upgrade text", ref errors);

            DraftPresenter draft = FindSceneComponent<DraftPresenter>();
            RequireSerializedArrayLength(draft, "choiceButtons", 3, "draft choice buttons", ref errors);
            RequireSerializedArrayLength(draft, "choiceLabels", 3, "draft choice labels", ref errors);
            RequireImageAlpha("Draft", 0.5f, "draft backdrop", ref errors);

            ResultsPresenter results = FindSceneComponent<ResultsPresenter>();
            RequireSerializedObjectReference(results, "resultText", "results outcome text", ref errors);
            RequireSerializedObjectReference(results, "timeText", "results time text", ref errors);
            RequireSerializedObjectReference(results, "summaryText", "results summary text", ref errors);
            RequireSerializedString(results, "gameSceneName", "results restart scene name", ref errors);
            RequireSerializedString(results, "mainMenuSceneName", "results main menu scene name", ref errors);
            RequireImageAlpha("Results", 0.5f, "results backdrop", ref errors);
            RequireButtonListener("Restart", "results restart button", ref errors);
            RequireButtonListener("QuitToMenu", "results quit button", ref errors);

            MovementSlotPresenter[] movementSlots = UnityEngine.Object.FindObjectsByType<MovementSlotPresenter>(FindObjectsInactive.Include);
            if (movementSlots.Length < 3)
            {
                errors++;
                Debug.LogError($"Scene error: expected 3 movement slot presenters, found {movementSlots.Length}.");
            }

            foreach (MovementSlotPresenter movementSlot in movementSlots)
            {
                RequireSerializedObjectReference(movementSlot, "skillNameText", "movement slot skill label", ref errors);
                RequireSerializedObjectReference(movementSlot, "cooldownText", "movement slot cooldown text", ref errors);
                RequireSerializedObjectReference(movementSlot, "cooldownFill", "movement slot cooldown fill", ref errors);
            }

            if (!string.IsNullOrEmpty(currentScene) && currentScene != Root + "/Scenes/Game.unity")
            {
                EditorSceneManager.OpenScene(currentScene);
            }
        }

        private static void ValidateMainMenuScene(ref int errors)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(Root + "/Scenes/MainMenu.unity");

            RequireSceneObject<MainMenuPresenter>("MainMenuPresenter", ref errors);
            RequireSceneObject<Canvas>("MainMenu Canvas", ref errors);
            RequireSceneObject<EventSystem>("EventSystem", ref errors);
            RequireSceneObject<BackgroundMusicPlayer>("MainMenu BackgroundMusicPlayer", ref errors);
            RequireSceneObject<DemoAudioControls>("MainMenu DemoAudioControls", ref errors);
            RequireTextFonts("MainMenu scene text", ref errors);
            BackgroundMusicPlayer menuMusic = FindSceneComponent<BackgroundMusicPlayer>();
            RequireSerializedString(menuMusic, "resourcesClipPath", "main menu music Resources path", ref errors);

            Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
            if (buttons.Length == 0)
            {
                errors++;
                Debug.LogError("MainMenu scene error: no buttons found. Expected a Start Run button.");
            }

            RequireButtonListener("Start Run", "main menu start button", ref errors);

            if (!string.IsNullOrEmpty(currentScene) && currentScene != Root + "/Scenes/MainMenu.unity")
            {
                EditorSceneManager.OpenScene(currentScene);
            }
        }

        private static void ValidateResultsScene(ref int errors)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(Root + "/Scenes/Results.unity");

            RequireSceneObject<Canvas>("Results Canvas", ref errors);
            RequireSceneObject<BackgroundMusicPlayer>("Results BackgroundMusicPlayer", ref errors);
            RequireSceneObject<DemoAudioControls>("Results DemoAudioControls", ref errors);
            RequireTextFonts("Results scene text", ref errors);
            BackgroundMusicPlayer resultsMusic = FindSceneComponent<BackgroundMusicPlayer>();
            RequireSerializedString(resultsMusic, "resourcesClipPath", "results music Resources path", ref errors);

            if (!string.IsNullOrEmpty(currentScene) && currentScene != Root + "/Scenes/Results.unity")
            {
                EditorSceneManager.OpenScene(currentScene);
            }
        }

        private static void ValidateAssetExists(string path, string label, ref int errors)
        {
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) == null)
            {
                errors++;
                Debug.LogError($"Missing {label}: {path}");
            }
        }

        private static void ValidatePrefab<T>(string path, string label, ref int errors) where T : Component
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                errors++;
                Debug.LogError($"Missing {label}: {path}");
                return;
            }

            if (prefab.GetComponent<T>() == null)
            {
                errors++;
                Debug.LogError($"{label} is missing required component {typeof(T).Name}: {path}");
            }
        }

        private static void ValidatePrefabComponents(string path, string label, ref int errors, params Type[] components)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                return;
            }

            foreach (Type componentType in components)
            {
                if (prefab.GetComponent(componentType) != null)
                {
                    continue;
                }

                errors++;
                Debug.LogError($"{label} is missing required component {componentType.Name}: {path}");
            }
        }

        private static void ValidateEnemyDefinition(string path, string label, VisualPresetApplier.Preset expectedPreset, ref int errors)
        {
            EnemyDefinition definition = AssetDatabase.LoadAssetAtPath<EnemyDefinition>(path);
            if (definition == null)
            {
                errors++;
                Debug.LogError($"Missing {label}: {path}");
                return;
            }

            if (definition.VisualPreset != expectedPreset)
            {
                errors++;
                Debug.LogError($"{label} has visual preset {definition.VisualPreset}; expected {expectedPreset}: {path}");
            }
        }

        private static void ValidateFloatFieldMinimum<T>(string path, string propertyName, float minimumValue, string label, ref int errors) where T : Component
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            T component = prefab != null ? prefab.GetComponent<T>() : null;
            if (component == null)
            {
                return;
            }

            SerializedProperty property = new SerializedObject(component).FindProperty(propertyName);
            if (property == null || property.propertyType != SerializedPropertyType.Float || property.floatValue < minimumValue)
            {
                errors++;
                float value = property != null && property.propertyType == SerializedPropertyType.Float ? property.floatValue : 0f;
                Debug.LogError($"{label} must be at least {minimumValue}; found {value} on {path}.");
            }
        }

        private static void ValidateBuildSettings(ref int errors)
        {
            RequireBuildScene(Root + "/Scenes/MainMenu.unity", ref errors);
            RequireBuildScene(Root + "/Scenes/Game.unity", ref errors);
            RequireBuildScene(Root + "/Scenes/Results.unity", ref errors);
        }

        private static void RequireBuildScene(string path, ref int errors)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.path == path && scene.enabled)
                {
                    return;
                }
            }

            errors++;
            Debug.LogError($"Build settings error: missing enabled scene {path}.");
        }

        private static void RequireSceneObject<T>(string label, ref int count, bool warning = false) where T : UnityEngine.Object
        {
            T found = UnityEngine.Object.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (found != null)
            {
                return;
            }

            count++;
            if (warning)
            {
                Debug.LogWarning($"Scene warning: missing {label}");
            }
            else
            {
                Debug.LogError($"Scene error: missing {label}");
            }
        }

        private static T FindSceneComponent<T>() where T : Component
        {
            return UnityEngine.Object.FindAnyObjectByType<T>(FindObjectsInactive.Include);
        }

        private static void RequireTextFonts(string label, ref int errors)
        {
            TMP_Text[] texts = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            foreach (TMP_Text text in texts)
            {
                if (text.font != null)
                {
                    continue;
                }

                errors++;
                Debug.LogError($"Scene error: {label} has missing TMP font asset on {text.name}.");
            }
        }

        private static void RequireButtonListener(string buttonName, string label, ref int errors)
        {
            Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
            foreach (Button button in buttons)
            {
                if (button.name != buttonName)
                {
                    continue;
                }

                if (button.onClick.GetPersistentEventCount() > 0)
                {
                    return;
                }

                errors++;
                Debug.LogError($"Scene error: {label} `{buttonName}` has no persistent onClick listener.");
                return;
            }

            errors++;
            Debug.LogError($"Scene error: missing {label} `{buttonName}`.");
        }

        private static void RequireTopAnchoredRect(string objectName, string label, ref int errors)
        {
            GameObject go = FindSceneGameObject(objectName);
            RectTransform rect = go != null ? go.GetComponent<RectTransform>() : null;
            if (rect == null)
            {
                errors++;
                Debug.LogError($"Scene error: missing RectTransform for {label} `{objectName}`.");
                return;
            }

            if (rect.anchorMin.y < 0.99f || rect.anchorMax.y < 0.99f)
            {
                errors++;
                Debug.LogError($"Scene error: {label} `{objectName}` must be anchored to the top HUD, not the playfield center.");
            }
        }

        private static void RequireImageAlpha(string objectName, float minimumAlpha, string label, ref int errors)
        {
            GameObject go = FindSceneGameObject(objectName);
            Image image = go != null ? go.GetComponent<Image>() : null;
            if (image == null)
            {
                errors++;
                Debug.LogError($"Scene error: missing Image for {label} `{objectName}`.");
                return;
            }

            if (image.color.a < minimumAlpha)
            {
                errors++;
                Debug.LogError($"Scene error: {label} `{objectName}` alpha must be at least {minimumAlpha}; found {image.color.a}.");
            }
        }

        private static GameObject FindSceneGameObject(string objectName)
        {
            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            foreach (Transform transform in transforms)
            {
                if (transform.name == objectName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        private static void RequireSerializedObjectReference(UnityEngine.Object target, string propertyName, string label, ref int errors)
        {
            if (target == null)
            {
                errors++;
                Debug.LogError($"Scene error: cannot validate {label} because target component is missing.");
                return;
            }

            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            if (property == null || property.objectReferenceValue == null)
            {
                errors++;
                Debug.LogError($"Scene error: missing serialized reference for {label} on {target.name}.");
            }
        }

        private static void RequireSerializedString(UnityEngine.Object target, string propertyName, string label, ref int errors)
        {
            if (target == null)
            {
                errors++;
                Debug.LogError($"Scene error: cannot validate {label} because target component is missing.");
                return;
            }

            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            if (property == null || property.propertyType != SerializedPropertyType.String || string.IsNullOrWhiteSpace(property.stringValue))
            {
                errors++;
                Debug.LogError($"Scene error: missing serialized string for {label} on {target.name}.");
            }
        }

        private static void RequireSerializedArrayLength(UnityEngine.Object target, string propertyName, int minimumLength, string label, ref int errors)
        {
            if (target == null)
            {
                errors++;
                Debug.LogError($"Scene error: cannot validate {label} because target component is missing.");
                return;
            }

            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            if (property == null || !property.isArray || property.arraySize < minimumLength)
            {
                errors++;
                int length = property != null && property.isArray ? property.arraySize : 0;
                Debug.LogError($"Scene error: {label} expected at least {minimumLength}, found {length} on {target.name}.");
            }
        }
    }
}

