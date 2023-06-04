using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEditGrid : MonoBehaviour
{
    public GameObject editSlotPrefs;

    [HideInInspector]
    public List<GameObject> currentGridElement = new List<GameObject>();

    private void Awake()
    {
        GenerateSlot();
    }
    
    private void GenerateSlot()
    {
        int xQuantity = 2;
        int yQuantity = 2;

        Vector3 startPoint = new Vector3(-0.75f, 0, -(0.75f / 2f));           

        for (int i = 0; i < xQuantity; i++)
        {
            for (int j = 0; j < yQuantity; j++)
            {
                GameObject slot = Instantiate(editSlotPrefs, startPoint, Quaternion.identity , transform);
                slot.transform.localPosition = startPoint;
                slot.transform.rotation = new Quaternion(0, 0, 0, 0);

                currentGridElement.Add(slot);

                startPoint += new Vector3(0, 0, CharacterBehaviour.instance.moneyDecalScaleY);
            }

            startPoint += new Vector3(CharacterBehaviour.instance.moneyDecalScaleX, 0, (-yQuantity * CharacterBehaviour.instance.moneyDecalScaleY));
        }
    }    
}
