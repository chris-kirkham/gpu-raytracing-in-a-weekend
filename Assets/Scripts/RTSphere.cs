using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSphere : MonoBehaviour
{
    public float radius;
    public RaytraceMaterial mat;
    public bool isMetal;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = mat.col;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
