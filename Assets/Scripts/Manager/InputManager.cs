using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    float minXPos = -10f;
    float maxXPos = 10f;

    [SerializeField]
    float m_InputSensitivity = 1.5f;

    private Vector2 currentDirection;

    private bool movePlayer = false;

    private bool ignoreFirstPressUp = true;
    private float ignoreTime = 1;

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

        if (!GameManager.instance.IsInGameStatus())
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(IgnoreFirstUpDelay());

                GameManager.instance.StartGame();

                movePlayer = true;
                CharacterBehaviour.instance.moving = true;
            }
        }

        // CharacterBehaviour.instance.transform.position = new Vector3(Mathf.Clamp(CharacterBehaviour.instance.transform.position.x, minXPos, maxXPos), CharacterBehaviour.instance.transform.position.y, CharacterBehaviour.instance.transform.position.z);
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

        if(Input.GetMouseButtonUp(0) && !ignoreFirstPressUp)
        {
            movePlayer = false;
            CharacterBehaviour.instance.moving = false;
        }

        if (Input.GetMouseButtonUp(0) && ignoreFirstPressUp)
        {
            ignoreFirstPressUp = false;
        }
    }         

    private IEnumerator IgnoreFirstUpDelay()
    {
        yield return new WaitForSeconds(ignoreTime);

        ignoreFirstPressUp = false;
    }
}