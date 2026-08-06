using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;

    void LateUpdate()
    {
        // Prevents WebGL from crashing if target is missing
        if (target == null) return;

        Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);

        // Lerp is generally preferred over Slerp for flat 2D tracking
        transform.position = Vector3.Lerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }
}