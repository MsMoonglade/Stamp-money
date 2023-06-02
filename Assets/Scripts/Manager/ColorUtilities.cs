using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUtilities : MonoBehaviour
{
    public static ColorUtilities instance;

    public Color[] decalColor;
    public Material[] decalMaterial;
    public Material[] editObjectMat;
    public Material[] editObjectMat_Side;

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
                return decalMaterial[0];
                break;
            case 2:
                return decalMaterial[1];
                break;
            case 3:
                return decalMaterial[2];
                break;
            case 4:
                return decalMaterial[3];
                break;
            case 5:
                return decalMaterial[4];
                break;
            case 6:
                return decalMaterial[5];
                break;
            case 7:
                return decalMaterial[6];
                break;
            case 8:
                return decalMaterial[7];
                break;
            case 9:
                return decalMaterial[8];
                break;
            case 10:
                return decalMaterial[9];
                break;            
        }

        return null;
    }
    public Color GetIndexColor(int value)
    {
        switch (value)
        {
            case 0:
                return Color.green;
                break;
            case 1:
                return decalColor[0];
                break;
            case 2:
                return decalColor[1];
                break;
            case 3:
                return decalColor[2];
                break;
            case 4:
                return decalColor[3];
                break;
            case 5:
                return decalColor[4];
                break;
            case 6:
                return decalColor[5];
                break;
            case 7:
                return decalColor[6];
                break;
            case 8:
                return decalColor[7];
                break;
            case 9:
                return decalColor[8];
                break;
            case 10:
                return decalColor[9];
                break;
        }

        return Color.green;
    }

    public Material GetEditObjectMaterial(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
                break;
            case 1:
                return editObjectMat[0];
                break;
            case 2:
                return editObjectMat[1];
                break;
            case 3:
                return editObjectMat[2];
                break;
            case 4:
                return editObjectMat[3];
                break;
            case 5:
                return editObjectMat[4];
                break;
            case 6:
                return editObjectMat[5];
                break;
            case 7:
                return editObjectMat[6];
                break;
            case 8:
                return editObjectMat[7];
                break;
            case 9:
                return editObjectMat[8];
                break;
            case 10:
                return editObjectMat[9];
                break;
        }

        return null;
    }

    public Material GetEditObjectMaterialSide(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
                break;
            case 1:
                return editObjectMat_Side[0];
                break;
            case 2:
                return editObjectMat_Side[1];
                break;
            case 3:
                return editObjectMat_Side[2];
                break;
            case 4:
                return editObjectMat_Side[3];
                break;
            case 5:
                return editObjectMat_Side[4];
                break;
            case 6:
                return editObjectMat_Side[5];
                break;
            case 7:
                return editObjectMat_Side[6];
                break;
            case 8:
                return editObjectMat_Side[7];
                break;
            case 9:
                return editObjectMat_Side[8];
                break;
            case 10:
                return editObjectMat_Side[9];
                break;
        }

        return null;
    }
}
