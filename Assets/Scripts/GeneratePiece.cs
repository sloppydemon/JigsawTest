using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class GeneratePiece : MonoBehaviour
{
    
    // Start is called before the first frame update
    public static void Build(GameObject go, Vector3[] points, float extrusion, bool addCollider, Material frontMat, Material cardMat)
    {
        ProBuilderMesh m_Mesh = go.AddComponent<ProBuilderMesh>();

        m_Mesh.CreateShapeFromPolygon(points, extrusion, false);
        List<Face> newface = new List<Face>();
        newface.Add(m_Mesh.faces[0]);
        IEnumerable<Face> newfaces = newface;
        m_Mesh.SetMaterial(m_Mesh.faces, cardMat);
        m_Mesh.SetSelectedFaces(newfaces);
        IEnumerable<Face> selected_faces = m_Mesh.GetSelectedFaces();
        m_Mesh.SetMaterial(selected_faces, frontMat);
        m_Mesh.SetMaterial(newface, frontMat);
        m_Mesh.ToMesh();
        m_Mesh.Refresh();
        m_Mesh.faces[0].uv = new AutoUnwrapSettings()
        {
            anchor = AutoUnwrapSettings.Anchor.UpperLeft,
            fill = AutoUnwrapSettings.Fill.Stretch,
            scale = new Vector2(1, 1)
        };
        m_Mesh.Refresh(RefreshMask.UV);
        if (addCollider)
        {
            m_Mesh.gameObject.AddComponent<MeshCollider>().enabled = true;
            m_Mesh.gameObject.GetComponent<MeshCollider>().convex = true;
        }
        //m_Mesh.faces[5].submeshIndex = 0;

    }
}
