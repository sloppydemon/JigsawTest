using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NormalGenerator : MonoBehaviour
{
    public static Texture2D GenerateNormals(Texture2D texture, float str = 1.0f)
    {
        
        Color[] colors = texture.GetPixels();
        Texture2D normal = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        for (int x = 1; x < texture.width - 1; x++)
            for (int y = 1; y < texture.height - 1; y++)
            {
                //using Sobel operator
                float tl, t, tr, l, right, bl, bot, br;
                tl = Intensity(texture.GetPixel(x - 1, y - 1));
                t = Intensity(texture.GetPixel(x - 1, y));
                tr = Intensity(texture.GetPixel(x - 1, y + 1));
                right = Intensity(texture.GetPixel(x, y + 1));
                br = Intensity(texture.GetPixel(x + 1, y + 1));
                bot = Intensity(texture.GetPixel(x + 1, y));
                bl = Intensity(texture.GetPixel(x + 1, y - 1));
                l = Intensity(texture.GetPixel(x, y - 1));

                //Sobel filter
                float dX = (tr + 2.0f * right + br) - (tl + 2.0f * l + bl);
                float dY = (bl + 2.0f * bot + br) - (tl + 2.0f * t + tr);
                float dZ = 1.0f / str;

                Vector3 vc = new Vector3(str * dX, str * dY, dZ);
                vc.Normalize();

                normal.SetPixel(x, y, new Color(vc.x, 0.5f, vc.y, vc.z));
                //normal.SetPixel(x, y, new Color(0.5f + 0.5f * vc.x, 0.5f + 0.5f * vc.y, 0.5f + 0.5f * vc.z, 0.0f));
            }
        normal.Apply();
        return normal;
    }

    //public static float intensity(float r, float g, float b)
    //{
    //    return (r + g + b) / 3.0f;
    //}

    public static float Intensity(Color color)
    {
        return (0.299f * color.r + 0.587f * color.g + 0.114f * color.b);
    }

}
