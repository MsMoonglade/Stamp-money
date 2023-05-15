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

    void Update()
    {
        if (CharacterBehaviour.instance == null)
        {
            return;
        }

        if (GameManager.instance.IsInGameStatus())
        {
            SwipeMovement();
        }

        if (!GameManager.instance.IsInGameStatus())
        {
            if(Input.GetMouseButtonDown(0))
                GameManager.instance.StartGame();   
        }

        // CharacterBehaviour.instance.transform.position = new Vector3(Mathf.Clamp(CharacterBehaviour.instance.transform.position.x, minXPos, maxXPos), CharacterBehaviour.instance.transform.position.y, CharacterBehaviour.instance.transform.position.z);
    }

    private void SwipeMovement()
    {
        if (Input.touchCount > 0)
        {
            CharacterBehaviour.instance.moving = true;

            Touch touch = Input.GetTouch(0);

            currentDirection = touch.deltaPosition * m_InputSensitivity;
            currentDirection = new Vector2(currentDirection.x, 0);

            CharacterBehaviour.instance.Move(currentDirection);
        }

        else
        {
            if(CharacterBehaviour.instance.moving)
            CharacterBehaviour.instance.moving = false;

            CharacterBehaviour.instance.Move(Vector3.zero);
        }
    }
}
