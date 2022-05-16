using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mesh4D
{
    [HideInInspector]
    public Vector4[] vertices;
    [HideInInspector]
    // Indicies used for drawing triangles
    public int[] indices;
    [HideInInspector]
    // Indices for used for lines, lines are used to determine where a 4d vertex gets drawn in 3d
    public int[] indicesL;

    public Mesh4D()
    {
        vertices = new Vector4[0];
        indices = new int[0];
        indicesL = new int[0];
    }
    public Mesh4D(Vector4[] vertices, int[] indices, int[] indicesL)
    {
        this.vertices = vertices;
        this.indices = indices;
        this.indicesL = indicesL;
    }

    // Default hypercube mesh
    public static Mesh4D Hypercube = new Mesh4D(new Vector4[16]
    {
        new Vector4(-1, -1, -1, -1),
        new Vector4( 1, -1, -1, -1),
        new Vector4( 1,  1, -1, -1),
        new Vector4(-1,  1, -1, -1),

        new Vector4( 1, -1,  1, -1),
        new Vector4(-1, -1,  1, -1),
        new Vector4(-1,  1,  1, -1),
        new Vector4( 1,  1,  1, -1),

        new Vector4(-2, -2, -2,  1),
        new Vector4( 2, -2, -2,  1),
        new Vector4( 2,  2, -2,  1),
        new Vector4(-2,  2, -2,  1),

        new Vector4( 2, -2,  2,  1),
        new Vector4(-2, -2,  2,  1),
        new Vector4(-2,  2,  2,  1),
        new Vector4( 2,  2,  2,  1)
    }, new int[288]
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

        2,  10, 1,  1,  10, 9,
        8,  11, 0,  0,  11, 3,
        2,  3,  10, 10, 3,  11,
        8,  0,  9,  9,  0,  1,

        4,  5,  12, 12, 5,  13,
        6,  14, 5,  5,  14, 13,
        15, 14, 7,  7,  14, 6,
        15, 7,  12, 12, 7,  4,

        8,  13, 0,  0,  13, 5,
        6,  14, 3,  3,  14, 11,
        13, 14, 5,  5,  14, 6,
        3,  11, 0,  0,  11, 8,

        1,  4,  9,  9,  4,  12,
        15, 7,  10, 10, 7,  2,
        9,  10, 1,  1,  10, 2,
        4,  7,  12, 12, 7,  15,

        13, 5,  12, 12, 5,  4,
        5,  13, 0,  0,  13, 8,
        12, 4,  9,  9,  4,  1,
        1,  0,  9,  9,  0,  8,

        6,  14, 7,  7,  14, 15,
        11, 14, 3,  3,  14, 6,
        2,  7,  10, 10, 7,  15,
        11, 3,  10, 10, 3,  2
    }, new int[]
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
    });

    public static Mesh4D Hyperpyramid = new Mesh4D(new Vector4[10]
    {
        new Vector4(-1, -1, -1, -1),
        new Vector4( 1, -1, -1, -1),
        new Vector4( 0,  1,  0, -1),

        new Vector4( 1, -1,  1, -1),
        new Vector4(-1, -1,  1, -1),

        new Vector4(-2, -2, -2,  1),
        new Vector4( 2, -2, -2,  1),
        new Vector4( 0,  2,  0,  1),

        new Vector4( 2, -2,  2,  1),
        new Vector4(-2, -2,  2,  1),
    }, new int[144]
    {
        0, 2, 1,
        3, 2, 4,
        4, 2, 0,
        1, 2, 3,
        4, 0, 3, 3, 0, 1,

        1, 2, 0,
        4, 2, 3,
        0, 2, 4,
        3, 2, 1,
        1, 0, 3, 3, 0, 4,

        5, 7, 6,
        8, 7, 9,
        9, 7, 5,
        6, 7, 8,
        9, 5, 8, 8, 5, 6,

        6, 7, 5,
        9, 7, 8,
        5, 7, 9,
        8, 7, 6,
        6, 5, 8, 8, 5, 9,

        5, 0, 6, 6, 0, 1,
        5, 7, 0, 0, 7, 2,
        1, 2, 6, 6, 2, 7,

        8, 3, 9, 9, 3, 4,
        8, 7, 3, 3, 7, 2,
        4, 2, 9, 9, 2, 7,

        9, 4, 5, 5, 4, 0,
        9, 7, 4, 4, 7, 2,
        2, 7, 0, 0, 7, 5,

        6, 1, 8, 8, 1, 3,
        6, 7, 1, 1, 7, 2,
        3, 2, 8, 8, 2, 7

    }, new int[]
    {
        0, 1,
        1, 2,
        2, 1,
        3, 4,
        4, 2,
        2, 3,
        4, 0,
        3, 1,

        5, 6,
        6, 7,
        7, 5,
        8, 9,
        9, 7,
        7, 8,
        9, 5,
        8, 6,

        0, 5,
        1, 6,
        2, 7,
        3, 8,
        4, 9
    });
}
