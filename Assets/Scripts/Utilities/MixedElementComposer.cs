using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedElementComposer : MonoBehaviour
{
    public int rightPos;
    public int leftPos;

    public GameObject[] possibleWallObj;
    public GameObject[] possibleTowerObj;

    void OnEnable()
    {
        GenerateElement();
    }

    private void GenerateElement()
    {
        GameObject wallO = Instantiate(possibleWallObj[Random.Range(0, possibleWallObj.Length)], Vector3.zero, Quaternion.identity, transform);
        GameObject towerO = Instantiate(possibleTowerObj[Random.Range(0, possibleTowerObj.Length)], Vector3.zero, Quaternion.identity, transform);

        float index = Random.Range(0, 1);

        if(index <= 0.5f)
        {
            wallO.transform.localPosition = new Vector3(rightPos , wallO.transform.localPosition.y , 0);
            towerO.transform.localPosition = new Vector3(leftPos, towerO.transform.localPosition.y, 0);
        }

        else
        {
            wallO.transform.localPosition = new Vector3(leftPos , wallO.transform.localPosition.y, 0); ;
            towerO.transform.localPosition = new Vector3(rightPos , towerO.transform.localPosition.y, 0); ;
        }
    }
}
