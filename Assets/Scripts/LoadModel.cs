using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Loads a 4-D model from a file, models must be in an exact order, down to the order in which triangles are drawn
public class LoadModel
{
    public static Mesh4D LoadModelFromFile(string filename)
    {
        Mesh4D mesh = new Mesh4D();

        string text = File.ReadAllText(Application.dataPath + "/Models/4D/" + filename + ".txt");

        string[] data = text.Split(',');

        // Extracting vertices from data
        int numVertices = int.Parse(data[0]);
        Vertex[] vertices = new Vertex[numVertices];
        int start = 1;
        int end = start + numVertices * 4;
        for (int i = start; i < end; i += 4)
        {
            Vertex vertex = new Vertex();
            vertex.position.x = float.Parse(data[i]);
            vertex.position.y = float.Parse(data[i + 1]);
            vertex.position.z = float.Parse(data[i + 2]);
            vertex.position.w = float.Parse(data[i + 3]);

            vertices[(i - 1) / 4] = vertex;
        }

        // Extracting triangles from data
        int numTriangles = int.Parse(data[end]);
        Triangle[] triangles = new Triangle[numTriangles];
        start = end + 1;
        end = start + numTriangles * 3;
        for (int i = start; i < end; i += 3)
        {
            Triangle triangle = new Triangle();
            triangle[0] = int.Parse(data[i]);
            triangle[1] = int.Parse(data[i + 1]);
            triangle[2] = int.Parse(data[i + 2]);
            triangles[(i - start) / 3] = triangle;
        }

        // Setting mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Generating lines from triangles, this is necessary for generating the cross section
        mesh.lines = Mesh4D.GenerateLines(triangles);

        // Also necessary is telling each vertex which vertices it is connected to
        for (int i = 0; i < mesh.vertices.Length; i++)
            mesh.vertices[i].connections = Mesh4D.GetConnections(i, mesh.lines);

        return mesh;
    }
}
