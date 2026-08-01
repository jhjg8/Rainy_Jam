using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class Indicator : MonoBehaviour
{
    [SerializeField] WorldToCasvas wtc;
    [SerializeField] TextMeshProUGUI Cardside;
    [SerializeField] TextMeshProUGUI Suits;
    [SerializeField] CanvasGroup Cg;
    [SerializeField] private Image targetImage;
    public static Indicator instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;

    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            cancle();
        }
    }
    public void Move(Transform target , string name, string suits, bool blue)
    {
        if (wtc.worldTarget == target) return;
        Cg.DOKill();
        targetImage.DOKill();
        Cg.DOFade(1f, 0.25f).SetEase(Ease.OutSine);
        if (blue)
        {
            Color color = Color.blue; 
            targetImage.DOColor(color, 0.3f);
        }
        else
        {
            Color color = Color.red;
            targetImage.DOColor(color, 0.3f);
        }
            wtc.worldTarget = target;
    }
    public void cancle()
    {
        Cg.DOKill();
        Cg.DOFade(0f, 0.25f).SetEase(Ease.OutSine); 
        wtc.worldTarget = null;
    }
}
