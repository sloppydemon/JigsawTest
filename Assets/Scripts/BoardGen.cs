using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGen : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The image for the Jigsaw puzzle")]
    Texture2D img;
    //string ImageFilename;
    

    // The opaque sprite. 
    Sprite mBaseSpriteOpaque;

    // The transparent (or Ghost sprite)
    Sprite mBaseSpriteTransparent;

    // The game object that holds the opaque sprite.
    // This should be SetActive to false by default.
    GameObject mGameObjectOpaque;

    // The game object that holds the transparent sprite.
    GameObject mGameObjectTransparent;

    public int NumTilesX { get; private set; }
    public int NumTilesY { get; private set; }

    Puzzle.Tile[,] mTiles = null;
    GameObject[,] mTileGameObjects = null;

    Sprite LoadBaseTexture()
    {
        //Texture2D tex = SpriteUtils.LoadTexture(ImageFilename);
        Texture2D tex = img;
        if (!tex.isReadable)
        {
            Debug.Log("Error: Texture is not readable");
            return null;
        }

        if (tex.width % Puzzle.Tile.TileSize != 0 || tex.height % Puzzle.Tile.TileSize != 0)
        {
            Debug.Log("Error: Image must be of size that is multiple of <" + Puzzle.Tile.TileSize + ">");
            return null;
        }

        // Add padding to the image.
        Texture2D newTex = new Texture2D(
            tex.width + Puzzle.Tile.Padding * 2,
            tex.height + Puzzle.Tile.Padding * 2,
            TextureFormat.ARGB32,
            false);

        // Set the default colour as white
        for (int x = 0; x < newTex.width; ++x)
        {
            for (int y = 0; y < newTex.height; ++y)
            {
                newTex.SetPixel(x, y, Color.white);
            }
        }

        // Copy the colours.
        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                Color color = tex.GetPixel(x, y);
                color.a = 1.0f;
                newTex.SetPixel(x + Puzzle.Tile.Padding, y + Puzzle.Tile.Padding, color);
            }
        }
        newTex.Apply();

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            newTex,
            0,
            0,
            newTex.width,
            newTex.height);
        return sprite;
    }

    Sprite CreateTransparentView()
    {
        Texture2D tex = mBaseSpriteOpaque.texture;

        // Add padding to the image.
        Texture2D newTex = new Texture2D(
            tex.width,
            tex.height,
            TextureFormat.ARGB32,
            false);

        //for (int x = Tile.Padding; x < Tile.Padding + Tile.TileSize; ++x)
        for (int x = 0; x < newTex.width; ++x)
        {
            //for (int y = Tile.Padding; y < Tile.Padding + Tile.TileSize; ++y)
            for (int y = 0; y < newTex.height; ++y)
            {
                Color c = tex.GetPixel(x, y);
                if (x > Puzzle.Tile.Padding && x < (newTex.width - Puzzle.Tile.Padding) &&
                    y > Puzzle.Tile.Padding && y < newTex.height - Puzzle.Tile.Padding)
                {
                    c.a = 0.2f;
                }
                newTex.SetPixel(x, y, c);
            }
        }

        newTex.Apply();

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            newTex,
            0,
            0,
            newTex.width,
            newTex.height);
        return sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        mBaseSpriteOpaque = LoadBaseTexture();
        mGameObjectOpaque = new GameObject();
        mGameObjectOpaque.name = img + "_Opaque";
        mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
        mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

        mBaseSpriteTransparent = CreateTransparentView();
        mGameObjectTransparent = new GameObject();
        mGameObjectTransparent.name = img + "_Transparent";
        mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
        mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
