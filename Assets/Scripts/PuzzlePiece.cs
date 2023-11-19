using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
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
    public List<GameObject> joinablePieces;
    public List<GameObject> islandPieces;
    public List<Vector3> islandOffsets;
    public bool joinedBelow;
    public bool joinedRight;
    public bool joinedAbove;
    public bool joinedLeft;
    public GameObject ghost;
    public OutlineQ ghostOL;
    public ProBuilderMesh ghostPM;
    public Rigidbody ghostRB;
    public MeshCollider ghostColl;
    public PieceGhost ghostProps;
    public Vector3 nextBelowOffset;
    public Vector3 nextRightOffset;
    public Vector3 nextAboveOffset;
    public Vector3 nextLeftOffset;
    public BoxCollider pieceCollider;
    public PhysicMaterial physMat;
    private Camera cam;
    private Rigidbody rb;
    private MeshCollider coll;
    private FixedJoint jointAbove;
    private FixedJoint jointBelow;
    private FixedJoint jointLeft;
    private FixedJoint jointRight;
    private GameObject cornerUL;
    private GameObject cornerLR;
    private Vector3 vecUL;
    private Vector3 vecLR;
    public bool joinable;
    public bool joinimated;
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
    public float pieceSizeX;
    public float pieceSizeY;
    Vector3 startRotation;
    CameraMouse camMouse;
    public List<Vector3> piecePoints;
    GameObject gameGO;
    MeshGen game;
    AudioSource sndSource;
    RaycastHit hitinfo;
    bool hit;
    Vector3 dirMod;
    
    void Start()
    {
        islandPieces = new List<GameObject> ();
        islandOffsets = new List<Vector3> ();
        joinedAbove = false;
        joinedBelow = false;
        joinedLeft = false;
        joinedRight = false;
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
            rb.mass = (pieceSizeX*pieceSizeY) / 0.4f;
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
        gameObject.layer = 2;
        var rot = rb.transform.eulerAngles;
        if (Input.GetMouseButton(1))
        {
            StartCoroutine(PickedPiece(0.5f, rot));
        }
        else
        {
            StartCoroutine(PickedPiece(1f, rot));
        }
        ghost = Instantiate(gameObject, game.table.transform);
        ghost.GetComponent<PuzzlePiece>().enabled = false;
        ghostOL = ghost.GetComponent<OutlineQ>();
        ghostOL.enabled = false;
        ghostPM = ghost.GetComponent<ProBuilderMesh>();
        ghost.tag = "PuzzlePieceGhost";
        ghost.layer = 2;
        ghostPM.SetMaterial(ghostPM.faces, game.ghostMaterial);
        ghostPM.ToMesh();
        ghostPM.Refresh();
        ghostRB = ghost.GetComponent<Rigidbody>();
        ghostRB.isKinematic = true;
        ghostRB.useGravity = false;
        ghostColl = ghost.GetComponent<MeshCollider>();
        ghostColl.isTrigger = true;
        ghostOL.enabled = true;
        ghostOL.OutlineColor = Color.cyan;
        ghostProps = ghost.gameObject.AddComponent<PieceGhost>();
        ghostProps.joinable = joinable;
        islandOffsets.Clear();
        if (islandPieces.Count > 0 )
        {
            for (int i = 0; i < islandPieces.Count; i++)
            {
                if (islandPieces[i].name != name)
                {
                    islandPieces[i].GetComponent<Rigidbody>().useGravity = false;
                    //islandPieces[i].GetComponent<Rigidbody>().isKinematic = true;
                    islandPieces[i].GetComponent<Rigidbody>().mass = 0.1f;
                    islandPieces[i].GetComponent<Rigidbody>().drag = 0.0f;
                }
                
            }
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

        if (camMouse.closerLook)
        {
            hitinfo = new RaycastHit();
            Debug.DrawRay(transform.position, cam.transform.forward * 15f, Color.green);
            hit = Physics.Raycast(transform.position, cam.transform.forward, out hitinfo, 15f);
            if (hit)
            {
                //Debug.Log($"{hitinfo.collider.tag}, {hitinfo.point}");
                if (hitinfo.collider.tag == "Table" || hitinfo.collider.tag == "PuzzlePiece")
                {
                    ghost.transform.position = hitinfo.point + new Vector3(0, 0.12f, 0);
                    ghost.transform.eulerAngles = new Vector3(startRotation.x, rb.transform.eulerAngles.y, startRotation.z);
                }
                else
                {
                    ghost.transform.position = new Vector3(0, 0, -20);
                }
            }
        }
        else
        {
            hitinfo = new RaycastHit();
            dirMod = camMouse.ray.direction + new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f));
            hit = Physics.Raycast(camMouse.ray.origin, dirMod, out hitinfo, 20f);
            if (hit)
            {
                Debug.DrawRay(camMouse.ray.origin, dirMod * hitinfo.distance, Color.cyan);
                //Debug.Log($"{hitinfo.collider.tag}, {hitinfo.point}");
                if (hitinfo.collider.tag == "Table" || hitinfo.collider.tag == "PuzzlePiece")
                {
                    ghost.transform.position = hitinfo.point + new Vector3(0, 0.12f, 0);
                    ghost.transform.eulerAngles = new Vector3(startRotation.x, rb.transform.eulerAngles.y + Random.Range(-1f, 1f) * hitinfo.distance, startRotation.z);
                }
                else
                {
                    ghost.transform.position = new Vector3(0, 0, -20);
                }
            }
            else
            {
                ghost.transform.position = new Vector3(0, 0, -20);
            }
        }
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
            if (hit)
            {
                if (hitinfo.collider.tag == "Table")
                {
                    rb.transform.position = ghostRB.transform.position;
                    rb.transform.eulerAngles = ghostRB.transform.eulerAngles;
                    rb.velocity = Vector3.zero;
                    joinable = true;
                    ghostProps.joinable = joinable;
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }
            else
            {
                joinable = false;
                ghostProps.joinable = joinable;
            }
            gameObject.layer = 0;
        }
        else
        {
            Vector3 lowAngle;
            Vector3 highAngle;
            if (hit)
            {
                if (hitinfo.collider.tag == "Table" || hitinfo.collider.tag == "PuzzlePiece")
                {
                    Ballistics.SolveBallisticArc(camMouse.ray.origin, 20f, hitinfo.point, -Physics.gravity.y, out lowAngle, out highAngle);
                    rb.velocity += lowAngle;
                }
                else
                {
                    Ballistics.SolveBallisticArc(camMouse.ray.origin, 50f, hitinfo.point, -Physics.gravity.y, out lowAngle, out highAngle);
                    rb.velocity += lowAngle;
                }
            }
            else
            {
                Ballistics.SolveBallisticArc(camMouse.ray.origin, 100f, camMouse.ray.origin + (camMouse.ray.direction * 100f), -Physics.gravity.y, out lowAngle, out highAngle);
                rb.velocity += lowAngle;
                joinable = false;
                ghostProps.joinable = false;
            }
        }
        gameObject.layer = 0;
        Destroy(ghost);
        rb.angularDrag = 0.05F;
        if (islandPieces.Count > 0)
        {
            for (int i = 0; i < islandPieces.Count; i++)
            {
                if (islandPieces[i].name != name)
                {
                    islandPieces[i].GetComponent<Rigidbody>().useGravity = true;
                    //islandPieces[i].GetComponent<Rigidbody>().isKinematic = false;
                    islandPieces[i].GetComponent<Rigidbody>().mass = (pieceSizeX * pieceSizeY) / 0.4f;
                    islandPieces[i].GetComponent<Rigidbody>().drag = 0.1f;
                    islandPieces[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioClip snd;
        if (collision.gameObject.tag == "PuzzlePiece")
        {
            
            if (collision.impulse.magnitude > 0.15f)
            {
                //Debug.Log($"Piece on Piece collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPiecePiece[Random.Range(0, game.soundsImpactPiecePiece.Count - 1)];
                sndSource.pitch = Random.Range(0.95f, 1.05f) - collision.impulse.magnitude / 10;
                sndSource.PlayOneShot(snd, collision.impulse.magnitude);
            }

            if (joinable && collision.gameObject.GetComponent<PuzzlePiece>().joinable)
            {
                float angleDiff = Vector3.Angle(collision.transform.eulerAngles, transform.eulerAngles);
                Vector3 offsetCheck = collision.transform.position - transform.position;

                if (hasNextAbove && collision.gameObject == nextAbove)
                {
                    float offsetDiff = Vector3.Distance(offsetCheck, nextAboveOffset);
                    if (offsetDiff < joinThreshold && angleDiff < joinRotThreshold)
                    {
                        if (!joinedAbove)
                        {
                            joinedAbove = true;
                            collision.gameObject.GetComponent<PuzzlePiece>().joinedBelow = true;
                            if (!islandPieces.Contains(gameObject))
                            {
                                islandPieces.Add(gameObject);
                            }
                            if (!islandPieces.Contains(collision.gameObject))
                            {
                                islandPieces.Add(collision.gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(collision.gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(collision.gameObject);
                            }
                            collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Distinct().ToList();
                            transform.position = collision.transform.position + collision.gameObject.GetComponent<PuzzlePiece>().nextBelowOffset;
                            transform.eulerAngles = collision.transform.eulerAngles;
                            jointAbove = gameObject.AddComponent<FixedJoint>();
                            jointAbove.connectedBody = collision.rigidbody;
                            StartCoroutine(JoinFlash(islandPieces, game));
                        }
                        print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                }
                else if (hasNextBelow && collision.gameObject == nextBelow)
                {
                    float offsetDiff = Vector3.Distance(offsetCheck, nextBelowOffset);
                    if (offsetDiff < joinThreshold && angleDiff < joinRotThreshold)
                    {
                        if (!joinedBelow)
                        {
                            joinedBelow = true;
                            collision.gameObject.GetComponent<PuzzlePiece>().joinedAbove = true;
                            if (!islandPieces.Contains(gameObject))
                            {
                                islandPieces.Add(gameObject);
                            }
                            if (!islandPieces.Contains(collision.gameObject))
                            {
                                islandPieces.Add(collision.gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(collision.gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(collision.gameObject);
                            }
                            collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Distinct().ToList();
                            transform.position = collision.transform.position + collision.gameObject.GetComponent<PuzzlePiece>().nextAboveOffset;
                            transform.eulerAngles = collision.transform.eulerAngles;
                            jointBelow = gameObject.AddComponent<FixedJoint>();
                            jointBelow.connectedBody = collision.rigidbody;
                            StartCoroutine(JoinFlash(islandPieces, game));
                        }
                        print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                }
                else if (hasNextRight && collision.gameObject == nextRight)
                {
                    float offsetDiff = Vector3.Distance(offsetCheck, nextRightOffset);
                    if (offsetDiff < joinThreshold && angleDiff < joinRotThreshold)
                    {
                        if (!joinedRight)
                        {
                            joinedRight = true;
                            collision.gameObject.GetComponent<PuzzlePiece>().joinedLeft = true;
                            if (!islandPieces.Contains(gameObject))
                            {
                                islandPieces.Add(gameObject);
                            }
                            if (!islandPieces.Contains(collision.gameObject))
                            {
                                islandPieces.Add(collision.gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(collision.gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(collision.gameObject);
                            }
                            collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Distinct().ToList();
                            transform.position = collision.transform.position + collision.gameObject.GetComponent<PuzzlePiece>().nextLeftOffset;
                            transform.eulerAngles = collision.transform.eulerAngles;
                            jointRight = gameObject.AddComponent<FixedJoint>();
                            jointRight.connectedBody = collision.rigidbody;
                            StartCoroutine(JoinFlash(islandPieces, game));
                        }
                        print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                }
                else if (hasNextLeft && collision.gameObject == nextLeft)
                {
                    float offsetDiff = Vector3.Distance(offsetCheck, nextLeftOffset);
                    if (offsetDiff < joinThreshold && angleDiff < joinRotThreshold)
                    {
                        if (!joinedLeft)
                        {
                            joinedLeft = true;
                            collision.gameObject.GetComponent<PuzzlePiece>().joinedRight = true;
                            if (!islandPieces.Contains(gameObject))
                            {
                                islandPieces.Add(gameObject);
                            }
                            if (!islandPieces.Contains(collision.gameObject))
                            {
                                islandPieces.Add(collision.gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(gameObject);
                            }
                            if (!collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Contains(collision.gameObject))
                            {
                                collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Add(collision.gameObject);
                            }
                            islandPieces.Distinct().ToList();
                            collision.gameObject.GetComponent<PuzzlePiece>().islandPieces.Distinct().ToList();
                            transform.position = collision.transform.position + collision.gameObject.GetComponent<PuzzlePiece>().nextRightOffset;
                            transform.eulerAngles = collision.transform.eulerAngles;
                            jointLeft = gameObject.AddComponent<FixedJoint>();
                            jointLeft.connectedBody = collision.rigidbody;
                            StartCoroutine(JoinFlash(islandPieces, game));
                        }
                        print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                    else
                    {
                        print($"{name} and {collision.gameObject.name} fit together, but not like this.\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                    }
                }
            }
        }

        if (collision.gameObject.tag == "PuzzleBox")
        {
            if (collision.impulse.magnitude > 0.35f)
            {
                //Debug.Log($"Piece on Box collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPieceBox[Random.Range(0, game.soundsImpactPieceBox.Count - 1)];
                sndSource.pitch = Random.Range(0.95f, 1.05f) - collision.impulse.magnitude / 10;
                sndSource.PlayOneShot(snd, collision.impulse.magnitude);
            }
        }

        if (collision.gameObject.tag == "Table")
        {
            if (collision.impulse.magnitude > 0.05f)
            {
                //Debug.Log($"Piece on Table collision impulse: {collision.impulse.magnitude}");
                snd = game.soundsImpactPieceTable[Random.Range(0, game.soundsImpactPieceTable.Count - 1)];
                float newPitch = Random.Range(1f, 1.05f) - Mathf.Log(collision.impulse.magnitude*rb.mass, rb.mass)/5;
                sndSource.pitch = newPitch;
                sndSource.PlayOneShot(snd, 1f/newPitch);
            }
        }
    }


    void Update()
    {
        if (rb != null)
        {
            if (rb.transform.position.y < 0.5f)
            {
                AudioClip snd;
                snd = game.soundsPieceOnFloor[Random.Range(0, game.soundsPieceOnFloor.Count - 1)];
                sndSource.PlayOneShot(snd, 1);
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
            distanceToScreen -= 30f * Time.deltaTime;
            rb.angularDrag += 5F * Time.deltaTime;  
            if (distanceToScreen < minDist)
            {
                distanceToScreen = minDist;
            }
            i++;
            yield return null;
        }
    }

    IEnumerator JoinFlash(List<GameObject> island, MeshGen game)
    {
        float lerp = 0f;
        for (int i = 0; i < island.Count; i++)
        {
            island[i].GetComponent<OutlineQ>().enabled = true;
            game.AddJoined(island[i]);
        }
        while ( lerp < 1f)
        {
            lerp += 0.2f * Time.deltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            for (int i = 0;i < island.Count;i++)
            {
                island[i].GetComponent<OutlineQ>().OutlineColor = Color.Lerp(Color.clear, Color.white, lerp);
            }
            yield return null;
        }
        while (lerp > 0f)
        {
            lerp -= 0.2f * Time.deltaTime;
            if (lerp < 0f)
            {
                lerp = 0f;
            }
            for (int i = 0; i < island.Count; i++)
            {
                island[i].GetComponent<OutlineQ>().OutlineColor = Color.Lerp(Color.clear, Color.white, lerp);
            }
            yield return null;
        }

    }

}
