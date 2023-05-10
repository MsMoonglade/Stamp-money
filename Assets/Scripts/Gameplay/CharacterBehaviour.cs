using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterBehaviour : MonoBehaviour
{
    public static CharacterBehaviour instance;

    public GameObject model;
    public GameObject handler;
    public GameObject printerObject;    

    public float moveSpeed;
    public float jumpHeight;
    public float jumpSpeed;

    public float moneyDecalScaleX;
    public float moneyDecalScaleY;

    private Coroutine jumpCoroutine;

    //***************** PLAYER MOVE ***********************
    private bool moveByTouch;
    private Vector3 mouseStartPos, playerStartPos;
    private Camera camera;

    private Vector2 printerObjectScale;

    private void Awake()
    {
        instance = this;

        jumpCoroutine = null;

        printerObjectScale = new Vector2(printerObject.transform.localScale.x , printerObject.transform.localScale.z);
    }

    private void Start()
    {
        camera = Camera.main;
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
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
           // MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.instance.IsInGameStatus())
        {
            moveByTouch = true;

            var plane = new Plane(Vector3.up, 0);
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                mouseStartPos = ray.GetPoint(distance + 1f);
                playerStartPos = transform.position;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            moveByTouch = false;
        }

        if (moveByTouch)
        {
            var plane = new Plane(Vector3.up, 0);
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                var mousePos = ray.GetPoint(distance + 1f);
                var move = mousePos - mouseStartPos;
                var control = playerStartPos + move;   
                                   
                control.x = Mathf.Clamp(control.x, -5, 5);

                transform.DOMoveX(control.x, (Time.deltaTime * moveSpeed) * 2);

               // transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * moveSpeed), transform.position.y, transform.position.z);
            }
        }
    }

    public void Move(Vector3 direction)
    {
        // transform.DOMoveX(transform.position.x + direction.x, Time.deltaTime * moveSpeed);

        transform.Translate(direction * Time.deltaTime * moveSpeed) ;
    }


    private void PrintDecal()
    {
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
                /*
                GameObject decal = Instantiate(moneyDecal.gameObject, startPoint, moneyDecal.transform.rotation);
                startPoint += new Vector3(0, 0, moneyDecalScaleY);
                */

                GameObject decal = PoolManager.instance.GetItem(GameManager.instance.moneyDecalObj , startPoint , GameManager.instance.moneyDecalParent);
                startPoint += new Vector3(0, 0, moneyDecalScaleY);
            }

            startPoint += new Vector3(moneyDecalScaleX, 0, (-yQuantity * moneyDecalScaleY));
        }

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(handler.transform.DOScaleY(0.8f, jumpSpeed / 2));

        mySequence.Append(handler.transform.DOScaleY(1, jumpSpeed / 2));
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Wall"))
        {
            WallBehaviour thisWall = col.transform.GetComponent<WallBehaviour>();

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
            }
    }

    private void ApplyPrinterScale()
    {
        if (printerObjectScale.x >= 1 && printerObjectScale.y >= 1)
        {
            printerObject.transform.DOScale(new Vector3(printerObjectScale.x, printerObject.transform.localScale.y, printerObjectScale.y), 0.1f)
                .SetEase(Ease.OutBounce);
        }

        else
        {
            EventManager.TriggerEvent(Events.die);
        }
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

    }

    private void OnEndGame(object sender)
    {
        if(jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;   
        }
    }
}