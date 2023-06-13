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

    [HideInInspector]
    public bool canMove = false;

    private NavMeshAgent agent;
    private void Awake()
    {
        instance = this;

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
}