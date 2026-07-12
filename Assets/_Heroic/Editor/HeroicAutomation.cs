using System;
using UnityEditor;
using UnityEngine;

namespace Heroic.Editor
{
    public static class HeroicAutomation
    {
        public static void BuildPrototypeValidateAndWebGL()
        {
            int exitCode = 0;
            try
            {
                HeroicPrototypeBuilder.BuildPrototypeContent();

                HeroicReadinessValidator.ValidationSummary validation = HeroicReadinessValidator.ValidatePrototypeContent();
                if (!validation.Passed)
                {
                    Debug.LogError($"Heroic automation stopped after validation failed with {validation.Errors} error(s) and {validation.Warnings} warning(s).");
                    exitCode = 2;
                }
                else if (!HeroicWebGLBuilder.BuildWebGLPlayer())
                {
                    exitCode = 3;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                exitCode = 1;
            }

            CompleteAutomation(exitCode);
        }

        public static void BuildPrototypeAndValidate()
        {
            int exitCode = 0;
            try
            {
                HeroicPrototypeBuilder.BuildPrototypeContent();
                HeroicReadinessValidator.ValidationSummary validation = HeroicReadinessValidator.ValidatePrototypeContent();
                if (!validation.Passed)
                {
                    Debug.LogError($"Heroic automation validation failed with {validation.Errors} error(s) and {validation.Warnings} warning(s).");
                    exitCode = 2;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                exitCode = 1;
            }

            CompleteAutomation(exitCode);
        }

        private static void CompleteAutomation(int exitCode)
        {
            if (exitCode == 0)
            {
                Debug.Log("Heroic automation completed successfully.");
            }
            else
            {
                Debug.LogError($"Heroic automation failed with exit code {exitCode}.");
            }

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(exitCode);
            }
        }
    }
}
