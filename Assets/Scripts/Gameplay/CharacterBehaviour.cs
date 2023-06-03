using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Globalization;
using System;
using System.Linq;
using Unity.Mathematics;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEditor;

public class CharacterBehaviour : MonoBehaviour
{
    public static CharacterBehaviour instance;

    public GameObject model;
    public GameObject handler;
    public GameObject printerObject;   

    public LayerMask moneyMachineButtonLayer;

    public float moveXLimit;
    public float moveSpeed;
    public float jumpHeight;
    public float jumpSpeed;

    public float bulletActiveTime;

    public float moneyDecalScaleX;
    public float moneyDecalScaleY;

    public Vector3 editPositionOffset;
    public Vector3 editRotationOffset;
    public float editAnimSpeed;

    private Vector3 startPos;
    private quaternion startRot;

    private Coroutine jumpCoroutine;

    //***************** PLAYER MOVE ***********************
    private bool moveByTouch;
    private Vector3 mouseStartPos, playerStartPos;
    private Camera camera;

    //***************** LOCAL VARIABLE  ***********************
    private Vector2 printerObjectScale;
    private Rigidbody rb;
    private Collider col;

    private Coroutine dieCoroutine;

    [HideInInspector]
    public bool moving;
    private float notMovingJumpSpeed;
    private float movingJumpSpeed;

    [HideInInspector]
    public float maxEnergy = 2.5f;
    [HideInInspector]
    public float currentEnergy;
    private float energyConsumption = 1.25f;
    private float energyIncreaseValue = 0.4f;

    //***************** EDIT  ***********************
    public GameObject editBG;
    public GameObject editObject;
    public GameObject editObjectParent;
    public GameObject editBGParent;
    public CharacterEditGrid characterGrid;
    public List<int> gridElementValue = new List<int>();
    public List<Vector3> gridElementPos = new List<Vector3>();
    public List<EditObjectBehaviour> editObjectList = new List<EditObjectBehaviour> ();

    private void Awake()
    {
        instance = this;

        dieCoroutine = null;
        jumpCoroutine = null;
        moving = true;

        startPos = transform.position;
        startRot = transform.rotation;

        Vector3 startScale = new Vector3(0, 1, 0);

        if (PlayerPrefs.HasKey("Xsize"))
            startScale.x = PlayerPrefs.GetFloat("Xsize");
        else
        {
            startScale.x = 3f;
            PlayerPrefs.SetFloat("Xsize", startScale.x);
        }

        if (PlayerPrefs.HasKey("Zsize"))
            startScale.z = PlayerPrefs.GetFloat("Zsize");
        else
        {
            startScale.z = 0.75f;
            PlayerPrefs.SetFloat("Zsize", startScale.z);
        }

        printerObject.transform.localScale = startScale;
        printerObjectScale = new Vector2(printerObject.transform.localScale.x, printerObject.transform.localScale.z);

        LoadPlayerValue();
    }


