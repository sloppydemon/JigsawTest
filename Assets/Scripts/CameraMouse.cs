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
    void Start()
    {
        cam = Camera.main;
        dollyLvl = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        currentRot = transform.rotation.eulerAngles;
        currentMousePosition = new Vector2(((Input.mousePosition.x / Screen.width) - 0.5f) * 2f, Input.mousePosition.y / Screen.height);
        currentRot = centerRotation + (currentMousePosition.x * new Vector3(0, 28.76f, 12.1f));
        newRot = currentRot + (currentMousePosition.y * new Vector3(-40f, 0, 0));
        quatRot.eulerAngles = newRot;
        transform.rotation = quatRot;
        if (Input.GetMouseButton(0))
        {

        }
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
            if (!Input.GetMouseButton(0))
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
                transform.position = Vector3.Lerp(minDolly, maxDolly, dollyLvl);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetMouseButton(0))
            {

            }
            else
            {

            }
        }
    }
}
