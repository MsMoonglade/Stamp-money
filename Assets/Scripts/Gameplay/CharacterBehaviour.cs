using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using Unity.Mathematics;
using System.Text;
using UnityEditor;
using Lofelt.NiceVibrations;

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

    public ParticleSystem powerUpParticle;
    public ParticleSystem powerDownParticle;

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

    public float maxEnergy = 2.5f;
    [HideInInspector]
    public float currentEnergy;
    private float energyConsumption = 1f;
    private float energyIncreaseValue = 0.5f;

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

        //JumpSped
        if (PlayerPrefs.HasKey("JumpSpeed"))
            jumpSpeed = PlayerPrefs.GetFloat("JumpSpeed");
        else
        {            
            PlayerPrefs.SetFloat("JumpSpeed", jumpSpeed);
        }

        printerObject.transform.localScale = startScale;
        printerObjectScale = new Vector2(printerObject.transform.localScale.x, printerObject.transform.localScale.z);

        LoadPlayerValue();

        currentEnergy = maxEnergy / 2;
    }


    private void Start()
    {
        camera = Camera.main;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        notMovingJumpSpeed = jumpSpeed / 2;
        movingJumpSpeed = jumpSpeed;
        
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
        if (GameManager.instance.IsInGameStatus() && dieCoroutine == null)
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
        if (DeviceCapabilities.isVersionSupported)
        {
            if (DeviceCapabilities.meetsAdvancedRequirements)
            {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            }
        }

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
        if (coll.transform.CompareTag("EndElement"))
        {
            EndGameBehaviour.instance.StartEndGame();
        }
    }

    public void Die()
    {
        if (dieCoroutine == null)
        {
            dieCoroutine = StartCoroutine(StartDieCoroutine());
        }
    }

    public void ApplyPrinterScale(Vector3 newScale, bool right)
    {
        printerObject.transform.DOScale(newScale, 0.5f)
            .SetEase(Ease.OutSine);

        PlayerPrefs.SetFloat("Xsize", newScale.x);
        PlayerPrefs.SetFloat("Zsize", newScale.z);

        printerObjectScale = new Vector2(newScale.x, newScale.z);

        for (int i = 0; i < gridElementPos.Count; i++)
        {
            Vector3 fixedPos = gridElementPos[i];

            if (right)
                fixedPos += new Vector3(-0.75f, 0, 0);

            else
                fixedPos += new Vector3(0, 0, 0.75f/2);

            gridElementPos[i] = fixedPos;
        }

        foreach(EditObjectBehaviour o in editObjectList)
        {
            Vector3 fixedPos = o.transform.localPosition;

            if (right)
                fixedPos += new Vector3(-0.75f, 0, 0);

            else
                fixedPos += new Vector3(0, 0, 0.75f / 2);

            o.transform.DOLocalMove(fixedPos, 0.5f);
        }

        foreach (Transform o in editBGParent.transform)
        {
            o.transform.DOScale(Vector3.zero, 0.2f);
        }

        Invoke("RegenerateGridSlotAfterSizeChange", 0.55f);

        SavePlayerValue();
    }

    private void RegenerateGridSlotAfterSizeChange()
    {
        characterGrid.RemakeSlot();
        foreach (GameObject o in characterGrid.currentGridElement)
        {
            GameObject bg = Instantiate(editBG, o.transform.position + new Vector3(0, 0.05f, 0), o.transform.rotation, editBGParent.transform);
            bg.transform.localPosition = o.transform.localPosition;
            bg.transform.localScale = Vector3.zero;
            bg.transform.DOScale(Vector3.one, 0.2f);
        }
    }

    private IEnumerator StartDieCoroutine()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }

        handler.transform.parent = null;
        printerObject.transform.parent = null;

        editObject.transform.DOScale(Vector3.zero, 0.25f);

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

    public void EditFireRateInLocalGame(float amount)
    {
        jumpSpeed -= amount;

        if (jumpSpeed <= 0.15f)
            jumpSpeed = 0.15f;

        if(amount > 0)      
            powerUpParticle.Play();
       else
            powerDownParticle.Play();

        notMovingJumpSpeed = jumpSpeed / 2;
        movingJumpSpeed = jumpSpeed;
    }

    public void IncreaseJumpSpeed(float increaseAmount)
    {
        jumpSpeed -= increaseAmount;
        
        if (jumpSpeed <= 0.15f)
            jumpSpeed = 0.15f;

        PlayerPrefs.SetFloat("JumpSpeed", jumpSpeed);

        notMovingJumpSpeed = jumpSpeed / 2;
        movingJumpSpeed = jumpSpeed;
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

        LoadPlayerValue();
    }

    public void LoadPlayerValue()
    {
        int xQuantity = (int)(printerObject.transform.localScale.x / moneyDecalScaleX);
        int yQuantity = (int)(printerObject.transform.localScale.z / moneyDecalScaleY);

        gridElementPos.Clear();
        gridElementValue.Clear();

        if (PlayerPrefs.HasKey("SavedValue") && PlayerPrefs.HasKey("SavedPos"))
        {
            //LoadValue
            string[] tempValue = PlayerPrefs.GetString("SavedValue").Split(new[] { "###" }, StringSplitOptions.None);

            if (tempValue[0] != "")
            {
                if (tempValue.Length >= 1)
                    for (int i = 0; i < tempValue.Length; i++)
                    {
                        gridElementValue.Add(int.Parse(tempValue[i]));
                    }

                //LoadPos
                string posStringNotSplitted = PlayerPrefs.GetString("SavedPos");
                Vector3[] allPosSplitted = DeserializeVector3Array(posStringNotSplitted);
                if (allPosSplitted.Length >= 1)
                    for (int i = 0; i < allPosSplitted.Length; i++)
                    {
                        gridElementPos.Add(allPosSplitted[i]);
                    }
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