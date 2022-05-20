using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mesh4D
{
    [HideInInspector]
    public Vertex[] vertices;
    [HideInInspector]
    public Triangle[] triangles;
    [HideInInspector]
    public Line[] lines;

    public Mesh4D()
    {
        
    }

    // Converts the triangle data type to an array of ints
    public static int[] TrianglesToArray(Triangle[] triangles)
    {
        int[] indices = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++)
        {
            indices[i * 3] = triangles[i][0];
            indices[i * 3 + 1] = triangles[i][1];
            indices[i * 3 + 2] = triangles[i][2];
        }

        return indices;
    }
    // Converts the line data type to an array of ints
    public static int[] LinesToArray(Line[] lines)
    {
        int[] indices = new int[lines.Length * 2];

        for (int i = 0; i < lines.Length; i++)
        {
            indices[i * 2] = lines[i][0];
            indices[i * 2 + 1] = lines[i][1];
        }

        return indices;
    }
    // Converts the vertex data type to an array of vector3s
    public static Vector3[] VerticesToArray(Vertex[] vertices)
    {
        Vector3[] verts = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            verts[i] = vertices[i].position;

        return verts;
    }

    // Takes in a triangle mesh and then generates an array of lines
    public static Line[] GenerateLines(Triangle[] triangles)
    {
        //Line[] lines = new Line[triangles.Length * 2];
        List<Line> lines = new List<Line>();


        for (int i = 0; i < triangles.Length; i++)
        {
            Line l1 = new Line();
            Line l2 = new Line();

            l1[0] = triangles[i][0];
            l2[0] = triangles[i][0];
            l1[1] = triangles[i][1];
            l2[1] = triangles[i][2];

            if (!lines.Contains(l1))
                lines.Add(l1);
            if (!lines.Contains(l2))
                lines.Add(l2);

            /*lines[i * 2] = new Line();
            lines[i * 2 + 1] = new Line();

            lines[i * 2][0] = triangles[i][0];
            lines[i * 2 + 1][0] = triangles[i][0];

            lines[i * 2][1] = triangles[i][1];
            lines[i * 2 + 1][1] = triangles[i][2];*/
        }

        return lines.ToArray();
    }
    // The opposite of the above, takes in an array of lines and generates a triangle mesh
    public static Triangle[] GenerateTriangles(Line[] lines, Vector3[] vertices)
    {
        List<Triangle> triangles = new List<Triangle>();

        // Looping for each line
        for (int i = 0; i < lines.Length; i++)
        {
            Line l1 = lines[i];

            // Looking at the other lines in the mesh
            for (int j = i + 2; j < lines.Length; j++)
            {
                Line l2 = lines[j];

                int intersectIndex = IntersectIndex(l1, l2);

                // If these two lines intersect, we draw a triangle from them
                if (intersectIndex != -1)
                {
                    Triangle t1 = new Triangle(intersectIndex, l1.GetOtherEnd(intersectIndex), l2.GetOtherEnd(intersectIndex));
                    Triangle t2 = new Triangle(l2.GetOtherEnd(intersectIndex), l1.GetOtherEnd(intersectIndex), intersectIndex);
                    if (!ContainsTriangle(triangles, t1, vertices))
                        triangles.Add(t1);
                    if (!ContainsTriangle(triangles, t2, vertices))
                        triangles.Add(t2);
                }
            }
        }

        return triangles.ToArray();
    }
    
    // Gets the connections of an index u from an array of lines
    public static int[] GetConnections(int u, Line[] lines)
    {
        List<int> connections = new List<int>();

        foreach (Line line in lines)
        {
            int connection = line.GetOtherEnd(u);
            if (connection != -1)
                connections.Add(connection);
        }

        return connections.ToArray();
    }

    // Returns the index between two lines where they meet
    private static int IntersectIndex(Line l1, Line l2)
    {
        if (l1[0] == l2[0])
            return l1[0];
        else if (l1[0] == l2[1])
            return l1[0];
        else if (l1[1] == l2[0])
            return l1[1];
        else if (l1[1] == l2[1])
            return l1[1];

        return -1;
    }

    private static bool ContainsTriangle(List<Triangle> triangles, Triangle triangle, Vector3[] vertices)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle other = triangles[i];

            if (vertices[triangle[0]] == vertices[other[0]] && vertices[triangle[1]] == vertices[other[1]] && vertices[triangle[2]] == vertices[other[2]])
            {
                return true;
            }
        }

        return false;
    }
}
