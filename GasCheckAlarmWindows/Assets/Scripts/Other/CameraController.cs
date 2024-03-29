﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 dirVector3;
    private Vector3 rotaVector3;

    public float translateSpeed = 1f;
    public float rotateXSpeed = -10f;
    public float rotateYSpeed = 10f;

    Camera mainCamera = null;
    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        rotaVector3 = mainCamera.transform.localEulerAngles;
    }

    void FixedUpdate()
    {
        if (mainCamera)
        {
            //旋转
            if (Input.GetMouseButtonDown(1))
            {
                rotaVector3 = mainCamera.transform.localEulerAngles;
            }
            else if (Input.GetMouseButton(1))
            {
                rotaVector3.y += Input.GetAxis("Mouse X") * rotateYSpeed;
                rotaVector3.x += Input.GetAxis("Mouse Y") * rotateXSpeed;
                mainCamera.transform.rotation = Quaternion.Euler(rotaVector3);
            }
            //移动
            dirVector3 = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.z = 3;
                else dirVector3.z = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.z = -3;
                else dirVector3.z = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.x = -3;
                else dirVector3.x = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.x = 3;
                else dirVector3.x = 1;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.y = -3;
                else dirVector3.y = -1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                if (Input.GetKey(KeyCode.LeftShift)) dirVector3.y = 3;
                else dirVector3.y = 1;
            }
            mainCamera.transform.Translate(dirVector3 * translateSpeed, Space.Self);
        }
    }
}
