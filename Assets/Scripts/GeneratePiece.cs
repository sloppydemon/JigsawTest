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
    
    public static void Build(GameObject go, List<Vector3> points, float extrusion, bool addCollider, Material frontMat, Material cardMat, bool flipNormals, Vector2 uvScale, Vector2 uvOffset)
    {
        ProBuilderMesh m_Mesh = go.AddComponent<ProBuilderMesh>();
        points = points.Distinct().ToList();
        m_Mesh.CreateShapeFromPolygon(points, extrusion*0.1f, flipNormals);
        if (m_Mesh.vertexCount == 0)
        {
            print($"{go.name} didn't materialize.");
        }
        else
        {
            List<Face> newface = new List<Face>();
            if (flipNormals)
            {
                newface.Add(m_Mesh.faces[1]);
            }
            else
            {
                newface.Add(m_Mesh.faces[0]);
            }
            IEnumerable<Face> newfaces = newface;
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

    public static GameObject CollectPoints(float sizeX, float sizeY, int i, int j, float bezDetail, int numX, int numY)
    {
        GameObject piece = new GameObject();
        piece.transform.position = new Vector3(-sizeX * 0.05f, 8.155f, -sizeY * 0.05f);
        piece.name = $"PuzzlePiece{j}_{i}";
        PuzzlePiece pieceProps = piece.AddComponent<PuzzlePiece>();
        pieceProps.pieceX = j;
        pieceProps.pieceY = i;
        pieceProps.flipNormals = true;
        List<Vector3> piecePts = new List<Vector3>();
        if (i == 0)
        {
            if (j == 0)
            {
                pieceProps.flipNormals = false;
                piecePts = CalculatePiece.PiecePoints(true, true, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = false;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = true;
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(true, true, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = false;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, true, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
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
                pieceProps.flipNormals = false;
                piecePts = CalculatePiece.PiecePoints(true, false, false, true, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = false;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = false;
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(true, false, true, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = false;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = true;
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, false, false, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
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
                pieceProps.flipNormals = false;
                piecePts = CalculatePiece.PiecePoints(false, false, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = false;
                pieceProps.cornerPiece = false;
            }
            else if (j == numX - 1)
            {
                piecePts = CalculatePiece.PiecePoints(false, false, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = false;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
            else
            {
                piecePts = CalculatePiece.PiecePoints(false, false, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                pieceProps.hasNextBelow = true;
                pieceProps.hasNextRight = true;
                pieceProps.hasNextAbove = true;
                pieceProps.hasNextLeft = true;
                pieceProps.cornerPiece = false;
            }
        }
        pieceProps.piecePoints = piecePts;
        return piece;
    }
}
