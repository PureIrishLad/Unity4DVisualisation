using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperpyramidRenderer : Renderer4D
{
    private void Awake()
    {
        mesh4D = Mesh4D.Hyperpyramid;
    }
}
