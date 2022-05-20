using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains data for a line in a mesh
public class Line
{
    public int[] indices = new int[2];

    public Line()
    {
        indices = new int[2];
    }

    public Line(int one, int two)
    {
        indices = new int[] { one, two };
    }

    public int this[int index]
    {
        get { return indices[index]; }
        set { indices[index] = value; }
    }


    // Returns the index at the other end of this line given an index
    public int GetOtherEnd(int u)
    {
        if (u == indices[0])
            return indices[1];
        else if (u == indices[1])
            return indices[0];

        return -1;
    }

    public override bool Equals(object obj)
    {
        Line other = (Line)obj;
        if (indices[0] == other[0] && indices[1] == other[1] || indices[0] == other[1] && indices[1] == other[0])
            return true;

        return false;
    }
}
