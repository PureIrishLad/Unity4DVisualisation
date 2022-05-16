using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player4D : MonoBehaviour
{
    public Transform4D transform4D;

    private Renderer4D[] renderer4Ds;

    public float wAxisSpeed;
    public float planeRotationSpeed;

    private void Start()
    {
        GameObject[] objects4D = GameObject.FindGameObjectsWithTag("4DObject");
        renderer4Ds = new Renderer4D[objects4D.Length];

        for (int i = 0; i < objects4D.Length; i++)
        {
            renderer4Ds[i] = objects4D[i].GetComponent<Renderer4D>();
            renderer4Ds[i].player = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            transform4D.rotation = new Vector6();

        transform4D.position.x = transform.position.x;
        transform4D.position.y = transform.position.y;
        transform4D.position.z = transform.position.z;

        transform4D.rotation.xw = transform.rotation.x;
        transform4D.rotation.yw = transform.rotation.y;
        transform4D.rotation.zw = transform.rotation.z;

        float wMovement = Input.GetAxis("W Axis") * wAxisSpeed * Time.deltaTime;
        float xyRot = Input.GetAxis("XY Axis") * planeRotationSpeed * Time.deltaTime;
        float xzRot = Input.GetAxis("XZ Axis") * planeRotationSpeed * Time.deltaTime;
        float yzRot = Input.GetAxis("YZ Axis") * planeRotationSpeed * Time.deltaTime;


        transform4D.position.w += wMovement;
        transform4D.rotation += new Vector6(xyRot, xzRot, 0, yzRot, 0, 0);
    }
}
