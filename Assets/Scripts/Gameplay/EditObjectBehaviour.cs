using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EditObjectBehaviour : MonoBehaviour
{
    public MeshRenderer renderer;

    public int value;

    public void Setup(int val)
    {
        value = val;
       // SetMaterial(value);
    }

    public void IncreaseValue()
    {
        value++;
        SetMaterial(value);
    }

    public void DecreaseValue()
    {
        value--;
        SetMaterial(value);
    }

    public void SetMaterial(int val)
    {
        renderer.materials[0] = ColorUtilities.instance.GetEditObjectMaterialSide(val);
        renderer.materials[1] = ColorUtilities.instance.GetEditObjectMaterial(val);
    }
}