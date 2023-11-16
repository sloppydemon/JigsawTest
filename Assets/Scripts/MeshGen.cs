using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using TMPro;
using ProBuilder.Examples;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.Shapes;
using System.ComponentModel;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.SceneManagement;

public class MeshGen : MonoBehaviour
{
    List<Vector3> pts;
    GameObject imgObj;
    public Button generateButton;
    public Button startButton;
    public GameObject LinePrefab;
    public GameObject table;
    public GameObject piece;
    public Texture2D img;
    public Texture2D heightMap;
    public Slider sizeSlider;
    public Slider numberSlider;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI sizeSubText;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI numberSubText;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI momentText;
    public TextMeshProUGUI instructTextA;
    public TextMeshProUGUI instructTextB;
    public Button retryButton;
    public Button newSettingsButton;
    public bool meshingFailed;
    public TMP_InputField inputNumber;
    public float sizeInCms;
    public float sizeFactor;
    public float bezDetail;
    public float bezDetailLines;
    public Material cardboardMaterial;
    public Material frontMaterial;
    public PhysicMaterial piecePhysicalMaterial;
    public float pieceSleepThreshold;
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
    public float bezierRetryIncrementUp;
    public float bezierRetryIncrementDown;
    public float pieceJoinThreshold;
    public float pieceJoinRotationThreshold;
    public float pieceRotationSpeed;
    public int numberOfBezierRetries;
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
    public List<GameObject> puzzlePiecesToDestroy;
    public bool offsetsCalculated;
    public List<AudioClip> soundsImpactPiecePiece;
    public List<AudioClip> soundsImpactPieceBox;
    public List<AudioClip> soundsImpactPieceTable;
    public List<AudioClip> soundsPutPiece;
    public List<AudioClip> soundsJoinPiece;
    public List<AudioClip> soundsPickPieceBox;
    public List<AudioClip> soundsPickPieceTable;

