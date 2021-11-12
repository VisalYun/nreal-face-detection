﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracking : MonoBehaviour
{
    public Camera viewCamera;

    public HeadTracker tracker;
    bool IsMoving = false;

    void Update()
    {
        //Rotate the card base on the rotation of headset
        float cRotY = viewCamera.transform.eulerAngles.y;
        float cRotX = viewCamera.transform.eulerAngles.x;
        transform.rotation = Quaternion.Euler(cRotX, cRotY, 0);

        transform.position = Vector3.MoveTowards(transform.position, tracker.resultingPosition, 3 * Time.deltaTime);
    }
}
