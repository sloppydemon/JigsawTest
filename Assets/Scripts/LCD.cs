using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCD : MonoBehaviour
{
    // calculates Least Common Denominator

    // get GCD or HCF of two numbers
    static int GCD(int a, int b)
    {
        // find Minimum of a and b
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

    // return LCM of two numbers
    static int LCM(int a, int b)
    {
        return (a / GCD(a, b)) * b;
    }

    static void GetLCD(int num1, int den1, int num2, int den2)
    {
        int lcd = LCM(den1, den2);

        num1 *= (lcd / den1);
        num2 *= (lcd / den2);


    }


    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
