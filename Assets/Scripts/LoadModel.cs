using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadModel
{
    public static Mesh4D LoadModelFromFile(string filename)
    {
        Mesh4D mesh = new Mesh4D();

        string text = File.ReadAllText(Application.dataPath + "/Models/4D/" + filename + ".txt");

        string[] data = text.Split(',');

        int numVertices = int.Parse(data[0]);
        Vector4[] vertices = new Vector4[numVertices];
        int start = 1;
        int end = start + numVertices * 4;
        for (int i = start; i < end; i += 4)
        {
            Vector4 vertex = new Vector4();
            vertex.x = float.Parse(data[i]);
            vertex.y = float.Parse(data[i + 1]);
            vertex.z = float.Parse(data[i + 2]);
            vertex.w = float.Parse(data[i + 3]);

            vertices[(i - 1) / 4] = vertex;
        }

        int numIndices = int.Parse(data[end]);
        int[] indices = new int[numIndices];
        start = end + 1;
        end = start + numIndices;
        for (int i = start; i < end; i++)
        {
            indices[i - start] = int.Parse(data[i]);
        }

        int numIndicesL = int.Parse(data[end]);
        int[] indicesL = new int[numIndicesL];
        start = end + 1;
        end = start + numIndicesL;
        for (int i = start; i < end; i++)
        {
            indicesL[i - start] = int.Parse(data[i]);
        }

        mesh.vertices = vertices;
        mesh.indices = indices;
        mesh.indicesL = indicesL;

        return mesh;
    }
}
