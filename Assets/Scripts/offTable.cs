using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offTable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.velocity = -rb.velocity;
        }
    }
}
