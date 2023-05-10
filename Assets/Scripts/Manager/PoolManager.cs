using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager instance;

    [HideInInspector]
    public List<GameObject> decalInPool = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetItem(GameObject obj, Vector3 pos, GameObject parent)
    {
        bool spawned = false;

        for (int i = 0; i < decalInPool.Count; i++)
        {
            if (decalInPool[i].gameObject.name.Contains(obj.name) && !decalInPool[i].activeInHierarchy)
            {
                decalInPool[i].transform.parent = parent.transform;
                decalInPool[i].transform.position = pos;
                decalInPool[i].SetActive(true);
                spawned = true;

                return decalInPool[i];
            }
        }

        return null;
    }    

    public void InstantiateInPool(GameObject obj, GameObject parent)
    {
        GameObject inst = Instantiate(obj, parent.transform.position, obj.transform.rotation, parent.transform);
        decalInPool.Add(inst);
        inst.SetActive(false);
    }

    public void ClearPool()
    {
        decalInPool.Clear();
    }
}