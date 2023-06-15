using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{  
    public static InventoryManager Instance;

    public GameObject inventoryParent;
    public GameObject inventoryBgParent;
    public LayerMask moneyLayerMask;

    public InventoryEditGrid grid;

    public List<int> gridElementValue = new List<int>();
    public List<Vector3> gridElementPos = new List<Vector3>();

    private void Awake()
    {
        Instance = this;
        LoadInvValue();
    }

    private void Start()
    {
        foreach (GameObject o in grid.currentGridElement)
        {
            int myValue = 0;

            for (int i = 0; i < gridElementPos.Count; i++)
            {
                if (o.transform.localPosition == gridElementPos[i])
                {
                    myValue = gridElementValue[i];
                    break;
                }
            }

            GameObject edit = Instantiate(CharacterBehaviour.instance.editObject, o.transform.position, o.transform.rotation, inventoryParent.transform);
            edit.transform.localPosition = o.transform.localPosition;
            edit.GetComponent<EditObjectBehaviour>().Setup(myValue);

            //BG
            GameObject bg = Instantiate(CharacterBehaviour.instance.editBG, o.transform.localPosition, Quaternion.identity, inventoryBgParent.transform);
            bg.transform.localPosition = o.transform.localPosition + new Vector3(0, -0.03f, 0);
            bg.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

    }

    public Transform ReturnTransformParent(GameObject slot)
    {
        foreach (GameObject g in grid.currentGridElement)
        {
            if(slot.gameObject == g)
                return inventoryParent.transform;
        }        

        return CharacterBehaviour.instance.shootElementParent.transform;
    }

    public bool HaveFreeSlot()
    {
        if (inventoryParent.transform.childCount < 4)
            return true;
        else 
            return false;
    }

    public GameObject ReturnFirstFreePos()
    {
        foreach(GameObject slot in grid.currentGridElement)
        {
            RaycastHit hit;
            Vector3 pos = slot.transform.position + slot.transform.TransformDirection(Vector3.up) * 3;

            if (!Physics.Raycast(pos, slot.transform.TransformDirection(Vector3.down), out hit, 5, moneyLayerMask))
            {
                return slot;
            }
        }

        return null;
    }

    public void GenerateNewMoney()
    {
        if (HaveFreeSlot())
        {
            Vector3 pos = ReturnFirstFreePos().transform.localPosition;

            int myValue = 1;

            gridElementPos.Add(pos);
            gridElementValue.Add(myValue);

            GameObject edit = Instantiate(CharacterBehaviour.instance.editObject, pos, new Quaternion(0,0,0,0) , inventoryParent.transform);
            edit.transform.localPosition = pos;
            edit.GetComponent<EditObjectBehaviour>().Setup(myValue);
        
            SaveInvValue();
        }
    }

    public void SaveInvValue()
    {
        int[] valueToSave = new int[inventoryParent.transform.childCount];
        Vector3[] posToSave = new Vector3[inventoryParent.transform.childCount];

        for (int i = 0; i < inventoryParent.transform.childCount; i++)
        {
            valueToSave[i] = inventoryParent.transform.GetChild(i).GetComponent<EditObjectBehaviour>().value;
            posToSave[i] = inventoryParent.transform.GetChild(i).transform.localPosition;
        }

        PlayerPrefs.DeleteKey("SavedInvValue");
        PlayerPrefs.SetString("SavedInvValue", string.Join("###", valueToSave));

        string posToSaveString = SerializeVector3Array(posToSave);
        PlayerPrefs.DeleteKey("SavedInvPos");
        PlayerPrefs.SetString("SavedInvPos", posToSaveString);
    }

    public void LoadInvValue()
    {
        int xQuantity = 2;
        int yQuantity = 2;

        if (PlayerPrefs.HasKey("SavedInvValue") && PlayerPrefs.HasKey("SavedInvPos"))
        {
            //LoadValue
            string[] tempValue = PlayerPrefs.GetString("SavedInvValue").Split(new[] { "###" }, StringSplitOptions.None);

            if (tempValue[0] != "")
            {
                if (tempValue.Length >= 1)
                    for (int i = 0; i < tempValue.Length; i++)
                    {
                        gridElementValue.Add(int.Parse(tempValue[i]));
                    }

                //LoadPos
                string posStringNotSplitted = PlayerPrefs.GetString("SavedInvPos");
                Vector3[] allPosSplitted = DeserializeVector3Array(posStringNotSplitted);
                if (allPosSplitted.Length >= 1)
                    for (int i = 0; i < allPosSplitted.Length; i++)
                    {
                        gridElementPos.Add(allPosSplitted[i]);
                    }
            }
        }
    }

    public static string SerializeVector3Array(Vector3[] aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public static Vector3[] DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }    
}