    private void Start()
    {
        camera = Camera.main;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        notMovingJumpSpeed = jumpSpeed / 2;
        movingJumpSpeed = jumpSpeed;

        currentEnergy = maxEnergy / 2;

        
        foreach(GameObject o in characterGrid.currentGridElement)
        {
            int myValue = 0;           

            for (int i = 0; i < gridElementPos.Count; i++)
            {
                if(o.transform.localPosition == gridElementPos[i])
                {
                    myValue = gridElementValue[i];
                    break;
                }
            }

            GameObject edit = Instantiate(editObject, o.transform.position, o.transform.rotation, editObjectParent.transform);
            editObjectList.Add(edit.GetComponent<EditObjectBehaviour>());
            edit.GetComponent<EditObjectBehaviour>().Setup(myValue);

            //BG
            GameObject bg = Instantiate(editBG, o.transform.position + new Vector3(0 , 0.05f , 0), o.transform.rotation, editBGParent.transform);
        }       
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.playGame, OnPlayGame);
        EventManager.StartListening(Events.endGame, OnEndGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.playGame, OnPlayGame);
        EventManager.StopListening(Events.endGame, OnEndGame);
    }

    private void Update()
    {
        if (GameManager.instance.IsInGameStatus())
        {
            if (moving)            
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            if (moving && jumpSpeed != movingJumpSpeed)
                jumpSpeed = movingJumpSpeed;
            else if (!moving && jumpSpeed != notMovingJumpSpeed)
                jumpSpeed = notMovingJumpSpeed;

            transform.position = new Vector3(Mathf.Clamp(CharacterBehaviour.instance.transform.position.x, -moveXLimit, moveXLimit), CharacterBehaviour.instance.transform.position.y, CharacterBehaviour.instance.transform.position.z);

            if (moving)
            {
                currentEnergy += energyIncreaseValue * Time.deltaTime; // Cap at some max value too
            }            
            if (!moving)
            {
                currentEnergy -= energyConsumption * Time.deltaTime; // Cap at some min value too
            }
            
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
    }

    public void Move(Vector3 direction)
    {
        if(GameManager.instance.IsInGameStatus())       
            transform.Translate(direction * Time.deltaTime * moveSpeed) ;
    }

    public void StartEdit()
    {
        transform.DOMove(editPositionOffset, editAnimSpeed);
        transform.DORotate(editRotationOffset , editAnimSpeed);
    }
    public void ConfirmEdit()
    {
        transform.DOMove(startPos, editAnimSpeed);
        transform.DORotateQuaternion(startRot, editAnimSpeed)
            .OnComplete(()=> GameManager.instance.inEdit = false);
    }

    private void PrintDecal()
    {
        for (int i = 0; i < editObjectList.Count; i++)
        {
            editObjectList[i].Print();
        }  

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(handler.transform.DOScaleY(0.7f, jumpSpeed / 2));

        mySequence.Append(handler.transform.DOScaleY(1, jumpSpeed / 2));        
    }

    private void OnTriggerEnter(Collider coll)
    {
        /*
        if (coll.transform.CompareTag("Wall"))
        {
            WallBehaviour thisWall = coll.transform.GetComponent<WallBehaviour>();

            switch (thisWall.sizeModifier)
            {
                case SizeModifier.Width:

                    if (thisWall.positive)
                        printerObjectScale += new Vector2(1, 0);
                    else
                        printerObjectScale -= new Vector2(1, 0);

                    ApplyPrinterScale();

                    break;

                case SizeModifier.lenght:

                    if (thisWall.positive)
                        printerObjectScale += new Vector2(0, 1);
                    else
                        printerObjectScale -= new Vector2(0, 1);

                    ApplyPrinterScale();

                    break;
            }
             
            thisWall.transform.GetComponent<Collider>().enabled = false;

            //WallCol
            coll.enabled = false;
            
            col.enabled = false;
            StartCoroutine(ReEnableCol());
        }
        */

        if (coll.transform.CompareTag("EndElement"))
        {
            EventManager.TriggerEvent(Events.endGame);
        }
    }

    public void Die()
    {
        if (dieCoroutine == null)
        {
            dieCoroutine = StartCoroutine(StartDieCoroutine());
        }
    }

    private void ApplyPrinterScale()
    {
        if (printerObjectScale.x >= 1 && printerObjectScale.y >= 1)
        {
            printerObject.transform.DOScale(new Vector3(printerObjectScale.x, printerObject.transform.localScale.y, printerObjectScale.y), 0.5f)
                .SetEase(Ease.OutSine);
        }

        else
        {
            if (dieCoroutine == null)
            {
               dieCoroutine =  StartCoroutine(StartDieCoroutine());
            }
        }
    }

    private IEnumerator StartDieCoroutine()
    {
        handler.transform.parent = null;
        printerObject.transform.parent = null;

        col.enabled = false;

        EnablePhysics(handler.transform.GetComponent<Rigidbody>() , handler.transform.GetComponent<Collider>());
        EnablePhysics(printerObject.transform.GetComponent<Rigidbody>(), printerObject.transform.GetComponent<Collider>());

        yield return new WaitForSeconds(0.5f);

        EventManager.TriggerEvent(Events.die);
    }

    private IEnumerator JumpCoroutine()
    {
        while (GameManager.instance.IsInGameStatus())
        {         
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(model.transform.DOLocalMoveY(transform.localPosition.y + jumpHeight, jumpSpeed/2)               
                .SetEase(Ease.InOutSine));

            mySequence.Append(model.transform.DOLocalMoveY(0, jumpSpeed / 2)                          
                .SetEase(Ease.InOutSine)
                .OnComplete(PrintDecal));

            yield return new WaitForSeconds(jumpSpeed + 0.05f);
        }
    }

    private void OnPlayGame(object sender)
    {
        if (jumpCoroutine == null)
            jumpCoroutine = StartCoroutine(JumpCoroutine());

        moving = true;
    }

    private void OnEndGame(object sender)
    {
        if(jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;   
        }

        moving = false;
    }

    private void EnablePhysics(Rigidbody rig , Collider colli)
    {
        rig.isKinematic = false;
        rig.constraints = RigidbodyConstraints.None;
        rig.useGravity = true;

        colli.enabled = true;

        rig.AddTorque(transform.forward * 10);
        rig.AddForce(transform.forward * 300);
    }

    private IEnumerator ReEnableCol()
    {
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
    }


    public void SavePlayerValue()
    {       
        int[] valueToSave = new int[editObjectParent.transform.childCount];
        Vector3[] posToSave = new Vector3[editObjectParent.transform.childCount];

        for(int i = 0; i < editObjectParent.transform.childCount; i++)
        {
            valueToSave[i] = editObjectParent.transform.GetChild(i).GetComponent<EditObjectBehaviour>().value;
            posToSave[i] = editObjectParent.transform.GetChild(i).transform.localPosition;
        }

        PlayerPrefs.DeleteKey("SavedValue");
        PlayerPrefs.SetString("SavedValue", string.Join("###", valueToSave));

        string posToSaveString = SerializeVector3Array(posToSave);
        PlayerPrefs.DeleteKey("SavedPos");
        PlayerPrefs.SetString("SavedPos", posToSaveString);
    }

    public void LoadPlayerValue()
    {
        int xQuantity = (int)(printerObject.transform.localScale.x / moneyDecalScaleX);
        int yQuantity = (int)(printerObject.transform.localScale.z / moneyDecalScaleY);

        if (PlayerPrefs.HasKey("SavedValue") && PlayerPrefs.HasKey("SavedPos"))
        {
            //LoadValue
            string[] tempValue = PlayerPrefs.GetString("SavedValue").Split(new[] { "###" }, StringSplitOptions.None);
            if(tempValue.Length >= 1)
            for (int i = 0; i < tempValue.Length; i++)
            {
                gridElementValue.Add(int.Parse(tempValue[i]));
            }

            //LoadPos
            string posStringNotSplitted = PlayerPrefs.GetString("SavedPos");
            Vector3[] allPosSplitted = DeserializeVector3Array(posStringNotSplitted);
            if(allPosSplitted.Length >= 1)
            for(int i = 0; i < allPosSplitted.Length; i++)
            {
                gridElementPos.Add(allPosSplitted[i]);
            }
        }

        else
        {
            Vector3 startPoint = new Vector3(          
                printerObject.transform.position.x - (printerObjectScale.x / 2) + (moneyDecalScaleX / 2),          
                0,          
                printerObject.transform.position.z - (printerObjectScale.y / 2) + (moneyDecalScaleY / 2));

            List<int> tempValue = new List<int>();
            List<float> tempX = new List<float>();
            List<float> tempZ = new List<float>();

            for (int i = 0; i < xQuantity; i++)
            {
                for (int j = 0; j < yQuantity; j++)
                {
                    tempValue.Add(1);
                    tempX.Add(startPoint.x);
                    tempZ.Add(startPoint.z);

                    startPoint += new Vector3(0, 0, moneyDecalScaleY);
                }

                startPoint += new Vector3(moneyDecalScaleX, 0, (-yQuantity * moneyDecalScaleY));
            }

            int[] valueToSave = new int[tempValue.Count];
            Vector3[] posToSave = new Vector3[tempX.Count];

            for (int i = 0; i < valueToSave.Length; i++)
            {
                valueToSave[i] = tempValue[i];
                gridElementValue.Add(tempValue[i]);

                posToSave[i] = new Vector3(tempX[i], -0.1f, tempZ[i]);
                gridElementPos.Add(posToSave[i]);
            }

            string posToSaveString = SerializeVector3Array(posToSave);          

            PlayerPrefs.SetString("SavedValue", string.Join("###", valueToSave));
            PlayerPrefs.SetString("SavedPos", posToSaveString);
        }
    }

    public static string SerializeVector3Array(Vector3[] aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public static Vector3[] DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }
}