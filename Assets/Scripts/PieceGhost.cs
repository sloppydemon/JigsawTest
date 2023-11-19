using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceGhost : MonoBehaviour
{
    public bool compatible;
    public bool touchingPiece;
    Camera cam;
    CameraMouse camMouse;
    GameObject gameGO;
    MeshGen game;
    public bool closeLook;
    public GameObject[] touchedPieces;
    public List<GameObject> joinablePieces;
    public PuzzlePiece pieceProps;
    public bool joinable;
    public bool hasNextBelow;
    public bool hasNextRight;
    public bool hasNextAbove;
    public bool hasNextLeft;
    public bool cornerPiece;
    public GameObject nextBelow;
    public GameObject nextRight;
    public GameObject nextAbove;
    public GameObject nextLeft;
    public Vector3 nextAboveOffset;
    public Vector3 nextBelowOffset;
    public Vector3 nextRightOffset;
    public Vector3 nextLeftOffset;
    public float joinThreshold;
    public float joinRotThreshold;


    void Start()
    {
        pieceProps = gameObject.GetComponent<PuzzlePiece>();
        gameGO = GameObject.FindGameObjectWithTag("GameController");
        game = gameGO.GetComponent<MeshGen>();
        cam = Camera.main;
        camMouse = cam.GetComponent<CameraMouse>();
        hasNextAbove = pieceProps.hasNextAbove;
        hasNextBelow = pieceProps.hasNextBelow;
        hasNextRight = pieceProps.hasNextRight;
        hasNextLeft = pieceProps.hasNextLeft;
        nextAbove = pieceProps.nextAbove;
        nextBelow = pieceProps.nextBelow;
        nextLeft = pieceProps.nextLeft;
        nextRight = pieceProps.nextRight;
        nextAboveOffset = pieceProps.nextAboveOffset;
        nextBelowOffset = pieceProps.nextBelowOffset;
        nextLeftOffset = pieceProps.nextLeftOffset;
        nextRightOffset = pieceProps.nextRightOffset;
        joinThreshold = pieceProps.joinThreshold;
        joinRotThreshold = pieceProps.joinRotThreshold;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "PuzzlePiece")
        {
            touchingPiece = true;
            touchedPieces.Append(collision.gameObject);
            if (closeLook)
            {
                if (joinable && collision.gameObject.GetComponent<PuzzlePiece>().joinable)
                {
                    float angleDiff = Vector3.Angle(collision.transform.eulerAngles, transform.eulerAngles);
                    Vector3 offsetCheck = collision.transform.position - transform.position;

                    if (hasNextAbove && collision.gameObject == nextAbove)
                    {
                        float offsetDiff = Vector3.Distance(offsetCheck, nextAboveOffset);
                        if (offsetDiff < joinThreshold && angleDiff < joinRotThreshold)
                        {
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
                            print($"{name} and {collision.gameObject.name} fit together!\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                        }
                        else
                        {
                            print($"{name} and {collision.gameObject.name} fit together, but not like this.\nDifference from initial offset:{offsetDiff}\nDifference from initial rotational offset: {angleDiff}");
                        }
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
