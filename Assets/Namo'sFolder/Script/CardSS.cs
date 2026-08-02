using UnityEngine;
public class CardSS : MonoBehaviour
{
    public void OnPlayCard()
    {
        // 1. ทำ Logic วางการ์ด / คิดคำนวณดาเมจ
        
        // 2. สั่งกล้องสั่น (ไม่ต้องดึง Variable กล้องมาใส่)
        CameraShake.Instance.Shake(0.15f, 0.2f); 
    }
}