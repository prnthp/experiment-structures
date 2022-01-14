using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomColorPicker : MonoBehaviour
{
    public List<Gradient> gradients;

    public Color LeftColor;
    public Color RightColor;

    public int gradientIdx { get; set; }
    
    public void SetGradient()
    {
        gradientIdx = Random.Range(0, gradients.Count);
    }

    public void GetNewColors()
    {
        LeftColor = gradients[gradientIdx].Evaluate(Random.Range(0, 1f));
        RightColor = gradients[gradientIdx].Evaluate(Random.Range(0, 1f));
    }
}
