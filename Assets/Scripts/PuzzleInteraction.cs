using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInteraction : MonoBehaviour
{
    private Camera cam;


    void Start()
    {
        cam = Camera.main; 
    }

    private void OnMouseDrag()
    {
        float distanceToScreen = cam.WorldToScreenPoint(transform.position).z;

        transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
    }
}
