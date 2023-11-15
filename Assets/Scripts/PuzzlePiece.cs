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
    public bool cornerPiece;
    public bool flipNormals;
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
    public float joinThreshold;
    public float joinRotThreshold;
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
        if (gameObject.GetComponent<Rigidbody>() != null )
        {
            rb = gameObject.GetComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.drag = 0.1f;
            startRotation = rb.transform.eulerAngles;
        }
        gameObject.tag = "PuzzlePiece";
        cornerUL = GameObject.FindGameObjectWithTag("BoxCornerUL");
        cornerLR = GameObject.FindGameObjectWithTag("BoxCornerLR");
        vecUL = cornerUL.transform.position;
        vecLR = cornerLR.transform.position;
        ol = gameObject.AddComponent<OutlineQ>();
        ol.enabled = false;
        

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
        joinable = true;
        var rot = rb.transform.eulerAngles;
        if (Input.GetMouseButton(1))
        {
            StartCoroutine(PickedPiece(0.5f, rot));
        }
        else
        {
            StartCoroutine(PickedPiece(1f, rot));
        }
    }

    private void OnMouseDrag()
    {
        rb.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
        var rot = rb.transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            rot += new Vector3(0, 0, Input.mouseScrollDelta.y * 10f);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            rot += new Vector3(Input.mouseScrollDelta.y * 10f, 0, 0);
        }
        else
        {
            rot += new Vector3(0, Input.mouseScrollDelta.y * 10f, 0);
        }
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
        CameraMouse cameraMouse = cam.GetComponent<CameraMouse>();
        if (cameraMouse.matchingLook == true | cameraMouse.closerLook == true)
        {
            RaycastHit hitinfo = new RaycastHit();
            bool hit = Physics.Raycast(transform.position, cam.transform.forward, out  hitinfo, 10f);
            Debug.DrawRay(transform.position, cam.transform.forward, Color.red, 1);
            rb.transform.position = hitinfo.point + new Vector3(0,0.12f,0);
            rb.transform.eulerAngles = new Vector3(startRotation.x, rb.transform.eulerAngles.y, startRotation.z);
            rb.velocity = Vector3.zero;
            joinable = true;
        }
        else
        {
            rb.velocity = (rb.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z))) * 20f + new Vector3(0,5,0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (joinable)
        {
            if (collision.gameObject.tag == "PuzzlePiece")
            {
                if (hasNextAbove && collision.gameObject == nextAbove)
                {
                    if (Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextAboveOffset) < joinThreshold && Vector3.Distance((collision.rigidbody.transform.eulerAngles -  rb.transform.eulerAngles), nextAboveRotOS) < joinRotThreshold)
                    {
                        Debug.Log("They fit!");
                    }
                }
            }
        }
    }

    void Update()
    {
        if (rb != null)
        {
            if (rb.transform.position.y < 0.5f)
            {
                float tossX = Random.Range(vecUL.x, vecLR.x);
                float tossY = Random.Range(vecUL.y, vecLR.y);
                float tossZ = Random.Range(vecUL.z, vecLR.z);
                rb.transform.position = new Vector3(tossX, tossY, tossZ);
                joinable = false;
            }
            if (rb.velocity.magnitude > 1000)
            {
                rb.velocity = Vector3.zero;
            }
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
            yield return null;
        }
    }

}
