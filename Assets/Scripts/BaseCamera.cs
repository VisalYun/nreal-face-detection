using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

/// <summary>
/// Base WebCamera class that takes care about video capturing.
/// Is intended to be sub-classed and partially overridden to get
/// desired behavior in the user Unity script
/// </summary>
public abstract class BaseCamera : MonoBehaviour
{
    /// <summary>
    /// Target surface to render WebCam stream
    /// </summary>
    public GameObject Surface;
    public RawImage CaptureImage;
    public Camera camera;
    public bool isTaken = false;
    public bool isDetected = true;

    private Texture2D webCamTexture = null;
    private RenderTexture rt = null;
    private Texture2D renderedTexture = null;
    private UnityEngine.Rect rect;
    private NRKernal.NRRGBCamTextureYUV YuvCamTexture { get; set; }
    int resWidth = 1920;
    int resHeight = 1080;

    protected OpenCvSharp.Unity.TextureConversionParams TextureParameters { get; private set; }


    /// <summary>
    /// Default initializer for MonoBehavior sub-classes
    /// </summary>
    protected virtual void Awake()
    {
        rt = new RenderTexture(resWidth, resHeight, 24);
        webCamTexture = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        rect = new UnityEngine.Rect(0, 0, resWidth, resHeight);

        YuvCamTexture = new NRKernal.NRRGBCamTextureYUV();
        BindYuvTexture(YuvCamTexture.GetTexture());
        YuvCamTexture.Play();
    }


    /// <summary>
    /// Updates web camera texture
    /// </summary>
    private void Update()
    {
        // Debug.Log(isTaken);
        // Debug.Log(webCamTexture != null);
        if (webCamTexture != null && !isTaken)
        {
            // this must be called continuously
            isTaken = true;
            BindYuvTexture(YuvCamTexture.GetTexture());
            StartCoroutine(takeScreenShot());

            // process texture with whatever method sub-class might have in mind
            if (ProcessTexture(webCamTexture, ref renderedTexture))
            {
                //isDetected = false;
                RenderFrame();
                //ProcessImageToServer();
                if (isDetected)
                {
                    isDetected = false;
                    ProcessImageToServer();
                }
            }
        }
    }


    private void BindYuvTexture(NRKernal.NRRGBCamTextureYUV.YUVTextureFrame frame)
    {
        CaptureImage.material.SetTexture("_MainTex", frame.textureY);
        CaptureImage.material.SetTexture("_UTex", frame.textureU);
        CaptureImage.material.SetTexture("_VTex", frame.textureV);
    }


    IEnumerator takeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        webCamTexture.ReadPixels(rect, 0, 0);
        //camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        //Destroy(rt);
    }


    /// <summary>
    /// Processes current texture
    /// This function is intended to be overridden by sub-classes
    /// </summary>
    /// <param name="input">Input WebCamTexture object</param>
    /// <param name="output">Output Texture2D object</param>
    /// <returns>True if anything has been processed, false if output didn't change</returns>
    protected abstract bool ProcessTexture(Texture2D input, ref Texture2D output);

    protected abstract void ProcessImageToServer();


    /// <summary>
    /// Renders frame onto the surface
    /// </summary>
    private void RenderFrame()
    {
        if (renderedTexture != null)
        {
            // apply
            Surface.GetComponent<RawImage>().texture = renderedTexture;
            isTaken = false;
        }
    }
}
