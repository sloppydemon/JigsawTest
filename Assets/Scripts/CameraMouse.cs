using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraMouse : MonoBehaviour
{
    private Camera cam;
    public Vector3 centerRotation;
    public Vector3 maxTopRotation;
    public Vector3 maxBottomRotation;
    public Vector3 maxLeftRotation;
    public Vector3 maxRightRotation;
    public Vector3 initDolly;
    public Vector3 maxDolly;
    public Vector3 minDolly;
    private Vector3 newRot;
    private Quaternion quatRot;
    public Vector2 currentMousePosition;
    public Vector3 currentRot;
    public float dollyLvl;
    public float closerLerp;
    public float matchLerp;
    Vector3 posBeforeLerp;
    public float closerLerpSpeed;
    public float matchLerpSpeed;
    public bool closerLook;
    public bool matchingLook;
    public bool holding;

    void Start()
    {
        cam = Camera.main;
        dollyLvl = 0.8f;
        closerLerp = 0.0f;
        matchLerp = 0.0f;
        holding = false;
        posBeforeLerp = new Vector3 (0, 14.3f, -3f);
    }

    // Update is called once per frame
    void Update()
    {



        

        if (Input.GetMouseButton(2))
        {
            cam.fieldOfView -= (((currentMousePosition.y - 0.5f) *2f) * 5f);
            if (cam.fieldOfView < 10)
            {
                cam.fieldOfView = 10;
            }
            if (cam.fieldOfView > 90)
            {
                cam.fieldOfView = 90;
            }
        }
        else
        {
            if (!holding)
            {
                dollyLvl += Input.mouseScrollDelta.y * 0.1f;
                if (dollyLvl < 0)
                {
                    dollyLvl = 0;
                }
                else if (dollyLvl > 1)
                {
                    dollyLvl = 1;
                }
                posBeforeLerp = Vector3.Lerp(minDolly, maxDolly, dollyLvl);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            holding = true;
            if (Input.GetMouseButton(1))
            {
                StartCoroutine(GettingACloserLook());
                StopCoroutine(LookingForMatch());
                closerLook = true;
                matchingLook = false;
            }
            else
            {

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;
        }

            if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetMouseButton(0))
            {
                StartCoroutine (GettingACloserLook());
                StopCoroutine(LookingForMatch());
                closerLook = true;
                matchingLook = false;
            }
            else
            {
                StartCoroutine(LookingForMatch());
                StopCoroutine(GettingACloserLook());
                matchingLook = true;
                closerLook = false;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (closerLook)
            {
                StopCoroutine(GettingACloserLook());
                StartCoroutine(StoppingCloserLook());
                closerLook = false;
            }
            if (matchingLook)
            {
                StopCoroutine(LookingForMatch());
                StartCoroutine(StoppingMatching());
                matchingLook = false;
            }
            
            
        }

        currentRot = transform.rotation.eulerAngles;
        currentMousePosition = Vector2.Lerp(new Vector2(((Input.mousePosition.x / Screen.width) - 0.5f) * 2f, Input.mousePosition.y / Screen.height), new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height), Mathf.Max(closerLerp, matchLerp));
        currentRot = centerRotation + (currentMousePosition.x * new Vector3(0, 28.76f, 12.1f));
        newRot = currentRot + (currentMousePosition.y * new Vector3(-40f, 0, 0));
        quatRot.eulerAngles = Vector3.Lerp(newRot, new Vector3(90f, 0f, 0f), Mathf.Max(closerLerp, matchLerp));
        transform.rotation = quatRot;
        Vector3 posLerp = Vector3.Lerp(posBeforeLerp, new Vector3((-3.8f) + (7.8f * currentMousePosition.x), 10f, ( - 5.37f) + (7.74f * currentMousePosition.y)), matchLerp);
        transform.position = Vector3.Lerp(posLerp, new Vector3((-3.8f) + (7.8f * currentMousePosition.x), 9.5f, (-2.53f) + (4.9f * currentMousePosition.y)), closerLerp);
    }

    IEnumerator GettingACloserLook()
    {
        while (closerLerp < 1)
        {
            closerLerp += closerLerpSpeed;
            yield return null;
        }
        if (closerLerp > 1)
        {
            closerLerp = 1;
            yield return null;
        }
    }

    IEnumerator LookingForMatch()
    {
        while (matchLerp < 1)
        {
            matchLerp += matchLerpSpeed;
            yield return null;
        }
        if (matchLerp > 1)
        {
            matchLerp = 1;
            yield return null;
        }
    }

    IEnumerator StoppingCloserLook()
    {
        while (closerLerp > 0)
        {
            closerLerp -= closerLerpSpeed;
            yield return null;
        }
        if (closerLerp < 0)
        {
            closerLerp = 0;
            yield return null;
        }
    }

    IEnumerator StoppingMatching()
    {
        while (matchLerp > 0)
        {
            matchLerp -= matchLerpSpeed;
            yield return null;
        }
        if (matchLerp < 0)
        {
            matchLerp = 0;
            yield return null;
        }
    }
}
