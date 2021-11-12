using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using OpenCvSharp;

public class FaceDetector : BaseCamera
{
    public TextAsset faces;
    public TextAsset eyes;
    public TextAsset shapes;
    public RawImage FaceInfo;
    public Button DoneBtn;
    public Text StatusText;

    private FaceProcessorLive<WebCamTexture> processor;

    private List<UnityEngine.Texture2D> FaceTextures = new List<UnityEngine.Texture2D>();

    /// <summary>
    /// Default initializer for MonoBehavior sub-classes
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        byte[] shapeDat = shapes.bytes;
        if (shapeDat.Length == 0)
        {
            string errorMessage =
                "In order to have Face Landmarks working you must download special pre-trained shape predictor " +
                "available for free via DLib library website and replace a placeholder file located at " +
                "\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
                "Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
				// query user to download the proper shape predictor
				if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
					Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
            UnityEngine.Debug.Log(errorMessage);
#endif
        }

        processor = new FaceProcessorLive<WebCamTexture>();
        processor.Initialize(faces.text, eyes.text, shapes.bytes);

        // data stabilizer - affects face rects, face landmarks etc.
        processor.DataStabilizer.Enabled = true;        // enable stabilizer
        processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
        processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

        // performance data - some tricks to make it work faster
        processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
        processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
    }

    /// <summary>
    /// Per-frame video capture processor
    /// </summary>
    protected override bool ProcessTexture(Texture2D input, ref Texture2D output)
    {
        if (base.isDetected)
        {
            StatusText.text = "Detecting....................";
        }

        // detect everything we're interested in
        processor.ProcessTexture(input, TextureParameters);

        // mark detected objects
        FaceTextures = processor.MarkDetected();

        // processor.Image now holds data we'd like to visualize
        output = OpenCvSharp.Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

        return true;
    }

    protected override void ProcessImageToServer()
    {
        if (FaceTextures.Count == 0)
        {
            base.isDetected = true;
            return;
        }

        Texture2D texture = FaceTextures[0];
        byte[] imgBytes = texture.EncodeToPNG();
        //System.IO.File.WriteAllBytes(Application.dataPath + "/temp.jpg", imgBytes);
        StartCoroutine(RecognizeFace(texture));
    }


    IEnumerator RecognizeFace(Texture2D texture)
    {
        StatusText.text = "Sending to Server.......";
        Debug.Log("Sending to Server.......");

        yield return new WaitForSeconds(10);
        StatusText.text = "Done!!!!";
        Debug.Log("Done!!!!");

        DoneBtn.gameObject.SetActive(true);
        FaceInfo.gameObject.SetActive(true);
        FaceInfo.texture = texture;
    }

    public void BackToDetect()
    {
        FaceInfo.gameObject.SetActive(false);
        DoneBtn.gameObject.SetActive(false);
        base.isDetected = true;
    }
}