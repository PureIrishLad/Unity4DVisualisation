using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HypercubeRenderer : MonoBehaviour
{
    public bool cool4D = false;

    // Hypercube vertices
    private Vector4[] vertices = new Vector4[16]
    {
        new Vector4( 4, -1, -1, -1), // 0
        new Vector4( 6, -1, -1, -1), // 1
        new Vector4( 6,  1, -1, -1), // 2
        new Vector4( 4,  1, -1, -1), // 3

        new Vector4( 6, -1,  1, -1), // 4
        new Vector4( 4, -1,  1, -1), // 5
        new Vector4( 4,  1,  1, -1), // 6
        new Vector4( 6,  1,  1, -1), // 7

        new Vector4(-2, -2, -2,  3), // 8
        new Vector4( 2, -2, -2,  3), // 9
        new Vector4( 2,  2, -2,  3), // 10
        new Vector4(-2,  2, -2,  3), // 11

        new Vector4( 2, -2,  2,  3), // 12
        new Vector4(-2, -2,  2,  3), // 13
        new Vector4(-2,  2,  2,  3), // 14
        new Vector4( 2,  2,  2,  3)  // 15
    };

    // Indicies used for drawing triangles
    private int[] indices = new int[288]
    {
        0,  3,  1,  1,  3,  2,
        4,  7,  5,  5,  7,  6,
        5,  6,  0,  0,  6,  3,
        1,  2,  4,  4,  2,  7,
        3,  6,  2,  2,  6,  7,
        0,  1,  5,  5,  1,  4,

        8,  11, 9,  9,  11, 10,
        12, 15, 13, 13, 15, 14,
        13, 14, 8,  8,  14, 11,
        9,  10, 12, 12, 10, 15,
        11, 14, 10, 10, 14, 15,
        8,  9,  13, 13,  9, 12,

        2,  3,  1,  1,  3,  0,
        6,  7,  5,  5,  7,  4,
        3,  6,  0,  0,  6,  5,
        7,  2,  4,  4,  2,  1,
        7,  6,  2,  2,  6,  3,
        4,  1,  5,  5,  1,  0,

        10, 11, 9,  9,  11, 8,
        14, 15, 13, 13, 15, 12,
        11, 14, 8,  8,  14, 13,
        15, 10, 12, 12, 10, 9,
        15, 14, 10, 10, 14, 11,
        12, 9,  13, 13,  9, 8,

        // Front
        2,  10, 1,  1,  10, 9,
        8,  11, 0,  0,  11, 3,
        2,  3,  10, 10, 3,  11,
        8,  0,  9,  9,  0,  1,

        // Back
        4,  5,  12, 12, 5,  13,
        6,  14, 5,  5,  14, 13,
        15, 14, 7,  7,  14, 6,
        15, 7,  12, 12, 7,  4,

        // Left
        8,  13, 0,  0,  13, 5,
        6,  14, 3,  3,  14, 11,
        13, 14, 5,  5,  14, 6,
        3,  11, 0,  0,  11, 8,

        // Right
        1,  4,  9,  9,  4,  12,
        15, 7,  10, 10, 7,  2,
        9,  10, 1,  1,  10, 2,
        4,  7,  12, 12, 7,  15,

        // Bottom
        13, 5,  12, 12, 5,  4,
        5,  13, 0,  0,  13, 8,
        12, 4,  9,  9,  4,  1,
        1,  0,  9,  9,  0,  8,

        // Top
        6,  14, 7,  7,  14, 15,
        11, 14, 3,  3,  14, 6,
        2,  7,  10, 10, 7,  15,
        11, 3,  10, 10, 3,  2
    };
    // Indices for ysed for lines, lines are used to determine where a 4d vertex gets drawn in 3d
    private int[] indicesL = new int[]
    {
        0, 1,
        1, 2,
        2, 3,
        3, 0,

        4, 5,
        5, 6,
        6, 7,
        7, 4,

        5, 0,
        6, 3,
        4, 1,
        7, 2,

        8, 9,
        9, 10,
        10, 11,
        11, 8,

        12, 13,
        13, 14,
        14, 15,
        15, 12,

        13, 8,
        14, 11,
        12, 9,
        15, 10,

        0, 8,
        1, 9,
        2, 10,
        3, 11,

        4, 12,
        5, 13,
        6, 14,
        7, 15,
    };

    private Mesh mesh;
    private MeshFilter filter;

    private Player player;

    public Vector4 position = new Vector4(0, 0, 0, 0);
    public Vector4 rotation = new Vector4(0, 0, 0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.Clear();

        filter = GetComponent<MeshFilter>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        filter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // Translating the vertices
        Vector4[] verts4D = Translate(vertices, position);

        // Calculating the new positions for the vertices based on their intercept with w = 0
        Vector3[] verts3D = ProjectTo3D(verts4D);

        if (verts3D.Length > 0)
        {
            mesh.SetVertices(verts3D);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
        }
        // If there are no vertices it means there were no intersections so we clear the mesh
        else
        {
            mesh.Clear();
        }
    }

    // Projecects the given 4-Dimensional vertex positions to the 3-Dimensional world
    private Vector3[] ProjectTo3D(Vector4[] vertices)
    {
        Vector3[] newVertices = new Vector3[vertices.Length];
        int intersections = 0;
        float playerW = player.w;

        // We want to loop for each edge in the hypercube, so we use a array of lines
        for (int i = 0; i < indicesL.Length; i += 2)
        {
            Vector4 start = vertices[indicesL[i]];
            Vector4 end = vertices[indicesL[i + 1]];

            float x = start.x;
            float y = start.y;
            float z = start.z;
            float w = start.w;

            float x2 = end.x;
            float y2 = end.y;
            float z2 = end.z;

            // If this line intersects with the players w position
            if (Intersects(start, end, player.w))
            {
                intersections++;

                // Calculating the vector of the line
                Vector4 v = end - start;

                // Finding the intercept point t on the line with the 3d world
                float t = (playerW - w) / v.w;
                float t2 = (1 - t);

                if (cool4D)
                    t2 = t;

                // Calculating the new xyz coords based on t
                x = x + v.x * t;
                y = y + v.y * t;
                z = z + v.z * t;

                x2 = x2 - v.x * t2;
                y2 = y2 - v.y * t2;
                z2 = z2 - v.z * t2;
            }

            newVertices[indicesL[i]] = new Vector3(x, y, z);
            newVertices[indicesL[i + 1]] = new Vector3(x2, y2, z2);
        }

        // If there are no intersecting points we clear the vertices
        if (intersections == 0)
            newVertices = new Vector3[0];

        return newVertices;
    }

    // Translates the objects vertices by a given position
    private Vector4[] Translate(Vector4[] vertices, Vector4 translation)
    {
        Vector4[] output = new Vector4[vertices.Length];
        vertices.CopyTo(output, 0);

        for (int i = 0; i < output.Length; i++)
        {
            output[i] += translation;
        }

        return output;
    }

    // Returns true if the line given by start and end intersects at the point w on the w-axis
    private bool Intersects(Vector4 start, Vector4 end, float w)
    {
        return start.w >= w && end.w <= w || start.w <= w && end.w >= w;
    }
}
