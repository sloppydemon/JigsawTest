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
    public PhysicMaterial physMat;
    private Camera cam;
    private Rigidbody rb;
    private MeshCollider coll;
    private GameObject cornerUL;
    private GameObject cornerLR;
    private Vector3 vecUL;
    private Vector3 vecLR;
    public bool joinable;
    private bool held;
    private bool heldDown;
    public bool insideBox;
    private bool incompatible;
    private bool compatible;
    public float joinThreshold;
    public float joinRotThreshold;
    public float rotSpeed;
    public float sleepThreshold;
    public OutlineQ ol;
    public Vector3 vel;
    float distanceToScreen;
    Vector3 startRotation;
    CameraMouse camMouse;
    public List<Vector3> piecePoints;
    GameObject gameGO;
    MeshGen game;
    AudioSource sndSource;
    
    void Start()
    {
        sndSource = gameObject.AddComponent<AudioSource>();
        joinable = false;
        gameGO = GameObject.FindGameObjectWithTag("GameController");
        game = gameGO.GetComponent<MeshGen>();
        hand = GameObject.FindGameObjectWithTag("Hand");
        finger = GameObject.FindGameObjectWithTag("IndexFinger");
        cam = Camera.main;
        camMouse = cam.GetComponent<CameraMouse>();
        if (gameObject.GetComponent<Rigidbody>() != null )
        {
            rb = gameObject.GetComponent<Rigidbody>();
            coll = gameObject.GetComponent<MeshCollider>();
            rb.mass = 0.1f;
            rb.drag = 0.1f;
            startRotation = rb.transform.eulerAngles;
            coll.material = physMat;
            rb.sleepThreshold = sleepThreshold;

        }
        gameObject.tag = "PuzzlePiece";
        cornerUL = GameObject.FindGameObjectWithTag("BoxCornerUL");
        cornerLR = GameObject.FindGameObjectWithTag("BoxCornerLR");
        vecUL = cornerUL.transform.position;
        vecLR = cornerLR.transform.position;
        ol = gameObject.AddComponent<OutlineQ>();
        ol.enabled = false;
    }

    private void OnMouseDown()
    {
        ol.enabled = true;
        ol.OutlineColor = Color.blue;
        ol.OutlineWidth = 5;
        AudioClip snd;
        if (insideBox)
        {
            snd = game.soundsPickPieceBox[Random.Range(0, game.soundsPickPieceBox.Count-1)];
        }
        else
        {
            snd = game.soundsPickPieceTable[Random.Range(0, game.soundsPickPieceTable.Count - 1)];
        }
        AudioSource.PlayClipAtPoint(snd, rb.transform.position);
        camMouse.holding = true;
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
            rot += new Vector3(0, 0, Input.mouseScrollDelta.y * rotSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            rot += new Vector3(Input.mouseScrollDelta.y * rotSpeed, 0, 0);
        }
        else
        {
            rot += new Vector3(0, Input.mouseScrollDelta.y * rotSpeed, 0);
        }
        Quaternion quatRot = new Quaternion();
        quatRot.eulerAngles = rot;
        rb.transform.rotation = quatRot;
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
        camMouse.holding = false;
        if (camMouse.closerLook == true)
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
        AudioClip snd;
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            
            if (collision.impulse.magnitude > 0.15f)
            {
                Debug.Log($"Piece on Piece collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPiecePiece[Random.Range(0, game.soundsImpactPiecePiece.Count - 1)];
                sndSource.pitch = Random.Range(0.95f, 1.05f) - collision.impulse.magnitude / 10;
                sndSource.PlayOneShot(snd, collision.impulse.magnitude);
            }

            if (joinable)
            {
                if (hasNextAbove && collision.gameObject == nextAbove)
                {
                    if (Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextAboveOffset) < joinThreshold && Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextAboveRotOS) < joinRotThreshold)
                    {
                        print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextAboveOffset)}\nDifference from initial rotational offset: {Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextAboveRotOS)}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.");
                    }
                }
                else if (hasNextBelow && collision.gameObject == nextBelow)
                {
                    if (Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextBelowOffset) < joinThreshold && Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextBelowRotOS) < joinRotThreshold)
                    {
                        Debug.Log($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextBelowOffset)}\nDifference from initial rotational offset: {Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextBelowRotOS)}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.");
                    }
                }
                else if (hasNextRight && collision.gameObject == nextRight)
                {
                    if (Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextRightOffset) < joinThreshold && Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextRightRotOS) < joinRotThreshold)
                    {
                        Debug.Log($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextRightOffset)}\nDifference from initial rotational offset: {Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextRightRotOS)}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.");
                    }
                }
                else if (hasNextLeft && collision.gameObject == nextLeft)
                {
                    if (Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextLeftOffset) < joinThreshold && Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextLeftRotOS) < joinRotThreshold)
                    {
                        Debug.Log($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{Vector3.Distance((collision.rigidbody.transform.position - rb.transform.position), nextLeftOffset)}\nDifference from initial rotational offset: {Vector3.Distance((collision.rigidbody.transform.eulerAngles - rb.transform.eulerAngles), nextLeftRotOS)}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.");
                    }
                }
            }
        }

        if (collision.gameObject.tag == "PuzzleBox")
        {
            if (collision.impulse.magnitude > 0.35f)
            {
                Debug.Log($"Piece on Box collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPieceBox[Random.Range(0, game.soundsImpactPieceBox.Count - 1)];
                sndSource.pitch = Random.Range(0.95f, 1.05f) - collision.impulse.magnitude / 10;
                sndSource.PlayOneShot(snd, collision.impulse.magnitude);
            }
        }

        if (collision.gameObject.tag == "Table")
        {
            if (collision.impulse.magnitude > 0.05f)
            {
                Debug.Log($"Piece on Table collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPieceTable[Random.Range(0, game.soundsImpactPieceTable.Count - 1)];
                sndSource.pitch = Random.Range(1f, 1.05f) - collision.impulse.magnitude / 10;
                sndSource.PlayOneShot(snd, collision.impulse.magnitude);
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
