using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed = 1;

    private void Update()
    {
        transform.Rotate(axis, Time.deltaTime * speed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = new Color(Mathf.Abs(axis.x), Mathf.Abs(axis.y), Mathf.Abs(axis.z), 1);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawRay(transform.position, axis.normalized);
    }
}
