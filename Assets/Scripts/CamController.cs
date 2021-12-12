using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;

    private Rigidbody rb;
    private Vector3 forceToApply;
    private float turnAmount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Assert(rb, "No Rigidbody component!");
    }


    private void Update()
    {
        forceToApply = Vector3.zero;
        turnAmount = 0f;

        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            forceToApply += rb.transform.forward * moveSpeed;
        }

        //move back
        if (Input.GetKey(KeyCode.S))
        {
            forceToApply -= rb.transform.forward * moveSpeed;
        }

        //strafe left
        if (Input.GetKey(KeyCode.A))
        {
            forceToApply -= rb.transform.right * moveSpeed;
        }

        //strafe right
        if (Input.GetKey(KeyCode.D))
        {
            forceToApply += rb.transform.right * moveSpeed;
        }

        //turn left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnAmount -= turnSpeed * Time.deltaTime;
        }

        //turn left
        if (Input.GetKey(KeyCode.RightArrow))
        {
            turnAmount += turnSpeed * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(rb)
        {
            rb.AddForce(forceToApply, ForceMode.Force);
            rb.transform.Rotate(rb.transform.up, turnAmount);
        }
    }

}
