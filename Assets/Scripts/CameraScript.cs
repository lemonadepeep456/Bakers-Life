using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float FollowSpeed = 2f; //How fast the camera follows the player
    public float yOffset = 1f; //Offset
    public Transform target; //the game object it's following

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f); //makes it follow to its position
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime); //detects the amount of time its taking to get to the position(hence follow speed and the target)

    }
}