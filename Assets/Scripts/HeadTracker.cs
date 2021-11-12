using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracker : MonoBehaviour
{
    public float maxCursorDistance;
    public Camera viewCamera;

    public Vector3 resultingPosition;

    // Update is called once per frame
    void Update()
    {
        //Find the direction where your head is headed
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        //Set the position repective to the center of front camera
        resultingPosition = ray.origin + new Vector3(ray.direction.x, ray.direction.y, ray.direction.z) * maxCursorDistance;
    }
}
