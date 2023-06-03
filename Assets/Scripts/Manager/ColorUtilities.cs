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
    public Material[] moneyParticleMat;

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
            case 1:
                return decalMaterial[0];
            case 2:
                return decalMaterial[1];                
            case 3:
                return decalMaterial[2];
            case 4:
                return decalMaterial[3];
            case 5:
                return decalMaterial[4];
            case 6:
                return decalMaterial[5];
            case 7:
                return decalMaterial[6];
            case 8:
                return decalMaterial[7];
            case 9:
                return decalMaterial[8];
            case 10:
                return decalMaterial[9];                            
        }

        return null;
    }
    public Color GetIndexColor(int value)
    {
        switch (value)
        {
            case 0:
                return Color.green;
            case 1:
                return decalColor[0];
            case 2:
                return decalColor[1];
            case 3:
                return decalColor[2];
            case 4:
                return decalColor[3];
            case 5:
                return decalColor[4];
            case 6:
                return decalColor[5];
            case 7:
                return decalColor[6];
            case 8:
                return decalColor[7];
            case 9:
                return decalColor[8];
            case 10:
                return decalColor[9];
        }

        return Color.green;
    }

    public Material GetEditObjectMaterial(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
            case 1:
                return editObjectMat[0];
            case 2:
                return editObjectMat[1];
            case 3:
                return editObjectMat[2];
            case 4:
                return editObjectMat[3];
            case 5:
                return editObjectMat[4];
            case 6:
                return editObjectMat[5];
            case 7:
                return editObjectMat[6];
            case 8:
                return editObjectMat[7];
            case 9:
                return editObjectMat[8];
            case 10:
                return editObjectMat[9];
        }

        return null;
    }

    public Material GetEditObjectMaterialSide(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
            case 1:
                return editObjectMat_Side[0];
            case 2:
                return editObjectMat_Side[1];
            case 3:
                return editObjectMat_Side[2];
            case 4:
                return editObjectMat_Side[3];
            case 5:
                return editObjectMat_Side[4];
            case 6:
                return editObjectMat_Side[5];
            case 7:
                return editObjectMat_Side[6];
            case 8:
                return editObjectMat_Side[7];
            case 9:
                return editObjectMat_Side[8];
            case 10:
                return editObjectMat_Side[9];
        }

        return null;
    }

    public Material GetIndexParticleMat(int value)
    {
        switch (value)
        {
            case 0:
                return basicColor;
            case 1:
                return moneyParticleMat[0];
            case 2:
                return moneyParticleMat[1];
            case 3:
                return moneyParticleMat[2];
            case 4:
                return moneyParticleMat[3];
            case 5:
                return moneyParticleMat[4];
            case 6:
                return moneyParticleMat[5];
            case 7:
                return moneyParticleMat[6];
            case 8:
                return moneyParticleMat[7];
            case 9:
                return moneyParticleMat[8];
            case 10:
                return moneyParticleMat[9];
        }

        return null;
    }
}
