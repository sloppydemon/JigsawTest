using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PuzzlePieceCalc : MonoBehaviour
{

    public static int GCD(int a, int b)
    {
        int result = Mathf.Min(a, b);
        while (result > 0)
        {
            if (a % result == 0 && b % result == 0)
            {
                break;
            }
            result--;
        }
        return result;
    }

    public static int DimensionX(int x, int y)
    {
        int result = x / GCD(x, y);
        return result;
    }

    public static int DimensionY(int x, int y)
    {
        int result = y / GCD(x, y);
        return result;
    }

    public static int NumPiecesY(int x, int y, int numPieces)
    {
        int dimY = DimensionY(x, y);
        int dimX = DimensionX(x, y);
        float fac = MathF.Sqrt((float)numPieces / (float)dimX / (float)dimY);
        float calc = fac * dimY;
        int result = (int)calc;
        return result;
    }

    public static int NumPiecesX(int x, int y, int numPieces)
    {
        int dimY = DimensionY(x, y);
        int dimX = DimensionX(x, y);
        float fac = MathF.Sqrt((float)numPieces / (float)dimX / (float)dimY);
        float calc = fac * dimX;
        int result = (int)calc;
        return result;
    }

    public static int FactorReduction(int val, int otherval, int numPieces, bool isFirst)
    {
        int sum = val * otherval;
        int remain = numPieces % sum;
        int valsAbs = Mathf.Abs(val - otherval);
        int valsAbsMax = Mathf.Max(valsAbs, 2);
        int valUp;
        int valDown;
        if (numPieces % val == 0)
        {
            if (isFirst)
            {
                return val;
            }
            else
            {
                if (numPieces % otherval == 0)
                {
                    if (remain == numPieces)
                    {
                        while (val * otherval > numPieces)
                        {
                            val -= 1;
                        }
                        return val;
                    }
                    else
                    {
                        while (val * otherval < numPieces)
                        {
                            val += 1;
                        }
                        return val;
                    }
                }
                else
                {
                    return val;
                }
            }
        }
        else
        {
            if (numPieces % otherval == 0)
            {
                if (remain == numPieces)
                {
                    while (val * otherval > numPieces)
                    {
                        val -= 1;
                    }
                    return val;
                }
                else
                {
                    while (val * otherval < numPieces)
                    {
                        val += 1;
                    }
                    return val;
                }
            }
            else
            {
                for (int i = 0; i < valsAbsMax; i++)
                {
                    valUp = val + i;
                    valDown = val - i;

                    if (numPieces % valUp == 0)
                    {
                        return valUp;
                    }
                    if (numPieces % valDown == 0)
                    {
                        return valDown;
                    }
                }
            }
            return val;
        }
    }


    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
