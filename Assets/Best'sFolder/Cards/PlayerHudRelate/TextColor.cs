using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TextColor : MonoBehaviour
{
    [SerializeField] private Button target;
    [SerializeField] private Image image;
    private TMPro.TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }
    void Update()
    {
        Color targetColor;
        if (image == null)
        {
            targetColor = target.targetGraphic.canvasRenderer.GetColor();
        }
        else
        {
            targetColor = image.color;
        }
        targetColor.r = 1f - targetColor.r;
        targetColor.g = 1f - targetColor.g;
        targetColor.b = 1f - targetColor.b;

        text.DOColor(targetColor, 0.5f);
    }
}