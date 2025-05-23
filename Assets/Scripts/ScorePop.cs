using DG.Tweening;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] float popScale   = 10f;
    [SerializeField] float upDur      = 0.5f;
    [SerializeField] float downDur    = 0.5f;
    [SerializeField] float riseAmount = 200f;  // in pixels (for UI) or world units
    [SerializeField] private RectTransform rt;
    void Start()
    {
        // 1) grab your RectTransform
       
        rt.localScale = Vector3.zero;

        // 2) build a sequence
        var seq = DOTween.Sequence();

        // a) scale up & move up simultaneously
        seq.AppendCallback(() =>
            {
                rt.DOScale(popScale, upDur).SetEase(Ease.OutBack);
                rt.DOAnchorPosY(rt.anchoredPosition.y + riseAmount, upDur)
                    .SetEase(Ease.OutQuad);
            })

            // b) scale back down
            .AppendInterval(upDur) // wait for the up‐tweens to finish
            .Append(rt.DOScale(0, downDur).SetEase(Ease.InBack))

            // c) destroy when done
            .OnComplete(() => Destroy(gameObject));
    }
}