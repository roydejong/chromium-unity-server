using CefUnityLib;
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CefIntegrationBehavior : MonoBehaviour
{
    protected RawImage targetImage;
    protected CefController cef;
    protected Texture2D browserTexture;

    protected byte[] frameBuffer;
    protected bool frameBufferChanged;

    public string PipeName = "default";

    void Start()
    {
        targetImage = GetComponent<RawImage>();

        browserTexture = new Texture2D(1024, 768, TextureFormat.BGRA32, false);

        cef = new CefController(PipeName);

        if (StartAndConnectServer())
        {
            UnityEngine.Debug.Log("[CEF] Connected to proxy server process.");
        }

        targetImage.texture = browserTexture;
        targetImage.uvRect = new Rect(0f, 0f, 1f, -1f);

        cef.MessageReceived += OnCefMessage;

        frameBuffer = new byte[0];
        frameBufferChanged = false;
    }

    protected bool StartAndConnectServer()
    {
        // First connection attempt
        try
        {
            cef.Connect();
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("[CEF] Proxy server not responding, attempting to start server executable. Connection error details: " + e.Message);
        }

        // Determine path to CEF Unity Server
        //string cefPath = Application.dataPath;
        string cefPath = @"C:\YOUR_TEST_PATH\";
        string cefPathExec = cefPath + "/CefUnityServer.exe";

        // Start the process, hide it, and listen to its output
        var processInfo = new ProcessStartInfo();
        processInfo.Arguments = cef.PipeName;
        processInfo.CreateNoWindow = true;
        processInfo.FileName = cefPathExec;
        processInfo.WorkingDirectory = cefPath;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardInput = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.WindowStyle = ProcessWindowStyle.Hidden;

        var process = Process.Start(processInfo);
        process.ErrorDataReceived += Process_ErrorDataReceived;
        process.OutputDataReceived += Process_OutputDataReceived;

        // Basic wait time to let the server start (usually takes a quarter second or so on a reasonable machine)
        Thread.Sleep(250);

        // Wait for the app to start - as long as it doesn't fail and we don't exceed a certain timeout
        int attemptsRemaining = 10;
        Exception lastEx = null;

        do
        {
            try
            {
                // Connect - if okay, break out and proceed
                cef.Connect();
                return true;
            }
            catch (Exception ex)
            {
                // Connect failed, wait a bit and try again
                UnityEngine.Debug.Log("[CEF] Proxy server not responding. {0} attempt(s) remaining. Connection error details: " + ex.Message);

                attemptsRemaining--;
                lastEx = ex;

                if (attemptsRemaining <= 0)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        while (true);

        UnityEngine.Debug.Log("[CEF] Proxy server failed to start! (Hard failure)");
        throw lastEx;
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    protected void OnCefMessage(object sender, PipeProtoMessage p)
    {
        switch (p.Opcode)
        {
            case PipeProto.OPCODE_FRAME:

                frameBuffer = p.Payload;
                frameBufferChanged = true;
                break;
        }
    }

    void Update()
    {
        if (frameBufferChanged)
        {
            browserTexture.LoadRawTextureData(frameBuffer);
            browserTexture.Apply();

            frameBufferChanged = false;
        }
    }
}