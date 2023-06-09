using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EditObjectBehaviour : MonoBehaviour
{
    public MeshRenderer renderer;

    public int value;


    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    public void Setup(int val)
    {        
        if(val == 0)
        {
            CharacterBehaviour.instance.editObjectList.Remove(this.GetComponent<EditObjectBehaviour>());
            Destroy(this.gameObject);
        }       

        value = val;
        SetMaterial(value);
    }
    public void Print()
    {
        Vector3 pos = new Vector3(transform.position.x, 0.01f, transform.position.z);
        GameObject decal = PoolManager.instance.GetItem(GameManager.instance.moneyDecalObj, pos, GameManager.instance.moneyDecalParent);

        decal.GetComponent<MoneyBulletBehaviour>().SetValue(value);
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
        Material[] mats = new Material[2];

        mats[0] = ColorUtilities.instance.GetEditObjectMaterialSide(val);
        mats[1] = ColorUtilities.instance.GetEditObjectMaterial(val);

        renderer.materials = mats;
    }
}
