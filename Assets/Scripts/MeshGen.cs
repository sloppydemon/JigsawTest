using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using TMPro;
using ProBuilder.Examples;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;

public class MeshGen : MonoBehaviour
{
    List<Vector3> pts;
    GameObject imgObj;
    public Texture2D img;
    public Slider sizeSlider;
    public TextMeshProUGUI sizeText;
    public float sizeInCms;
    public float sizeFactor;
    public Material cardboardMaterial;
    public Material frontMaterial;
    static float SizeFactor(float y, float x, float size)
    {
        float max = Mathf.Max(y, x);
        return size / max;
    }
    float sizeX;
    float sizeY;

    // Start is called before the first frame update
    void Start()
    {
        pts = new List<Vector3>();
        sizeInCms = Mathf.Max(img.height, img.width) / 10f;
        sizeSlider.maxValue = 200f;
        sizeSlider.minValue = 5f;
        sizeSlider.value = sizeInCms;
        sizeX = img.width * SizeFactor(img.width, img.height, sizeInCms);
        sizeY = img.height * SizeFactor(img.width, img.height, sizeInCms);
        sizeFactor = SizeFactor(img.width, img.height, sizeInCms);
        sizeText.text = $"{sizeX.ToString("0.0")}x{sizeY.ToString("0.0")} cm";
        sizeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        imgObj = new GameObject();
        Vector3[] imgObjVectors = new Vector3[]
            {
                new Vector3(-sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, sizeY*0.05f),
                new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f)
            };
        GeneratePiece.Build(imgObj, imgObjVectors, 0.005f, false, frontMaterial, cardboardMaterial, "ImageObject");
        //imgObj = ProBuilderMesh.Create(
        //    new Vector3[]
        //    {
        //        new Vector3(-sizeX*0.005f, 0, -sizeY*0.005f),
        //        new Vector3(sizeX*0.005f, 0, -sizeY*0.005f),
        //        new Vector3(-sizeX*0.005f, 0, sizeY*0.005f),
        //        new Vector3(sizeX * 0.005f, 0, sizeY * 0.005f)
        //    },
        //    new Face[] { new Face(new int[] {0, 1, 2, 1, 3, 2 })}
        //    );
        //imgObj.Extrude(imgObj.faces, ExtrudeMethod.IndividualFaces, 1);
        //imgObj.SetMaterial(imgObj.faces, cardboardMaterial);
        //IEnumerable<Face> selected_faces = imgObj.GetSelectedFaces();
        //imgObj.SetMaterial(selected_faces, frontMaterial);
        //imgObj.RefreshUV(selected_faces);
        //imgObj.Refresh();
    }

    public void ValueChangeCheck()
    {
        sizeInCms = sizeSlider.value;
        sizeX = img.width * SizeFactor(img.width, img.height, sizeInCms);
        sizeY = img.height * SizeFactor(img.width, img.height, sizeInCms);
        sizeFactor = SizeFactor(img.width, img.height, sizeInCms);
        sizeText.text = $"{sizeX.ToString("0.0")}x{sizeY.ToString("0.0")} cm";

        Destroy(imgObj);
        imgObj = new GameObject();
        Vector3[] imgObjVectors = new Vector3[]
            {
                new Vector3(-sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, sizeY*0.05f),
                new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f)
            };
        GeneratePiece.Build(imgObj, imgObjVectors, 0.005f, false, frontMaterial, cardboardMaterial, "ImageObject");
        //imgObj = ProBuilderMesh.Create(
        //    new Vector3[]
        //    {
        //        new Vector3(-sizeX*0.005f, 0, -sizeY*0.005f),
        //        new Vector3(sizeX*0.005f, 0, -sizeY*0.005f),
        //        new Vector3(-sizeX*0.005f, 0, sizeY*0.005f),
        //        new Vector3(sizeX * 0.005f, 0, sizeY * 0.005f)
        //    },
        //    new Face[] { new Face(new int[] { 0, 1, 2, 1, 3, 2 }) }
        //    );
        //imgObj.Extrude(imgObj.faces, ExtrudeMethod.IndividualFaces, 1);
        //imgObj.SetMaterial(imgObj.faces, cardboardMaterial);
        //IEnumerable<Face> selected_faces = imgObj.GetSelectedFaces();
        //imgObj.SetMaterial(selected_faces, frontMaterial);
        //imgObj.RefreshUV(selected_faces);
        //imgObj.Refresh();

    }

    void Update()
    {

    }
}
