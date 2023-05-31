using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Globalization;
using System;
using System.Linq;

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

    public float moneyDecalScaleX;
    public float moneyDecalScaleY;

    public ParticleSystem particle;

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

    private int[,] localMoneyValue;

    private void Awake()
    {
        instance = this;

        dieCoroutine = null;
        jumpCoroutine = null;
        moving = true;

        Vector3 startScale = new Vector3(0, 1, 0);

        if (PlayerPrefs.HasKey("Xsize"))
            startScale.x = PlayerPrefs.GetFloat("Xsize");
        else
        {
            startScale.x = 3;
            PlayerPrefs.SetFloat("Xsize", startScale.x);
        }

        if (PlayerPrefs.HasKey("Zsize"))
            startScale.z = PlayerPrefs.GetFloat("Zsize");
        else
        {
            startScale.z = 1.5f;
            PlayerPrefs.SetFloat("Zsize", startScale.z);
        }

        printerObject.transform.localScale = startScale;
        printerObjectScale = new Vector2(printerObject.transform.localScale.x, printerObject.transform.localScale.z);

        int xQuantity = (int)(printerObject.transform.localScale.x / moneyDecalScaleX);
        int yQuantity = (int)(printerObject.transform.localScale.z / moneyDecalScaleY);
        
        localMoneyValue = new int[xQuantity, yQuantity];

        if (PlayerPrefs.HasKey("LocalMoneyValueX") && PlayerPrefs.HasKey("LocalMoneyValueZ"))
        {
            //Return X Value
            string[] localXValue = PlayerPrefs.GetString("LocalMoneyValueX").Split(new[] { "###" }, StringSplitOptions.None);
            for (int i = 0; i < localXValue.Length; i++)
            {
                localMoneyValue[i , 0] = int.Parse(localXValue[i]);
            }

            //Return Z Value
            string[] localZValue = PlayerPrefs.GetString("LocalMoneyValueZ").Split(new[] { "###" }, StringSplitOptions.None);
            for (int i = 0; i < localZValue.Length; i++)
            {
                localMoneyValue[0, i] = int.Parse(localZValue[i]);
            }
        }
        else
        {
            //Setup startXValue
            int[] startX = new int[xQuantity];
            for(int i = 0; i < startX.Length; i++)
            {
                startX[i] = 1;
                localMoneyValue[i, 0] = 1;
            }
            PlayerPrefs.SetString("LocalMoneyValueX", string.Join("###", startX));

            //Setup startZValue
            int[] startZ = new int[yQuantity];
            for (int i = 0; i < startZ.Length; i++)
            {
                startZ[i] = 1;
                localMoneyValue[0, i] = 1;
            }
            PlayerPrefs.SetString("LocalMoneyValueZ", string.Join("###", startZ));
        }
    }

    private void Start()
    {
        camera = Camera.main;

        rb = GetComponent<Rigidbody>(); 
        col = GetComponent<Collider>();

        notMovingJumpSpeed = jumpSpeed / 2;
        movingJumpSpeed = jumpSpeed;

        currentEnergy = maxEnergy / 2;       
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

            // MovePlayer();


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

    public void ConfirmEdit()
    {
        
    }

    private void PrintDecal()
    {
        particle.Play();

        int xQuantity =  (int)(printerObject.transform.localScale.x / moneyDecalScaleX);
        int yQuantity = (int)(printerObject.transform.localScale.z / moneyDecalScaleY);

        Vector3 startPoint = new Vector3(
            printerObject.transform.position.x - (printerObjectScale.x /2) + (moneyDecalScaleX /2) ,
            0.01f ,
            printerObject.transform.position.z - (printerObjectScale.y/2 ) + (moneyDecalScaleY / 2));

        for(int i = 0; i < xQuantity; i++)
        {
            for (int j = 0; j < yQuantity; j++)
            {               
                bool haveButton = false;

                //print in money machine button
                RaycastHit hit;
                if (Physics.Raycast(startPoint + new Vector3(0, 1, 0), Vector3.down, out hit, 5, moneyMachineButtonLayer))
                {
                    hit.transform.GetComponent<MoneyPrinterButton>().Print();
                
                    haveButton = true;
                }

                //print normal decal in road
                if (!haveButton)
                {
                    GameObject decal = PoolManager.instance.GetItem(GameManager.instance.moneyDecalObj, startPoint, GameManager.instance.moneyDecalParent);
                 
                    decal.GetComponent<MoneyBulletBehaviour>().SetValue(localMoneyValue[i,j]);
                }              

                startPoint += new Vector3(0, 0, moneyDecalScaleY);
            }

            startPoint += new Vector3(moneyDecalScaleX, 0, (-yQuantity * moneyDecalScaleY));
        }

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(handler.transform.DOScaleY(0.8f, jumpSpeed / 2));

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
}