using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderText : MonoBehaviour
{
    public TextMeshProUGUI val;

    public void SetText(int x)
    {
        val.text = x + " / 100";
    }
}
