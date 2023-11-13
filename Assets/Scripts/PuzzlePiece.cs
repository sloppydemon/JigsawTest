using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public bool hasNextBelow;
    public bool hasNextRight;
    public bool hasNextAbove;
    public bool hasNextLeft;
    public int pieceX;
    public int pieceY;
    public GameObject hand;
    public GameObject finger;
    public GameObject nextBelow;
    public GameObject nextRight;
    public GameObject nextAbove;
    public GameObject nextLeft;
    public Vector3 nextBelowOffset;
    public Vector3 nextRightOffset;
    public Vector3 nextAboveOffset;
    public Vector3 nextLeftOffset;
    public Vector3 nextBelowRotOS;
    public Vector3 nextRightRotOS;
    public Vector3 nextAboveRotOS;
    public Vector3 nextLeftRotOS;
    public BoxCollider pieceCollider;
    private Camera cam;
    private Rigidbody rb;

    public List<Vector3> piecePoints;
    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.FindGameObjectWithTag("Hand");
        finger = GameObject.FindGameObjectWithTag("IndexFinger");
        cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
        tag = "PuzzlePiece";
        //pieceCollider = gameObject.AddComponent<BoxCollider>();
        //pieceCollider.isTrigger = true;
        //pieceCollider.enabled = true;
        //pieceCollider.size = new Vector3(1.1f, 1.1f, 1.1f);


    }

    //private void OnMouseDown()
    //{
    //    float distanceToScreen = cam.WorldToScreenPoint(rb.transform.localPosition).z;
    //    Vector3 offset = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen)) - rb.transform.localPosition;
    //}

    private void OnMouseDrag()
    {
        float distanceToScreen = cam.WorldToScreenPoint(rb.transform.position).z;
        rb.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
        var rot = rb.rotation;
        rot.y += Input.mouseScrollDelta.y; 
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
