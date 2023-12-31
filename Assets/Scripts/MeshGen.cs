using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class MeshGen : MonoBehaviour
{
    #region GameObjectReferences
    [Header("Game Object References:")]
    public GameObject LinePrefab;
    public GameObject table;
    public GameObject piece;
    public GameObject pieceGhost;
    public List<GameObject> GOsToDestroy;
    public List<GameObject> puzzlePieces;
    public List<GameObject> puzzlePiecesToDestroy;
    public List<GameObject> joinedPieces;
    GameObject imgObj;
    #endregion

    #region TextureVariables
    [Header("Texture references:")]
    public Texture2D img;
    public Texture2D heightMap;
    public int softenIterations;
    #endregion

    #region UIReferences
    [Header("UI references:")]
    public UnityEngine.UI.Slider sizeSlider;
    public UnityEngine.UI.Slider numberSlider;
    public UnityEngine.UI.Slider joinedSlider;
    public TextMeshProUGUI joinedText;
    public TextMeshProUGUI joinedSubText;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI sizeSubText;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI numberSubText;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI momentText;
    public TextMeshProUGUI instructTextA;
    public TextMeshProUGUI instructTextB;
    public UnityEngine.UI.Button generateButton;
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button retryButton;
    public UnityEngine.UI.Button newSettingsButton;
    public TMP_InputField inputNumber;
    #endregion

    #region Booleans
    [Header("Public booleans:")]
    public bool meshingFailed;
    public bool buildable;
    #endregion

    #region PuzzleParameters
    [Header("Puzzle parameters:")]
    public float sizeInCms;
    public float sizeFactor;
    public int ratioX;
    public int ratioY;
    public int numX;
    public int numY;
    public int gcd;
    static float SizeFactor(float y, float x, float size)
    {
        float max = Mathf.Max(y, x);
        return size / max;
    }
    float sizeX;
    float sizeY;
    public int numberOfPieces;
    #endregion

    #region LinePreviewParameters
    [Header("Preview line parameters:")]
    public float bezDetailLines;
    #endregion

    #region MeshingParameters
    [Header("Puzzle piece meshing parameters:")]
    public float bezDetail;
    public float pieceThickness;
    public float bezierRetryIncrementUp;
    public float bezierRetryIncrementDown;
    public int numberOfBezierRetries;
    #endregion

    #region PuzzlePieceSettings
    [Header("Puzzle piece parameters:")]
    public Material cardboardMaterial;
    public Material frontMaterial;
    public Material ghostMaterial;
    public PhysicMaterial piecePhysicalMaterial;
    public float pieceSleepThreshold;
    public float pieceJoinThreshold;
    public float pieceJoinRotationThreshold;
    public float pieceRotationSpeed;
    [SerializeField]
    float approxPieceSizeX;
    [SerializeField]
    float approxPieceSizeY;
    #endregion

    #region CameraSettings
    [Header("Camera settings:")]
    public Camera cam;
    public float fieldOfViewMinimum;
    public float fieldOfViewMaxAddend;
    #endregion

    #region AudioClips
    [Header("Audio clip lists:")]
    public List<AudioClip> soundsImpactPiecePiece;
    public List<AudioClip> soundsImpactPieceBox;
    public List<AudioClip> soundsImpactPieceTable;
    public List<AudioClip> soundsPutPiece;
    public List<AudioClip> soundsJoinPiece;
    public List<AudioClip> soundsPickPieceBox;
    public List<AudioClip> soundsPickPieceTable;
    public List<AudioClip> soundsPieceOnFloor;
    #endregion

    void Start()
    {
        #region InitialDeclarations
        cam.gameObject.GetComponent<CameraMouse>().enabled = false;
        alertText.text = "";
        startButton.interactable = false;
        startButton.enabled = false;
        startButton.gameObject.SetActive(false);
        GOsToDestroy = new List<GameObject>();
        sizeInCms = 80f;
        sizeSlider.maxValue = 200f;
        sizeSlider.minValue = 5f;
        sizeSlider.value = sizeInCms;
        numberSlider.maxValue = 3000;
        numberSlider.minValue = 20;
        joinedSlider.gameObject.SetActive(false);
        sizeX = img.width * SizeFactor(img.width, img.height, sizeInCms);
        sizeY = img.height * SizeFactor(img.width, img.height, sizeInCms);
        sizeFactor = SizeFactor(img.width, img.height, sizeInCms);
        sizeText.text = $"{sizeX}x{sizeY} cm";
        cam.fieldOfView = fieldOfViewMinimum + (sizeSlider.normalizedValue * fieldOfViewMaxAddend);
        joinedPieces = new List<GameObject>();
        #endregion

        #region UIDelegation
        sizeSlider.onValueChanged.AddListener(delegate { SizeSliderChange(); });
        numberSlider.onValueChanged.AddListener(delegate { NumberSliderChange(); });
        inputNumber.onEndEdit.AddListener(delegate { NumberInputChanged(inputNumber.text); });
        generateButton.onClick.AddListener(delegate { GenerateClicked(); });
        startButton.onClick.AddListener(delegate { StartPuzzling(); });
        retryButton.onClick.AddListener(delegate { RetryClicked(); });
        newSettingsButton.onClick.AddListener(delegate { NewSettingsClicked(); });
        joinedSlider.onValueChanged.AddListener(delegate { JoinedChanged(); });
        #endregion

        #region InitialPuzzlePreview
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
            generateButton.interactable = true;
            GenerateLines();
        }
        else
        {
            for (int i = 0; i < GOsToDestroy.Count; i++)
            {
                Destroy(GOsToDestroy[i]);
            }
            generateButton.interactable = false;
        }
        #endregion
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
        approxPieceSizeX = indSizeX * 0.1f;
        approxPieceSizeY = indSizeY * 0.1f;

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
            joinedSlider.maxValue = numberOfPieces;
            joinedSlider.minValue = 0;
            joinedSlider.value = 0;
            heightMap = HeightMapGenerator.GenerateHeightMap(img, sizeX, sizeY, true, softenIterations);
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
                    pieceProps.pieceSizeX = approxPieceSizeX;
                    pieceProps.pieceSizeY = approxPieceSizeY;
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
        float xCalc = (sizeX / (float)numX) * 0.1f;
        float yCalc = (sizeY / (float)numY) * 0.1f;
        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            PuzzlePiece pieceProps = puzzlePieces[i].GetComponent<PuzzlePiece>();

            if (pieceProps.hasNextBelow)
            {
                pieceProps.nextBelowOffset = new Vector3(0, 0, -yCalc);
                //pieceProps.nextBelowOffset = pieceProps.nextBelow.transform.position - puzzlePieces[i].transform.position;
            }
            if (pieceProps.hasNextRight)
            {
                pieceProps.nextRightOffset = new Vector3(xCalc, 0, 0);
                //pieceProps.nextRightOffset = pieceProps.nextRight.transform.position - puzzlePieces[i].transform.position;
            }
            if (pieceProps.hasNextAbove)
            {
                pieceProps.nextAboveOffset = new Vector3(0, 0, yCalc);
                //pieceProps.nextAboveOffset = pieceProps.nextAbove.transform.position - puzzlePieces[i].transform.position;
            }
            if (pieceProps.hasNextLeft)
            {
                pieceProps.nextLeftOffset = new Vector3(-xCalc, 0, 0);
                //pieceProps.nextLeftOffset = pieceProps.nextLeft.transform.position - puzzlePieces[i].transform.position;
            }
        }
    }

    public void AddJoined(GameObject joinedPiece)
    {
        joinedSlider.gameObject.SetActive(true);
        if (!joinedPieces.Contains(joinedPiece))
        {
            joinedPieces.Add(joinedPiece);
        }
        joinedSlider.value = joinedPieces.Count;
        JoinedChanged();
        //return (joinedPieces.Count);
    }

    public void JoinedChanged()
    {
        joinedText.text = $"Joined Pieces: {joinedSlider.value}";
        joinedSubText.text = $"({numberOfPieces-joinedSlider.value} pcs. remaining.)";
        if (joinedPieces.Count == numberOfPieces)
        {
            WonGame();
        }
    }

    public void WonGame()
    {
        alertText.gameObject.SetActive (true);
        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            puzzlePieces[i].gameObject.GetComponent<PuzzlePiece>().enabled = false;
        }
        joinedSlider.gameObject.SetActive(false);
        instructTextA.gameObject.SetActive(false);
        instructTextB.gameObject.SetActive(false);
        joinedText.text = "Press ESC to start new game";
        joinedSubText.text = "Press SHIFT+ESC to quit";
        alertText.text = $"Congratulations!\nYou finished the {numberOfPieces} puzzle.";
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
            float tossX = UnityEngine.Random.Range(vecUL.x, vecLR.x);
            float tossY = UnityEngine.Random.Range(vecUL.y, vecLR.y);
            float tossZ = UnityEngine.Random.Range(vecUL.z, vecLR.z);
            tossRB.transform.position = new Vector3(tossX, tossY, tossZ);
            yield return null;
            float rotateX = UnityEngine.Random.Range(0f, 360f);
            float rotateY = UnityEngine.Random.Range(0f, 360f);
            float rotateZ = UnityEngine.Random.Range(0f, 360f);
            tossRB.transform.eulerAngles = new Vector3(rotateX, rotateY, rotateZ);
            yield return null;
        }
    }

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