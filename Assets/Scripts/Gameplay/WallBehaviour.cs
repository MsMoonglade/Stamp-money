using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    public Material positiveMat;
    public Material negativeMat;

    public MeshRenderer renderer;

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
        }
        else
        {
            positive = false;
            renderer.material = negativeMat;
        }
    }
}


public enum SizeModifier
{
    Width , 
    lenght
}