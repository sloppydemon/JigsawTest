using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class GeneratePiece : MonoBehaviour
{
    
    // Start is called before the first frame update
    public static void Build(GameObject go, List<Vector3> points, float extrusion, bool addCollider, Material frontMat, Material cardMat, bool flipNormals, Vector2 uvScale, Vector2 uvOffset, Vector3 pivot)
    {
        ProBuilderMesh m_Mesh = go.AddComponent<ProBuilderMesh>();
        points = points.Distinct().ToList();
        Debug.Log(m_Mesh.CreateShapeFromPolygon(points, extrusion, flipNormals));
        if (m_Mesh.vertexCount == 0)
        {
            print($"{go.name} didn't materialize.");
        }
        List<Face> newface = new List<Face>();
        if (flipNormals)
        {
            newface.Add(m_Mesh.faces[1]);
        }
        else
        {
            newface.Add(m_Mesh.faces[0]);
        }
        newface.Add(m_Mesh.faces[0]);
        IEnumerable<Face> newfaces = newface;
        m_Mesh.SetMaterial(m_Mesh.faces, cardMat);
        m_Mesh.SetSelectedFaces(newfaces);
        IEnumerable<Face> selected_faces = m_Mesh.GetSelectedFaces();
        m_Mesh.SetMaterial(selected_faces, frontMat);
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
        //m_Mesh.faces[5].submeshIndex = 0;

    }
}