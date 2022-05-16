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
        if (rotation.xy > 2 * Mathf.PI)
            rotation.xy = 0;
        else if (rotation.xy < 0)
            rotation.xy = 2 * Mathf.PI;

        if (rotation.xz > 2 * Mathf.PI)
            rotation.xz = 0;
        else if (rotation.xz < 0)
            rotation.xz = 2 * Mathf.PI;

        if (rotation.xw > 2 * Mathf.PI)
            rotation.xw = 0;
        else if (rotation.xw < 0)
            rotation.xw = 2 * Mathf.PI;

        if (rotation.yz > 2 * Mathf.PI)
            rotation.yz = 0;
        else if (rotation.yz < 0)
            rotation.yz = 2 * Mathf.PI;

        if (rotation.yw > 2 * Mathf.PI)
            rotation.yw = 0;
        else if (rotation.yw < 0)
            rotation.yw = 2 * Mathf.PI;

        if (rotation.zw > 2 * Mathf.PI)
            rotation.zw = 0;
        else if (rotation.zw < 0)
            rotation.zw = 2 * Mathf.PI;

    }
}
