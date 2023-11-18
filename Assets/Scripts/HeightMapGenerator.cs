using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    public static Texture2D GenerateHeightMap(Texture2D img, float sizeX, float sizeY, bool soften, int softI)
    {
        List<Vector3> xPoints = new List<Vector3>();
        Texture2D blank;
        blank = GenerateBlankMap(img);
        GameObject[] GOsX = GameObject.FindGameObjectsWithTag("LineHolder");
        for (int i = 0; i < GOsX.Length; i++)
        {
            LinePreview lp = GOsX[i].GetComponent<LinePreview>();
            xPoints.AddRange(lp.bezPts);
            DrawLinesPerAxis(blank, lp.bezPts, sizeX, sizeY);
        }

        DrawLine(blank, new Vector2(0, 0), new Vector2(blank.width-1, 0), Color.black);
        DrawLine(blank, new Vector2(blank.width - 1, 0), new Vector2(blank.width - 1, blank.height - 1), Color.black);
        DrawLine(blank, new Vector2(blank.width - 1, blank.height), new Vector2(0, blank.height - 1), Color.black);
        DrawLine(blank, new Vector2(0, blank.height - 1), new Vector2(0, 0), Color.black);

        if (soften)
        {
            if (softI < 1)
            {
                softI = 1;
            }
            for (int i = 0;i < softI;i++)
            {
                if (i < 1)
                {
                    Soften(blank, Color.white);
                }
                else
                {
                    Soften(blank, Color.green);
                }
            }
        }

        blank.Apply();
        return blank;
    }

    public static void DrawLinesPerAxis(Texture2D tex, List<Vector3> points, float sizeX, float sizeY)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (i == points.Count - 1)
            {
                float x = (points[i].x / sizeX) * 10 * (tex.width - 1);
                float y = (points[i].z / sizeY) * 10 * (tex.height - 1);
                tex.SetPixel((int)x+4, (int)y+5, Color.black);
            }
            else
            {
                float x1 = (points[i].x / sizeX) * 10 * (tex.width - 1);
                float y1 = (points[i].z / sizeY) * 10 * (tex.height - 1);
                float x2 = (points[i + 1].x / sizeX) * 10 * (tex.width - 1);
                float y2 = (points[i + 1].z / sizeY) * 10 * (tex.height - 1);
                DrawLine(tex, new Vector2(x1+4, y1 + 5), new Vector2(x2 + 4, y2 + 5), Color.black);
            }
        }
    }

    public static Texture2D GenerateBlankMap(Texture2D img)
    {
        Texture2D blankMap = new Texture2D(img.width*2, img.height*2, TextureFormat.ARGB32, false);
        for (int i = 0; i < blankMap.width; i++)
        {
            for (int j = 0; j < blankMap.height; j++)
            {
                blankMap.SetPixel(i, j, Color.white);
            }
        }
        return blankMap;
    }

    public static void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }

    public static void Soften(Texture2D tex, Color col)
    {
        // col here is the color we don't want softened
        for (int i = 0; i <  tex.width; i++)
        {
            for (int j = 0;j < tex.height; j++)
            {
                if (tex.GetPixel (i, j) != col)
                {
                    if (i != 0)
                    {
                        Color col1 = tex.GetPixel (i, j);
                        Color col2 = tex.GetPixel (i - 1, j);
                        Color newCol = Color.Lerp(col1, col2, 0.5f);
                        tex.SetPixel (i - 1, j, newCol);
                    }
                    if (i != tex.width - 1)
                    {
                        Color col1 = tex.GetPixel(i, j);
                        Color col2 = tex.GetPixel(i + 1, j);
                        Color newCol = Color.Lerp(col1, col2, 0.5f);
                        tex.SetPixel(i + 1, j, newCol);
                    }
                    if (j != 0)
                    {
                        Color col1 = tex.GetPixel(i, j);
                        Color col2 = tex.GetPixel(i, j - 1);
                        Color newCol = Color.Lerp(col1, col2, 0.5f);
                        tex.SetPixel(i, j - 1, newCol);
                    }
                    if (j != tex.height - 1)
                    {
                        Color col1 = tex.GetPixel(i, j);
                        Color col2 = tex.GetPixel(i, j + 1);
                        Color newCol = Color.Lerp(col1, col2, 0.5f);
                        tex.SetPixel(i, j + 1, newCol);
                    }
                }
            }
        }
    }
}
