using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Camera mainCamera;
    public Camera captureCamera;

    // Update is called once per frame
    void Update()
    {
        captureCamera.transform.rotation = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y + 180.0f, mainCamera.transform.rotation.eulerAngles.z);
    }
}
