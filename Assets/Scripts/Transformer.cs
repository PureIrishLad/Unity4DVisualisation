using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour
{
    public Vector4 translation;
    public Vector6 rotation;
    public Vector4 scaling;

    public Transform4D transform4D;

    private void Update()
    {
        if (transform4D)
        {
            transform4D.position += translation * Time.deltaTime;
            transform4D.rotation += rotation * Time.deltaTime;
            transform4D.scale += scaling * Time.deltaTime;
        }
    }
}
