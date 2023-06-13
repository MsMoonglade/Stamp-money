using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    float minXPos = -10f;
    float maxXPos = 10f;

    [SerializeField]
    float m_InputSensitivity = 1.5f;

    private Vector2 currentDirection;

    private bool movePlayer = false;

    private bool ignoreFirstPressUp = true;
    private float ignoreTime = 1;

    bool started = false;

    private Vector2 firstPressPos;
    private Vector2 direction;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (CharacterBehaviour.instance == null)
        {
            return;
        }

        if (GameManager.instance.IsInGameStatus())
        {
            DetectInputChange();

            if (movePlayer)
            {
                SwipeMovement();
            }

            else
            {
                if (CharacterBehaviour.instance.moving)
                    CharacterBehaviour.instance.moving = false;

                CharacterBehaviour.instance.Move(Vector3.zero);
            }
        }

        if (GameManager.instance.IsInEndGameStatus())
        {
            /*
            JoistickMovement();
            if (direction != Vector2.zero)
            {
                EndGameCharacterBehaviour.instance.Move(new Vector3(direction.x, 0, direction.y));
            }
            */

            SwipeMovingEditor();
        }

        // CharacterBehaviour.instance.transform.position = new Vector3(Mathf.Clamp(CharacterBehaviour.instance.transform.position.x, minXPos, maxXPos), CharacterBehaviour.instance.transform.position.y, CharacterBehaviour.instance.transform.position.z);
    }

    private void SwipeMovingEditor()
    {
        //start
        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        //moving
        if (Input.GetMouseButton(0))
        {
            direction = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - firstPressPos).normalized;
            EndGameCharacterBehaviour.instance.Move(new Vector3(direction.x, 0, direction.y));
        }

        //ending
        if (Input.GetMouseButtonUp(0))
        {
            firstPressPos = Vector2.zero;
            direction = Vector3.zero;

            EndGameCharacterBehaviour.instance.StopAnim();

        }
    }

    public void FirstInput()
    {
        if (!started)
        {
            StartCoroutine(IgnoreFirstUpDelay());

            movePlayer = true;
            CharacterBehaviour.instance.moving = true;

            started = true;
        }
    }

    private void SwipeMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            currentDirection = touch.deltaPosition * m_InputSensitivity;
            currentDirection = new Vector2(currentDirection.x, 0);

            CharacterBehaviour.instance.Move(currentDirection);
        }  
    }

    private void DetectInputChange()
    {
        if (Input.GetMouseButtonDown(0))
        {
            movePlayer = true;
            CharacterBehaviour.instance.moving = true;
        }

        if(CharacterBehaviour.instance.currentEnergy <= 0)
        {
            movePlayer = true;
            CharacterBehaviour.instance.moving = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            movePlayer = false;
            CharacterBehaviour.instance.moving = false;
        }
    }    
    
    private void JoistickMovement()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            //start
            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);

              //  UiManager.instance.SetTouchDownImage(firstPressPos);
            }

            //moving
            if (t.phase == TouchPhase.Moved)
            {
                direction = (new Vector2(t.position.x, t.position.y) - firstPressPos).normalized;
                //UiManager.instance.MoveJoystickFront(new Vector2(t.position.x, t.position.y));
            }

            //ending
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                direction = Vector3.zero;
                firstPressPos = Vector2.zero;

            //    CharacterBehaviour.instance.StopMoveAnim();
            //   UiManager.instance.DisableTouchDownImage();
            }
        }
    }

    private IEnumerator IgnoreFirstUpDelay()
    {
        yield return new WaitForSeconds(ignoreTime);

        ignoreFirstPressUp = false;
    }
}