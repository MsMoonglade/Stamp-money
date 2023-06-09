using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EndGameCharacterBehaviour : MonoBehaviour
{
    public static EndGameCharacterBehaviour instance;

    public float moveSpeed;

    public GameObject diamondUi;
    public TMP_Text diamondText;
    public GameObject diamondParent;

    public Animator anim;

    public bool canMove ;

    private NavMeshAgent agent;
    private void Awake()
    {
        instance = this;

        canMove = false;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        diamondText.text = ((diamondParent.transform.childCount - 1).ToString());
    }

    public void Move(Vector3 direction)
    {
        if (canMove) {
            // agent.Move(direction * agent.speed * Time.deltaTime);

            //   transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

            //transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 1);

            anim.SetBool("Run", true);

            agent.Move(direction * agent.speed * Time.deltaTime);

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 1);
            /*
            if (!anim.GetBool("Run") && direction.magnitude >= 0.2f)
            {
                anim.SetBool("Run", true);
            }
            */
        }
    }

    public void StopAnim()
    {
        anim.SetBool("Run", false);
    }

    public int CurrentDiamond()
    {
        return diamondParent.transform.childCount - 1;
    }

    public void RemoveDiamond(int amount , GameObject destination)
    {
        List<GameObject> list = new List<GameObject>();
       
        for (int i = diamondParent.transform.childCount -1 ; i > diamondParent.transform.childCount -1 - amount; i--)
        {
            list.Add(diamondParent.transform.GetChild(i).gameObject);
        }

        StartCoroutine(RemoveDiamondWithDelay(list, destination));
    }

    public GameObject TakeOneDiamond()
    {
        GameObject lastDiamond = null;

        if (CurrentDiamond() >= 1)
        {          
            lastDiamond = diamondParent.transform.GetChild(diamondParent.transform.childCount - 1).gameObject;
        }

        return lastDiamond;
    }

    public void RemoveDiamond(int amount)
    {
        List<GameObject> list = new List<GameObject>();       
          
        list.Add(diamondParent.transform.GetChild(diamondParent.transform.childCount - 1).gameObject);        

        foreach (GameObject go in list)
        {
            go.transform.parent = null;
            go.SetActive(false);
        }
    }

    public void SetEdit()
    {
        canMove = false;
    }

    public void ExitEdit()
    {
        canMove = true;
    }

    private IEnumerator RemoveDiamondWithDelay(List<GameObject> list , GameObject dest)
    {
        foreach (GameObject go in list)
        {
            go.transform.parent = null;
            go.transform.DOMove(dest.transform.position, 0.5f)
                .OnComplete(() => go.SetActive(false));

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForEndOfFrame();
    }
}