using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TileGen : MonoBehaviour
{
    public Texture2D img;
    private Texture2D mOriginalTex;
    //private Texture2D mTextureOriginal;

    void CreateBaseTexture()
    {
        // Load the main image.
        //mTextureOriginal = SpriteUtils.LoadTexture(Image);

        SpriteRenderer spriteRenderer =
          gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
            img,
            0,
            0,
            img.width,
            img.height);
    }

    void TestTileCurves()
    {
        Puzzle.Tile tile = new Puzzle.Tile(img);
        tile.DrawCurve(Puzzle.Tile.Direction.UP, Puzzle.Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Puzzle.Tile.Direction.UP, Puzzle.Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Puzzle.Tile.Direction.UP, Puzzle.Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Puzzle.Tile.Direction.RIGHT, Puzzle.Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Puzzle.Tile.Direction.RIGHT, Puzzle.Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Puzzle.Tile.Direction.RIGHT, Puzzle.Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Puzzle.Tile.Direction.DOWN, Puzzle.Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Puzzle.Tile.Direction.DOWN, Puzzle.Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Puzzle.Tile.Direction.DOWN, Puzzle.Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Puzzle.Tile.Direction.LEFT, Puzzle.Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Puzzle.Tile.Direction.LEFT, Puzzle.Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Puzzle.Tile.Direction.LEFT, Puzzle.Tile.PosNegType.NONE, Color.white);
    }

    void TestTileFloodFill()
    {
        Puzzle.Tile tile = new Puzzle.Tile(img);

        tile.SetPosNegType(Puzzle.Tile.Direction.UP, Puzzle.Tile.PosNegType.POS);
        tile.SetPosNegType(Puzzle.Tile.Direction.RIGHT, Puzzle.Tile.PosNegType.POS);
        tile.SetPosNegType(Puzzle.Tile.Direction.DOWN, Puzzle.Tile.PosNegType.POS);
        tile.SetPosNegType(Puzzle.Tile.Direction.LEFT, Puzzle.Tile.PosNegType.POS);

        // Uncomment the following 4 lines of code if you want to see the 
        // curves drawn on the tile too.
        tile.DrawCurve(Puzzle.Tile.Direction.UP, Puzzle.Tile.PosNegType.POS, Color.white);
        tile.DrawCurve(Puzzle.Tile.Direction.RIGHT, Puzzle.Tile.PosNegType.POS, Color.white);
        tile.DrawCurve(Puzzle.Tile.Direction.DOWN, Puzzle.Tile.PosNegType.POS, Color.white);
        tile.DrawCurve(Puzzle.Tile.Direction.LEFT, Puzzle.Tile.PosNegType.POS, Color.white);

        tile.Apply();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            tile.FinalCut,
            0,
            0,
            tile.FinalCut.width,
            tile.FinalCut.height);
        spriteRenderer.sprite = sprite;
    }

    void Start()
    {
        CreateBaseTexture();
        //TestTileCurves();
        TestTileFloodFill();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
