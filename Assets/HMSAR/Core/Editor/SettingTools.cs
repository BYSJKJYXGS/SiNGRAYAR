using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.Rendering;

namespace HMS.Core
{
    public class SettingTools : EditorWindow
    {
        private static string defaultPackageName = "com.singray.com";

        [MenuItem("HMS/Tools/Setting/Configure Android Project Settings")]

        public static void ConfigureSettings()
        {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (currentBuildTarget != BuildTarget.Android)
            {
                Debug.Log("Switching to Android platform...");
                bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

                if (!switchResult)
                {
                    Debug.LogError("Failed to switch platform to Android.");
                    return;
                }

                EditorApplication.update += WaitForPlatformSwitch;
            }
            else
            {
                ShowPackageNamePrompt();
            }
        }
        private static void WaitForPlatformSwitch()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                EditorApplication.update -= WaitForPlatformSwitch;
                ShowPackageNamePrompt();
            }
        }

        private static void ConfigureAndApplyAllSettings()
        {
            ConfigureGraphicsAPI();
            SetGammaRendering();
            SetDefaultOrientation();
            SetApiLevels();
            SetScriptingBackend();
            SetApiCompatibilityLevel();
            SetIncrementalGC();
            SetTargetArchitectures();
            SetInternetAccess();
            SetWritePermission();

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, string.IsNullOrEmpty(packageName) ? defaultPackageName : packageName);
            Debug.Log("Package Name set to: " + (string.IsNullOrEmpty(packageName) ? defaultPackageName : packageName));
        }



        [MenuItem("Tools/Setting/Switch to Android Platform")]
        public static void CheckAndSwitchToAndroid()
        {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (currentBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                Debug.Log("Switched to Android platform.");
            }
            else
            {
                Debug.Log("Current platform is already Android.");
            }
        }
        [MenuItem("Tools/Setting/RenderingSetting")]
        private static void SetGammaRendering()
        {
            // 将Rendering设置为Gamma
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            Debug.Log("Rendering Color Space set to Gamma.");
        }

        [MenuItem("Tools/Setting/Graphics")]

        private static void ConfigureGraphicsAPI()
        {
            Debug.Log("Configuring Graphics API...");

            var graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);

            if (graphicsAPIs == null || graphicsAPIs.Length == 0 || graphicsAPIs[0] != GraphicsDeviceType.OpenGLES3)
            {
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            }

            Debug.Log("Graphics API set to OpenGLES3 only.");
        }
        [MenuItem("Tools/Setting/SetDefaultOrientation")]
        private static void SetDefaultOrientation()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            Debug.Log("Default Orientation set to Landscape Left.");
        }

        private static string packageName = "com.singray.com";
        [MenuItem("Tools/Setting/PackageName")]
        private static void ShowPackageNamePrompt()
        {
            var window = GetWindow<SettingTools>(true, "Set Package Name", true);
            window.minSize = new Vector2(300, 110);
            window.Show();
        }
        private void OnGUI()
        {
            GUILayout.Label("Please enter the package name for your Android application:", EditorStyles.wordWrappedLabel);
            packageName = EditorGUILayout.TextField("Package Name", packageName);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                ConfigureAndApplyAllSettings();
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private static void ConfigureProjectSettings()
        {
            ConfigureGraphicsAPI();
            SetGammaRendering();
            SetDefaultOrientation();
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
            Debug.Log("Package Name set to: " + packageName);
        }
        [MenuItem("Tools/Setting/SetApiLevels")]
        private static void SetApiLevels()
        {
            Debug.Log("Setting API Levels...");

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26; // Android 8.0
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel27; // Android 8.1

            Debug.Log("Minimum API Level set to Android 8.0 (API Level 26).");
            Debug.Log("Target API Level set to Android 8.1 (API Level 27).");
        }

        [MenuItem("Tools/Setting/SetScriptingBackend")]
        private static void SetScriptingBackend()
        {
            Debug.Log("Setting Scripting Backend to IL2CPP...");
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            Debug.Log("Scripting Backend set to IL2CPP.");
        }
        [MenuItem("Tools/Setting/SetApiCompatibilityLevel")]

        private static void SetApiCompatibilityLevel()
        {
            Debug.Log("Setting API Compatibility Level to .NET 4.x...");
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
            Debug.Log("API Compatibility Level set to .NET 4.x.");
        }
        [MenuItem("Tools/Setting/SetIncrementalGC")]

        private static void SetIncrementalGC()
        {
            Debug.Log("Disabling Incremental GC...");
            PlayerSettings.gcIncremental = false;
            Debug.Log("Incremental GC disabled.");
        }
        [MenuItem("Tools/Setting/SetTargetArchitectures")]

        private static void SetTargetArchitectures()
        {
            Debug.Log("Setting Target Architectures to ARM64...");
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            Debug.Log("Target Architectures set to ARM64.");
        }
        [MenuItem("Tools/Setting/SetInternetAccess")]
        private static void SetInternetAccess()
        {
            Debug.Log("Setting Internet Access to Require...");
            PlayerSettings.Android.forceInternetPermission = true;
            Debug.Log("Internet Access set to Require.");
        }
        [MenuItem("Tools/Setting/SetWritePermission")]
        private static void SetWritePermission()
        {
            Debug.Log("Setting Write Permission to External...");
            PlayerSettings.Android.forceSDCardPermission = true;
            Debug.Log("Write Permission set to External.");
        }

    }
}