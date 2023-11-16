using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : MonoBehaviour
{
    public List<GameObject> piecesInBox;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {

            PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            piecesInBox.Add(collision.gameObject);
            PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece>();
            pieceProps.insideBox = true;
            pieceProps.joinable = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            piecesInBox.Remove(collision.gameObject);
            PuzzlePiece pieceProps = collision.gameObject.GetComponent<PuzzlePiece>();
            pieceProps.insideBox = false;
        }
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
