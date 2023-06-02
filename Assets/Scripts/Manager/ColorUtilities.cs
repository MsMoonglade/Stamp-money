using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUtilities : MonoBehaviour
{
    public static ColorUtilities instance;

    public Material[] colors;
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
