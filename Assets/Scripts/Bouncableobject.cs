using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncableObject : MonoBehaviour
{
    public float moveSpeed = 5f;   // how fast it glides
    public float destroyX = -10f;  // X position to destroy at

    void Update()
    {
        // Move left smoothly
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Destroy when it passes the boundary
        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
