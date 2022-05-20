using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains vertex data
public class Vertex
{
    public Vector4 position; // Position of the vertex
    public int[] connections; // The vertices this vertex is connected to
    public Vertex() : this(Vector4.zero) { }
    public Vertex(Vector4 position)
    {
        this.position = position;
    }

    // Returns true if the given index u is connected to this vertex
    public bool IsConnected(int u)
    {
        foreach (int i in connections)
            if (u == i)
                return true;

        return false;
    }

    // Operator that allows vector 4s to be added to the vertexs position
    public static Vertex operator +(Vertex v, Vector4 u)
    {
        v.position += u;
        return v;
    }
}
