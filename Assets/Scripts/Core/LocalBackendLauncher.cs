using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LocalBackendLauncher : MonoBehaviour
{
    [Header("Backend")]
    [SerializeField] private bool launchBackendOnStart = true;
    [SerializeField] private string backendExeRelativePath = "Backend/server.exe";
    [SerializeField] private string backendHealthUrl = "http://127.0.0.1:8000/";
    [SerializeField] private float startupTimeoutSeconds = 8f;

    private Process backendProcess;

    private void Start()
    {
        if (!launchBackendOnStart)
            return;

        StartCoroutine(StartBackendRoutine());
    }

    private IEnumerator StartBackendRoutine()
    {
        yield return CheckBackendAlreadyRunning();

        if (IsBackendAvailable)
        {
            UnityEngine.Debug.Log("Backend already running.");
            yield break;
        }

        StartBackendProcess();

        float elapsed = 0f;

        while (elapsed < startupTimeoutSeconds)
        {
            yield return CheckBackendAlreadyRunning();

            if (IsBackendAvailable)
            {
                UnityEngine.Debug.Log("Backend started successfully.");
                yield break;
            }

            elapsed += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        UnityEngine.Debug.LogWarning("Backend was launched, but health check did not respond in time.");
    }

    private bool IsBackendAvailable { get; set; }

    private IEnumerator CheckBackendAlreadyRunning()
    {
        IsBackendAvailable = false;

        using UnityWebRequest request = UnityWebRequest.Get(backendHealthUrl);
        request.timeout = 1;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            IsBackendAvailable = true;
        }
    }

    private void StartBackendProcess()
    {
        string backendPath = Path.Combine(Application.dataPath, "..", backendExeRelativePath);
        backendPath = Path.GetFullPath(backendPath);

        if (!File.Exists(backendPath))
        {
            UnityEngine.Debug.LogWarning("Backend executable not found: " + backendPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = backendPath,
            WorkingDirectory = Path.GetDirectoryName(backendPath),
            UseShellExecute = false,
            CreateNoWindow = true
        };

        backendProcess = new Process
        {
            StartInfo = startInfo
        };

        backendProcess.Start();

        UnityEngine.Debug.Log("Backend process launched: " + backendPath);
    }

    private void OnApplicationQuit()
    {
        if (backendProcess == null)
            return;

        try
        {
            if (!backendProcess.HasExited)
            {
                backendProcess.Kill();
                backendProcess.Dispose();
            }
        }
        catch
        {
            // Игнорируем ошибку закрытия процесса при выходе из игры
        }
    }
}