using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_Viz : MonoBehaviour
{
    public List<Vector3> ControlPoints;
    public GameObject PointPrefab;
    LineRenderer[] mLineRenderers;
    LineRenderer lineRenderer;
    LineRenderer curveRenderer;
    public List<GameObject> mPointGameObjects;
    public Color BezierCurveColour;
    public Color LineColour;
    public float LineWidth;
    public Material LineMaterial;
    List<Vector3> pts;


    private LineRenderer CreateLine()
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(LineMaterial);
        lr.startColor = LineColour;
        lr.endColor = LineColour;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;
        return lr;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector2 rayPos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y
                    );
                InsertNewControlPoint(rayPos);
            }
        }
    }

    void InsertNewControlPoint(Vector2 p)
    {
        if (mPointGameObjects.Count >= 16)
        {
            Debug.Log("Cannot create any new control points. Max number is 16");
            return;
        }
        GameObject ob = Instantiate(PointPrefab, p, Quaternion.identity);
        ob.name = "ControlPoint_" + mPointGameObjects.Count.ToString();
        mPointGameObjects.Add(ob);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create the two LineRenderers.
        mLineRenderers = new LineRenderer[2];
        mLineRenderers[0] = CreateLine();
        mLineRenderers[1] = CreateLine();
        lineRenderer = mLineRenderers[0];
        curveRenderer = mLineRenderers[1];

        // set a name to the game objects for the LineRenderers
        // to distingush them.
        mLineRenderers[0].gameObject.name = "LineRenderer_obj_0";
        mLineRenderers[1].gameObject.name = "LineRenderer_obj_1";

        //Create the instances of PointPrefab
        //to show the control points.
        for (int i = 0; i < ControlPoints.Count; ++i)
        {
            GameObject ob = Instantiate(PointPrefab,
              ControlPoints[i],
              Quaternion.identity);
            ob.name = "ControlPoint_" + i.ToString();
            mPointGameObjects.Add(ob);
        }

    }

    // Update is called once per frame
    void Update()
    {


        pts = new List<Vector3>();

        for (int k = 0; k < mPointGameObjects.Count; ++k)
        {
            pts.Add(mPointGameObjects[k].transform.position);
        }

        // create a line renderer for showing the straight
        //lines between control points.
        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; ++i)
        {
            lineRenderer.SetPosition(i, pts[i]);
        }

        // we take the control points from the list of points in the scene.
        // recalculate points every frame.
        List<Vector3> curve = BezierCurve.PointList3(pts, 0.01f);
        curveRenderer.startColor = BezierCurveColour;
        curveRenderer.endColor = BezierCurveColour;
        curveRenderer.positionCount = curve.Count;
        for (int i = 0; i < curve.Count; ++i)
        {
            curveRenderer.SetPosition(i, curve[i]);
        }
    }




}
