using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePreview : MonoBehaviour
{
    public List<Vector3> pts;
    public LineRenderer lr;
    public float bezierDetail;
    public int numCtrlPts;
    public int numForThisAxis;
    public int rowBefore;
    public int rowAfter;
    public List<Vector3> bezPts;
    public List<Vector3> incCtrlPts;
    public List<Vector3> incBezPts;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        bezPts = new List<Vector3> ();
        for (int i = 0; i < numForThisAxis; i++)
        {
            incCtrlPts = new List<Vector3> ();
            for (int j = 0; j < numCtrlPts; j++)
            {
                incCtrlPts.Add(pts[i * numCtrlPts + j]);
            }
            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            bezPts.AddRange(incBezPts);
        }
        lr.positionCount = bezPts.Count;
        for (int i = 0;i < bezPts.Count; i++)
        {
            lr.SetPosition(i, bezPts[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        lr = GetComponent<LineRenderer>();
        bezPts = new List<Vector3>();
        for (int i = 0; i < numForThisAxis; i++)
        {
            incCtrlPts = new List<Vector3>();
            for (int j = 0; j < numCtrlPts; j++)
            {
                incCtrlPts.Add(pts[i * numCtrlPts + j]);
            }
            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            bezPts.AddRange(incBezPts);
        }
        lr.positionCount = bezPts.Count;
        for (int i = 0; i < bezPts.Count; i++)
        {
            lr.SetPosition(i, bezPts[i]);
        }

        lr.material.mainTextureOffset += new Vector2(0.03f, 0); 
    }
}
