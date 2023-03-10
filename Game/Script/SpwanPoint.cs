using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPoint : MonoBehaviour
{
    public GameObject EnemyToSpwan;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3(0f, 0.5f, 0f);
        Gizmos.DrawWireCube(center, Vector3.one);
        Gizmos.DrawLine(center, center + transform.forward * 2);
    }
}
