using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MoneyPrinterBehaviour : MonoBehaviour
{
    public TMP_Text currentText;

    public int minTakableMoney;
    public int maxTakableMoney;

    public GameObject takableParent;
    public GameObject takenParent;
    public ParticleSystem moneyParticle;

    public GameObject moneyPrefs;
    public float positionYOffset;

    private bool destoryed = false;
    [HideInInspector]
    public GameObject mybutton;

    private Rigidbody rb;

    private int takableMoney;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        takableMoney = Random.Range(minTakableMoney, maxTakableMoney);

        currentText.text = takableMoney.ToString();

        for(int i = 0; i < takableMoney; i++)
        {
            GameObject money = Instantiate(moneyPrefs, takableParent.transform.position, moneyPrefs.transform.rotation, takableParent.transform);
            money.transform.position += new Vector3(0, i * positionYOffset, 0);
        }
    }

    private void Update()
    {
        if(takableParent.transform.childCount == 0 && !destoryed)
        {
            DisablePrinter();
        }
    }

    public void TakeMoney()
    {
        if (!destoryed)
        {
            for (int i = 0; i < takableParent.transform.childCount; i++)
            {
                takableParent.transform.GetChild(i).transform.DOLocalMoveY(i * positionYOffset, 0.4f);
            }

            GameObject currentMoney = takableParent.transform.GetChild(0).gameObject;

            currentMoney.transform.parent = takenParent.transform;
            currentMoney.transform.SetSiblingIndex(0);

            currentMoney.transform.localPosition = new Vector3(0 , 0 , 1.8f);
            currentMoney.transform.DOLocalMoveZ(0, 0.4f);

            currentText.text = takableParent.transform.childCount.ToString();

            for (int i = 0; i < takenParent.transform.childCount; i++)
            {
                takenParent.transform.GetChild(i).transform.DOLocalMoveY(i * positionYOffset, 0.4f);
            }
        }        
    }

    private void DisablePrinter()
    {
        destoryed = true;

        moneyParticle.Play();

        mybutton.transform.DOScale(Vector3.zero, 0.6f);
        mybutton.transform.GetComponent<Collider>().enabled = false;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;

        rb.AddTorque(transform.forward * 10);
        rb.AddForce(transform.forward * 100);

        foreach(Transform o in takenParent.transform)
        {
            Rigidbody rib = o.GetComponent<Rigidbody>();

            rib.isKinematic = false;
            rib.constraints = RigidbodyConstraints.None;
            rib.useGravity = true;

            rib.AddTorque(transform.forward * 10);
            rib.AddForce(transform.up * 50);
        }

        StartCoroutine(DestroyThisPrinter());

    }

    private IEnumerator DestroyThisPrinter()
    {
        yield return new WaitForSeconds(5);

        this.gameObject.SetActive(false);
    }
}