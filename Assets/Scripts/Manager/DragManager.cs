using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    public static DragManager instance;

    private bool dragging = false;

    private Transform objectDrag;
    private Vector3 startPos;

    public LayerMask draggableMask;
    public LayerMask possiblePositionMask;
    public LayerMask inputDetectMask;

    Ray rayPos;
    RaycastHit hitPos;

    Transform targetPos = null;
    private bool haveTarget = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {

        if (!GameManager.instance.IsInGameStatus() && GameManager.instance.inEdit)
        {
            //IF DRAGGING AND DETECT A SLOT    
            if (dragging)
            {
                rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayPos, out hitPos, Mathf.Infinity, possiblePositionMask))
                {
                    if (hitPos.transform != null && hitPos.transform.CompareTag("Slot"))
                    {
                        targetPos = hitPos.transform;
                    }

                    else
                    {
                        targetPos = null;
                        haveTarget = false;
                    }
                }
                else
                {
                    targetPos = null;
                    haveTarget = false;
                }
            }

            //HAVE TARGET BOOL
            if (targetPos != null)
            {
                haveTarget = true;
            }

            if (Input.touchCount != 1)
            {
                dragging = false;
                return;
            }

            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;

            //startInput
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

                if (hit)
                {
                    //SET DRAGGABLE OBJECT
                    if (hitInfo.collider.transform.CompareTag("Draggable"))
                    {
                        objectDrag = hitInfo.collider.transform;
                      //  objectDrag.GetComponent<Collider>().enabled = false;
                        startPos = objectDrag.position;
                        dragging = true;
                    }
                }
            }

            //ACTUAL DRAG
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, inputDetectMask);
                if (hit)
                {
                    objectDrag.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                }
            }

            //END DRAG
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);

                //END INPUT IN GRID SLOT
                if (haveTarget && targetPos != null)
                {
                    objectDrag.position = targetPos.position;
                    //objectDrag.GetComponent<Collider>().enabled = true;

                    dragging = false;                   
                }

                /*
                //merge
                else if (haveTarget && targetPos != null
                    && objectDrag.GetComponent<EditObjectBehaviour>() != null
                    && CheckCanMerge())
                {
                    GameObject standingElement = ReturnCanMerge();

                    //increase old element value
                    standingElement.transform.GetComponent<MultiplyBehaviour>().Merge();

                    //destroy moving one
                    Destroy(objectDrag.gameObject);
                    dragging = false;

                    CharacterShooterBehaviour.instance.CheckShootingRow();
                }
                */

                //END INPUT AND RETURN
                else
                {
                    objectDrag.position = startPos;
                    objectDrag.GetComponent<Collider>().enabled = true;
                    dragging = false;
                }
            }
        }
    }

    /*
    private bool CheckCanMerge()
    {
        bool canMerge = false;

        RaycastHit hitInfo;
        Vector3 pos = Input.mousePosition;

        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.transform.GetComponent<MergerBehaviour>() != null
            && hitInfo.transform.GetComponent<MultiplyBehaviour>().value == objectDrag.transform.GetComponent<MultiplyBehaviour>().value)
        {
            canMerge = true;
        }


        return canMerge;
    }

    private GameObject ReturnCanMerge()
    {
        GameObject obj = null;

        RaycastHit hitInfo;
        Vector3 pos = Input.mousePosition;

        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.transform.GetComponent<MergerBehaviour>() != null
            && hitInfo.transform.GetComponent<MultiplyBehaviour>().value == objectDrag.transform.GetComponent<MultiplyBehaviour>().value)
        {
            obj = hitInfo.transform.gameObject;
        }

        return obj;
    }
    */
}
