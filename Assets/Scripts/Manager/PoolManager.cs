using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager instance;

    [HideInInspector]
    public List<GameObject> arrowInPool = new List<GameObject>();


    private void Awake()
    {
        instance = this;
    }

    public GameObject GetItem(GameObject obj, Vector3 pos, GameObject parent)
    {
        bool spawned = false;

        for (int i = 0; i < arrowInPool.Count; i++)
        {
            if (arrowInPool[i].gameObject.name.Contains(obj.name) && !arrowInPool[i].activeInHierarchy)
            {
                arrowInPool[i].transform.parent = parent.transform;
                arrowInPool[i].transform.position = pos;
                arrowInPool[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                arrowInPool[i].GetComponent<SphereCollider>().enabled = true;
                arrowInPool[i].SetActive(true);
                spawned = true;

                return arrowInPool[i];
                break;
            }
        }

        if (!spawned)
        {
            StartCoroutine(ShootSpawned(obj, parent));
            return null;
        }

        return null;
    }    

    public void InstantiateInPool(GameObject obj, GameObject parent)
    {
        GameObject inst = Instantiate(obj, parent.transform.position, Quaternion.identity, parent.transform);
        arrowInPool.Add(inst);
        inst.SetActive(false);
    }

    public void ClearPool()
    {
        arrowInPool.Clear();
    }

    public bool HaveActiveBullet()
    {
        foreach (GameObject b in arrowInPool)
        {
            if (b.activeSelf)
                return true;
        }

        return false;
    }

    public bool ObjectActive()
    {
        for (int i = 0; i < arrowInPool.Count; i++)
        {
            if (arrowInPool[i].gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator ShootSpawned(GameObject obj, GameObject parent)
    {
        InstantiateInPool(obj, parent);
        yield return null;
        GetItem(obj, parent.transform.position, parent);
    }
}