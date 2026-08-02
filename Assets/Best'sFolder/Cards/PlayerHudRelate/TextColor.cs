using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TextColor : MonoBehaviour
{
    [SerializeField] private Button target;
    [SerializeField] private Image image;
    private TMPro.TextMeshProUGUI text;
    private Color color = Color.white;
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
        if (color == targetColor) return;
        color = targetColor;
        targetColor.r = 1f - targetColor.r;
        targetColor.g = 1f - targetColor.g;
        targetColor.b = 1f - targetColor.b;

        text.color = targetColor;
    }
}