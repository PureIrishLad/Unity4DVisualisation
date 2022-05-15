using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypercubeRenderer : MonoBehaviour
{
    private Vector4[] vertices = new Vector4[16]
    {
        new Vector4(-1, -1, -1, -1), // 0
        new Vector4( 1, -1, -1, -1), // 1
        new Vector4( 1,  1, -1, -1), // 2
        new Vector4(-1,  1, -1, -1), // 3

        new Vector4( 1, -1,  1, -1), // 4
        new Vector4(-1, -1,  1, -1), // 5
        new Vector4(-1,  1,  1, -1), // 6
        new Vector4( 1,  1,  1, -1), // 7

        new Vector4(-1, -1, -1,  1), // 8
        new Vector4( 1, -1, -1,  1), // 9
        new Vector4( 1,  1, -1,  1), // 10
        new Vector4(-1,  1, -1,  1), // 11

        new Vector4( 1, -1,  1,  1), // 12
        new Vector4(-1, -1,  1,  1), // 13
        new Vector4(-1,  1,  1,  1), // 14
        new Vector4( 1,  1,  1,  1)  // 15
    };
    private int[] indices = new int[216]
    {
        0,  3,  1,  1,  3,  2,
        4,  7,  5,  5,  7,  6,
        5,  6,  0,  0,  6,  3,
        1,  2,  4,  4,  2,  7,
        3,  6,  2,  2,  6,  7,
        4,  5,  0,  0,  1,  4,

        8,  11, 9,  9,  11, 10,
        12, 15, 13, 13, 15, 14,
        13, 14, 8,  8,  14, 11,
        9,  10, 12, 12, 10, 15,
        11, 14, 10, 10, 14, 15,
        12, 13, 8,  8,  9,  12,

        4,  5,  12, 12, 5, 13,
        1,  9,  10, 10, 2,  1,
        5,  13, 14, 14, 6,  5,
        0,  8,  11, 11, 3,  0,
        6,  7,  15, 15, 14, 6,
        3,  2,  10, 10, 11, 3,

        4,  12, 15, 15, 7,  4,
        9,  8,  0,  0,  1,  9,
        1,  4,  12, 12, 9,  1,
        7,  2,  10, 10, 15, 7,
        5,  0,  8,  8,  13, 5,
        3,  6,  14, 14, 11, 3,

        4, 12, 13, 13, 5,   4,
        1, 2,  9,  9,  2,  10,
        5, 6,  13, 13, 6,  14,
        0, 3,  8,  8,  3,  11,
        6, 14, 7,  7,  14, 15,
        3, 11, 2,  2,  11, 10,

        4,  7, 12, 12, 7, 15,
        9,  1, 8,  8, 1,   0,
        1,  9, 4,  4, 9,  12,
        7, 15, 2,  2, 15, 10,
        5, 13, 0,  0, 13,  8,
        3, 11, 6,  6, 11, 14,
    };

    private Mesh mesh;
    public MeshFilter filter;
    private MeshRenderer renderer;

    public Player player;

    public float W;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.Clear();
        //mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        filter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] vertices = CalcVertices();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private Vector3[] CalcVertices()
    {
        Vector3[] vertices = new Vector3[this.vertices.Length];

        for (int i = 0; i < indices.Length; i += 2)
        {
            Vector4 start = this.vertices[indices[i]] + new Vector4(0, 0, 0, W);
            Vector4 end = this.vertices[indices[i + 1]] + new Vector4(0, 0, 0, W);

            float a = end.w;
            float x = start.x;
            float y = start.y;
            float z = start.z;
            float w = start.w;

            float b = a + w;

            if (a == w && a != 0)
               continue;

            if (b == 0)
            {
                x = (x + end.x) / 2;
                y = (y + end.y) / 2;
                z = (z + end.z) / 2;
            }
            else
            {
                x = x / b;
                y = y / b;
                z = z / b;
            }

            vertices[indices[i]] = new Vector3(x, y, z);
            
        }

        /*for (int i = 0; i < this.vertices.Length; i++)
        {
            Vector4 vertex = this.vertices[i];
            Vector3 thisVertex = new Vector3(vertex.x, vertex.y, vertex.z);

            vertices[i] = thisVertex;
        }*/

        return vertices;
    }
}
