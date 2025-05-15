using DG.Tweening;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] float popScale = 10f;
    [SerializeField] float upDur = 0.5f;
    [SerializeField] float downDur = 0.5f;
    [SerializeField] float riseAmount = 200f;
    [SerializeField] private RectTransform rt;

    void Start()
    {
        rt.localScale = Vector3.zero;


        var seq = DOTween.Sequence();


        seq.AppendCallback(() =>
            {
                rt.DOScale(popScale, upDur).SetEase(Ease.OutBack);
                rt.DOAnchorPosY(rt.anchoredPosition.y + riseAmount, upDur)
                    .SetEase(Ease.OutQuad);
            })
            .AppendInterval(upDur)
            .Append(rt.DOScale(0, downDur).SetEase(Ease.InBack))
            .OnComplete(() => Destroy(gameObject));
    }
}