using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transform4D : MonoBehaviour
{
    public Vector4 worldPosition;

    public Vector4 position; // Position in 4D
    public Vector6 rotation; // Rotation in 4D 
    public Vector4 scale = new Vector4(1, 1, 1, 1); // Scale in 4D
    // In 4D, rotations occur on 2D planes and not 1D lines like in 3D, so we need 6 values to represent rotation on each plane 

    [HideInInspector]
    public bool moved;
    private Vector4 prevPos;
    private Vector6 prevRot;

    public Vector4 forward
    {
        get
        {
            Vector4 f = new Vector4(0, 0, 1, 0);

            Vector6 rot = new Vector6(rotation.xy, -rotation.yz, rotation.xw, 0, rotation.yw, rotation.zw);

            Matrix4x4 R = RotationMatrix(rot);

            f = Rotate(f, R);

            return f;
        }
    }

    public Vector4 right
    {
        get
        {
            Vector4 f = new Vector4(1, 0, 0, 0);

            Vector6 rot = new Vector6(rotation.xy, -rotation.yz, rotation.xw, 0, rotation.yw, rotation.zw);
            
            Matrix4x4 R = RotationMatrix(rot);

            f = Rotate(f, R);

            return f;
        }
    }

    public Vector4 up
    {
        get
        {
            Vector4 f = new Vector4(0, 1, 0, 0);

            Vector6 rot = new Vector6(rotation.xy, -rotation.yz, rotation.xw, 0, rotation.yw, rotation.zw);
            
            Matrix4x4 R = RotationMatrix(rot);

            f = Rotate(f, R);

            return f;
        }
    }

    public Vector4 wDir
    {
        get
        {
            Vector4 f = new Vector4(0, 0, 0, 1);

            Vector6 rot = new Vector6(rotation.xy, -rotation.yz, rotation.xw, 0, rotation.yw, rotation.zw);
            
            Matrix4x4 R = RotationMatrix(rot);

            f = Rotate(f, R);

            return f;
        }
    }

    private void Awake()
    {
        moved = true;
        //rotation = new Vector6();
        prevPos = position;
        prevRot = rotation;
    }

    private void Update()
    {
        if (prevPos != position || !prevRot.Equals(rotation))
            moved = true;
        prevRot = rotation;
        prevPos = position;
    }

    private void LateUpdate()
    {
        rotation %= 2 * Mathf.PI;
    }

    // Rotations performed in 4D are done across 2D planes and not 1D points like they are in a 3D space

    public static Vector4 Rotate(Vector4 point, Matrix4x4 R)
    {
        point = MM(R, point);

        return point;
    }

    public Vector4 Forward()
    {
        Matrix4x4 A = RotationMatrix(rotation);

        float a11 = A[0, 0];
        float a21 = A[1, 0];
        float a31 = A[2, 0];
        float a41 = A[3, 0];
        float a12 = A[0, 1];
        float a22 = A[1, 1];
        float a32 = A[2, 1];
        float a42 = A[3, 1];
        float a13 = A[0, 2];
        float a23 = A[1, 2];
        float a33 = A[2, 2];
        float a43 = A[3, 2];

        float a14 = A[0, 3];
        float a24 = A[1, 3];
        float a34 = A[2, 3];
        float a44 = A[3, 3];

        // Calculating the angle axis
        float theta = Mathf.Abs(Mathf.Acos((a11 + a22 + a33 + a44 - 1) / 3.0f));
        float st = 2 * Mathf.Sin(theta);
        float e1 = (a32 - a23) / st;
        float e2 = (a13 - a31) / st;
        float e3 = (a21 - a12) / st;
        float e4 = (a24 - a42) / st;
        float e5 = (a43 - a34) / st;
        float e6 = (a41 - a14) / st;

        Vector6 a = new Vector6(e1, e2, e3, e4, e5, e6);
        //Debug.Log(a.ToString());
        
        if (st < 0)
        {
            e1 = 0;
            e2 = 0;
            e3 = 0;
            e4 = 0;
            e5 = 0;
            e6 = 0;
        }

        return new Vector4(a11, a22, a33, a44);
    }

    public static Matrix4x4 RotationMatrix(Vector6 rot)
    {
        Matrix4x4 R = new Matrix4x4();

        float xy = rot.xy;
        float xz = rot.xz;
        float xw = rot.xw;
        float yz = rot.yz;
        float yw = rot.yw;
        float zw = rot.zw;

        float cxy = Mathf.Cos(xy);
        float sxy = Mathf.Sin(xy);
        float cxz = Mathf.Cos(xz);
        float sxz = Mathf.Sin(xz);
        float cxw = Mathf.Cos(xw);
        float sxw = Mathf.Sin(xw);
        float cyz = Mathf.Cos(yz);
        float syz = Mathf.Sin(yz);
        float cyw = Mathf.Cos(yw);
        float syw = Mathf.Sin(yw);
        float czw = Mathf.Cos(zw);
        float szw = Mathf.Sin(zw);

        R[0, 0] = cxy * cxz * cxw;
        R[0, 1] = cyw * (-sxy * cyz - cxy * sxz * syz) - cxy * cxz * sxw * syw;
        R[0, 2] = czw * (sxy * syz - cxy * sxz * cyz) + szw * (-cxy * cxz * sxw * cyw - syw * (-sxy * cyz - cxy * sxz * syz));
        R[0, 3] = czw * (-cxy * cxz * sxw * cyw - syw * (-sxy * cyz - cxy * sxz * syz)) - szw * (sxy * syz - cxy * sxz * cyz);
        R[1, 0] = sxy * cxz * cxw;
        R[1, 1] = cyw * (cxy * cyz - sxy * sxz * syz) - sxy * cxz * sxw * syw;
        R[1, 2] = czw * (-cxy * syz - sxy * sxz * cyz) + szw * (-sxy * cxz * sxw * cyw - syw * (cxy * cyz - sxy * sxz * syz));
        R[1, 3] = czw * (-sxy * cxz * sxw * cyw - syw * (-cxy * cyz - sxy * sxz * syz)) - szw * (-cxy * syz - sxy * sxz * cyz);
        R[2, 0] = sxz * cxw;
        R[2, 1] = cxz * syz * cyw - sxz * sxw * syw;
        R[2, 2] = cxz * cyz * czw + szw * (-cxz * syz * syw - sxz * sxw * cyw);
        R[2, 3] = czw * (-cxz * syz * syw - sxz * sxw * cyw) - cxz * cyz * szw;
        R[3, 0] = sxw;
        R[3, 1] = cxw * syw;
        R[3, 2] = cxw * cyw * szw;
        R[3, 3] = cxw * cyw * czw;

        return R;
    }

    // Matrix multiplication between a 4x4 and 1x4 matrix
    public static Vector4 MM(Matrix4x4 A, Vector4 B)
    {
        Vector4 output = new Vector4();

        for (int r = 0; r < 4; r++)
            for (int i = 0; i < 4; i++)
                output[r] += A[r, i] * B[i];

        return output;
    }
}
