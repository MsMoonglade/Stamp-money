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
                    if (hitPos.collider.transform != null && hitPos.collider.transform.CompareTag("Slot"))
                    {
                        targetPos = hitPos.collider.transform;
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
                        objectDrag.GetComponent<Collider>().enabled = false;
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
                if (haveTarget && targetPos != null && GridSlotFree()  && !CheckCanMerge())
                {
                    objectDrag.transform.parent = InventoryManager.Instance.ReturnTransformParent(targetPos.gameObject);

                    if(objectDrag.transform.parent == InventoryManager.Instance.inventoryParent.transform)                 
                        CharacterBehaviour.instance.editObjectList.Remove(objectDrag.GetComponent<EditObjectBehaviour>());

                    objectDrag.localPosition = targetPos.localPosition;
                    objectDrag.GetComponent<Collider>().enabled = true;

                    dragging = false;                   
                }
                
                //END INPUT CHECK FOR MERGE
                else if (haveTarget && targetPos != null
                    && objectDrag.GetComponent<EditObjectBehaviour>() != null
                    && CheckCanMerge())
                {
                    GameObject mergedObj = ReturnMergedObject();

                    //increase old element value
                    mergedObj.transform.GetComponent<EditObjectBehaviour>().IncreaseValue();

                    //destroy moving one
                    CharacterBehaviour.instance.editObjectList.Remove(objectDrag.GetComponent<EditObjectBehaviour>());
                    Destroy(objectDrag.gameObject);
                    dragging = false;
                }                
                
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

    private bool GridSlotFree()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, draggableMask);
        if (hit)
        {
            return false;
        }

        return true;
    }
    
    
    private bool CheckCanMerge()
    {
        bool canMerge = false;

        RaycastHit hitInfo;
        Vector3 pos = Input.mousePosition;

        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.collider.transform.GetComponent<EditObjectBehaviour>() != null
            && hitInfo.collider.transform.GetComponent<EditObjectBehaviour>().value == objectDrag.transform.GetComponent<EditObjectBehaviour>().value)
        {
            canMerge = true;
        }

        return canMerge;
    }

    private GameObject ReturnMergedObject()
    {
        GameObject obj = null;

        RaycastHit hitInfo;
        Vector3 pos = Input.mousePosition;

        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.collider.transform.GetComponent<EditObjectBehaviour>() != null
            && hitInfo.collider.transform.GetComponent<EditObjectBehaviour>().value == objectDrag.transform.GetComponent<EditObjectBehaviour>().value)
        {
            obj = hitInfo.collider.transform.gameObject;
        }

        return obj;
    }   
}
