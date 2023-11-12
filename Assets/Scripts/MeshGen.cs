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
using UnityEngine.ProBuilder.Shapes;

public class MeshGen : MonoBehaviour
{
    List<Vector3> pts;
    GameObject imgObj;
    public Button generateButton;
    public GameObject LinePrefab;
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
    public float bezDetail;
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
    public float pieceThickness;
    static float SizeFactor(float y, float x, float size)
    {
        float max = Mathf.Max(y, x);
        return size / max;
    }
    float sizeX;
    float sizeY;
    public int numberOfPieces;
    public List<GameObject> GOsToDestroy;
    public List<GameObject> puzzlePieces;

    
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
        GOsToDestroy = new List<GameObject>();
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
        sizeSlider.onValueChanged.AddListener(delegate { SizeSliderChange(); });
        numberSlider.onValueChanged.AddListener(delegate { NumberSliderChange(); });
        inputNumber.onEndEdit.AddListener(delegate { NumberInputChanged(inputNumber.text); });
        generateButton.onClick.AddListener(delegate { GenerateClicked(); });
        cam.fieldOfView = fieldOfViewMinimum + (sizeSlider.normalizedValue * fieldOfViewMaxAddend);
        
        gcd = PuzzlePieceCalc.GCD(img.width, img.height);
        ratioX = PuzzlePieceCalc.DimensionX(img.width, img.height);
        ratioY = PuzzlePieceCalc.DimensionY(img.width, img.height);
        sizeSubText.text = $"Aspect Ratio: {ratioX}:{ratioY}";
        numberOfPieces = ratioX * ratioY * 25;
        numberSlider.value = numberOfPieces;

        NumberSliderChange();
        

