using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AnimateUiElement : MonoBehaviour
{
    public static AnimateUiElement instance;

    public GameObject mainMenuHand;
    public float handMoveSpeed;
    public float hideHandSpeed;


    private void Awake()
    {
        instance = this;

        //ANIMATE HAND
        mainMenuHand.transform.DOLocalMoveX(Mathf.Abs(mainMenuHand.transform.GetComponent<RectTransform>().localPosition.x), handMoveSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1 , LoopType.Yoyo);
    }


    public void HideHand()
    {
        mainMenuHand.GetComponent<Image>().DOFade(0 , hideHandSpeed);
    }

    public void ShowHand()
    {
        mainMenuHand.GetComponent<Image>().DOFade(1, hideHandSpeed);
    }
}
