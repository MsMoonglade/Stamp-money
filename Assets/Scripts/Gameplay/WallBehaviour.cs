using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallBehaviour : MonoBehaviour
{
    public Material positiveMat;
    public Material negativeMat;

    public MeshRenderer renderer;

    public GameObject H_Expand;
    public GameObject H_Reduce;
    public GameObject V_Expand;
    public GameObject V_Reduce;


    [HideInInspector]
    public SizeModifier sizeModifier;
    [HideInInspector]
    public bool positive;

    private void Awake()
    {
        RandomizeModifier();
    }


    private void RandomizeModifier()
    {
        sizeModifier = (SizeModifier)Random.Range(0, 2);

        if (Random.value >= 0.5)
        {
            positive = true;
            renderer.material = positiveMat;

            if(sizeModifier == SizeModifier.Width)            
                H_Expand.SetActive(true);
            else
                V_Expand.SetActive(true);
            
        }
        else
        {
            positive = false;
            renderer.material = negativeMat;

            if (sizeModifier == SizeModifier.Width)
                H_Reduce.SetActive(true);
            else
                V_Reduce.SetActive(true);
        }
    }
}


public enum SizeModifier
{
    Width , 
    lenght
}