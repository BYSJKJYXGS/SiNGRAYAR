//using singrayAR;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HMS.Core
{
    public class SiNGRAYARFoundation : EditorWindow
    {
        
        private string version;
        private string inputString = ""; // 用户输入的字符串  
        private string message = "";     // 验证结果信息  
        private bool isValid = false;    // 验证结果  

        [MenuItem("HMS/Validator")]
        public static void ShowWindow()
        {
            GetWindow<SiNGRAYARFoundation>("Validator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Please enter the validation string", EditorStyles.boldLabel);
            inputString = EditorGUILayout.TextField("Validation String", inputString);

            if (GUILayout.Button("Validate"))
            {
                // 调用 DLL 中的方法进行验证  
                try
                {
                    //isValid = SingrayAR.SARValidator.Validate(inputString);
                    //if (isValid)
                    //{
                    //    message = "Validation successful! You can now proceed with the build.";
                    //    ValidationStatus.IsValid = true;
                    //}
                    //else
                    //{
                    //    message = "Validation failed! Cannot proceed with the build.";
                    //    ValidationStatus.IsValid = false;
                    //}
                }
                catch (System.Exception ex)
                {
                    message = "An error occurred during validation: " + ex.Message;
                    ValidationStatus.IsValid = false;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Validation Result:", message);
            EditorGUILayout.LabelField("Current Validation Status:", isValid ? "Passed" : "Failed");
        }
    }

    public static class ValidationStatus
    {
        private const string IsValidKey = "SingRAYValidation_IsValid";

        public static bool IsValid
        {
            get => EditorPrefs.GetBool(IsValidKey, false);
            set => EditorPrefs.SetBool(IsValidKey, value);
        }
    }

    public class BuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // 在构建前检查验证状态  
            if (!ValidationStatus.IsValid)
            {
                // 弹出验证窗口  
                SiNGRAYARFoundation.ShowWindow();

                // 抛出异常阻止构建  
                throw new BuildFailedException("Build blocked: Validation not passed. Please validate in the 'HMS > Validator' window.");
            }
        }
    }

    public class VersionWindow : EditorWindow
    {
        private Texture2D logoTexture;
        private string version;

        [MenuItem("HMS/Show Version")]
        public static void ShowWindow()
        {
            VersionWindow window = GetWindow<VersionWindow>("SiNGRAY AR SDK Version");
            window.Show();
        }

        private void OnEnable()
        {
            try
            {
                // 获取版本信息  
                //version = SingrayAR.GetVersion();

                // 加载图片资源  
                string imagePath = "Assets/SiNGRAY/Core/Editor/texture/singraylogo.png";
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
                if (logoTexture == null)
                {
                    Debug.LogError($"Unable to load image resource: {imagePath}");
                }

                // 设置窗口尺寸（最小和最大尺寸设置为 512x256）  
                minSize = new Vector2(512, 256);
                maxSize = new Vector2(512, 256);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error: {ex}");
                version = "Error retrieving version";
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (logoTexture != null)
            {
                float imageWidth = logoTexture.width / 2f;   // 512 / 2 = 256  
                float imageHeight = logoTexture.height / 2f; // 256 / 2 = 128  

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logoTexture, GUILayout.Width(imageWidth), GUILayout.Height(imageHeight));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Unable to load image.");
            }

            GUILayout.Space(20);

            // 显示版本信息  
            GUILayout.Label("SiNGRAY AR SDK", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label($"Current Version: {version}", EditorStyles.label);
        }
    }
}