using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public int[] indices = new int[3];

    public Triangle()
    {
        indices = new int[3];
    }

    public Triangle(int one, int two, int three)
    {
        indices = new int[] { one, two, three };
    }

    public int this[int index]
    {
        get { return indices[index]; }
        set { indices[index] = value; }
    }

    public override bool Equals(object obj)
    {
        Triangle other = (Triangle)obj;

        if (indices[0] == other[0] && indices[1] == other[1] && indices[2] == other[2])
            return true;
        return false;
    }
}
