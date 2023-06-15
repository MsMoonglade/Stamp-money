using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AnimateUiElement : MonoBehaviour
{
    public static AnimateUiElement instance;

    [Header ("Hand")]
    public GameObject mainMenuHand;
    public float handMoveSpeed;
    public float hideHandSpeed;

    [Header("Main Menu Button")]
    public GameObject[] mainMenuButtons;
    public float buttonsAnimSpeed;
    public float buttonsHideSpeed;
    private Tween[] buttonsTween;

    [Header ("Edit Button")]
    public GameObject editButton;
    public GameObject editButtonBg;
    public float editbuttonsAnimSpeed;
    public float editbuttonsHideSpeed;
    private Sequence editbuttonsTweens;

    [Header("Confirm Edit Button")]
    public GameObject confirmeditButton;
    public float confirmEditbuttonsAnimSpeed;
    public float confirmEditbuttonsHideSpeed;
    Sequence confirmSequence;

    [Header("Edit Menu Button")]
    public GameObject[] editMenuButtons;
    public float editAnimSpeed;
    public float editHideSpeed;
    private Tween[] editPanelButtonsTween;

    [Header("Other to Hide")]
    public GameObject energyslider;

    [Header("3D Object Inventort")]
    public GameObject inventoryObj;
    public float inventoryHideSpeed;

    private void Awake()
    {
        instance = this;

        //StartDisableEdit();

        //ANIMATE HAND
        mainMenuHand.transform.DOLocalMoveX(Mathf.Abs(mainMenuHand.transform.GetComponent<RectTransform>().localPosition.x), handMoveSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        //ANIMATE BUTTONS
        buttonsTween = new Tween[mainMenuButtons.Length];
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            buttonsTween[i] = mainMenuButtons[i].transform.DOScale(1.05f, buttonsAnimSpeed)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
        }



        //EDIT BUTTONS
        editbuttonsTweens = DOTween.Sequence().SetLoops(-1, LoopType.Restart);

        editbuttonsTweens.Append(editButton.transform.DOLocalMoveX(Mathf.Abs(editButton.transform.GetComponent<RectTransform>().localPosition.x - 50), editbuttonsAnimSpeed)
            .SetLoops(2, LoopType.Yoyo));
        editbuttonsTweens.PrependInterval(editbuttonsAnimSpeed / 4);
        editbuttonsTweens.Append(editButton.transform.DOLocalMoveX(Mathf.Abs(editButton.transform.GetComponent<RectTransform>().localPosition.x - 50), editbuttonsAnimSpeed)            
            .SetLoops(2, LoopType.Yoyo));
        editbuttonsTweens.PrependInterval(editbuttonsAnimSpeed );

        editButtonBg.transform.DOLocalRotate(new Vector3(0, 0, 360), editbuttonsAnimSpeed * 10, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);

        editPanelButtonsTween = new Tween[editMenuButtons.Length];

        HideEdit();
    }


    public void HideElements()
    {
        //HAND
        mainMenuHand.GetComponent<Image>().DOFade(0 , hideHandSpeed);

        //BUTTONS
        for (int i = 0; i < buttonsTween.Length; i++)
        {
            buttonsTween[i].Pause();
        }

        foreach (GameObject o in mainMenuButtons)
        {
            o.transform.DOScale(0, buttonsHideSpeed);                
        }

        //EDIT    
        editButton.transform.DOScale(0, editbuttonsHideSpeed);

        energyslider.transform.DOScale(0, buttonsHideSpeed);

        ShowEdit();
    }
    public void HideAll()
    {
        //HAND
        mainMenuHand.GetComponent<Image>().DOFade(0, hideHandSpeed);

        //BUTTONS
        for (int i = 0; i < buttonsTween.Length; i++)
        {
            buttonsTween[i].Pause();
        }

        foreach (GameObject o in mainMenuButtons)
        {
            o.transform.DOScale(0, buttonsHideSpeed);
        }

        //EDIT    
        editButton.transform.DOScale(0, editbuttonsHideSpeed);
    }

    public void ShowElements()
    {
        //HAND
        mainMenuHand.GetComponent<Image>().DOFade(1, hideHandSpeed *2);

        //BUTTONS
        foreach (GameObject o in mainMenuButtons)
        {
            o.transform.DOScale(1, buttonsHideSpeed *2);
        }

        for (int i = 0; i < buttonsTween.Length; i++)
        {
            buttonsTween[i].Restart();
        }

        //EDIT    
        editButton.transform.DOScale(1, editbuttonsHideSpeed * 2);

        energyslider.transform.DOScale(1, buttonsHideSpeed);

        HideEdit();
    }

    public void ShowEdit()
    {
        //CONFIRM EDIT BUTTONS
        confirmSequence = DOTween.Sequence();
        confirmSequence.Append(confirmeditButton.transform.DOScale(1f, confirmEditbuttonsHideSpeed));
        confirmSequence.Append(confirmeditButton.transform.DOScale(1.05f, confirmEditbuttonsAnimSpeed)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo));


        //EDIT PANEL BUTTONS

        for (int i = 0; i < editMenuButtons.Length; i++)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(editMenuButtons[i].transform.DOScale(1f, editHideSpeed));
            editPanelButtonsTween[i] = seq.Append(editMenuButtons[i].transform.DOScale(1.025f, editAnimSpeed)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo));
        }

        inventoryObj.transform.DOScale(1, inventoryHideSpeed);
    }

    public void HideEdit()
    {
        confirmSequence.Pause();
        confirmeditButton.transform.DOScale(0, confirmEditbuttonsHideSpeed);

        for (int i = 0; i < editMenuButtons.Length; i++)
        {
            if (editPanelButtonsTween[i] != null)            
                editPanelButtonsTween[i].Kill();
        }

        for (int i = 0; i < editMenuButtons.Length; i++)
        {
            editMenuButtons[i].transform.DOScale(0, editbuttonsHideSpeed);
        }

        inventoryObj.transform.DOScale(0, inventoryHideSpeed);
    }

    private void StartDisableEdit()
    {
        inventoryObj.transform.localScale = Vector3.zero;

        confirmeditButton.transform.localScale = Vector3.zero;

        for(int i = 0; i < editMenuButtons.Length; i ++)
        {
            editMenuButtons[i].transform.localScale = Vector3.zero;
        }
    }
}
