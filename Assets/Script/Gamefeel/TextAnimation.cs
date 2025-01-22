using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    void Start()
    {
        Sequence seq = DOTween.Sequence();
        Tween one = rectTransform.DOScale(new Vector3(1.4f, 1.4f), 0.5f).SetEase(Ease.InOutSine);
        Tween two = rectTransform.DOScale(new Vector3(1, 1), 0.5f).SetEase(Ease.InOutSine);
        seq.Append(one).Append(two).SetLoops(-1);
    }
}
