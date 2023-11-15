using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {

            PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece>();
            pieceProps.joinable = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece>();
            pieceProps.joinable = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece> ();
        pieceProps.joinable = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
