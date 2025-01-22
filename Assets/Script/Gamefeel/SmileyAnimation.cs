using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmileyAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    void Start()
    {
        Sequence seq = DOTween.Sequence();
        Tween one = rectTransform.DOJump(transform.position, 100 ,1 , 1).SetEase(Ease.OutSine);
        Tween two = rectTransform.DOPunchRotation(new Vector3(0,0,10), 0.5f);
        seq.Append(one).Append(two).SetLoops(-1);
    }
}
