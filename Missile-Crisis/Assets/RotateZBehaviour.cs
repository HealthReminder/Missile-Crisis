using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZBehaviour : MonoBehaviour
{
    public float rotation;
    void Update()
    {
        transform.Rotate(new Vector3(0,0,rotation));
    }
}
