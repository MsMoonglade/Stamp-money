using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaterialFixer : MonoBehaviour
{ 
    void Awake()
    {
        transform.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1 , -(transform.localScale.z / 4));
    }
}
