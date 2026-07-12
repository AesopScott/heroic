using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Heroic.Editor
{
    public static class HeroicWebGLBuilder
    {
        public const string BuildPath = "Builds/WebGL";

        [MenuItem("Heroic/Build WebGL 1.0")]
        public static void BuildWebGL()
        {
            BuildWebGLPlayer();
        }

        public static bool BuildWebGLPlayer()
        {
            if (!Directory.Exists(BuildPath))
            {
                Directory.CreateDirectory(BuildPath);
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Heroic/Scenes/MainMenu.unity",
                    "Assets/_Heroic/Scenes/Game.unity",
                    "Assets/_Heroic/Scenes/Results.unity"
                },
                locationPathName = BuildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Heroic WebGL build succeeded: {summary.totalSize} bytes at {BuildPath}");
                return true;
            }

            Debug.LogError($"Heroic WebGL build failed: {summary.result}");
            return false;
        }
    }
}
