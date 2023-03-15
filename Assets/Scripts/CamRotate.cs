using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{

    public float speed;
    Transform camPoint;

    void Start()
    {
        camPoint = transform;

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        camPoint.Rotate(new Vector3(0, Time.deltaTime * speed, 0), Space.World);
    }
}