        imgObj = new GameObject();
        imgObj.transform.position = new Vector3(0, 8.155f, 0);
        imgObj.name = "ImageObject";
        List<Vector3> imgObjVectors = new List<Vector3>();
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f));
        GeneratePiece.Build(imgObj, imgObjVectors, pieceThickness, false, frontMaterial, cardboardMaterial, false, new Vector2(), new Vector2());

        if (buildable)
        {
            //generateButton.GetComponent <Text>().color = Color.black;
            generateButton.interactable = true;
            GenerateLines();
        }
        else
        {
            for (int i = 0; i < GOsToDestroy.Count; i++)
            {
                Destroy(GOsToDestroy[i]);
            }
            //generateButton.GetComponent <Text>().color = Color.grey;
            generateButton.interactable = false;
        }
    }

    public void SizeSliderChange()
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
        List<Vector3> imgObjVectors = new List<Vector3>();
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f));
        GeneratePiece.Build(imgObj, imgObjVectors, pieceThickness, false, frontMaterial, cardboardMaterial, false, new Vector2(), new Vector2());
        if (sizeInCms > 95f)
        {
            table.transform.localScale = new Vector3(sizeInCms*0.0102f, 1f, sizeInCms * 0.0102f);
        }
        cam.fieldOfView = fieldOfViewMinimum + (sizeSlider.normalizedValue * fieldOfViewMaxAddend);

        if (buildable)
        {
            //generateButton.GetComponent<Text>().color = Color.black;
            generateButton.interactable = true;
            GenerateLines();
        }
        else
        {
            for (int i = 0; i < GOsToDestroy.Count; i++)
            {
                Destroy(GOsToDestroy[i]);
            }
            //generateButton.GetComponent<Text>().color = Color.grey;
            generateButton.interactable = false;
        }
    }

    public void NumberInputChanged(string input)
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

    public void NumberSliderChange()
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

        if (buildable)
        {
            //generateButton.GetComponent<Text>().color = Color.black;
            generateButton.interactable = true;
            GenerateLines();
        }
        else
        {
            for (int i = 0; i < GOsToDestroy.Count; i++)
            {
                Destroy(GOsToDestroy[i]);
            }
            //generateButton.GetComponent<Text>().color = Color.grey;
            generateButton.interactable = false;
        }
    }

    public void GenerateLines()
    {
        for (int i = 0; i < GOsToDestroy.Count; i++)
        {
            Destroy(GOsToDestroy[i]);
        }

        // horizontal lines
        for (int i = 1; i < numY;  i++)
        {
            List<Vector3> linePts = new List<Vector3>();
            linePts = LineGenerator.GenerateLine(false, sizeX*0.1f, sizeY*0.1f, numX, numY, i, 0f);
            GameObject lPF = Instantiate(LinePrefab, transform.position + new Vector3(-sizeX * 0.05f, 0.01f, -sizeY * 0.05f), Quaternion.identity);
            lPF.name = $"X{i}";
            LinePreview lPV = lPF.GetComponent<LinePreview>();
            lPV.bezierDetail = bezDetail;
            lPV.numCtrlPts = LineGenerator.PointsDown.Count;
            lPV.numForThisAxis = numX;
            lPV.pts = linePts;
            GOsToDestroy.Add(lPF);
        }

        // vertical lines
        for (int i = 1; i < numX; i++)
        {
            List<Vector3> linePts = new List<Vector3>();
            linePts = LineGenerator.GenerateLine(true, sizeX * 0.1f, sizeY * 0.1f, numX, numY, i, 0f);
            GameObject lPF = Instantiate(LinePrefab, transform.position + new Vector3(-sizeX*0.05f, 0.01f, -sizeY * 0.05f), Quaternion.identity);
            lPF.name = $"Y{i}";
            LinePreview lPV = lPF.GetComponent<LinePreview>();
            lPV.bezierDetail = bezDetail;
            lPV.numCtrlPts = LineGenerator.PointsDown.Count;
            lPV.numForThisAxis = numY;
            lPV.pts = linePts;
            GOsToDestroy.Add(lPF);
        }
    }

    public void GenerateClicked()
    {
        if (buildable)
        {
            puzzlePieces = new List<GameObject> ();
            for (int i = 0; i < numY; i++)
            {
                for (int j = 0; j < numX; j++)
                {
                    GameObject piece = new GameObject();
                    piece.transform.position = new Vector3(-sizeX * 0.05f, 8.155f, -sizeY * 0.05f);
                    piece.name = $"PuzzlePiece{j}_{i}";
                    PuzzlePiece pieceProps = piece.AddComponent<PuzzlePiece>();
                    pieceProps.pieceX = j;
                    pieceProps.pieceY = i;
                    bool flipNormals = true;
                    List<Vector3> piecePts = new List<Vector3>();
                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            flipNormals = false;
                            piecePts = CalculatePiece.PiecePoints(true, true, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = false;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = false;
                        }
                        else if (j == numX - 1)
                        {
                            piecePts = CalculatePiece.PiecePoints(true, true, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = false;
                            pieceProps.hasNextRight = false;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = true;
                        }
                        else
                        {
                            piecePts = CalculatePiece.PiecePoints(false, true, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = false;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = true;
                        }
                    }
                    else if (i == numY - 1)
                    {
                        if (j == 0)
                        {
                            flipNormals = false;
                            piecePts = CalculatePiece.PiecePoints(true, false, false, true, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = false;
                            pieceProps.hasNextLeft = false;
                        }
                        else if (j == numX - 1)
                        {
                            piecePts = CalculatePiece.PiecePoints(true, false, true, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = false;
                            pieceProps.hasNextAbove = false;
                            pieceProps.hasNextLeft = true;
                        }
                        else
                        {
                            piecePts = CalculatePiece.PiecePoints(false, false, false, true, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = false;
                            pieceProps.hasNextLeft = true;
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            flipNormals = false;
                            piecePts = CalculatePiece.PiecePoints(false, false, false, false, true, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = false;
                        }
                        else if (j == numX - 1)
                        {
                            piecePts = CalculatePiece.PiecePoints(false, false, true, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = false;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = true;
                        }
                        else
                        {
                            piecePts = CalculatePiece.PiecePoints(false, false, false, false, false, sizeX, sizeY, j, i, LineGenerator.PointsDown.Count, bezDetail);
                            pieceProps.hasNextBelow = true;
                            pieceProps.hasNextRight = true;
                            pieceProps.hasNextAbove = true;
                            pieceProps.hasNextLeft = true;
                        }
                    }
                    pieceProps.piecePoints = piecePts;
                    Vector2 uvScale = new Vector2(1f/ratioX,1f/ratioY);
                    float xFac = 1f / (float)numX;
                    float yFac = 1f / (float)numY;
                    Vector2 uvOffset = new Vector2(-j*xFac, -i*yFac);
                    //Vector2 uvOffset = new Vector2((sizeX / numX) + (j * (sizeX / numX)), (sizeY / numY) + (j * (sizeY / numY)));
                    //Vector2 uvOffset = new Vector2(-((j+1)/numX), -((i + 1) / numY));
                    GeneratePiece.Build(piece, piecePts, pieceThickness, true, frontMaterial, cardboardMaterial, flipNormals, uvScale, uvOffset);
                    //StartCoroutine(Waiter(piece, piecePts));
                    puzzlePieces.Add(piece);
                    
                }
            }

            for (int i = 0; i < puzzlePieces.Count; i++)
            {
                PuzzlePiece pieceProps = puzzlePieces[i].GetComponent<PuzzlePiece>();
                if (pieceProps.hasNextBelow)
                {
                    pieceProps.nextBelow = GameObject.Find($"PuzzlePiece{pieceProps.pieceX}_{pieceProps.pieceY - 1}");
                }
                if (pieceProps.hasNextRight)
                {
                    pieceProps.nextRight = GameObject.Find($"PuzzlePiece{pieceProps.pieceX + 1}_{pieceProps.pieceY}");
                }
                if (pieceProps.hasNextAbove)
                {
                    pieceProps.nextAbove = GameObject.Find($"PuzzlePiece{pieceProps.pieceX}_{pieceProps.pieceY + 1}");
                }
                if (pieceProps.hasNextLeft)
                {
                    pieceProps.nextLeft = GameObject.Find($"PuzzlePiece{pieceProps.pieceX - 1}_{pieceProps.pieceY}");
                }
            }
        }

        for (int i = 0; i < GOsToDestroy.Count; i++)
        {
            Destroy(GOsToDestroy[i]);
        }
        Destroy(imgObj);
    }

    //IEnumerator Waiter(GameObject piece, List<Vector3> piecePts)
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    GeneratePiece.Build(piece, piecePts, pieceThickness, true, frontMaterial, cardboardMaterial);
    //}
    void Update()
    {

    }
}
