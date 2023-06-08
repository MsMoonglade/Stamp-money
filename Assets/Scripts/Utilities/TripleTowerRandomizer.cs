using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleTowerRandomizer : MonoBehaviour
{
    GameObject[] child;

    private void Awake()
    {
        child = new GameObject[transform.childCount];

        for(int i = 0; i < child.Length ; i++)
        {
            child[i] = transform.GetChild(i).gameObject;
        }

        RandomizePosition();   
    }

    void RandomizePosition()
    {
        for(int i = 0; i < child.Length; i++)
        {
            int indexToSwap = Random.Range(0, child.Length);

            GameObject newObjPos = child[indexToSwap];

            Vector3 pos = newObjPos.transform.localPosition;

            newObjPos.transform.localPosition = child[i].transform.localPosition;
            child[i].transform.localPosition = pos;
        }
    }
}