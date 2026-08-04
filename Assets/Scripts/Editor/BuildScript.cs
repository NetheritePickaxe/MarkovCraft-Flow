#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    public static void Build()
    {
        // Get all scenes from build settings
        var scenes = EditorBuildSettings.scenes;
        string[] scenePaths = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled)
                scenePaths[i] = scenes[i].path;
        }

        string outputPath = System.Environment.GetEnvironmentVariable("BUILD_PATH");
        if (string.IsNullOrEmpty(outputPath))
            outputPath = "build";

        string targetStr = System.Environment.GetEnvironmentVariable("TARGET_PLATFORM");
        BuildTarget target = BuildTarget.StandaloneWindows64;
        if (!string.IsNullOrEmpty(targetStr))
        {
            if (targetStr == "Android")
                target = BuildTarget.Android;
            else if (targetStr == "StandaloneOSX")
                target = BuildTarget.StandaloneOSX;
        }

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenePaths,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"Build failed: {summary.result}");
            foreach (var step in report.steps)
            {
                foreach (var msg in step.messages)
                {
                    if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        Debug.LogError($"  [{msg.type}] {msg.content}");
                }
            }
            EditorApplication.Exit(1);
        }
    }
}
#endif