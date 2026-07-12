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
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Heroic.Editor
{
    public static class HeroicPrototypeBuilder
    {
        private const string Root = "Assets/_Heroic";
        private const string Prefabs = Root + "/Prefabs";
        private const string Scenes = Root + "/Scenes";
        private const string ScriptableObjects = Root + "/ScriptableObjects";
        private const string RuntimeFontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
        private static readonly string[] CrashFramePaths =
        {
            "Assets/mobs/Crash I_frame1.png",
            "Assets/mobs/Crash I_frame2.png"
        };

        private static readonly string[] Crash2FramePaths =
        {
            "Assets/mobs/Crash II_frame1.png",
            "Assets/mobs/Crash II_frame2.png"
        };

        private static readonly string[] Crash3FramePaths =
        {
            "Assets/mobs/Crash III_frame1.png",
            "Assets/mobs/Crash III_frame2.png"
        };

        private static readonly string[] Crash4FramePaths =
        {
            "Assets/mobs/Crash IV_frame1.png",
            "Assets/mobs/Crash IV_frame2.png"
        };

        private static readonly string[] Crash5FramePaths =
        {
            "Assets/mobs/Crash V_frame1.png",
            "Assets/mobs/Crash V_frame2.png"
        };

        private static readonly string[] Wall1FramePaths =
        {
            "Assets/mobs/Wall I_frame1.png",
            "Assets/mobs/Wall I_frame2.png"
        };

        private static readonly string[] Thrower1FramePaths =
        {
            "Assets/mobs/thrower I_frame1.png",
            "Assets/mobs/thrower I_frame2.png"
        };

        private static readonly string[] PlayerLevel1FramePaths =
        {
            "Assets/mobs/Player I_frame1.png",
            "Assets/mobs/Player I_frame2.png",
            "Assets/mobs/Player I_frame3.png",
            "Assets/mobs/Player I_frame4.png"
        };

        private static readonly string[] PlayerLevel2FramePaths =
        {
            "Assets/mobs/Player II_frame1.png",
            "Assets/mobs/Player II_frame2.png",
            "Assets/mobs/Player II_frame3.png",
            "Assets/mobs/Player II_frame4.png"
        };

        private static readonly string[] PlayerLevel6FramePaths =
        {
            "Assets/mobs/Player VI_frame1.png",
            "Assets/mobs/Player VI_frame2.png",
            "Assets/mobs/Player VI_frame3.png",
            "Assets/mobs/Player VI_frame4.png"
        };

        private static TMP_FontAsset runtimeFont;

        [MenuItem("Heroic/Build 1.0 Prototype Content")]
        public static void BuildPrototypeContent()
        {
            EnsureFolders();
            runtimeFont = CreateOrLoadRuntimeFont();

            GameObject xpPickup = CreateXpPickupPrefab();
            GameObject projectile = CreateMagicMissilePrefab();
            GameObject fireProjectile = CreateFireProjectilePrefab();
            GameObject enemyMissile = CreateEnemyMissilePrefab();
            GameObject orb = CreateArcaneOrbPrefab();
            GameObject enemy = CreateEnemyPrefab(xpPickup);
            GameObject wall = CreateWallPrefab(xpPickup);
            GameObject Thrower = CreateThrowerEnemyPrefab(xpPickup, enemyMissile);
            GameObject boss = CreateBossPrefab(xpPickup);

            EnemyDefinition crashOneDefinition = CreateEnemyDefinition("Enemy_Crash_01", "Crash I", enemy, 10, 2f, 10, 1, VisualPresetApplier.Preset.CrashLevel1, false);
            EnemyDefinition crashTwoDefinition = CreateEnemyDefinition("Enemy_Crash_02", "Crash II", enemy, 12, 2.15f, 10, 1, VisualPresetApplier.Preset.CrashLevel2, false);
            EnemyDefinition crashThreeDefinition = CreateEnemyDefinition("Enemy_Crash_03", "Crash III", enemy, 15, 2.3f, 10, 1, VisualPresetApplier.Preset.CrashLevel3, false);
            EnemyDefinition crashFourDefinition = CreateEnemyDefinition("Enemy_Crash_04", "Crash IV", enemy, 15, 2.875f, 10, 1, VisualPresetApplier.Preset.CrashLevel4, false);
            EnemyDefinition crashFiveDefinition = CreateEnemyDefinition("Enemy_Crash_05", "Crash V", enemy, 18, 3.15f, 12, 2, VisualPresetApplier.Preset.CrashLevel5, false);
            EnemyDefinition wallOneDefinition = CreateEnemyDefinition("Enemy_Wall_01", "Wall I", wall, 40, 0f, 14, 2, VisualPresetApplier.Preset.WallLevel1, false);
            EnemyDefinition ThrowerDefinition = CreateEnemyDefinition("Enemy_Thrower_01", "Thrower I", Thrower, 25, 1.5f, 15, 2, VisualPresetApplier.Preset.ThrowerLevel1, false);
            EnemyDefinition bossDefinition = CreateEnemyDefinition("Enemy_Boss_ArcaneWarden", "Arcane Warden", boss, 900, 1.6f, 18, 30, VisualPresetApplier.Preset.Boss, true);

            WaveDefinition waveOne = CreateWave("Wave_001", 1, 0f, 120f, 0.18f, 1, 2, crashOneDefinition, crashTwoDefinition, crashThreeDefinition, crashFourDefinition, crashFiveDefinition, wallOneDefinition, ThrowerDefinition);
            WaveDefinition waveTwo = CreateWave("Wave_002", 2, 120f, 180f, 1.15f, 1, 1, crashOneDefinition, crashTwoDefinition, crashThreeDefinition, crashFourDefinition, crashFiveDefinition, wallOneDefinition, ThrowerDefinition);
            WaveDefinition waveThree = CreateWave("Wave_003", 3, 300f, 240f, 0.8f, 1, 1, crashOneDefinition, crashTwoDefinition, crashThreeDefinition, crashFourDefinition, crashFiveDefinition, wallOneDefinition, ThrowerDefinition);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            CreateGameScene(projectile, fireProjectile, orb, enemy, boss, xpPickup, bossDefinition, new[] { waveOne, waveTwo, waveThree });
            CreateMenuScene("MainMenu");
            CreateMenuScene("Results");
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Heroic 1.0 prototype content generated.");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Heroic");
            EnsureFolder(Root, "Prefabs");
            EnsureFolder(Prefabs, "Enemies");
            EnsureFolder(Prefabs, "Projectiles");
            EnsureFolder(Prefabs, "Pickups");
            EnsureFolder(Prefabs, "Spells");
            EnsureFolder(Root, "Scenes");
            EnsureFolder(Root, "Resources");
            EnsureFolder(Root + "/Resources", "Audio");
            EnsureFolder(Root + "/Resources/Audio", "Music");
            EnsureFolder(Root, "ScriptableObjects");
            EnsureFolder(ScriptableObjects, "Enemies");
            EnsureFolder(ScriptableObjects, "Waves");
        }

        private static TMP_FontAsset CreateOrLoadRuntimeFont()
        {
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RuntimeFontPath);
            if (existing != null)
            {
                return existing;
            }

            CreateRuntimeFontFolders();
            TMP_FontAsset generated = CreateRuntimeFontFromBuiltinFont();
            if (generated != null)
            {
                return generated;
            }

            string packagePath = FindPackagePath("com.unity.ugui");
            if (string.IsNullOrEmpty(packagePath))
            {
                throw new InvalidOperationException("Could not locate com.unity.ugui in the Unity package cache. TextMesh Pro essential resources cannot be imported.");
            }

            string packageFile = Path.Combine(packagePath, "Package Resources", "TMP Essential Resources.unitypackage");
            if (!File.Exists(packageFile))
            {
                throw new FileNotFoundException("Could not locate TMP Essential Resources unitypackage.", packageFile);
            }

            AssetDatabase.ImportPackage(packageFile, false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            TMP_FontAsset imported = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RuntimeFontPath);
            if (imported == null)
            {
                imported = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            }

            if (imported == null)
            {
                throw new InvalidOperationException($"Could not create or import a TMP runtime font asset. Expected {RuntimeFontPath} or the default LiberationSans SDF asset.");
            }

            return imported;
        }

        private static void CreateRuntimeFontFolders()
        {
            EnsureFolder("Assets", "TextMesh Pro");
            EnsureFolder("Assets/TextMesh Pro", "Resources");
            EnsureFolder("Assets/TextMesh Pro/Resources", "Fonts & Materials");
        }

        private static TMP_FontAsset CreateRuntimeFontFromBuiltinFont()
        {
            Font sourceFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (sourceFont == null)
            {
                sourceFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            if (sourceFont == null)
            {
                return null;
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
            if (fontAsset == null)
            {
                return null;
            }

            fontAsset.name = "Heroic Runtime SDF";
            AssetDatabase.CreateAsset(fontAsset, RuntimeFontPath);
            AssetDatabase.SaveAssetIfDirty(fontAsset);
            AssetDatabase.ImportAsset(RuntimeFontPath, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RuntimeFontPath);
        }

        private static string FindPackagePath(string packageName)
        {
            string projectPackagePath = Path.GetFullPath(Path.Combine("Packages", packageName));
            if (Directory.Exists(projectPackagePath))
            {
                return projectPackagePath;
            }

            string packageCachePath = Path.GetFullPath(Path.Combine("Library", "PackageCache"));
            if (!Directory.Exists(packageCachePath))
            {
                return string.Empty;
            }

            string[] matchingPackages = Directory.GetDirectories(packageCachePath, packageName + "@*");
            Array.Sort(matchingPackages, StringComparer.OrdinalIgnoreCase);
            return matchingPackages.Length > 0 ? matchingPackages[matchingPackages.Length - 1] : string.Empty;
        }

        private static GameObject CreateMagicMissilePrefab()
        {
            GameObject go = new GameObject("Projectile_MagicMissile");
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<Projectile>();
            go.AddComponent<ProjectileHit>();
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.MagicMissile);
            return SavePrefab(go, Prefabs + "/Projectiles/Projectile_MagicMissile.prefab");
        }

        private static GameObject CreateFireProjectilePrefab()
        {
            GameObject go = new GameObject("Projectile_FireBolt");
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<Projectile>();
            go.AddComponent<ProjectileHit>();
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.FireProjectile);
            return SavePrefab(go, Prefabs + "/Projectiles/Projectile_FireBolt.prefab");
        }

        private static GameObject CreateEnemyMissilePrefab()
        {
            GameObject go = new GameObject("Projectile_EnemyMissile");
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<EnemyProjectile>();
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.EnemyMissile);
            return SavePrefab(go, Prefabs + "/Projectiles/Projectile_EnemyMissile.prefab");
        }

        private static GameObject CreateArcaneOrbPrefab()
        {
            GameObject go = new GameObject("ArcaneOrbitOrb");
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            go.AddComponent<ArcaneOrbitOrb>();
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.ArcaneOrb);
            return SavePrefab(go, Prefabs + "/Spells/ArcaneOrbitOrb.prefab");
        }

        private static GameObject CreateXpPickupPrefab()
        {
            GameObject go = new GameObject("XP_Pickup");
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            ExperiencePickup pickup = go.AddComponent<ExperiencePickup>();
            SetFloat(pickup, "magnetRange", 20f);
            SetFloat(pickup, "magnetSpeed", 11f);
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.ExperiencePickup);
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Pickup, 0.3f);
            return SavePrefab(go, Prefabs + "/Pickups/XP_Pickup.prefab");
        }

        private static GameObject CreateEnemyPrefab(GameObject xpPickup)
        {
            GameObject go = new GameObject("Enemy_Crash");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            go.AddComponent<EnemyController>();
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.CrashLevel1);
            SetObjectArray(visual, "crashLevel2Frames", LoadTextures(Crash2FramePaths));
            SetObjectArray(visual, "crashLevel3Frames", LoadTextures(Crash3FramePaths));
            SetObjectArray(visual, "crashLevel4Frames", LoadTextures(Crash4FramePaths));
            SetObjectArray(visual, "crashLevel5Frames", LoadTextures(Crash5FramePaths));
            SetObjectArray(visual, "wallLevel1Frames", LoadTextures(Wall1FramePaths));
            SetObjectArray(visual, "throwerLevel1Frames", LoadTextures(Thrower1FramePaths));
            CrashSpriteAnimator crashAnimator = go.AddComponent<CrashSpriteAnimator>();
            SetObjectArray(crashAnimator, "sourceFrames", LoadTextures(CrashFramePaths));
            SetFloat(crashAnimator, "secondsPerFrame", 0.35f);
            SetInt(crashAnimator, "sortingOrder", 20);
            SetFloat(crashAnimator, "pixelsPerUnit", 384f);
            go.AddComponent<HitFlashVisual>();
            go.AddComponent<DeathBurstVisual>();
            go.AddComponent<WorldHealthBar>();
            go.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Enemy, 0.32f);
            return SavePrefab(go, Prefabs + "/Enemies/Enemy_Crash.prefab");
        }

        private static GameObject CreateWallPrefab(GameObject xpPickup)
        {
            GameObject go = new GameObject("Enemy_Wall");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetBool(controller, "destroyAfterContactDamage", false);
            SetBool(controller, "suppressExperienceOnContactDamage", false);
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.WallLevel1);
            SetObject(visual, "wallLevel1Texture", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/mobs/Wall I.png"));
            go.AddComponent<HitFlashVisual>();
            DeathBurstVisual burst = go.AddComponent<DeathBurstVisual>();
            SetFloat(burst, "burstScale", 1.8f);
            go.AddComponent<WorldHealthBar>();
            go.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Enemy, 0.32f);
            return SavePrefab(go, Prefabs + "/Enemies/Enemy_Wall.prefab");
        }

        private static GameObject CreateThrowerEnemyPrefab(GameObject xpPickup, GameObject enemyMissile)
        {
            GameObject go = new GameObject("Enemy_Thrower");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetEnum(controller, "behavior", EnemyController.EnemyBehavior.Thrower);
            SetObject(controller, "projectilePrefab", enemyMissile.GetComponent<EnemyProjectile>());
            SetFloat(controller, "ThrowerRange", 50f);
            SetFloat(controller, "ThrowerFireInterval", 5f);
            SetFloat(controller, "ThrowerProjectileSpeed", 4f);
            SetInt(controller, "ThrowerProjectileDamage", 15);
            SetBool(controller, "destroyAfterContactDamage", false);
            SetBool(controller, "suppressExperienceOnContactDamage", false);
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.ThrowerLevel1);
            go.AddComponent<HitFlashVisual>();
            go.AddComponent<DeathBurstVisual>();
            go.AddComponent<WorldHealthBar>();
            go.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Enemy, 0.32f);
            return SavePrefab(go, Prefabs + "/Enemies/Enemy_Thrower.prefab");
        }

        private static GameObject CreateBossPrefab(GameObject xpPickup)
        {
            GameObject go = new GameObject("Enemy_Boss_ArcaneWarden");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetBool(controller, "destroyAfterContactDamage", false);
            SetBool(controller, "suppressExperienceOnContactDamage", false);
            go.AddComponent<BossController>();
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.Boss);
            go.AddComponent<HitFlashVisual>();
            DeathBurstVisual burst = go.AddComponent<DeathBurstVisual>();
            SetFloat(burst, "burstScale", 2.5f);
            go.AddComponent<WorldHealthBar>();
            go.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Boss, 0.5f);
            return SavePrefab(go, Prefabs + "/Enemies/Enemy_Boss_ArcaneWarden.prefab");
        }

        private static void CreateGameScene(GameObject projectile, GameObject fireProjectile, GameObject orb, GameObject enemy, GameObject boss, GameObject xpPickup, EnemyDefinition bossDefinition, WaveDefinition[] waves)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Game";

            GameObject arena = new GameObject("ArenaBackdrop");
            arena.transform.position = Vector3.zero;
            arena.AddComponent<ArenaBackdrop>();

            GameObject managers = new GameObject("GameManagers");
            RunManager runManager = managers.AddComponent<RunManager>();
            managers.AddComponent<RunBootstrapper>();
            RunEndWatcher runEndWatcher = managers.AddComponent<RunEndWatcher>();
            managers.AddComponent<PauseInputHandler>();
            DemoSafetyHotkeys demoSafetyHotkeys = managers.AddComponent<DemoSafetyHotkeys>();
            managers.AddComponent<GameStateManager>();
            EnemySpawner enemySpawner = managers.AddComponent<EnemySpawner>();
            managers.AddComponent<EnemyDirector>();
            BossSpawner bossSpawner = managers.AddComponent<BossSpawner>();
            InvestorShowcaseMode showcaseMode = managers.AddComponent<InvestorShowcaseMode>();
            SetBool(showcaseMode, "enabledForPrototype", false);
            UpgradeManager upgradeManager = managers.AddComponent<UpgradeManager>();
            RunBuildState buildState = managers.AddComponent<RunBuildState>();
            UpgradeChoiceApplier choiceApplier = managers.AddComponent<UpgradeChoiceApplier>();
            ArcaneUpgradeApplier arcaneUpgradeApplier = managers.AddComponent<ArcaneUpgradeApplier>();
            FireUpgradeApplier fireUpgradeApplier = managers.AddComponent<FireUpgradeApplier>();
            UIManager uiManager = managers.AddComponent<UIManager>();
            BackgroundMusicPlayer music = managers.AddComponent<BackgroundMusicPlayer>();
            SetString(music, "resourcesClipPath", "Audio/Music/HeroicDemoLoop");
            SetFloat(music, "volume", 0.24f);
            managers.AddComponent<DemoAudioControls>();

            GameObject player = CreateScenePlayer(projectile, fireProjectile, orb, upgradeManager);
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            PlayerExperience playerExperience = player.GetComponent<PlayerExperience>();
            SpellCaster spellCaster = player.GetComponent<SpellCaster>();
            MovementCaster movementCaster = player.GetComponent<MovementCaster>();
            TerritoryCastingController territoryCasting = player.GetComponent<TerritoryCastingController>();

            SetObject(runEndWatcher, "runManager", runManager);
            SetObject(runEndWatcher, "playerHealth", playerHealth);
            SetObject(demoSafetyHotkeys, "runManager", runManager);

            SetObject(enemySpawner, "enemyPrefab", enemy.GetComponent<EnemyController>());
            SetObject(enemySpawner, "playerTarget", player.transform);
            SetObject(enemySpawner, "runManager", runManager);
            SetObject(enemySpawner, "playerExperience", playerExperience);
            SetObjectArray(enemySpawner, "waves", LoadWaveAssets(waves));

            SetObject(bossSpawner, "runManager", runManager);
            SetObject(bossSpawner, "runEndWatcher", runEndWatcher);
            SetObject(bossSpawner, "fallbackBossPrefab", boss.GetComponent<EnemyController>());
            SetObject(bossSpawner, "playerTarget", player.transform);
            SetObject(bossSpawner, "bossDefinition", bossDefinition);

            SetObject(playerExperience, "upgradeManager", upgradeManager);
            SetObject(upgradeManager, "buildState", buildState);

            SetObject(choiceApplier, "upgradeManager", upgradeManager);
            SetObject(choiceApplier, "buildState", buildState);
            SetObject(choiceApplier, "spellCaster", spellCaster);
            SetObject(choiceApplier, "movementCaster", movementCaster);
            SetObject(choiceApplier, "territoryCasting", territoryCasting);
            SetObject(choiceApplier, "arcaneUpgradeApplier", arcaneUpgradeApplier);
            SetObject(choiceApplier, "fireUpgradeApplier", fireUpgradeApplier);

            WireArcaneUpgradeApplier(arcaneUpgradeApplier, player);
            WireFireUpgradeApplier(fireUpgradeApplier, player);

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 9.1f;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            CameraFollow2D follow = cameraObject.AddComponent<CameraFollow2D>();
            SetObject(follow, "target", player.transform);
            CameraShakeFeedback cameraShake = cameraObject.AddComponent<CameraShakeFeedback>();
            SetObject(cameraShake, "playerHealth", playerHealth);
            SetObject(cameraShake, "movementCaster", movementCaster);
            SetObject(cameraShake, "bossSpawner", bossSpawner);

            TMP_Text showcaseLabel = CreateGameUi(uiManager, runManager, upgradeManager, buildState, playerHealth, playerExperience, movementCaster, bossSpawner);
            SetObject(showcaseMode, "spellCaster", spellCaster);
            SetObject(showcaseMode, "movementCaster", movementCaster);
            SetObject(showcaseMode, "playerExperience", playerExperience);
            SetObject(showcaseMode, "bossSpawner", bossSpawner);
            SetObject(showcaseMode, "showcaseLabel", showcaseLabel);
            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, Scenes + "/Game.unity");
        }

        private static GameObject CreateScenePlayer(GameObject projectile, GameObject fireProjectile, GameObject orb, UpgradeManager upgradeManager)
        {
            GameObject player = new GameObject("Player");
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            player.AddComponent<CircleCollider2D>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerPickupMagnet>();
            player.AddComponent<CloudWalkController>();
            player.AddComponent<SpellStatModifier>();
            player.AddComponent<TerritoryCastingController>();
            player.AddComponent<PlayerHealth>();
            PlayerExperience playerExperience = player.AddComponent<PlayerExperience>();
            SetInt(playerExperience, "baseExperienceToLevel", 15);
            player.AddComponent<ArcaneDoubleCast>();
            SpellEchoCaster spellEcho = player.AddComponent<SpellEchoCaster>();
            MagicMissileCaster magicMissile = player.AddComponent<MagicMissileCaster>();
            ArcaneBlastCaster arcaneBlast = player.AddComponent<ArcaneBlastCaster>();
            WarpPulseCaster warpPulse = player.AddComponent<WarpPulseCaster>();
            ArcaneOrbitCaster arcaneOrbit = player.AddComponent<ArcaneOrbitCaster>();
            FireBoltCaster fireBolt = player.AddComponent<FireBoltCaster>();
            FlameWaveCaster flameWave = player.AddComponent<FlameWaveCaster>();
            BurningGroundCaster burningGround = player.AddComponent<BurningGroundCaster>();
            SpellCaster spellCaster = player.AddComponent<SpellCaster>();
            MovementCaster movementCaster = player.AddComponent<MovementCaster>();
            SetBool(movementCaster, "equipPrototypeMovementSetOnStart", false);
            PlayerVisualController visual = player.AddComponent<PlayerVisualController>();
            SetObjectArray(visual, "levelOneFrames", LoadTextures(PlayerLevel1FramePaths));
            SetObjectArray(visual, "levelTwoFrames", LoadTextures(PlayerLevel2FramePaths));
            SetObjectArray(visual, "levelSixFrames", LoadTextures(PlayerLevel6FramePaths));
            player.AddComponent<HitFlashVisual>();
            player.AddComponent<WorldHealthBar>();
            player.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(player, ProceduralAudioFeedback.Preset.Player, 0.45f);

            GameObject firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(player.transform);
            firePoint.transform.localPosition = new Vector3(0.35f, 0f, 0f);

            SetObject(magicMissile, "projectilePrefab", projectile.GetComponent<Projectile>());
            SetObject(magicMissile, "firePoint", firePoint.transform);
            SetObject(magicMissile, "spellEcho", spellEcho);
            SetInt(magicMissile, "damage", 10);
            SetObject(arcaneBlast, "spellEcho", spellEcho);
            SetObject(warpPulse, "spellEcho", spellEcho);
            SetObject(arcaneOrbit, "orbPrefab", orb.GetComponent<ArcaneOrbitOrb>());
            SetObject(fireBolt, "projectilePrefab", fireProjectile.GetComponent<Projectile>());
            SetObject(fireBolt, "firePoint", firePoint.transform);
            SetObject(fireBolt, "spellEcho", spellEcho);
            SetObject(flameWave, "spellEcho", spellEcho);
            SetObject(burningGround, "spellEcho", spellEcho);
            SetObject(spellCaster, "magicMissileCaster", magicMissile);
            SetObject(spellCaster, "arcaneBlastCaster", arcaneBlast);
            SetObject(spellCaster, "warpPulseCaster", warpPulse);
            SetObject(spellCaster, "spellEchoCaster", spellEcho);
            SetObject(spellCaster, "arcaneOrbitCaster", arcaneOrbit);
            SetObject(spellCaster, "fireBoltCaster", fireBolt);
            SetObject(spellCaster, "flameWaveCaster", flameWave);
            SetObject(spellCaster, "burningGroundCaster", burningGround);
            return player;
        }

        private static void AddAudioFeedback(GameObject go, ProceduralAudioFeedback.Preset preset, float volume)
        {
            if (go.GetComponent<AudioSource>() == null)
            {
                go.AddComponent<AudioSource>();
            }

            ProceduralAudioFeedback feedback = go.AddComponent<ProceduralAudioFeedback>();
            SetEnum(feedback, "preset", preset);
            SetFloat(feedback, "volume", volume);
        }

        private static void WireArcaneUpgradeApplier(ArcaneUpgradeApplier applier, GameObject player)
        {
            SetObject(applier, "magicMissile", player.GetComponent<MagicMissileCaster>());
            SetObject(applier, "arcaneBlast", player.GetComponent<ArcaneBlastCaster>());
            SetObject(applier, "warpPulse", player.GetComponent<WarpPulseCaster>());
            SetObject(applier, "spellEcho", player.GetComponent<SpellEchoCaster>());
            SetObject(applier, "arcaneOrbit", player.GetComponent<ArcaneOrbitCaster>());
        }

        private static void WireFireUpgradeApplier(FireUpgradeApplier applier, GameObject player)
        {
            SetObject(applier, "fireBolt", player.GetComponent<FireBoltCaster>());
            SetObject(applier, "flameWave", player.GetComponent<FlameWaveCaster>());
            SetObject(applier, "burningGround", player.GetComponent<BurningGroundCaster>());
        }

        private static TMP_Text CreateGameUi(UIManager uiManager, RunManager runManager, UpgradeManager upgradeManager, RunBuildState buildState, PlayerHealth health, PlayerExperience experience, MovementCaster movement, BossSpawner bossSpawner)
        {
            Canvas canvas = CreateCanvas("GameUI");
            GameObject gameRoot = CreateUiRoot("HUD", canvas.transform);
            GameObject draftRoot = CreateUiRoot("Draft", canvas.transform);
            GameObject pauseRoot = CreateUiRoot("Pause", canvas.transform);
            GameObject resultsRoot = CreateUiRoot("Results", canvas.transform);
            Image draftBackdrop = draftRoot.AddComponent<Image>();
            draftBackdrop.color = new Color(0.005f, 0.012f, 0.018f, 0.72f);
            Image pauseBackdrop = pauseRoot.AddComponent<Image>();
            pauseBackdrop.color = new Color(0.005f, 0.012f, 0.018f, 0.68f);
            Image resultsBackdrop = resultsRoot.AddComponent<Image>();
            resultsBackdrop.color = new Color(0.005f, 0.012f, 0.018f, 0.78f);

            HudPresenter hud = gameRoot.AddComponent<HudPresenter>();
            SetObject(hud, "playerHealth", health);
            SetObject(hud, "playerExperience", experience);
            SetObject(hud, "runManager", runManager);

            TMP_Text levelText = CreateText("LevelText", gameRoot.transform, "Level 1", new Vector2(150f, 32f), new Vector2(90f, -30f));
            TMP_Text timerText = CreateText("TimerText", gameRoot.transform, "00:00", new Vector2(150f, 32f), new Vector2(0f, -30f));
            TMP_Text healthText = CreateText("HealthText", gameRoot.transform, "HP 100/100", new Vector2(160f, 26f), new Vector2(-90f, -30f));
            TMP_Text experienceText = CreateText("ExperienceText", gameRoot.transform, "XP 0/15", new Vector2(160f, 24f), new Vector2(90f, -30f));
            TMP_Text showcaseText = CreateText("ShowcaseLabel", gameRoot.transform, "Heroic 1.0 Showcase", new Vector2(360f, 32f), new Vector2(0f, -68f));
            Slider healthSlider = CreateSlider("HealthBar", gameRoot.transform, new Vector2(300f, 14f), new Vector2(0f, -55f), new Color(0.92f, 0.24f, 0.2f, 0.95f));
            Slider experienceSlider = CreateSlider("ExperienceBar", gameRoot.transform, new Vector2(300f, 10f), new Vector2(0f, -77f), new Color(0.24f, 0.64f, 1f, 0.95f));
            AnchorTopCenter(healthText.rectTransform, new Vector2(-170f, -28f));
            AnchorTopCenter(timerText.rectTransform, new Vector2(0f, -28f));
            AnchorTopCenter(levelText.rectTransform, new Vector2(170f, -28f));
            AnchorTopCenter(experienceText.rectTransform, new Vector2(0f, -98f));
            AnchorTopCenter(healthSlider.GetComponent<RectTransform>(), new Vector2(0f, -56f));
            AnchorTopCenter(experienceSlider.GetComponent<RectTransform>(), new Vector2(0f, -78f));
            AnchorTopCenter(showcaseText.rectTransform, new Vector2(0f, -122f));
            SetObject(hud, "levelText", levelText);
            SetObject(hud, "timerText", timerText);
            SetObject(hud, "healthText", healthText);
            SetObject(hud, "experienceText", experienceText);
            SetObject(hud, "healthSlider", healthSlider);
            SetObject(hud, "experienceSlider", experienceSlider);
            SetObject(hud, "healthFillImage", GetSliderFillImage(healthSlider));
            SetObject(hud, "experienceFillImage", GetSliderFillImage(experienceSlider));
            SetObject(hud, "healthFillRect", GetSliderFillRect(healthSlider));
            SetObject(hud, "experienceFillRect", GetSliderFillRect(experienceSlider));

            CreateObjectivePanel(gameRoot.transform, runManager, experience, bossSpawner);

            for (int i = 0; i < 3; i++)
            {
                const float movementSlotSize = 144f;
                GameObject slot = CreateUiRoot("MovementSlot" + (i + 1), gameRoot.transform);
                RectTransform rect = slot.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.sizeDelta = new Vector2(movementSlotSize, movementSlotSize);
                rect.anchoredPosition = new Vector2(-170f + i * 170f, 88f);
                Image slotBackground = slot.AddComponent<Image>();
                slotBackground.color = new Color(0.04f, 0.08f, 0.12f, 0.72f);
                Image cooldownFill = CreateFilledImage("CooldownFill", slot.transform, new Vector2(movementSlotSize, movementSlotSize), Vector2.zero, Color.clear);
                cooldownFill.enabled = false;
                cooldownFill.raycastTarget = false;
                TMP_Text slotText = CreateText("Label", slot.transform, (i + 1).ToString(), new Vector2(132f, 36f), new Vector2(0f, 46f));
                slotText.fontSize = 32f;
                slotText.fontStyle = FontStyles.Bold;
                TMP_Text cooldownText = CreateText("Cooldown", slot.transform, string.Empty, new Vector2(132f, 92f), new Vector2(0f, -8f));
                cooldownText.fontSize = 54f;
                cooldownText.fontStyle = FontStyles.Bold;
                MovementSlotPresenter presenter = slot.AddComponent<MovementSlotPresenter>();
                SetObject(presenter, "movementCaster", movement);
                SetInt(presenter, "displayIndex", i);
                SetObject(presenter, "skillNameText", slotText);
                SetObject(presenter, "cooldownText", cooldownText);
                SetObject(presenter, "cooldownFill", cooldownFill);
            }

            DraftPresenter draft = draftRoot.AddComponent<DraftPresenter>();
            SetObject(draft, "upgradeManager", upgradeManager);
            SetObject(draft, "buildState", buildState);
            TMP_Text header = CreateText("Header", draftRoot.transform, "Choose your spellbook path", new Vector2(900f, 56f), new Vector2(0f, 322f));
            header.fontSize = 38f;
            header.color = new Color(0.82f, 0.96f, 1f);
            SetObject(draft, "headerText", header);

            Button[] buttons = new Button[5];
            TMP_Text[] labels = new TMP_Text[5];
            Image[] bars = new Image[5];
            Image[] categoryIconBackdrops = new Image[5];
            TMP_Text[] categoryIconLabels = new TMP_Text[5];
            Image[] skillIconBackdrops = new Image[5];
            TMP_Text[] skillIconLabels = new TMP_Text[5];
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = CreateButton("Choice" + (i + 1), draftRoot.transform, new Vector2(390f, 123f), new Vector2(0f, 246f - i * 128f));
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = new Color(0.07f, 0.16f, 0.2f, 0.98f);
                }

                Image bar = CreateFilledImage("SchoolBar", button.transform, new Vector2(10f, 123f), new Vector2(-190f, 0f), Color.white);
                bar.raycastTarget = false;

                Image categoryIcon = CreateFilledImage("CategoryIcon", button.transform, new Vector2(50f, 50f), new Vector2(-150f, 0f), new Color(0.03f, 0.08f, 0.1f, 0.96f));
                categoryIcon.raycastTarget = false;
                TMP_Text categoryLabel = CreateText("CategoryIconLabel", categoryIcon.transform, "ATK", new Vector2(50f, 50f), Vector2.zero);
                categoryLabel.fontSize = 16f;
                categoryLabel.fontStyle = FontStyles.Bold;
                categoryLabel.alignment = TextAlignmentOptions.Center;
                categoryLabel.raycastTarget = false;
                categoryLabel.color = new Color(0.92f, 0.98f, 1f);

                Image skillIcon = CreateFilledImage("SkillIcon", button.transform, new Vector2(50f, 50f), new Vector2(-92f, 0f), new Color(0.6f, 0.64f, 0.68f, 1f));
                skillIcon.raycastTarget = false;
                TMP_Text skillLabel = CreateText("SkillIconLabel", skillIcon.transform, "MM", new Vector2(50f, 50f), Vector2.zero);
                skillLabel.fontSize = 18f;
                skillLabel.fontStyle = FontStyles.Bold;
                skillLabel.alignment = TextAlignmentOptions.Center;
                skillLabel.raycastTarget = false;

                TMP_Text label = button.GetComponentInChildren<TMP_Text>();
                label.fontSize = 21f;
                label.alignment = TextAlignmentOptions.MidlineLeft;
                label.textWrappingMode = TextWrappingModes.Normal;
                label.overflowMode = TextOverflowModes.Ellipsis;
                label.margin = new Vector4(8f, 0f, 18f, 0f);
                RectTransform labelRect = label.rectTransform;
                labelRect.sizeDelta = new Vector2(235f, 110f);
                labelRect.anchoredPosition = new Vector2(70f, 0f);
                buttons[i] = button;
                labels[i] = label;
                bars[i] = bar;
                categoryIconBackdrops[i] = categoryIcon;
                categoryIconLabels[i] = categoryLabel;
                skillIconBackdrops[i] = skillIcon;
                skillIconLabels[i] = skillLabel;
            }
            SetObjectArray(draft, "choiceButtons", buttons);
            SetObjectArray(draft, "choiceLabels", labels);
            SetObjectArray(draft, "choiceBars", bars);
            SetObjectArray(draft, "categoryIconBackdrops", categoryIconBackdrops);
            SetObjectArray(draft, "categoryIconLabels", categoryIconLabels);
            SetObjectArray(draft, "skillIconBackdrops", skillIconBackdrops);
            SetObjectArray(draft, "skillIconLabels", skillIconLabels);

            CreatePausePanel(pauseRoot.transform);

            ResultsPresenter results = resultsRoot.AddComponent<ResultsPresenter>();
            SetObject(results, "runManager", runManager);
            SetString(results, "gameSceneName", "Game");
            SetString(results, "mainMenuSceneName", "MainMenu");

            GameObject resultsPanel = CreateCenteredPanel("ResultsPanel", resultsRoot.transform, new Vector2(560f, 320f), Vector2.zero, new Color(0.015f, 0.04f, 0.052f, 0.9f));
            TMP_Text resultText = CreateText("ResultText", resultsPanel.transform, "ARCANE WARDEN DEFEATED", new Vector2(500f, 58f), new Vector2(0f, 92f));
            resultText.fontSize = 28f;
            resultText.color = new Color(0.82f, 0.96f, 1f);
            TMP_Text timeText = CreateText("TimeText", resultsPanel.transform, "Run Time  00:00", new Vector2(420f, 36f), new Vector2(0f, 42f));
            timeText.fontSize = 21f;
            timeText.color = new Color(1f, 0.78f, 0.9f);
            TMP_Text summaryText = CreateText("SummaryText", resultsPanel.transform, "The living spellbook survived the arena and ended the Warden's pressure.", new Vector2(460f, 48f), new Vector2(0f, -10f));
            summaryText.fontSize = 17f;
            summaryText.color = new Color(0.74f, 0.9f, 0.94f);
            summaryText.textWrappingMode = TextWrappingModes.Normal;

            Button restartButton = CreateButton("Restart", resultsPanel.transform, new Vector2(230f, 50f), new Vector2(0f, -82f));
            UnityEventTools.AddPersistentListener(restartButton.onClick, results.Restart);
            TMP_Text restartLabel = restartButton.GetComponentInChildren<TMP_Text>();
            if (restartLabel != null)
            {
                restartLabel.text = "Run It Back";
                restartLabel.fontSize = 20f;
            }

            Button quitButton = CreateButton("QuitToMenu", resultsPanel.transform, new Vector2(230f, 46f), new Vector2(0f, -140f));
            UnityEventTools.AddPersistentListener(quitButton.onClick, results.QuitToMenu);
            TMP_Text quitLabel = quitButton.GetComponentInChildren<TMP_Text>();
            if (quitLabel != null)
            {
                quitLabel.text = "Main Menu";
                quitLabel.fontSize = 18f;
            }

            SetObject(results, "resultText", resultText);
            SetObject(results, "timeText", timeText);
            SetObject(results, "summaryText", summaryText);

            SetObject(uiManager, "gameUiRoot", gameRoot);
            SetObject(uiManager, "levelUpDraftRoot", draftRoot);
            SetObject(uiManager, "pauseRoot", pauseRoot);
            SetObject(uiManager, "resultsRoot", resultsRoot);
            draftRoot.SetActive(false);
            pauseRoot.SetActive(false);
            resultsRoot.SetActive(false);
            return showcaseText;
        }

        private static void CreatePausePanel(Transform parent)
        {
            GameObject panel = CreateCenteredPanel("PausePanel", parent, new Vector2(520f, 260f), Vector2.zero, new Color(0.015f, 0.04f, 0.052f, 0.9f));
            TMP_Text title = CreateText("PauseTitle", panel.transform, "PAUSED", new Vector2(420f, 58f), new Vector2(0f, 72f));
            title.fontSize = 34f;
            title.color = new Color(0.82f, 0.96f, 1f);

            TMP_Text resume = CreateText("PauseResume", panel.transform, "Press Esc to resume the run.", new Vector2(430f, 34f), new Vector2(0f, 18f));
            resume.fontSize = 20f;
            resume.color = new Color(0.9f, 0.96f, 0.98f);

            TMP_Text controls = CreateText("PauseControls", panel.transform, "Audio: M mute music   - / + master volume\nSafety: F8 defeat   F9 victory", new Vector2(450f, 70f), new Vector2(0f, -54f));
            controls.fontSize = 17f;
            controls.color = new Color(0.72f, 0.88f, 0.92f);
            controls.textWrappingMode = TextWrappingModes.Normal;
        }

        private static void CreateObjectivePanel(Transform parent, RunManager runManager, PlayerExperience experience, BossSpawner bossSpawner)
        {
            GameObject panel = new GameObject("ObjectivePanel");
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.sizeDelta = new Vector2(340f, 132f);
            panelRect.anchoredPosition = new Vector2(18f, -18f);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.005f, 0.014f, 0.02f, 0.64f);

            TMP_Text goalText = CreateText("GoalText", panel.transform, "DEMO GOAL\nSurvive. Build the spellbook.\nKill the Arcane Warden.", new Vector2(312f, 60f), new Vector2(14f, -12f));
            AnchorTopLeft(goalText.rectTransform, new Vector2(14f, -12f));
            goalText.alignment = TextAlignmentOptions.TopLeft;
            goalText.fontSize = 14f;
            goalText.color = new Color(0.82f, 0.96f, 1f);

            TMP_Text bossText = CreateText("BossText", panel.transform, "Boss in 02:00", new Vector2(312f, 24f), new Vector2(14f, -72f));
            AnchorTopLeft(bossText.rectTransform, new Vector2(14f, -74f));
            bossText.alignment = TextAlignmentOptions.Left;
            bossText.fontSize = 15f;
            bossText.color = new Color(1f, 0.72f, 0.9f);

            TMP_Text upgradeText = CreateText("UpgradeText", panel.transform, "Next draft: 0/5 XP", new Vector2(312f, 24f), new Vector2(14f, -100f));
            AnchorTopLeft(upgradeText.rectTransform, new Vector2(14f, -102f));
            upgradeText.alignment = TextAlignmentOptions.Left;
            upgradeText.fontSize = 15f;
            upgradeText.color = new Color(0.72f, 1f, 0.78f);

            ObjectivePresenter objective = panel.AddComponent<ObjectivePresenter>();
            SetObject(objective, "runManager", runManager);
            SetObject(objective, "playerExperience", experience);
            SetObject(objective, "bossSpawner", bossSpawner);
            SetObject(objective, "goalText", goalText);
            SetObject(objective, "bossText", bossText);
            SetObject(objective, "upgradeText", upgradeText);
        }

        private static Canvas CreateCanvas(string name)
        {
            GameObject canvasObject = new GameObject(name);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static GameObject CreateUiRoot(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }

        private static GameObject CreateCenteredPanel(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, Vector2 size, Vector2 anchoredPosition)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            TMP_Text label = go.AddComponent<TextMeshProUGUI>();
            if (runtimeFont != null)
            {
                label.font = runtimeFont;
            }

            label.text = text;
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 22f;
            return label;
        }

        private static Slider CreateSlider(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Color fillColor)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            Image background = root.AddComponent<Image>();
            background.color = new Color(0.03f, 0.05f, 0.07f, 0.78f);

            Image fill = CreateFilledImage("Fill", root.transform, size, Vector2.zero, fillColor);
            RectTransform fillRect = fill.rectTransform;
            fillRect.anchorMin = new Vector2(0f, 0.5f);
            fillRect.anchorMax = new Vector2(0f, 0.5f);
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.anchoredPosition = Vector2.zero;
            Slider slider = root.AddComponent<Slider>();
            slider.targetGraphic = background;
            slider.fillRect = fill.rectTransform;
            slider.interactable = false;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            return slider;
        }

        private static void AnchorTopCenter(RectTransform rect, Vector2 anchoredPosition)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
        }

        private static void AnchorTopLeft(RectTransform rect, Vector2 anchoredPosition)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
        }

        private static Image CreateFilledImage(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            Image image = go.AddComponent<Image>();
            image.color = color;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 1f;
            return image;
        }

        private static Image GetSliderFillImage(Slider slider)
        {
            return slider != null && slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;
        }

        private static RectTransform GetSliderFillRect(Slider slider)
        {
            return slider != null ? slider.fillRect : null;
        }

        private static Button CreateButton(string name, Transform parent, Vector2 size, Vector2 anchoredPosition)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            Image image = go.AddComponent<Image>();
            image.color = new Color(0.1f, 0.18f, 0.22f, 0.95f);
            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;
            TMP_Text label = CreateText("Label", go.transform, name, size, Vector2.zero);
            label.fontSize = 18f;
            return button;
        }

        private static void CreateMenuScene(string sceneName)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;
            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 9.1f;
            camera.backgroundColor = new Color(0.015f, 0.025f, 0.04f);
            camera.transform.position = new Vector3(0f, 0f, -10f);

            GameObject backdrop = new GameObject(sceneName + "Backdrop");
            backdrop.AddComponent<ArenaBackdrop>();

            GameObject musicObject = new GameObject(sceneName + "Music");
            BackgroundMusicPlayer music = musicObject.AddComponent<BackgroundMusicPlayer>();
            SetString(music, "resourcesClipPath", "Audio/Music/HeroicDemoLoop");
            SetFloat(music, "volume", sceneName == "MainMenu" ? 0.2f : 0.18f);
            musicObject.AddComponent<DemoAudioControls>();

            Canvas canvas = CreateCanvas(sceneName + "UI");
            if (sceneName == "MainMenu")
            {
                CreateMainMenu(canvas.transform);
            }
            else
            {
                CreateText(sceneName + "Title", canvas.transform, sceneName, new Vector2(500f, 80f), Vector2.zero);
            }

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, Scenes + "/" + sceneName + ".unity");
        }

        private static void CreateMainMenu(Transform parent)
        {
            GameObject presenterObject = new GameObject("MainMenuPresenter");
            presenterObject.transform.SetParent(parent, false);
            MainMenuPresenter presenter = presenterObject.AddComponent<MainMenuPresenter>();
            SetString(presenter, "gameSceneName", "Game");

            TMP_Text title = CreateText("Title", parent, "HEROIC", new Vector2(520f, 90f), new Vector2(0f, 170f));
            title.fontSize = 58f;
            title.color = new Color(0.72f, 0.96f, 1f);

            TMP_Text subtitle = CreateText("Subtitle", parent, "Living Spellbook Bullet Heaven Prototype", new Vector2(620f, 36f), new Vector2(0f, 108f));
            subtitle.fontSize = 22f;
            subtitle.color = new Color(0.78f, 0.88f, 0.92f);

            TMP_Text pitch = CreateText("Pitch", parent, "Arcane spells. Strategic movement. Survive the Warden.", new Vector2(680f, 44f), new Vector2(0f, 58f));
            pitch.fontSize = 20f;
            pitch.color = new Color(0.68f, 0.78f, 0.84f);

            Button startButton = CreateButton("Start Run", parent, new Vector2(260f, 56f), new Vector2(0f, -20f));
            UnityEventTools.AddPersistentListener(startButton.onClick, presenter.StartGame);
            TMP_Text startLabel = startButton.GetComponentInChildren<TMP_Text>();
            if (startLabel != null)
            {
                startLabel.text = "Start Run";
                startLabel.fontSize = 22f;
            }

            TMP_Text controls = CreateText("Controls", parent, "Move: WASD / Arrows    Skills: 1, 2, 3    Pause: Esc    Music: M    Volume: - / +", new Vector2(820f, 36f), new Vector2(0f, -100f));
            controls.fontSize = 18f;
            controls.color = new Color(0.7f, 0.84f, 0.9f);

            TMP_Text demoNote = CreateText("DemoNote", parent, "1.0 Showcase Mode preloads Arcane tools so the first run shows the core fantasy immediately.", new Vector2(760f, 48f), new Vector2(0f, -145f));
            demoNote.fontSize = 16f;
            demoNote.color = new Color(0.58f, 0.72f, 0.78f);
        }

        private static void CreateEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static EnemyDefinition CreateEnemyDefinition(string assetName, string displayName, GameObject prefab, int health, float speed, int contactDamage, int xp, VisualPresetApplier.Preset visualPreset, bool boss)
        {
            EnemyDefinition definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            SetString(definition, "id", assetName.ToLowerInvariant());
            SetString(definition, "enemyName", displayName);
            SetObject(definition, "prefab", prefab);
            SetInt(definition, "maxHealth", health);
            SetFloat(definition, "moveSpeed", speed);
            SetInt(definition, "contactDamage", contactDamage);
            SetInt(definition, "experienceValue", xp);
            SetEnum(definition, "visualPreset", visualPreset);
            SetBool(definition, "boss", boss);
            return SaveAsset(definition, ScriptableObjects + "/Enemies/" + assetName + ".asset");
        }

        private static WaveDefinition CreateWave(string assetName, int index, float startsAt, float duration, float interval, int minSpawnCount, int maxSpawnCount, params EnemyDefinition[] enemies)
        {
            WaveDefinition wave = ScriptableObject.CreateInstance<WaveDefinition>();
            SetInt(wave, "waveIndex", index);
            SetFloat(wave, "startsAtSeconds", startsAt);
            SetFloat(wave, "durationSeconds", duration);
            SetFloat(wave, "spawnInterval", interval);
            SetInt(wave, "minSpawnCount", minSpawnCount);
            SetInt(wave, "maxSpawnCount", maxSpawnCount);

            SerializedObject serialized = new SerializedObject(wave);
            SerializedProperty entries = serialized.FindProperty("spawnEntries");
            entries.arraySize = enemies.Length;
            for (int i = 0; i < enemies.Length; i++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("enemy").objectReferenceValue = enemies[i];
                entry.FindPropertyRelative("weight").intValue = 1;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return SaveAsset(wave, ScriptableObjects + "/Waves/" + assetName + ".asset");
        }

        private static WaveDefinition[] LoadWaveAssets(WaveDefinition[] waves)
        {
            WaveDefinition[] loadedWaves = new WaveDefinition[waves.Length];
            for (int i = 0; i < waves.Length; i++)
            {
                string path = waves[i] != null ? AssetDatabase.GetAssetPath(waves[i]) : string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    path = ScriptableObjects + "/Waves/Wave_" + (i + 1).ToString("000") + ".asset";
                }

                loadedWaves[i] = AssetDatabase.LoadAssetAtPath<WaveDefinition>(path);
                if (loadedWaves[i] == null)
                {
                    Debug.LogError($"Could not load wave asset for scene spawner: {path}");
                }
            }

            return loadedWaves;
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Scenes + "/MainMenu.unity", true),
                new EditorBuildSettingsScene(Scenes + "/Game.unity", true),
                new EditorBuildSettingsScene(Scenes + "/Results.unity", true)
            };
        }

        private static GameObject SavePrefab(GameObject go, string path)
        {
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static T SaveAsset<T>(T asset, string path) where T : Object
        {
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void SetObject(Object target, string property, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray<T>(Object target, string property, T[] values) where T : Object
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty array = serialized.FindProperty(property);
            if (array == null)
            {
                Debug.LogError($"Missing serialized array `{property}` on {target.name}.");
                return;
            }

            array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetInt(Object target, string property, int value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(Object target, string property, float value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string property, bool value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.boolValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string property, string value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum<T>(Object target, string property, T value) where T : System.Enum
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty serializedProperty = serialized.FindProperty(property);
            if (serializedProperty == null)
            {
                Debug.LogError($"Missing serialized property `{property}` on {target.name}.");
                return;
            }

            serializedProperty.enumValueIndex = System.Convert.ToInt32(value);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Texture2D[] LoadTextures(string[] assetPaths)
        {
            Texture2D[] textures = new Texture2D[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPaths[i]);
            }

            return textures;
        }
    }
}