    void Start()
    {
        cam.gameObject.GetComponent<CameraMouse>().enabled = false;
        alertText.text = "";
        startButton.interactable = false;
        startButton.enabled = false;
        startButton.gameObject.SetActive(false);
        offsetsCalculated = false;
        GOsToDestroy = new List<GameObject>();
        pts = new List<Vector3>();
        sizeInCms = 80f;
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
        startButton.onClick.AddListener(delegate { StartPuzzling(); });
        retryButton.onClick.AddListener(delegate { RetryClicked(); });
        newSettingsButton.onClick.AddListener(delegate { NewSettingsClicked(); });
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
        GeneratePiece.Build(imgObj, imgObjVectors, pieceThickness, false, frontMaterial, cardboardMaterial, new Vector2(), new Vector2(), 0, 0, 0, 0, 0, 0, 0, 0);

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
        sizeText.text = $"{sizeX}x{sizeY} cm";

        Destroy(imgObj);
        imgObj = new GameObject();
        imgObj.transform.position = new Vector3(0, 8.155f, 0);
        imgObj.name = "ImageObject";
        List<Vector3> imgObjVectors = new List<Vector3>();
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, -sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(sizeX * 0.05f, 0, sizeY * 0.05f));
        imgObjVectors.Add(new Vector3(-sizeX * 0.05f, 0, sizeY * 0.05f));
        GeneratePiece.Build(imgObj, imgObjVectors, pieceThickness, false, frontMaterial, cardboardMaterial, new Vector2(), new Vector2(), 0, 0, 0, 0, 0, 0, 0, 0);
        if (sizeInCms > 95f)
        {
            table.transform.localScale = new Vector3(sizeInCms * 0.01f, 1f, sizeInCms * 0.01f);
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
            numberText.color = Color.green;
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
        for (int i = 1; i < numY; i++)
        {
            List<Vector3> linePts = new List<Vector3>();
            linePts = LineGenerator.GenerateLine(false, sizeX * 0.1f, sizeY * 0.1f, numX, numY, i, 0f);
            GameObject lPF = Instantiate(LinePrefab, transform.position + new Vector3(-sizeX * 0.05f, 0.01f, -sizeY * 0.05f), Quaternion.identity);
            lPF.name = $"X{i}";
            LinePreview lPV = lPF.GetComponent<LinePreview>();
            lPV.bezierDetail = bezDetailLines;
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
            GameObject lPF = Instantiate(LinePrefab, transform.position + new Vector3(-sizeX * 0.05f, 0.01f, -sizeY * 0.05f), Quaternion.identity);
            lPF.name = $"Y{i}";
            LinePreview lPV = lPF.GetComponent<LinePreview>();
            lPV.bezierDetail = bezDetailLines;
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
            heightMap = HeightMapGenerator.GenerateHeightMap(img, sizeX, sizeY, true);
            frontMaterial.SetTexture("_Height", heightMap);

            puzzlePieces = new List<GameObject>();
            for (int i = 0; i < numY; i++)
            {
                for (int j = 0; j < numX; j++)
                {
                    piece = new GameObject();
                    piece.transform.position = new Vector3(-sizeX * 0.05f, 8.155f, -sizeY * 0.05f);
                    piece.name = $"PuzzlePiece{j}_{i}";
                    PuzzlePiece pieceProps = piece.AddComponent<PuzzlePiece>();
                    pieceProps.pieceX = j;
                    pieceProps.pieceY = i;
                    List<Vector3> piecePts = new List<Vector3>();
                    if (i == 0)
                    {
                        if (j == 0)
                        {
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
                    Vector2 uvScale = new Vector2(1f / ratioX, 1f / ratioY);
                    float xFac = 1f / (float)numX;
                    float yFac = 1f / (float)numY;
                    Vector2 uvOffset = new Vector2(-j * xFac, -i * yFac);
                    GeneratePiece.Build(piece, piecePts, pieceThickness, true, frontMaterial, cardboardMaterial, uvScale, uvOffset, numberOfBezierRetries, bezDetail, bezierRetryIncrementUp, bezierRetryIncrementDown, sizeX, sizeY, numX, numY);
                    pieceProps.joinThreshold = pieceJoinThreshold;
                    pieceProps.joinRotThreshold = pieceJoinRotationThreshold;
                    pieceProps.rotSpeed = pieceRotationSpeed;
                    pieceProps.physMat = piecePhysicalMaterial;
                    pieceProps.sleepThreshold = pieceSleepThreshold;
                    puzzlePieces.Add(piece);

                }
            }

            for (int k = 0; k < puzzlePiecesToDestroy.Count; k++)
            {
                puzzlePiecesToDestroy[k].name = $"destroyed_piece_no_{k}";
                Destroy(puzzlePiecesToDestroy[k] );
            }

            for (int i = 0; i < puzzlePieces.Count; i++)
            {
                PuzzlePiece pieceProps = puzzlePieces[i].GetComponent<PuzzlePiece>();
                if (pieceProps.hasNextBelow)
                {
                    pieceProps.nextBelow = GameObject.Find($"PuzzlePiece{pieceProps.pieceX}_{pieceProps.pieceY - 1}");
                    //pieceProps.nextBelowOffset = pieceProps.nextBelow.transform.position - puzzlePieces[i].transform.position;
                }
                if (pieceProps.hasNextRight)
                {
                    pieceProps.nextRight = GameObject.Find($"PuzzlePiece{pieceProps.pieceX + 1}_{pieceProps.pieceY}");
                    //pieceProps.nextRightOffset = pieceProps.nextRight.transform.position - puzzlePieces[i].transform.position;
                }
                if (pieceProps.hasNextAbove)
                {
                    pieceProps.nextAbove = GameObject.Find($"PuzzlePiece{pieceProps.pieceX}_{pieceProps.pieceY + 1}");
                    //pieceProps.nextAboveOffset = pieceProps.nextAbove.transform.position - puzzlePieces[i].transform.position;
                }
                if (pieceProps.hasNextLeft)
                {
                    pieceProps.nextLeft = GameObject.Find($"PuzzlePiece{pieceProps.pieceX - 1}_{pieceProps.pieceY}");
                    //pieceProps.nextLeftOffset = pieceProps.nextLeft.transform.position - puzzlePieces[i].transform.position;
                }
            }
        }

        for (int i = 0; i < GOsToDestroy.Count; i++)
        {
            Destroy(GOsToDestroy[i]);
        }
        Destroy(imgObj);
        generateButton.gameObject.SetActive(false);
        numberSlider.gameObject.SetActive(false);
        numberText.gameObject.SetActive(false);
        numberSubText.gameObject.SetActive(false);
        sizeSlider.gameObject.SetActive(false);
        sizeText.gameObject.SetActive(false);
        sizeSubText.gameObject.SetActive(false);
        momentText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        startButton.enabled = true;
        startButton.interactable = true;

        if (meshingFailed)
        {
            alertText.text = "Some pieces failed to mesh. \u2639";
            retryButton.gameObject.SetActive(true);
            newSettingsButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
    }

    public void RetryClicked()
    {
        meshingFailed = false;
        GenerateLines();
        GenerateClicked();
    }

    public void NewSettingsClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void StartPuzzling()
    {
        CalculateOffsets();
        StartCoroutine(PiecesToBox());
        cam.gameObject.GetComponent<CameraMouse>().enabled = true;
        startButton.gameObject.SetActive(false);
        instructTextA.gameObject.SetActive(true);
        instructTextB.gameObject.SetActive(true);
    }

    public void CalculateOffsets()
    {
        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            PuzzlePiece pieceProps = puzzlePieces[i].GetComponent<PuzzlePiece>();
            Rigidbody pieceRB = puzzlePieces[i].GetComponent<Rigidbody>();

            if (pieceProps.hasNextBelow)
            {
                pieceProps.nextBelowOffset = pieceProps.nextBelow.transform.position - puzzlePieces[i].transform.position;
                Rigidbody nextRB = pieceProps.nextBelow.GetComponent<Rigidbody>();
                pieceProps.nextBelowRotOS = nextRB.transform.eulerAngles - pieceRB.transform.eulerAngles;
            }
            if (pieceProps.hasNextRight)
            {
                pieceProps.nextRightOffset = pieceProps.nextRight.transform.position - puzzlePieces[i].transform.position;
                Rigidbody nextRB = pieceProps.nextRight.GetComponent<Rigidbody>();
                pieceProps.nextRightRotOS = nextRB.transform.eulerAngles - pieceRB.transform.eulerAngles;
            }
            if (pieceProps.hasNextAbove)
            {
                pieceProps.nextAboveOffset = pieceProps.nextAbove.transform.position - puzzlePieces[i].transform.position;
                Rigidbody nextRB = pieceProps.nextAbove.GetComponent<Rigidbody>();
                pieceProps.nextAboveRotOS = nextRB.transform.eulerAngles - pieceRB.transform.eulerAngles;
            }
            if (pieceProps.hasNextLeft)
            {
                pieceProps.nextLeftOffset = pieceProps.nextLeft.transform.position - puzzlePieces[i].transform.position;
                Rigidbody nextRB = pieceProps.nextLeft.GetComponent<Rigidbody>();
                pieceProps.nextLeftRotOS = nextRB.transform.eulerAngles - pieceRB.transform.eulerAngles;
            }
        }
    }

    IEnumerator PiecesToBox()
    {
        GameObject cornerUL = GameObject.FindGameObjectWithTag("BoxCornerUL");
        GameObject cornerLR = GameObject.FindGameObjectWithTag("BoxCornerLR");
        Vector3 vecUL = cornerUL.transform.position;
        Vector3 vecLR = cornerLR.transform.position;

        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            Rigidbody tossRB = puzzlePieces[i].GetComponent<Rigidbody>(); 
            float tossX = Random.Range(vecUL.x, vecLR.x);
            float tossY = Random.Range(vecUL.y, vecLR.y);
            float tossZ = Random.Range(vecUL.z, vecLR.z);
            tossRB.transform.position = new Vector3(tossX, tossY, tossZ);
            yield return null;
            float rotateX = Random.Range(0f, 360f);
            float rotateY = Random.Range(0f, 360f);
            float rotateZ = Random.Range(0f, 360f);
            tossRB.transform.eulerAngles = new Vector3(rotateX, rotateY, rotateZ);
            yield return null;
        }
    }

    //IEnumerator WaitText()
    //{
    //    while (startButton.enabled = false)
    //    {
    //        alertText.text = "Just a moment...";
    //        yield return null;
    //    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }
        }
    }
}