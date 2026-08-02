using UnityEngine;

public class TriggerCS : MonoBehaviour
{
    public CameraShake cameraShake; // ลาก Main Camera มาใส่ช่องนี้ใน Inspector

void OnPlayerHit() 
{
    // เรียกสั่นแบบกำหนดค่าเอง: Shake(ความแรงตำแหน่ง, ความแรงหมุน, ระยะเวลา)
    if (cameraShake != null) 
    {
        cameraShake.Shake(0.3f, 5.0f, 0.25f);
    }
}
}
