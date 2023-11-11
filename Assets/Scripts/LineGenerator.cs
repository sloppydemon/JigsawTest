using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    // template bezier curve control points
    public static readonly List<Vector3> PointsDown = new List<Vector3>()
    {
        new Vector3(0, 0, 0),
        new Vector3(0.35f, 0, 0.15f),
        new Vector3(0.47f, 0, 0.13f),
        new Vector3(0.45f, 0, 0.05f),
        new Vector3(0.48f, 0, 0),
        new Vector3(0.25f, 0, -0.05f),
        new Vector3(0.15f, 0, -0.18f),
        new Vector3(0.36f, 0, -0.2f),
        new Vector3(0.64f, 0, -0.2f),
        new Vector3(0.85f, 0, -0.18f),
        new Vector3(0.75f, 0, -0.05f),
        new Vector3(0.52f, 0, 0),
        new Vector3(0.55f, 0, 0.05f),
        new Vector3(0.53f, 0, 0.13f),
        new Vector3(0.65f, 0, 0.15f),
        new Vector3(1f, 0, 0)
    };

    public static List<Vector3> PointsUp = PointsDown.Select(i => new Vector3(i.x, i.y, -i.z)).ToList();
    public static List<Vector3> PointsLeft = PointsDown.Select(i => new Vector3(i.z, i.y, i.x)).ToList();
    public static List<Vector3> PointsRight = PointsLeft.Select(i => new Vector3(-i.x, i.y, i.z)).ToList();

    public List<Vector3> copyPointsDown;
    public List<Vector3> copyPointsUp;
    public List<Vector3> copyPointsLeft;
    public List<Vector3> copyPointsRight;

    public static List<Vector3> GenerateLine(bool vertical, float sizeX, float sizeY, int numberX, int numberY, int lineNum, float eccentricity)
    {
        List<Vector3> points = new List<Vector3>();
        float factorX = sizeX / numberX;
        float factorY = sizeY / numberY;
        float addendX = factorX * lineNum;
        float addendY = factorY * lineNum;

        if (vertical)
        {
            for (int i = 0; i < numberY; i++)
            {
                List<Vector3> ptsToAdd = new List<Vector3>();
                float toss = Random.Range(0.0f, 1.0f);
                if (toss < 0.5f)
                {
                    ptsToAdd.AddRange(PointsLeft);
                }
                else
                {
                    ptsToAdd.AddRange(PointsRight);
                }

                for (int j = 0; j < ptsToAdd.Count; j++)
                {
                    points.Add(new Vector3( (ptsToAdd[j].x*factorX)+addendX, ptsToAdd[j].y, (ptsToAdd[j].z * factorY) + (factorY * i)));
                }
            }
        }
        else
        {
            for (int i = 0; i < numberX; i++)
            {
                List<Vector3> ptsToAdd = new List<Vector3>();
                float toss = Random.Range(0.0f, 1.0f);
                if (toss < 0.5f)
                {
                    ptsToAdd.AddRange(PointsDown);
                }
                else
                {
                    ptsToAdd.AddRange(PointsUp);
                }

                for (int j = 0; j < ptsToAdd.Count; j++)
                {
                    points.Add(new Vector3((ptsToAdd[j].x * factorX) + (factorX * i), ptsToAdd[j].y, (ptsToAdd[j].z * factorY) + addendY));
                }
            }
        }
        return points;
    }

    // The template Bezier curve.
    //public static List<Vector2> BezCurve = BezierCurve.PointList2(ControlPoints, 0.001f);

    // Start is called before the first frame update
    void Start()
    {
        copyPointsDown = PointsDown;
        copyPointsUp = PointsUp;
        copyPointsLeft = PointsLeft;
        copyPointsRight = PointsRight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
