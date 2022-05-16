using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector6
{
    public float xy;
    public float xz;
    public float xw;
    public float yz;
    public float yw;
    public float zw;

    public Vector6() : this(0, 0, 0, 0, 0, 0) { }

    public Vector6(float xy, float xz, float xw, float yz, float yw, float zw)
    {
        this.xy = xy;
        this.xz = xz;
        this.xw = xw;
        this.yz = yz;
        this.yw = yw;
        this.zw = zw;
    }

    public float magnitude => Mathf.Sqrt(xy * xy + xz * xz + xw * xw + yz * yz + yw * yw + zw * zw);
    public Vector6 normalize => this / magnitude;

    public static Vector6 operator +(Vector6 a, Vector6 b)
    {
        return new Vector6(a.xy + b.xy, a.xz + b.xz, a.xw + b.xw, a.yz + b.yz, a.yw + b.yw, a.zw + b.zw);
    }
    public static Vector6 operator -(Vector6 a, Vector6 b)
    {
        return new Vector6(a.xy - b.xy, a.xz - b.xz, a.xw - b.xw, a.yz - b.yz, a.yw - b.yw, a.zw - b.zw);
    }

    public static Vector6 operator *(Vector6 a, float s)
    {
        return new Vector6(a.xy * s, a.xz * s, a.xw * s, a.yz * s, a.yw * s, a.zw * s);
    }

    public static Vector6 operator /(Vector6 a, float s)
    {
        return new Vector6(a.xy / s, a.xz / s, a.xw / s, a.yz / s, a.yw / s, a.zw / s);
    }

    public static Vector6 operator -(Vector6 a)
    {
        return new Vector6(-a.xy, -a.xz, -a.xw, -a.yz, -a.yz, -a.zw);
    }

    public static Vector6 operator %(Vector6 a, float m)
    {
        a += new Vector6(1, 1, 1, 1, 1, 1) * m;
        return new Vector6(a.xy % m, a.xz % m, a.xw % m, a.yz % m, a.yw % m, a.zw % m);
    }
}
