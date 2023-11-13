using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject cornerUL;
    private GameObject cornerLR;
    private Vector3 vecUL;
    private Vector3 vecLR;
    public bool joinable;
    private bool held;
    private bool heldDown;
    private bool incompatible;
    private bool compatible;
    public OutlineQ ol;
    public Vector3 vel;
    float distanceToScreen;
    Vector3 startRotation;

    public List<Vector3> piecePoints;
    
    void Start()
    {
        hand = GameObject.FindGameObjectWithTag("Hand");
        finger = GameObject.FindGameObjectWithTag("IndexFinger");
        cam = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.drag = 0.1f;
        gameObject.tag = "PuzzlePiece";
        cornerUL = GameObject.FindGameObjectWithTag("BoxCornerUL");
        cornerLR = GameObject.FindGameObjectWithTag("BoxCornerLR");
        vecUL = cornerUL.transform.position;
        vecLR = cornerLR.transform.position;
        ol = gameObject.AddComponent<OutlineQ>();
        ol.enabled = false;
        startRotation = rb.transform.eulerAngles;

        //pieceCollider = gameObject.AddComponent<BoxCollider>();
        //pieceCollider.isTrigger = true;
        //pieceCollider.enabled = true;
        //pieceCollider.size = new Vector3(1.1f, 1.1f, 1.1f);
    }

    private void OnMouseDown()
    {
        ol.enabled = true;
        ol.OutlineColor = Color.blue;
        ol.OutlineWidth = 5;
        distanceToScreen = cam.WorldToScreenPoint(rb.transform.position).z;
        var rot = rb.transform.eulerAngles;
        StartCoroutine(PickedPiece(1f, rot));
    }

    private void OnMouseDrag()
    {
        rb.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
        var rot = rb.transform.rotation.eulerAngles;
        rot += new Vector3(0, Input.mouseScrollDelta.y*5f, 0);
        Quaternion quatRot = new Quaternion();
        quatRot.eulerAngles = rot;
        rb.transform.rotation = quatRot;
        //rot.y += Input.mouseScrollDelta.y;
        //rb.transform.LookAt(cam.transform.position, rb.transform.up);
        vel = rb.velocity;
    }

    private void OnMouseEnter()
    {
        if (!Input.GetMouseButton(0))
        {
            ol.enabled = true;
            ol.OutlineColor = Color.yellow;
            ol.OutlineWidth = 3;
        }
    }

    private void OnMouseExit()
    {
        if (!Input.GetMouseButton(0))
        {
            ol.enabled = false;
        }
            
    }

    private void OnMouseUp()
    {
        ol.OutlineWidth = 3;
        ol.enabled = false;
        StopCoroutine(PickedPiece(2f, new Vector3(0,0,0)));
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.transform.position.y < 0.5f)
        {
            float tossX = Random.Range(vecUL.x, vecLR.x);
            float tossY = Random.Range(vecUL.y, vecLR.y);
            float tossZ = Random.Range(vecUL.z, vecLR.z);
            rb.transform.position = new Vector3(tossX, tossY, tossZ);
        }
        if (rb.velocity.magnitude > 1000)
        {
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator PickedPiece(float minDist, Vector3 initRot)
    {
        int i = 0;
        Vector3 curPos = rb.transform.position;
        while (distanceToScreen > minDist)
        {
            distanceToScreen -= 0.4f;
            if (distanceToScreen < minDist)
            {
                distanceToScreen = minDist;
            }
            i++;
            if (rb.transform.eulerAngles != new Vector3(startRotation.x, rb.transform.eulerAngles.y, startRotation.z))
            {
                rb.transform.eulerAngles = Vector3.Lerp(new Vector3(initRot.x, rb.transform.eulerAngles.y, initRot.z), new Vector3(startRotation.x, rb.transform.eulerAngles.y, startRotation.z), 0.075f * i);
            }
            yield return null;
        }
    }

}
