using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transform4D : MonoBehaviour
{
    public Vector4 position; // Position in 4D
    public Vector6 rotation; // Rotation in 4D 
    public Vector4 scale = new Vector4(1, 1, 1, 1); // Scale in 4D
    // In 4D, rotations occur on 2D planes and not 1D lines like in 3D, so we need 6 values to represent rotation on each plane 

    private void Update()
    {
        rotation %= 2 * Mathf.PI;
    }
}
