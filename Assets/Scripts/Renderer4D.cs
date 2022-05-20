using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Renderer for 4-Dimensional Objects
public class Renderer4D : MonoBehaviour
{
    [System.Serializable]
    public enum RenderType
    {
        Projection,
        CrossSection
    }

    public RenderType renderType; // How to render the object

    [HideInInspector]
    public Mesh4D mesh4D; // This objects mesh

    [HideInInspector]
    public float referenceW; // Origin w, normally the players w position

    private MeshFilter filter; // Mesh filter

    private MeshFilter filterO; // Outline filter

    private Transform4D transform4D; // The 4d transform component

    public string modelFilename;

    [HideInInspector]
    public Player4D player;

    private void Start()
    {
        // Loading the model from a file
        if (modelFilename.Length > 0)
            mesh4D = LoadModel.LoadModelFromFile(modelFilename);

        Mesh mesh = new Mesh();
        mesh.Clear();
        Mesh meshO = new Mesh();
        meshO.Clear();

        foreach (Transform child in transform)
        {
            if (child.name == "Volume")
            {
                filter = child.GetComponent<MeshFilter>();
                filter.mesh = mesh;
            }
            else if (child.name == "Outline")
            {
                filterO = child.GetComponent<MeshFilter>();
                filterO.mesh = meshO;
            }
        }

        transform4D = GetComponent<Transform4D>();
    }

    private void LateUpdate()
    {
        if (transform4D.moved || player.transform4D.moved)
        {
            UpdateMesh();
            transform4D.moved = false;
        }
    }

    // Updates the mesh
    private void UpdateMesh()
    {
        // Getting this objects offset
        Vector4 offset = transform4D.position;
        referenceW = player.transform4D.position.w;

        Vertex[] vertices = new Vertex[mesh4D.vertices.Length];
        Line[] lines = mesh4D.lines;

        // Getting the rotation matrix of this object
        Matrix4x4 R = Transform4D.RotationMatrix(transform4D.rotation);
        for (int i = 0; i < vertices.Length; i++)
        {
            // We make a copy of the meshes vertex data so that we don't overwrite it
            vertices[i] = new Vertex();
            vertices[i].position = mesh4D.vertices[i].position;
            vertices[i].connections = new int[mesh4D.vertices[i].connections.Length];
            mesh4D.vertices[i].connections.CopyTo(vertices[i].connections, 0);

            // Applying rotation
            vertices[i].position = Transform4D.Rotate(vertices[i].position, R);
        }

        Vector3[] verts = new Vector3[0];
        int[] indices = new int[0];
        int[] triangles = new int[0];

        switch (renderType)
        {
            case RenderType.Projection:
                verts = ProjectTo3D(vertices);
                indices = Mesh4D.LinesToArray(lines);
                triangles = Mesh4D.TrianglesToArray(Mesh4D.GenerateTriangles(lines, verts));
                break;
            case RenderType.CrossSection:
                Tuple<Vertex[], Line[]> t = CrossSection3D(vertices, lines);
                verts = Mesh4D.VerticesToArray(t.Item1);
                indices = Mesh4D.LinesToArray(t.Item2);
                triangles = Mesh4D.TrianglesToArray(Mesh4D.GenerateTriangles(t.Item2, verts));
                break;
        }

        filterO.sharedMesh.Clear();
        filterO.sharedMesh.SetVertices(verts);
        filterO.sharedMesh.SetIndices(indices, MeshTopology.Lines, 0);

        filter.sharedMesh.Clear();
        filter.sharedMesh.SetVertices(verts);
        filter.sharedMesh.SetIndices(triangles, MeshTopology.Triangles, 0);
    }

    // Generates a 3D cross sectional view of a 4D object 
    private Tuple<Vertex[], Line[]> CrossSection3D(Vertex[] vertices, Line[] lines)
    {
        // The vertices to be rendered
        List<Vertex> verts = new List<Vertex>();
        // The lines to be drawn between the vertices
        List<Line> l = new List<Line>();

        // Looping for each edge in the original objects mesh
        for (int i = 0; i < lines.Length; i++)
        {
            int uIndex = lines[i][0];
            int vIndex = lines[i][1];

            // Getting references to the start and end point vertices of this line
            Vertex u = vertices[uIndex];
            Vertex v = vertices[vIndex];

            // Determining if and where the two points intersect with the reference w
            if (Intersects(u.position, v.position, referenceW, out Tuple<Vector3, Vector3> I))
            {
                // When creating cross sections, sometimes a vertex may seemingly split into multiple other vertices
                // We need to, for each new vertex we create, give it the original vertices it was connected to so 
                // that it can properly create lines between them
                Vertex wu = new Vertex(I.Item1);
                Vertex wv = new Vertex(I.Item2);
                wu.connections = u.connections;
                wv.connections = v.connections;
                verts.Add(wu);
                verts.Add(wv);

                // Adding a new line between these vertices
                Line line = new Line(verts.Count - 2, verts.Count - 1);
                if (!l.Contains(line))
                    l.Add(line);

                // We loop through the list of vertices that we know intersect so that we may see if this vertex that intersects
                // with the reference w is connected to any other vertices that also intersect
                for (int j = 0; j < verts.Count; j++)
                {
                    if (verts[j].IsConnected(uIndex))
                    {
                        line = new Line(verts.Count - 2, j);
                        if (!l.Contains(line))
                            l.Add(line);
                    }
                    else if (verts[j].IsConnected(vIndex))
                    {
                        line = new Line(verts.Count - 1, j);
                        if (!l.Contains(line))
                            l.Add(line);
                    }
                }
            }

        }

        return new Tuple<Vertex[], Line[]>(verts.ToArray(), l.ToArray());
    }

    // Projects the 3D shadow of the 4D object onto the 3D world
    private Vector3[] ProjectTo3D(Vertex[] vertices)
    {
        Vector3[] verts = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            verts[i] = vertices[i].position;

        return verts;
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

    // Scales the object
    private Vector4[] Scale(Vector4[] vertices, Vector4 scale)
    {
        Vector4[] output = new Vector4[vertices.Length];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = new Vector4(vertices[i].x * scale.x, vertices[i].y * scale.y, vertices[i].z * scale.z, vertices[i].w * scale.w);
        }

        return output;
    }

    // Returns true if the line between u and v intersects with the given w value, also outputs the intersect point if there is an intersection
    private bool Intersects(Vector4 u, Vector4 v, float w, out Tuple<Vector3, Vector3> I)
    {
        // We know this line intersects if one point is on or behind the axis and the other is in front or on the axis
        bool intersects = u.w >= w && v.w <= w || v.w >= w && u.w <= w;

        if (!intersects)
        {
            I = new Tuple<Vector3, Vector3>(u, v);
            return false;
        }
        else
        {
            Vector4 d = v - u;

            float t = (w - u.w) / d.w;

            Vector4 wu = u + t * d;
            Vector4 wv = v - (1 - t) * d;

            I = new Tuple<Vector3, Vector3>(wu, wv);

            return true;
        }

    }
}
