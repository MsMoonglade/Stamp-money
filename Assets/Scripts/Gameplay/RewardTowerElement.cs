using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTowerElement : MonoBehaviour
{
    [Header ("EDIT")]
    public int value;

    [Header("NOT EDIT")]
    public int valuePerElement;

    public float elementRotOffset;
    public float elementYOffset;
    public float rewardYOffset;

    public GameObject towerParent;
    public GameObject rewardParent;
    public GameObject towerElementPrefs;

    private void Awake()
    {
        GenerateElement();
    }

    private void GenerateElement()
    {
        int necesaryElement = value / valuePerElement;
        Vector3 pos = Vector3.zero;

        for(int i = 0; i < necesaryElement; i++)
        {
            GameObject element = Instantiate(towerElementPrefs, pos, Quaternion.identity, towerParent.transform);
            element.transform.localPosition = pos;          
            element.transform.localRotation *= Quaternion.Euler(0, (elementRotOffset * i),0 );

            pos += new Vector3(0, elementYOffset, 0);
        }

        pos += new Vector3(0, rewardYOffset , 0);

        rewardParent.transform.localPosition = pos;
    }
}