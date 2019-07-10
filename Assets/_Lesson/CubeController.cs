using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public bool isTest;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTest)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Move(h, v);

            if(Input.GetKeyDown(KeyCode.J))
            {
                Jump();
            }
        }

    }

    public void Move(float h, float v)
    {
        body.MovePosition(body.position + transform.forward * v * 10 * Time.deltaTime);
        body.MoveRotation(Quaternion.Euler(0, h * 180 * Time.deltaTime, 0) * body.rotation);
    }

    public void Jump()
    {
        body.velocity += Vector3.up * 10;
    }
}
