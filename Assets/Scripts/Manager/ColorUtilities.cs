using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUtilities : MonoBehaviour
{
    public static ColorUtilities instance;

    public Material[] colors;
    public Material basicColor;

    private void Awake()
    {
        instance = this;
    }

    public Material GetIndexMaterial(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
                break;
            case 1:
                return colors[0];
                break;
            case 2:
                return colors[1];
                break;
            case 3:
                return colors[2];
                break;
            case 4:
                return colors[3];
                break;
            case 5:
                return colors[4];
                break;
            case 6:
                return colors[5];
                break;
            case 7:
                return colors[6];
                break;
            case 8:
                return colors[7];
                break;
            case 9:
                return colors[8];
                break;
            case 10:
                return colors[9];
                break;            
        }

        return null;
    }
}
