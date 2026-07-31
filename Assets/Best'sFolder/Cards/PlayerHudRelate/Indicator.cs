using UnityEngine;
using TMPro;
using DG.Tweening;
public class Indicator : MonoBehaviour
{
    [SerializeField] WorldToCasvas wtc;
    [SerializeField] TextMeshProUGUI Cardside;
    [SerializeField] TextMeshProUGUI Suits;
    [SerializeField] CanvasGroup Cg;
    public static Indicator instance;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            cancle();
        }
    }
    public void Move(Transform target , string name, string suits)
    {
        Cg.DOKill();
        Cg.DOFade(1f, 0.25f).SetEase(Ease.OutSine);
        wtc.worldTarget = target;
    }
    public void cancle()
    {
        Cg.DOKill();
        Cg.DOFade(0f, 0.25f).SetEase(Ease.OutSine); 
        wtc.worldTarget = null;
    }
}
