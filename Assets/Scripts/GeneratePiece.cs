using ProBuilder.ExampleActions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class GeneratePiece : MonoBehaviour
{
    
    public static void Build(GameObject go, List<Vector3> points, float extrusion, bool addCollider, Material frontMat, Material cardMat, Vector2 uvScale, Vector2 uvOffset, int numberOfBezierRetries, float bezDetail, float bezierRetryIncrementUp, float bezierRetryIncrementDown, float sizeX, float sizeY, int numX, int numY)
    {
        ProBuilderMesh m_Mesh = go.AddComponent<ProBuilderMesh>();
        GameObject game = GameObject.FindGameObjectWithTag("GameController");
        MeshGen gc = game.GetComponent<MeshGen>();
        PuzzlePiece pieceProps = go.GetComponent<PuzzlePiece>();
        points = points.Distinct().ToList();
        m_Mesh.CreateShapeFromPolygon(points, extrusion, false);
        if (m_Mesh.vertexCount == 0)
        {
            int retry = 0;
            List<Vector3> newPoints = new List<Vector3>();
            gc.meshingFailed = true;
            while (retry < numberOfBezierRetries)
            {
                newPoints.Clear();
                retry++;
                float retryIncrementDown = bezDetail - bezierRetryIncrementUp * retry;
                float retryIncrementUp = bezDetail + bezierRetryIncrementDown * retry;
                print($"Meshing of {go.name} FAILED! Retry no.: {retry}. Retrying at bezier resolution {retryIncrementUp}...");
                newPoints = CollectPoints(newPoints, sizeX, sizeY, pieceProps.pieceX, pieceProps.pieceY, retryIncrementUp, numX, numY);
                newPoints.Distinct().ToList();
                m_Mesh.CreateShapeFromPolygon(newPoints, extrusion, false);
                if (m_Mesh.vertexCount > 0)
                {
                    print($"SUCCESS! At bezier resolution: {retryIncrementUp}. Retry no.:{retry}");
                    pieceProps.piecePoints.Clear();
                    pieceProps.piecePoints = newPoints;
                    gc.meshingFailed = true;
                    break;
                }
                else
                {
                    if (retryIncrementDown > 0)
                    {
                        retry++;
                        print($"Remeshing FAILED! At bezier resolution {retryIncrementUp}. Retry no.: {retry}. Retrying at bezier resolution {retryIncrementDown}...");
                        newPoints.Clear();
                        newPoints = CollectPoints(newPoints, sizeX, sizeY, pieceProps.pieceX, pieceProps.pieceY, retryIncrementUp, numX, numY);
                        newPoints.Distinct().ToList();
                        m_Mesh.CreateShapeFromPolygon(newPoints, extrusion, false);
                        if (m_Mesh.vertexCount > 0)
                        {
                            print($"SUCCESS! At bezier resolution: {retryIncrementDown}. Retry no.:{retry}");
                            pieceProps.piecePoints.Clear();
                            pieceProps.piecePoints = newPoints;
                            gc.meshingFailed = true;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            List<Face> newface = new List<Face>();
            newface.Add(m_Mesh.faces[0]);
            Debug.Log($"{go.name}: {m_Mesh.GetWindingOrder(m_Mesh.faces[0])}, normal of face 0:{m_Mesh.GetNormals()[0]}");
            if (m_Mesh.GetNormals()[0] == new Vector3(0,-1f,0))
            {
                foreach (var face in m_Mesh.faces)
                    face.Reverse();
            }
            IEnumerable <Face> newfaces = newface;
            m_Mesh.SetMaterial(m_Mesh.faces, cardMat);
            m_Mesh.SetMaterial(newface, frontMat);
            m_Mesh.ToMesh();
            m_Mesh.Refresh();
            if (go.name == "ImageObject")
            {
                m_Mesh.faces[0].uv = new AutoUnwrapSettings()
                {
                    anchor = AutoUnwrapSettings.Anchor.UpperLeft,
                    fill = AutoUnwrapSettings.Fill.Stretch,
                    scale = new Vector2(1, 1)
                };
            }
            else
            {
                newface[0].uv = new AutoUnwrapSettings()
                {
                    anchor = AutoUnwrapSettings.Anchor.LowerLeft,
                    fill = AutoUnwrapSettings.Fill.Tile,
                    scale = uvScale,
                    offset = uvOffset
                };
            }
            m_Mesh.Refresh(RefreshMask.UV);
            int[] pivotCtrl = new int[m_Mesh.vertexCount];
            for (int i = 0; i < m_Mesh.vertexCount; i++)
            {
                pivotCtrl.Append(i);
            }
            m_Mesh.CenterPivot(pivotCtrl);
            m_Mesh.ToMesh();
            m_Mesh.Refresh();
            if (addCollider)
            {
                m_Mesh.gameObject.AddComponent<MeshCollider>().enabled = true;
                m_Mesh.gameObject.GetComponent<MeshCollider>().convex = true;
                m_Mesh.gameObject.AddComponent<Rigidbody>();
            }
        }
    }

    public static List<Vector3> CollectPoints(List<Vector3> piecePts, float sizeX, float sizeY, int j, int i, float bezDetail, int numX, int numY)
    {
        if (i == 0)
        {
            if (j == 0)
            {
                piecePts = CalculatePiece.PiecePoints(true, true, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(true, true, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, true, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
        }
        else if (i == numY - 1)
        {
            if (j == 0)
            {
                piecePts = CalculatePiece.PiecePoints(true, false, false, true, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(true, false, true, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, false, false, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
        }
        else
        {
            if (j == 0)
            {
                piecePts = CalculatePiece.PiecePoints(false, false, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(false, false, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, false, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
            }
        }
        return piecePts;
    }

    public static PuzzlePiece SetNeighbourBools(PuzzlePiece pieceProps, int i, int j, int numX, int numY)
    {
        if (i == 0)
        {
            if (j == 0)
            {
                pieceProps.hasNextBelow = false;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = true;
            }
            else if (j == numX - 1)
            {
                pieceProps.hasNextBelow = false;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
            else
            {
                pieceProps.hasNextBelow = false;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = true;
            }
        }
        else if (i == numY - 1)
        {
            if (j == 0)
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = false;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = false;
            }
            else if (j == numX - 1)
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = false;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = true;
            }
            else
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = false;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
        }
        else
        {
            if (j == 0)
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = false;
            }
            else if (j == numX - 1)
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
            else
            {
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
        }
        return pieceProps;
    }
}
