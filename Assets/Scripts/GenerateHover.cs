using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateHover : MonoBehaviour
{
    private bool hover;
    public TextMeshProUGUI momentText;

    private void Start()
    {
        hover = false;
        momentText.text = "";
    }
    private void OnMouseEnter()
    {
        hover = true;
        momentText.text = "Might take a while...";
        StartCoroutine(MightTakeAWhile());
    }

    private void OnMouseExit()
    {
        hover = false;
        momentText.text = "";
        StopCoroutine(MightTakeAWhile());
        momentText.alpha = 0.0f;
    }

    IEnumerator MightTakeAWhile()
    {
        bool flipflop = false;
        float f = 1f;
        while (hover == true)
        {
            if (flipflop)
            {
                f += 0.01f;
            }
            else 
            {
                f -= 0.01f;
            }

            if (f == 1f)
            {
                flipflop = false;
            }
            if (f == 0f)
            {
                flipflop = true;
            }
            momentText.alpha = f;
            yield return null;
        }
    }
}
