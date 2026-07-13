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
using Heroic.World;
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
        private const string PairedSystemIconSheetPath = "Assets/mobs/paired_systems.png";
        private const string PickupIconRoot = "Assets/Resources/PickupIcons/pickup-art";
        private const string AbilitiesCurrentSourcePath = "G:/My Drive/heroic/reference/abilities-current.md";
        private const string AbilitiesCurrentResourcePath = "Assets/Resources/Reference/abilities-current.txt";
        private static readonly string[] EarlyLevelMudTexturePaths =
        {
            "Assets/_Heroic/Art/TerrainSlices/terrain_i_mud.png",
            "Assets/_Heroic/Art/TerrainSlices/terrain_ii_mud.png",
            "Assets/_Heroic/Art/TerrainSlices/terrain_iii_mud.png",
            "Assets/_Heroic/Art/TerrainSlices/terrain_iv_mud.png",
            "Assets/_Heroic/Art/TerrainSlices/terrain_v_mud.png"
        };
        private const string DefaultArenaBackgroundPath = "Assets/mobs/dirt II 8192.png";
        private const int TerrainLayer = 8;
        private const int TerrainLayerMask = 1 << TerrainLayer;
        private static readonly string[] TerrainSheetPaths =
        {
            "Assets/mobs/terrain I.png",
            "Assets/mobs/terrain II.png",
            "Assets/mobs/terrain III.png",
            "Assets/mobs/terrain IV.png",
            "Assets/mobs/terrain V.png"
        };

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
            SyncAbilitiesCurrentReference();
            runtimeFont = CreateOrLoadRuntimeFont();

            GameObject xpPickup = CreateXpPickupPrefab();
            GameObject healthLoot = CreateLootPickupPrefab("Loot_HealthRestore", LootPickup.LootKind.HealthRestore);
            GameObject experienceLoot = CreateLootPickupPrefab("Loot_ExperienceBoost", LootPickup.LootKind.ExperienceBoost);
            GameObject speedLoot = CreateLootPickupPrefab("Loot_SpeedBoost", LootPickup.LootKind.SpeedBoost);
            GameObject invulnerabilityLoot = CreateLootPickupPrefab("Loot_Invulnerability", LootPickup.LootKind.Invulnerability);
            GameObject projectile = CreateMagicMissilePrefab();
            GameObject fireProjectile = CreateFireProjectilePrefab();
            GameObject enemyMissile = CreateEnemyMissilePrefab();
            GameObject orb = CreateArcaneOrbPrefab();
            GameObject enemy = CreateEnemyPrefab(xpPickup, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
            GameObject wall = CreateWallPrefab(xpPickup, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
            GameObject Thrower = CreateThrowerEnemyPrefab(xpPickup, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot, enemyMissile);
            GameObject boss = CreateBossPrefab(xpPickup, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);

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
            EnsureFolder("Assets", "Resources");
            EnsureFolder("Assets/Resources", "Reference");
        }

        private static void SyncAbilitiesCurrentReference()
        {
            if (!File.Exists(AbilitiesCurrentSourcePath))
            {
                Debug.LogWarning($"Canonical abilities-current source not found: {AbilitiesCurrentSourcePath}");
                return;
            }

            string targetDirectory = Path.GetDirectoryName(AbilitiesCurrentResourcePath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            File.Copy(AbilitiesCurrentSourcePath, AbilitiesCurrentResourcePath, true);
            AssetDatabase.ImportAsset(AbilitiesCurrentResourcePath, ImportAssetOptions.ForceSynchronousImport);
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
            SetFloat(pickup, "magnetRange", 1f);
            SetFloat(pickup, "magnetSpeed", 11f);
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.ExperiencePickup);
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Pickup, 0.3f);
            return SavePrefab(go, Prefabs + "/Pickups/XP_Pickup.prefab");
        }

        private static GameObject CreateLootPickupPrefab(string name, LootPickup.LootKind kind)
        {
            GameObject go = new GameObject(name);
            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            LootPickup pickup = go.AddComponent<LootPickup>();
            SetEnum(pickup, "kind", kind);
            SetFloat(pickup, "magnetRange", 0.5f);
            SetFloat(pickup, "magnetSpeed", 9f);
            Sprite icon = LoadPickupIcon(kind);
            if (icon != null)
            {
                SetObject(pickup, "iconSprite", icon);
            }

            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Pickup, 0.3f);
            return SavePrefab(go, Prefabs + "/Pickups/" + name + ".prefab");
        }

        private static Sprite LoadPickupIcon(LootPickup.LootKind kind)
        {
            string path = kind switch
            {
                LootPickup.LootKind.HealthRestore => PickupIconRoot + "/pickup_health_potion.png",
                LootPickup.LootKind.ExperienceBoost => PickupIconRoot + "/pickup_xp_crystal.png",
                LootPickup.LootKind.SpeedBoost => PickupIconRoot + "/pickup_speed_boot.png",
                LootPickup.LootKind.Invulnerability => PickupIconRoot + "/pickup_invulnerability_shield.png",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            EnsureTextureReadable(path);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static GameObject CreateEnemyPrefab(GameObject xpPickup, GameObject healthLoot, GameObject experienceLoot, GameObject speedLoot, GameObject invulnerabilityLoot)
        {
            GameObject go = new GameObject("Enemy_Crash");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetLayerMask(controller, "blockingLayers", TerrainLayerMask);
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            WireLootDropper(dropper, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
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

        private static GameObject CreateWallPrefab(GameObject xpPickup, GameObject healthLoot, GameObject experienceLoot, GameObject speedLoot, GameObject invulnerabilityLoot)
        {
            GameObject go = new GameObject("Enemy_Wall");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetLayerMask(controller, "blockingLayers", TerrainLayerMask);
            SetBool(controller, "destroyAfterContactDamage", false);
            SetBool(controller, "suppressExperienceOnContactDamage", false);
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            WireLootDropper(dropper, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
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

        private static GameObject CreateThrowerEnemyPrefab(GameObject xpPickup, GameObject healthLoot, GameObject experienceLoot, GameObject speedLoot, GameObject invulnerabilityLoot, GameObject enemyMissile)
        {
            GameObject go = new GameObject("Enemy_Thrower");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetLayerMask(controller, "blockingLayers", TerrainLayerMask);
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
            WireLootDropper(dropper, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
            VisualPresetApplier visual = go.AddComponent<VisualPresetApplier>();
            SetEnum(visual, "preset", VisualPresetApplier.Preset.ThrowerLevel1);
            SetObjectArray(visual, "throwerLevel1Frames", LoadTextures(Thrower1FramePaths));
            go.AddComponent<HitFlashVisual>();
            go.AddComponent<DeathBurstVisual>();
            go.AddComponent<WorldHealthBar>();
            go.AddComponent<DamageNumberEmitter>();
            AddAudioFeedback(go, ProceduralAudioFeedback.Preset.Enemy, 0.32f);
            return SavePrefab(go, Prefabs + "/Enemies/Enemy_Thrower.prefab");
        }

        private static GameObject CreateBossPrefab(GameObject xpPickup, GameObject healthLoot, GameObject experienceLoot, GameObject speedLoot, GameObject invulnerabilityLoot)
        {
            GameObject go = new GameObject("Enemy_Boss_ArcaneWarden");
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Damageable>();
            EnemyController controller = go.AddComponent<EnemyController>();
            SetLayerMask(controller, "blockingLayers", TerrainLayerMask);
            SetBool(controller, "destroyAfterContactDamage", false);
            SetBool(controller, "suppressExperienceOnContactDamage", false);
            go.AddComponent<BossController>();
            ExperienceDropper dropper = go.AddComponent<ExperienceDropper>();
            SetObject(dropper, "pickupPrefab", xpPickup.GetComponent<ExperiencePickup>());
            WireLootDropper(dropper, healthLoot, experienceLoot, speedLoot, invulnerabilityLoot);
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

        private static void WireLootDropper(ExperienceDropper dropper, GameObject healthLoot, GameObject experienceLoot, GameObject speedLoot, GameObject invulnerabilityLoot)
        {
            SetObject(dropper, "healthRestorePrefab", healthLoot.GetComponent<LootPickup>());
            SetObject(dropper, "experienceBoostPrefab", experienceLoot.GetComponent<LootPickup>());
            SetObject(dropper, "speedBoostPrefab", speedLoot.GetComponent<LootPickup>());
            SetObject(dropper, "invulnerabilityPrefab", invulnerabilityLoot.GetComponent<LootPickup>());
        }

        private static void CreateGameScene(GameObject projectile, GameObject fireProjectile, GameObject orb, GameObject enemy, GameObject boss, GameObject xpPickup, EnemyDefinition bossDefinition, WaveDefinition[] waves)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Game";

            GameObject arena = new GameObject("ArenaBackdrop");
            arena.transform.position = Vector3.zero;
            ArenaBackdrop arenaBackdrop = arena.AddComponent<ArenaBackdrop>();
            EnsureTextureReadable(DefaultArenaBackgroundPath);
            SetObject(arenaBackdrop, "dirtSourceTexture", AssetDatabase.LoadAssetAtPath<Texture2D>(DefaultArenaBackgroundPath));
            SetBool(arenaBackdrop, "useSourceTextureDirectly", true);

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
            MagicSystemController magicSystemController = player.GetComponent<MagicSystemController>();
            GameObject terrainObject = new GameObject("DynamicTerrainGrid");
            DynamicTerrainGrid terrainGrid = terrainObject.AddComponent<DynamicTerrainGrid>();
            SetObjectArray(terrainGrid, "terrainSheets", LoadTextures(TerrainSheetPaths));
            SetObjectArray(terrainGrid, "dirtTextures", LoadTerrainSlices("dirt", "rough_dirt_left", "rough_dirt_right"));
            SetObjectArray(terrainGrid, "decorativeTextures", LoadTerrainSlices("rock_grass", "brush", "raised_dirt"));
            SetObjectArray(terrainGrid, "mudTextures", LoadTerrainSlices("mud"));
            SetObjectArray(terrainGrid, "waterTextures", LoadTerrainSlices("water"));
            SetObjectArray(terrainGrid, "looseStoneTextures", LoadTerrainSlices("loose_stone"));
            SetObjectArray(terrainGrid, "highGroundTextures", LoadTerrainSlices("raised_dirt", "stone_floor"));
            SetObjectArray(terrainGrid, "blockerTextures", LoadTerrainSlices("boulder", "wall_left", "wall_right", "pillar"));
            SetObject(terrainGrid, "playerReference", player.transform);
            SetObject(terrainGrid, "playerExperience", playerExperience);
            SetInt(terrainGrid, "terrainLayer", TerrainLayer);

            SetObject(runEndWatcher, "runManager", runManager);
            SetObject(runEndWatcher, "playerHealth", playerHealth);
            SetObject(demoSafetyHotkeys, "runManager", runManager);

            SetObject(enemySpawner, "enemyPrefab", enemy.GetComponent<EnemyController>());
            SetObject(enemySpawner, "playerTarget", player.transform);
            SetObject(enemySpawner, "runManager", runManager);
            SetObject(enemySpawner, "playerExperience", playerExperience);
            SetObjectArray(enemySpawner, "waves", LoadWaveAssets(waves));
            SetObject(enemySpawner, "terrainGrid", terrainGrid);

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
            SetObject(choiceApplier, "magicSystemController", magicSystemController);
            SetObject(choiceApplier, "arcaneUpgradeApplier", arcaneUpgradeApplier);
            SetObject(choiceApplier, "fireUpgradeApplier", fireUpgradeApplier);

            WireArcaneUpgradeApplier(arcaneUpgradeApplier, player);
            WireFireUpgradeApplier(fireUpgradeApplier, player);

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
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
            player.AddComponent<MagicSystemController>();
            player.AddComponent<PlayerHealth>();
            player.AddComponent<PlayerStealth>();
            player.AddComponent<PlayerTemporaryBuffs>();
            PlayerExperience playerExperience = player.AddComponent<PlayerExperience>();
            SetInt(playerExperience, "baseExperienceToLevel", 15);
            player.AddComponent<ArcaneDoubleCast>();
            SpellEchoCaster spellEcho = player.AddComponent<SpellEchoCaster>();
            MagicMissileCaster magicMissile = player.AddComponent<MagicMissileCaster>();
            ArcaneBlastCaster arcaneBlast = player.AddComponent<ArcaneBlastCaster>();
            WarpPulseCaster warpPulse = player.AddComponent<WarpPulseCaster>();
            ArcaneOrbitCaster arcaneOrbit = player.AddComponent<ArcaneOrbitCaster>();
            ArcaneUtilityCaster forceField = player.AddComponent<ArcaneUtilityCaster>();
            ArcaneUtilityCaster timeWarp = player.AddComponent<ArcaneUtilityCaster>();
            ArcaneUtilityCaster haste = player.AddComponent<ArcaneUtilityCaster>();
            FireBoltCaster fireBolt = player.AddComponent<FireBoltCaster>();
            FlameWaveCaster flameWave = player.AddComponent<FlameWaveCaster>();
            BurningGroundCaster burningGround = player.AddComponent<BurningGroundCaster>();
            FlameShieldCaster flameShield = player.AddComponent<FlameShieldCaster>();
            FlameWallCaster flameWall = player.AddComponent<FlameWallCaster>();
            SpellCaster spellCaster = player.AddComponent<SpellCaster>();
            MovementCaster movementCaster = player.AddComponent<MovementCaster>();
            SetBool(movementCaster, "equipPrototypeMovementSetOnStart", false);
            SetLayerMask(movementCaster, "blockingLayers", TerrainLayerMask);
            PlayerVisualController visual = player.AddComponent<PlayerVisualController>();
            SetObjectArray(visual, "levelOneFrames", LoadTextures(PlayerLevel1FramePaths));
            SetObjectArray(visual, "levelTwoFrames", LoadTextures(PlayerLevel2FramePaths));
            SetObjectArray(visual, "levelSixFrames", LoadTextures(PlayerLevel6FramePaths));
            player.AddComponent<HitFlashVisual>();
            player.AddComponent<WorldHealthBar>();
            player.AddComponent<DamageNumberEmitter>();
            player.AddComponent<PlayerBuffReadout>();
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
            SetEnum(forceField, "mode", ArcaneUtilityCaster.ArcaneUtilityMode.ForceField);
            SetFloat(forceField, "castInterval", 4f);
            SetFloat(forceField, "radius", 1.45f);
            SetInt(forceField, "damage", 14);
            SetObject(forceField, "spellEcho", spellEcho);
            SetEnum(timeWarp, "mode", ArcaneUtilityCaster.ArcaneUtilityMode.TimeWarp);
            SetFloat(timeWarp, "castInterval", 5.5f);
            SetFloat(timeWarp, "range", 8.5f);
            SetFloat(timeWarp, "radius", 1.7f);
            SetInt(timeWarp, "damage", 8);
            SetObject(timeWarp, "spellEcho", spellEcho);
            SetEnum(haste, "mode", ArcaneUtilityCaster.ArcaneUtilityMode.Haste);
            SetFloat(haste, "castInterval", 6f);
            SetFloat(haste, "duration", 2.8f);
            SetFloat(haste, "speedMultiplier", 1.45f);
            SetObject(haste, "spellEcho", spellEcho);
            SetObject(fireBolt, "projectilePrefab", fireProjectile.GetComponent<Projectile>());
            SetObject(fireBolt, "firePoint", firePoint.transform);
            SetObject(fireBolt, "spellEcho", spellEcho);
            SetObject(flameWave, "spellEcho", spellEcho);
            SetObject(burningGround, "spellEcho", spellEcho);
            SetObject(flameShield, "spellEcho", spellEcho);
            SetObject(flameWall, "spellEcho", spellEcho);
            SetObject(spellCaster, "magicMissileCaster", magicMissile);
            SetObject(spellCaster, "arcaneBlastCaster", arcaneBlast);
            SetObject(spellCaster, "warpPulseCaster", warpPulse);
            SetObject(spellCaster, "spellEchoCaster", spellEcho);
            SetObject(spellCaster, "arcaneOrbitCaster", arcaneOrbit);
            SetObject(spellCaster, "forceFieldCaster", forceField);
            SetObject(spellCaster, "timeWarpCaster", timeWarp);
            SetObject(spellCaster, "hasteCaster", haste);
            SetObject(spellCaster, "fireBoltCaster", fireBolt);
            SetObject(spellCaster, "flameWaveCaster", flameWave);
            SetObject(spellCaster, "burningGroundCaster", burningGround);
            SetObject(spellCaster, "flameShieldCaster", flameShield);
            SetObject(spellCaster, "flameWallCaster", flameWall);
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
            SetObject(applier, "forceField", player.GetComponent<ArcaneUtilityCaster>());
            ArcaneUtilityCaster[] utilityCasters = player.GetComponents<ArcaneUtilityCaster>();
            if (utilityCasters.Length >= 3)
            {
                SetObject(applier, "forceField", utilityCasters[0]);
                SetObject(applier, "timeWarp", utilityCasters[1]);
                SetObject(applier, "haste", utilityCasters[2]);
            }
        }

        private static void WireFireUpgradeApplier(FireUpgradeApplier applier, GameObject player)
        {
            SetObject(applier, "fireBolt", player.GetComponent<FireBoltCaster>());
            SetObject(applier, "flameWave", player.GetComponent<FlameWaveCaster>());
            SetObject(applier, "burningGround", player.GetComponent<BurningGroundCaster>());
            SetObject(applier, "flameShield", player.GetComponent<FlameShieldCaster>());
            SetObject(applier, "flameWall", player.GetComponent<FlameWallCaster>());
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
            pauseBackdrop.raycastTarget = false;
            Image resultsBackdrop = resultsRoot.AddComponent<Image>();
            resultsBackdrop.color = new Color(0.005f, 0.012f, 0.018f, 0.78f);

            HudPresenter hud = gameRoot.AddComponent<HudPresenter>();
            SetObject(hud, "playerHealth", health);
            SetObject(hud, "playerExperience", experience);
            SetObject(hud, "runManager", runManager);

            TMP_Text levelText = CreateText("LevelText", gameRoot.transform, "Level 1", new Vector2(300f, 64f), new Vector2(180f, -30f));
            levelText.fontSize = 44f;
            TMP_Text timerText = CreateText("TimerText", gameRoot.transform, "00:00", new Vector2(300f, 64f), new Vector2(0f, -30f));
            timerText.fontSize = 44f;
            TMP_Text healthText = CreateText("HealthText", gameRoot.transform, "HP 100/100", new Vector2(320f, 52f), new Vector2(-180f, -30f));
            healthText.fontSize = 40f;
            TMP_Text experienceText = CreateText("ExperienceText", gameRoot.transform, "XP 0/15", new Vector2(320f, 48f), new Vector2(90f, -30f));
            experienceText.fontSize = 36f;
            TMP_Text showcaseText = CreateText("ShowcaseLabel", gameRoot.transform, "Heroic 1.0 Showcase", new Vector2(720f, 64f), new Vector2(0f, -68f));
            showcaseText.fontSize = 34f;
            Slider healthSlider = CreateSlider("HealthBar", gameRoot.transform, new Vector2(600f, 28f), new Vector2(0f, -55f), new Color(0.92f, 0.24f, 0.2f, 0.95f));
            Slider experienceSlider = CreateSlider("ExperienceBar", gameRoot.transform, new Vector2(600f, 20f), new Vector2(0f, -77f), new Color(0.24f, 0.64f, 1f, 0.95f));
            AnchorTopCenter(healthText.rectTransform, new Vector2(-340f, -36f));
            AnchorTopCenter(timerText.rectTransform, new Vector2(0f, -36f));
            AnchorTopCenter(levelText.rectTransform, new Vector2(340f, -36f));
            AnchorTopCenter(experienceText.rectTransform, new Vector2(0f, -128f));
            AnchorTopCenter(healthSlider.GetComponent<RectTransform>(), new Vector2(0f, -76f));
            AnchorTopCenter(experienceSlider.GetComponent<RectTransform>(), new Vector2(0f, -106f));
            AnchorTopCenter(showcaseText.rectTransform, new Vector2(0f, -168f));
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

            SkillSideHudPresenter sideHud = gameRoot.AddComponent<SkillSideHudPresenter>();
            SetObject(sideHud, "buildState", buildState);
            SetObject(sideHud, "pairedSystemIconSheet", AssetDatabase.LoadAssetAtPath<Texture2D>(PairedSystemIconSheetPath));
            CreateAudioControlsPanel(gameRoot.transform, new Vector2(-22f, 154f), true);

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
                SetObject(presenter, "buildState", buildState);
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
            TMP_Text[] elementNameLabels = new TMP_Text[5];
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

                TMP_Text elementLabel = CreateText("ElementName", button.transform, "Arcane", new Vector2(112f, 22f), new Vector2(-121f, -39f));
                elementLabel.fontSize = 15f;
                elementLabel.fontStyle = FontStyles.Bold;
                elementLabel.alignment = TextAlignmentOptions.Center;
                elementLabel.raycastTarget = false;

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
                elementNameLabels[i] = elementLabel;
            }
            SetObjectArray(draft, "choiceButtons", buttons);
            SetObjectArray(draft, "choiceLabels", labels);
            SetObjectArray(draft, "choiceBars", bars);
            SetObjectArray(draft, "categoryIconBackdrops", categoryIconBackdrops);
            SetObjectArray(draft, "categoryIconLabels", categoryIconLabels);
            SetObjectArray(draft, "skillIconBackdrops", skillIconBackdrops);
            SetObjectArray(draft, "skillIconLabels", skillIconLabels);
            SetObjectArray(draft, "elementNameLabels", elementNameLabels);

            CreatePausePanel(pauseRoot.transform);

            ResultsPresenter results = resultsRoot.AddComponent<ResultsPresenter>();
            SetObject(results, "runManager", runManager);
            SetString(results, "gameSceneName", "Game");
            SetString(results, "mainMenuSceneName", "MainMenu");

            GameObject resultsPanel = CreateCenteredPanel("ResultsPanel", resultsRoot.transform, new Vector2(560f, 320f), Vector2.zero, new Color(0.015f, 0.04f, 0.052f, 0.9f));
            CreateAudioControlsPanel(resultsRoot.transform, new Vector2(-22f, 154f), true);
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
            GameObject panel = CreateCenteredPanel("PausePanel", parent, new Vector2(820f, 420f), Vector2.zero, new Color(0.015f, 0.04f, 0.052f, 0.9f));
            TMP_Text title = CreateText("PauseTitle", panel.transform, "PAUSED", new Vector2(720f, 110f), new Vector2(0f, 124f));
            title.fontSize = 68f;
            title.color = new Color(0.82f, 0.96f, 1f);

            TMP_Text resume = CreateText("PauseResume", panel.transform, "Press Esc to resume the run.", new Vector2(720f, 70f), new Vector2(0f, 32f));
            resume.fontSize = 40f;
            resume.color = new Color(0.9f, 0.96f, 0.98f);

            TMP_Text controls = CreateText("PauseControls", panel.transform, "Audio: M mute music   - / + master volume\nSafety: F8 defeat   F9 victory", new Vector2(740f, 140f), new Vector2(0f, -106f));
            controls.fontSize = 34f;
            controls.color = new Color(0.72f, 0.88f, 0.92f);
            controls.textWrappingMode = TextWrappingModes.Normal;
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
            camera.gameObject.AddComponent<AudioListener>();
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
                CreateAudioControlsPanel(canvas.transform, new Vector2(-176f, -42f));
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

            TMP_Text title = CreateText("Title", parent, "HEROIC", new Vector2(900f, 170f), new Vector2(0f, 240f));
            title.fontSize = 116f;
            title.color = new Color(0.72f, 0.96f, 1f);

            TMP_Text subtitle = CreateText("Subtitle", parent, "Living Spellbook Bullet Heaven Prototype", new Vector2(1000f, 72f), new Vector2(0f, 126f));
            subtitle.fontSize = 44f;
            subtitle.color = new Color(0.78f, 0.88f, 0.92f);

            TMP_Text pitch = CreateText("Pitch", parent, "Arcane spells. Strategic movement. Survive the Warden.", new Vector2(1100f, 88f), new Vector2(0f, 48f));
            pitch.fontSize = 40f;
            pitch.color = new Color(0.68f, 0.78f, 0.84f);

            Button startButton = CreateButton("Start Run", parent, new Vector2(440f, 100f), new Vector2(0f, -70f));
            UnityEventTools.AddPersistentListener(startButton.onClick, presenter.StartGame);
            TMP_Text startLabel = startButton.GetComponentInChildren<TMP_Text>();
            if (startLabel != null)
            {
                startLabel.text = "Start Run";
                startLabel.fontSize = 44f;
            }

            TMP_Text controls = CreateText("Controls", parent, "Move: WASD / Arrows    Skills: 1, 2, 3    Pause: Esc    Music: M    Volume: - / +", new Vector2(1280f, 72f), new Vector2(0f, -190f));
            controls.fontSize = 36f;
            controls.color = new Color(0.7f, 0.84f, 0.9f);
            CreateAudioControlsPanel(parent, new Vector2(0f, -292f));

            TMP_Text demoNote = CreateText("DemoNote", parent, "1.0 Showcase Mode preloads Arcane tools so the first run shows the core fantasy immediately.", new Vector2(1280f, 96f), new Vector2(0f, -244f));
            demoNote.fontSize = 32f;
            demoNote.color = new Color(0.58f, 0.72f, 0.78f);
        }

        private static AudioControlsPresenter CreateAudioControlsPanel(Transform parent, Vector2 anchoredPosition, bool anchorBottomRight = false)
        {
            GameObject panel = new GameObject("AudioControls");
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorBottomRight ? new Vector2(1f, 0f) : new Vector2(1f, 1f);
            rect.anchorMax = anchorBottomRight ? new Vector2(1f, 0f) : new Vector2(1f, 1f);
            rect.pivot = anchorBottomRight ? new Vector2(1f, 0f) : new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(260f, 74f);
            rect.anchoredPosition = anchoredPosition;
            return panel.AddComponent<AudioControlsPresenter>();
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

        private static void EnsureTextureReadable(string assetPath)
        {
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            bool changed = false;
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                changed = true;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        private static void EnsureTexturesReadable(string[] assetPaths)
        {
            foreach (string assetPath in assetPaths)
            {
                EnsureTextureReadable(assetPath);
            }
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

        private static void SetLayerMask(Object target, string property, int value)
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

        private static Texture2D[] LoadTerrainSlices(params string[] types)
        {
            var textures = new System.Collections.Generic.List<Texture2D>();
            string[] packIds = { "i", "ii", "iii", "iv", "v" };
            foreach (string packId in packIds)
            {
                foreach (string type in types)
                {
                    string path = $"Assets/_Heroic/Art/TerrainSlices/terrain_{packId}_{type}.png";
                    EnsureTextureReadable(path);
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (texture != null)
                    {
                        textures.Add(texture);
                    }
                }
            }

            return textures.ToArray();
        }
    }
}
