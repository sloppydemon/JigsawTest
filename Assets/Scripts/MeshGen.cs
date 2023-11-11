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
using UnityEditor;
using Unity.VisualScripting;

public class MeshGen : MonoBehaviour
{
    List<Vector3> pts;
    GameObject imgObj;
    public GameObject table;
    public Texture2D img;
    public Slider sizeSlider;
    public Slider numberSlider;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI sizeSubText;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI numberSubText;
    public TMP_InputField inputNumber;
    public float sizeInCms;
    public float sizeFactor;
    public Material cardboardMaterial;
    public Material frontMaterial;
    public int ratioX;
    public int ratioY;
    public int numX;
    public int numY;
    public bool buildable;
    public int gcd;
    public Camera cam;
    public float fieldOfViewMinimum;
    public float fieldOfViewMaxAddend;
    static float SizeFactor(float y, float x, float size)
    {
        float max = Mathf.Max(y, x);
        return size / max;
    }
    float sizeX;
    float sizeY;
    public int numberOfPieces;

    
    void OnDrawGizmos()
    {
        int gcd = PuzzlePieceCalc.GCD(img.width, img.height);
        int ratioX = PuzzlePieceCalc.DimensionX(img.width, img.height);
        int ratioY = PuzzlePieceCalc.DimensionY(img.width, img.height);

        int numX = PuzzlePieceCalc.NumPiecesX(img.width, img.height, numberOfPieces);
        int numY = PuzzlePieceCalc.NumPiecesY(img.width, img.height, numberOfPieces);
        int diff = numberOfPieces - (numX * numY);
        if (diff != 0)
        {
            numX = PuzzlePieceCalc.FactorReduction(numX, numY, numberOfPieces, true);
            numY = PuzzlePieceCalc.FactorReduction(numY, numX, numberOfPieces, false);
        }
        int newdiff = numberOfPieces - (numX * numY);

        if (diff == 0)
        {
            Handles.color = Color.green;
            Handles.Label(transform.position, $"Image Aspect Ratio:{ratioX}:{ratioY}, GCD:{gcd}, X:{numX} Y:{numY}");
        }
        else
        {
            if (newdiff == 0)
            {
                Handles.color = Color.green;
                Handles.Label(transform.position, $"Image Aspect Ratio:{ratioX}:{ratioY}, GCD:{gcd}, X:{numX} Y:{numY}, was off by {diff}, corrected difference:{newdiff}");
            }
            else
            {
                if (numberOfPieces < Mathf.Max(ratioX, ratioY))
                {
                    Handles.color = Color.red;
                    Handles.Label(transform.position, $"Image Aspect Ratio:{ratioX}:{ratioY}, GCD:{gcd}, X:{numX} Y:{numY}, TOO FEW PIECES! Difference:{diff}");
                }
                else
                {
                    Handles.color = Color.yellow;
                    Handles.Label(transform.position, $"Image Aspect Ratio:{ratioX}:{ratioY}, GCD:{gcd}, X:{numX} Y:{numY}, BAD CALCULATION! Difference:{diff}");
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pts = new List<Vector3>();
        sizeInCms = Mathf.Max(img.height, img.width) / 10f;
        sizeSlider.maxValue = 200f;
        sizeSlider.minValue = 5f;
        sizeSlider.value = sizeInCms;
        numberSlider.maxValue = 3000;
        numberSlider.minValue = 20;
        sizeX = img.width * SizeFactor(img.width, img.height, sizeInCms);
        sizeY = img.height * SizeFactor(img.width, img.height, sizeInCms);
        sizeFactor = SizeFactor(img.width, img.height, sizeInCms);
        sizeText.text = $"{sizeX.ToString("0.0")}x{sizeY.ToString("0.0")} cm";
        sizeSlider.onValueChanged.AddListener(delegate { sizeSliderChange(); });
        numberSlider.onValueChanged.AddListener(delegate { numberSliderChange(); });
        inputNumber.onEndEdit.AddListener(delegate { numberInputChanged(inputNumber.text); });
        cam.fieldOfView = fieldOfViewMinimum + (sizeSlider.normalizedValue * fieldOfViewMaxAddend);
        
        gcd = PuzzlePieceCalc.GCD(img.width, img.height);
        ratioX = PuzzlePieceCalc.DimensionX(img.width, img.height);
        ratioY = PuzzlePieceCalc.DimensionY(img.width, img.height);
        sizeSubText.text = $"Aspect Ratio: {ratioX}:{ratioY}";
        numberOfPieces = ratioX * ratioY * 25;
        numberSlider.value = numberOfPieces;

        numberSliderChange();
        

        imgObj = new GameObject();
        imgObj.transform.position = new Vector3(0, 8.155f, 0);
        imgObj.name = "ImageObject";
        Vector3[] imgObjVectors = new Vector3[]
            {
                new Vector3(-sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, sizeY*0.05f),
                new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f)
            };
        GeneratePiece.Build(imgObj, imgObjVectors, 0.005f, false, frontMaterial, cardboardMaterial);
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

    public void sizeSliderChange()
    {
        sizeInCms = sizeSlider.value;
        sizeX = img.width * SizeFactor(img.width, img.height, sizeInCms);
        sizeY = img.height * SizeFactor(img.width, img.height, sizeInCms);
        sizeFactor = SizeFactor(img.width, img.height, sizeInCms);
        sizeText.text = $"{sizeX.ToString("0.0")}x{sizeY.ToString("0.0")} cm";

        Destroy(imgObj);
        imgObj = new GameObject();
        imgObj.transform.position = new Vector3(0, 8.155f, 0);
        imgObj.name = "ImageObject";
        Vector3[] imgObjVectors = new Vector3[]
            {
                new Vector3(-sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, -sizeY*0.05f),
                new Vector3(sizeX*0.05f, 0, sizeY*0.05f),
                new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f)
            };
        GeneratePiece.Build(imgObj, imgObjVectors, 0.005f, false, frontMaterial, cardboardMaterial);
        if (sizeInCms > 95f)
        {
            table.transform.localScale = new Vector3(sizeInCms*0.0102f, 1f, sizeInCms * 0.0102f);
        }
        cam.fieldOfView = fieldOfViewMinimum + (sizeSlider.normalizedValue * fieldOfViewMaxAddend);
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

    public void numberInputChanged(string input)
    {
        int inputInt;
        try
        {
            inputInt = int.Parse(input);
        }
        catch
        {
            if (input == "")
            {
                return;
            }
            else
            {
                numberSubText.text = $"ERROR! '{input}' is not an integer.";
                return;
            }
        }

        numberSlider.value = inputInt;
        inputNumber.text = "";
        
    }

    public void numberSliderChange()
    {
        numberOfPieces = (int)numberSlider.value;
        numX = PuzzlePieceCalc.NumPiecesX(img.width, img.height, numberOfPieces);
        numY = PuzzlePieceCalc.NumPiecesY(img.width, img.height, numberOfPieces);
        int diff = numberOfPieces - (numX * numY);
        if (diff != 0)
        {
            numX = PuzzlePieceCalc.FactorReduction(numX, numY, numberOfPieces, true);
            numY = PuzzlePieceCalc.FactorReduction(numY, numX, numberOfPieces, false);
        }
        int newdiff = numberOfPieces - (numX * numY);
        float indSizeX = sizeX / numX;
        float indSizeY = sizeY / numY;

        if (diff == 0)
        {
            numberText.text = $"{numberOfPieces} pcs.";
            numberSubText.text = $"({numX}x{numY} pcs., approx. {indSizeX.ToString("0.0")}x{indSizeY.ToString("0.0")} cm apiece)";
            numberText.color = Color.green ;
            numberSubText.color = Color.green;
            buildable = true;
        }
        else
        {
            if (newdiff == 0)
            {
                numberText.text = $"{numberOfPieces} pcs.";
                numberSubText.text = $"({numX}x{numY} pcs., approx. {indSizeX.ToString("0.0")}x{indSizeY.ToString("0.0")} cm apiece)";
                numberText.color = Color.green;
                numberSubText.color = Color.green;
                buildable = true;
            }
            else
            {
                if (numberOfPieces < Mathf.Max(ratioX, ratioY))
                {
                    numberText.text = $"{numberOfPieces} pcs.";
                    numberSubText.text = $"(NOT BUILDABLE! Try a greater number of pieces.)";
                    numberText.color = Color.yellow;
                    numberSubText.color = Color.yellow;
                    buildable = false;
                }
                else
                {
                    numberText.text = $"{numberOfPieces} pcs.";
                    numberSubText.text = $"(NOT BUILDABLE! Multiplication awry.)";
                    numberText.color = Color.red;
                    numberSubText.color = Color.red;
                    buildable = false;
                }
            }
        }
    }

    void Update()
    {

    }
}
