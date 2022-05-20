using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player4D : MonoBehaviour
{
    public Transform4D transform4D;

    private Renderer4D[] renderer4Ds;

    public float wAxisSpeed;
    public float planeRotationSpeed;

    public float xyzSpeed;

    public float sensitivity;

    private void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
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

        float wMovement = Input.GetAxis("W Axis") * wAxisSpeed * Time.deltaTime;
        float zwRot = Input.GetAxis("XY Axis") * planeRotationSpeed * Time.deltaTime;
        float ywRot = Input.GetAxis("XZ Axis") * planeRotationSpeed * Time.deltaTime;
        float xwRot = Input.GetAxis("YZ Axis") * planeRotationSpeed * Time.deltaTime;
        float horizontal = Input.GetAxis("Horizontal") * xyzSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * xyzSpeed * Time.deltaTime;
        //Debug.Log(transform4D.right);
        transform4D.position += transform4D.right * horizontal + transform4D.forward * vertical + transform4D.wDir * wMovement;
        transform4D.rotation += new Vector6(0, 0, xwRot, 0, ywRot, zwRot);
        transform.position = (Vector3)transform4D.position;
        Vector4 f = transform4D.Forward();
        //Debug.DrawLine((Vector3)transform4D.position, (Vector3)transform4D.position + (Vector3)f, Color.black);
        //Debug.Log(transform4D.position + f);
    }

    private void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform4D.rotation.xz -= mouseY * Time.deltaTime;
        transform4D.rotation.yz += mouseX * Time.deltaTime;

        float xz = transform4D.rotation.xz;

        if (xz > 0.5f * Mathf.PI && xz < 1.5f * Mathf.PI)
        {
            float d = 1 * Mathf.PI - xz;

            if (d < 0)
                xz = 1.5f * Mathf.PI;
            else
                xz = 0.5f * Mathf.PI;
        }

        transform4D.rotation.xz = xz;

        Camera.main.transform.eulerAngles = new Vector3(xz * (180f / Mathf.PI), transform4D.rotation.yz * (180f / Mathf.PI), 0.0f);
    }
}