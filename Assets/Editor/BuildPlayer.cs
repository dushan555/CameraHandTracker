using System;
using System.IO;
using UnityEditor;
using UnityEngine;


public class BuildPlayer : EditorWindow
{
    [SerializeField] private string appName;

    [MenuItem("Build/BuildWinApp")]
    private static void BuildWin()
    {
        var window = GetWindow<BuildPlayer>("打包工具");
        window.minSize = new Vector2(360, 120);
        window.appName = PlayerSettings.productName;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        appName = EditorGUILayout.TextField("应用名", appName);
        var name = (appName ?? string.Empty).Trim();
        var validName = !string.IsNullOrEmpty(name)
                        && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
                        && !name.EndsWith(".");
        if (!validName)
        {
            EditorGUILayout.HelpBox("请输入有效的应用名，不能包含文件名非法字符或以句点结尾。", MessageType.Warning);
        }

        EditorGUILayout.Space(10);
        using (new EditorGUI.DisabledScope(!validName || BuildPipeline.isBuildingPlayer))
        {
            if (GUILayout.Button("打包", GUILayout.Height(30)))
            {
                BuildWindowsPlayer(name);
            }
        }
    }

    private static void BuildWindowsPlayer(string applicationName)
    {
        PlayerSettings.productName = applicationName;
        var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");

        var localPath = Path.Combine(Environment.CurrentDirectory, "Build", $"{applicationName}_{timeStr}");
        Debug.Log(localPath);
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        var levels = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

        BuildPipeline.BuildPlayer(levels, Path.Combine(localPath, $"{applicationName}.exe"), BuildTarget.StandaloneWindows64,
            BuildOptions.None);
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }
}

