using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HypercubeRenderer : Renderer4D
{
    private void Awake()
    {
        mesh4D = Mesh4D.Hypercube;
    }
}
