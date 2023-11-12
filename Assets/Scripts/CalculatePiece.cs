using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatePiece : MonoBehaviour
{
    public static List<Vector3> PiecePoints(bool corner, bool edgeB, bool edgeR, bool edgeT, bool edgeL, float sizeX, float sizeY, int pieceX, int pieceY, int numCtrlPts, float bezierDetail)
    {
        string findLeft = new string($"Y{pieceX}");
        string findRight = new string($"Y{pieceX + 1}");
        string findAbove = new string($"X{pieceY + 1}");
        string findBelow = new string($"X{pieceY}");
        List<Vector3> bezPts = new List<Vector3>();
        List<Vector3> incCtrlPts;
        List<Vector3> incBezPts;
        if (!edgeB)
        {
            GameObject GOLineBelow = GameObject.Find(findBelow);
            LinePreview lineBelow = GOLineBelow.GetComponent<LinePreview>();
            incCtrlPts = new List<Vector3>();
            incBezPts = new List<Vector3>();
            for (int i = 0 + (pieceX * numCtrlPts); i < numCtrlPts + (pieceX * numCtrlPts); i++)
            {
                incCtrlPts.Add(new Vector3(Mathf.Round(lineBelow.pts[i].x * 10000)/10000, lineBelow.pts[i].y, Mathf.Round(lineBelow.pts[i].z * 10000) / 10000));
            }
            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            bezPts.AddRange(incBezPts);

            if (corner)
            {
                if (edgeR)
                {
                    bezPts.Add(new Vector3(sizeX * 0.1f, 0, sizeY * 0.1f));
                }
            }
        }
        if (!edgeR)
        {
            GameObject GOLineRight = GameObject.Find(findRight);
            LinePreview lineRight = GOLineRight.GetComponent<LinePreview>();
            incCtrlPts = new List<Vector3>();
            incBezPts = new List<Vector3>();
            for (int i = 0 + (pieceY * numCtrlPts); i < numCtrlPts + (pieceY * numCtrlPts); i++)
            {
                incCtrlPts.Add(new Vector3(Mathf.Round(lineRight.pts[i].x * 1000) / 1000, lineRight.pts[i].y, Mathf.Round(lineRight.pts[i].z * 1000) / 1000));
            }

            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            bezPts.AddRange(incBezPts);

            if (corner)
            {
                if (edgeT)
                {
                    bezPts.Add(new Vector3(0, 0, sizeY*0.1f));
                }
            }
        }
        if (!edgeT)
        {
            GameObject GOLineAbove = GameObject.Find(findAbove);
            LinePreview lineAbove = GOLineAbove.GetComponent<LinePreview>();
            incCtrlPts = new List<Vector3>();
            incBezPts = new List<Vector3>();
            for (int i = 0 + (pieceX * numCtrlPts); i < numCtrlPts + (pieceX * numCtrlPts); i++)
            {
                incCtrlPts.Add(new Vector3(Mathf.Round(lineAbove.pts[i].x * 1000) / 1000, lineAbove.pts[i].y, Mathf.Round(lineAbove.pts[i].z * 1000) / 1000));
            }
            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            incBezPts.Reverse();
            bezPts.AddRange(incBezPts);
            
            if (corner)
            {
                if (edgeL)
                {
                    bezPts.Add(new Vector3(0, 0, 0));
                }
            }
        }
        if (!edgeL)
        {
            GameObject GOLineLeft = GameObject.Find(findLeft);
            LinePreview lineLeft = GOLineLeft.GetComponent<LinePreview>();
            incCtrlPts = new List<Vector3>();
            incBezPts = new List<Vector3>();
            for (int i = 0 + (pieceY * numCtrlPts); i < numCtrlPts + (pieceY * numCtrlPts); i++)
            {
                incCtrlPts.Add(new Vector3(Mathf.Round(lineLeft.pts[i].x * 1000) / 1000, lineLeft.pts[i].y, Mathf.Round(lineLeft.pts[i].z * 1000) / 1000));
            }
            incBezPts = BezierCurve.PointList3(incCtrlPts, bezierDetail);
            incBezPts.Reverse();
            bezPts.AddRange(incBezPts);

            if (corner)
            {
                if (edgeB)
                {
                    bezPts.Add(new Vector3(sizeX * 0.1f, 0, 0));
                }
            }
        }
        return bezPts;
    }
}
