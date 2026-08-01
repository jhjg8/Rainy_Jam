using UnityEngine;
using DG.Tweening;

public class EpicFollow : MonoBehaviour
{   
    //
    public RectTransform BOX;

    public float MaxRadius = 200f;

    private Vector2 Original;

    public float MouseMaxRadius = 1000f;

    
    void Start()
    {
        Original = BOX.position;

    }

    
    void Update()
    {
        if (Original == null)return;

        
        Vector2 Mouse = Input.mousePosition;
        Vector2 range = Mouse - Original;
        float RAYA = range.magnitude;
        Vector2 Direction = range.normalized;
        
        float mousecompare = Mathf.InverseLerp(0f, MouseMaxRadius, RAYA);
        float targetf = Mathf.Lerp(0f, MaxRadius, mousecompare);
        Vector2 target = (targetf * Direction);

        BOX.DOKill();
        BOX.DOAnchorPos(target, 0.25f);
    

    }
}
