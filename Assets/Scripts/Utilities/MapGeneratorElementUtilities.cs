using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorElementUtilities : MonoBehaviour
{
    public float ElementDistanceLenght()
    {
        List<GameObject> childList = new List<GameObject>();

        float lenght = 0;

        for(int i = 0; i < transform.childCount; i++)
        {
            childList.Add(transform.GetChild(i).gameObject);
        }

        foreach(GameObject child in childList)
        {
            if(child.transform.localPosition.z >= lenght)
            {
                lenght = child.transform.localPosition.z;
            }
        }

        return lenght;
    }

}
