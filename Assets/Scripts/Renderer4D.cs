using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Renderer4D : MonoBehaviour
{
    [System.Serializable]
    public enum ProjectType
    {
        Intersect, 
        Flatten,
    }

    [System.Serializable]
    public enum RenderType
    {
        Volume,
        Outline,
        VolumeOutline
    }

    public ProjectType projectType; // How to project the points from 4D to 3D
    public RenderType renderType; // How to render the object

    public bool cool4D = false;
    [HideInInspector]
    public Mesh4D mesh4D; // This objects mesh

    public Material material; // material
    public Material outlineMaterial; // outline material

    [HideInInspector]
    public float referenceW; // Origin w, normally the players w position

    private MeshFilter filter; // Mesh filter
    private MeshRenderer renderer; // Mesh renderer

    private MeshFilter filterO; // Outline filter
    private MeshRenderer rendererO; // Outline renderer

    private Transform4D transform4D; // The 4d transform component

    private MeshCollider collider; // The objects collider


    public string modelFilename;

    public bool regen;

    [HideInInspector]
    public Player4D player;

    private void Start()
    {
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
                renderer = child.GetComponent<MeshRenderer>();
                filter = child.GetComponent<MeshFilter>();
                filter.mesh = mesh;
            }
            else if (child.name == "Outline")
            {
                rendererO = child.GetComponent<MeshRenderer>();
                filterO = child.GetComponent<MeshFilter>();
                filterO.mesh = meshO;
            }
        }

        collider = GetComponent<MeshCollider>();
        transform4D = GetComponent<Transform4D>();
    }

    private void LateUpdate()
    {
        if (regen)
        {
            Start();
            regen = false;
        }

        if (filter && transform4D)
            UpdateMesh();
    }

    // Updates the mesh each frame
    private void UpdateMesh() 
    {

        Vector4 viewOffset = new Vector4();
        Vector6 viewRot = new Vector6();

        if (player)
        {
            viewOffset = player.transform4D.position;
            viewRot = player.transform4D.rotation;
        }

        referenceW = viewOffset.w;
        renderer.material = material;

        Vector4 position = transform4D.position - viewOffset;
        Vector6 rotation = transform4D.rotation;
        Vector4 scale = transform4D.scale;

        Vector4[] verts4D = new Vector4[mesh4D.vertices.Length];
        mesh4D.vertices.CopyTo(verts4D, 0);

        verts4D = Rotate(verts4D, rotation);

        verts4D = Translate(verts4D, position);


        verts4D = Rotate(verts4D, -viewRot);

        verts4D = Translate(verts4D, viewOffset);

        verts4D = Scale(verts4D, scale);

        Vector3[] verts3D = new Vector3[0];

        switch (projectType)
        {
            case ProjectType.Intersect:
                // Calculating the new positions for the vertices based on their intercept with w = 0
                verts3D = Project4DTo3D(verts4D, mesh4D.indicesL);
                break;
            case ProjectType.Flatten:
                // Flattens all the points to be at w = 0;
                verts3D = Flatten4DTo3D(verts4D);
                break;
        }

        Mesh mesh = filter.sharedMesh;
        Mesh meshO = new Mesh();
        if (filterO)
            meshO = filterO.sharedMesh;

        if (verts3D.Length > 0)
        {
            mesh.SetVertices(verts3D);
            switch (renderType)
            {
                case RenderType.Volume:
                    mesh.SetIndices(mesh4D.indices, MeshTopology.Triangles, 0);
                    break;
                case RenderType.Outline:
                    mesh.SetIndices(mesh4D.indicesL, MeshTopology.Lines, 0);
                    if (!outlineMaterial)
                        outlineMaterial = material;
                    renderer.material = outlineMaterial;
                    break;
                case RenderType.VolumeOutline:
                    mesh.SetIndices(mesh4D.indices, MeshTopology.Triangles, 0);
                    if (filterO)
                    {
                        meshO.SetVertices(verts3D);
                        meshO.SetIndices(mesh4D.indicesL, MeshTopology.Lines, 0);

                        if (rendererO)
                        {
                            if (!outlineMaterial)
                                outlineMaterial = material;
                            rendererO.material = outlineMaterial;
                        }
                    }
                    break;

            }

            mesh.RecalculateBounds();


            if (collider)
                collider.sharedMesh = mesh;
        }
        // If there are no vertices it means there were no intersections so we clear the mesh
        else
        {
            mesh.Clear();
            meshO.Clear();
        }
    }

    // Projecects the given 4-Dimensional vertex positions to the 3-Dimensional world
    private Vector3[] Project4DTo3D(Vector4[] vertices, int[] lineIndices)
    {
        Vector3[] newVertices = new Vector3[vertices.Length];
        int intersections = 0;

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

            // If this line intersects with the w origin
            if (Intersects(start, end, referenceW))
            {
                intersections++;

                // Calculating the vector of the line
                Vector4 v = end - start;

                // Finding the intercept point t on the line with the 3d world
                float t = (referenceW - w) / v.w;
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

    // Flattens the w axis for this object
    private Vector3[] Flatten4DTo3D(Vector4[] vertices)
    {
        Vector3[] newVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            newVertices[i] = vertices[i];

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

    private Vector4[] Scale(Vector4[] vertices, Vector4 scale)
    {
        Vector4[] output = new Vector4[vertices.Length];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = new Vector4(vertices[i].x * scale.x, vertices[i].y * scale.y, vertices[i].z * scale.z, vertices[i].w * scale.w);
        }

        return output;
    }

    // Rotations performed in 4D are done across 2D planes and not 1D points like they are in a 3D space

    private Vector4[] Rotate(Vector4[] vertices, Vector6 rotation)
    {

        vertices = RotateZW(vertices, rotation.zw);

        vertices = RotateYW(vertices, rotation.yw);

        vertices = RotateYZ(vertices, rotation.yz);

        vertices = RotateXW(vertices, rotation.xw);

        vertices = RotateXZ(vertices, rotation.xz);

        vertices = RotateXY(vertices, rotation.xy);

        return vertices;
    }
    private Vector4[] RotateXY(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[0, 0] = 1;
        R[1, 1] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[2, 2] = c;
        R[3, 3] = c;
        R[2, 3] = -s;
        R[3, 2] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }
    private Vector4[] RotateXZ(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[0, 0] = 1;
        R[2, 2] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[1, 1] = c;
        R[3, 3] = c;
        R[1, 3] = -s;
        R[3, 1] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }
    private Vector4[] RotateXW(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[0, 0] = 1;
        R[3, 3] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[1, 1] = c;
        R[2, 2] = c;
        R[1, 2] = -s;
        R[2, 1] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }
    private Vector4[] RotateYZ(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[1, 1] = 1;
        R[2, 2] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[0, 0] = c;
        R[3, 3] = c;
        R[0, 3] = -s;
        R[3, 0] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }
    private Vector4[] RotateYW(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[1, 1] = 1;
        R[3, 3] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[0, 0] = c;
        R[2, 2] = c;
        R[0, 2] = -s;
        R[2, 0] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }
    private Vector4[] RotateZW(Vector4[] vertices, float angle)
    {
        Matrix4x4 R = new Matrix4x4();
        R[2, 2] = 1;
        R[3, 3] = 1;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        R[0, 0] = c;
        R[1, 1] = c;
        R[0, 1] = -s;
        R[1, 0] = s;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = MM(R, vertices[i]);

        return vertices;
    }

    // Returns true if the line given by start and end intersects at the point w on the w-axis
    private bool Intersects(Vector4 start, Vector4 end, float w)
    {
        return start.w >= w && end.w <= w || start.w <= w && end.w >= w;
    }

    // Matrix multiplication between a 4x4 and 1x4 matrix
    private Vector4 MM(Matrix4x4 A, Vector4 B)
    {
        Vector4 output = new Vector4();

        for (int r = 0; r < 4; r++)
        {
            float sum = 0;

            for (int i = 0; i < 4; i++)
                sum += A[r, i] * B[i];

            output[r] = sum;
        }

        return output;
    }
}
