using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screenshot : MonoBehaviour
{
    public GameObject target;
    public Camera camera;
    public int resWidth = 1920; 
    public int resHeight = 1080;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(takeScreenShot());
        }
    }

    IEnumerator takeScreenShot()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(target.transform.position);
        // int resWidth = Screen.width - (int)screenPos.x; 
        // int resHeight = Screen.height - (int)screenPos.y;
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        //camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        //Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/temp.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
}
