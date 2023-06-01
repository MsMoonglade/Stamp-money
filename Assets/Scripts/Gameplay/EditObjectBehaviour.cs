using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EditObjectBehaviour : MonoBehaviour
{
    public MeshRenderer renderer;

    [HideInInspector]
    public int value;

    public void Setup(int val)
    {
        value = val;
        SetMaterial(value);
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
        renderer.material = ColorUtilities.instance.GetEditObjectMaterial(val);
    }
}
