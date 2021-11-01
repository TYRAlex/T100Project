using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFScreenDetection : MonoBehaviour
{

    Transform tran;
    public Camera camera;
    public bool isLine = false;
    private float height = 0;
    public float Height
    {
        get => height;
        set
        {
            height = value;
        }
    }

    void Start()
    {
        tran = this.transform;
        if (!camera)
        {
            camera = tran.GetComponent<Camera>();
        }
    }
    private void Update()
    {
        if (Height != Screen.height)
        {
            Height = Screen.height;
            if (!isLine)
            {
                camera.orthographicSize = Height / 200;
            }
            else
            {
                camera.orthographicSize = Height / 2;
            }
        }
    }
}
