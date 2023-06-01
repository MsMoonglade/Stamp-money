using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditGrid : MonoBehaviour
{
    public GameObject editSlotPrefs;
    public GameObject printerBaseSize;

    private CharacterBehaviour character;
    
    [HideInInspector]
    public List<GameObject> currentGridElement = new List<GameObject>();

    private void Awake()
    {
        character = CharacterBehaviour.instance;

        GenerateSlot();
    }

    private void GenerateSlot()
    {
        int xQuantity = (int)(printerBaseSize.transform.localScale.x / character.moneyDecalScaleX);
        int yQuantity = (int)(printerBaseSize.transform.localScale.z / character.moneyDecalScaleY);

        Vector3 startPoint = new Vector3(
           printerBaseSize.transform.position.x - (printerBaseSize.transform.localScale.x / 2) + (character.moneyDecalScaleX / 2),
           -0.2f,
           printerBaseSize.transform.position.z - (printerBaseSize.transform.localScale.z / 2) + (character.moneyDecalScaleY / 2));

        for (int i = 0; i < xQuantity; i++)
        {
            for (int j = 0; j < yQuantity; j++)
            {
                GameObject slot = Instantiate(editSlotPrefs, startPoint, editSlotPrefs.transform.rotation, transform);              
                slot.transform.localPosition = startPoint;
             
                currentGridElement.Add(slot);

                startPoint += new Vector3(0, 0, character.moneyDecalScaleY);
            }

            startPoint += new Vector3(character.moneyDecalScaleX, 0, (-yQuantity * character.moneyDecalScaleY));
        }
    }
}