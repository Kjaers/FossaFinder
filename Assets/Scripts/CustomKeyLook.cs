﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomKeyLook : MonoBehaviour
{
    public Transform player;
    public float mousesensitivity = 10;

    private float x = 0;
    private float y = 0;

    public bool cameraon = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //input
        if (cameraon)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)) y += mousesensitivity;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) y -= mousesensitivity;
            if (Input.GetKeyDown(KeyCode.UpArrow)) x += mousesensitivity;
            if (Input.GetKeyDown(KeyCode.DownArrow)) x -= mousesensitivity;
        }
        

        //clamping
        x = Mathf.Clamp(x, -90, 90);

        //rotation

        transform.localRotation = Quaternion.Euler(x, 0, 0);
        player.transform.localRotation = Quaternion.Euler(x, y, 0);


        //cursorlocking
        if (Input.GetKeyDown("1") && (Cursor.lockState == CursorLockMode.Locked))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraon = false;
        }
        else if (Input.GetKeyDown("1") && (Cursor.lockState == CursorLockMode.None))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            cameraon = true;
        }
    }
}