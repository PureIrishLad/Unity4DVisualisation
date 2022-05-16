using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer4D : MonoBehaviour
{
    public bool cool4D = false;
    public Vector4 position = new Vector4(0, 0, 0, 0);
    public Vector4 rotation = new Vector4(0, 0, 0, 0);
    public Mesh4D mesh4D; // This renderers mesh

    private Mesh mesh;
    private MeshFilter filter;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.Clear();

        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Translating the vertices
        Vector4[] verts4D = Translate(mesh4D.vertices, position);

        // Calculating the new positions for the vertices based on their intercept with w = 0
        Vector3[] verts3D = ProjectTo3D(verts4D, mesh4D.indicesL);

        if (verts3D.Length > 0)
        {
            mesh.SetVertices(verts3D);
            mesh.SetIndices(mesh4D.indices, MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
        }
        // If there are no vertices it means there were no intersections so we clear the mesh
        else
        {
            mesh.Clear();
        }
    }

    // Projecects the given 4-Dimensional vertex positions to the 3-Dimensional world
    private Vector3[] ProjectTo3D(Vector4[] vertices, int[] lineIndices)
    {
        Vector3[] newVertices = new Vector3[vertices.Length];
        int intersections = 0;
        float playerW = player.w;

        // We want to loop for each edge in the hypercube, so we use a array of lines
        for (int i = 0; i < lineIndices.Length; i += 2)
        {
            Vector4 start = vertices[lineIndices[i]];
            Vector4 end = vertices[lineIndices[i + 1]];

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

            newVertices[lineIndices[i]] = new Vector3(x, y, z);
            newVertices[lineIndices[i + 1]] = new Vector3(x2, y2, z2);
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
