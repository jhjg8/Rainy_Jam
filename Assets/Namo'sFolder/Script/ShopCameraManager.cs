using UnityEngine;

public class ShopCameraManager : MonoBehaviour
{
    public static ShopCameraManager Instance;

    [Header("Camera Positions (ลาก Transform มาใส่)")]
    public Transform gameCameraPos; // ตำแหน่งกล้องตอนเล่นเกมปกติ
    public Transform shopCameraPos; // ตำแหน่งกล้องตอนส่องร้านค้า

    [Header("Movement Settings")]
    public float moveSpeed = 5f;    // ความเร็วในการเลื่อนกล้อง

    private Transform targetPos;
    private bool isInShop = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // เริ่มต้นให้เป้าหมายกล้องอยู่ที่มุมมองเกมหลัก
        if (gameCameraPos != null)
        {
            targetPos = gameCameraPos;
            transform.position = gameCameraPos.position;
            transform.rotation = gameCameraPos.rotation;
        }
    }

    private void Update()
    {
        if (targetPos == null) return;

        // เลื่อนตำแหน่งและหมุนกล้องไปยังเป้าหมายอย่างนุ่มนวล (Smooth Transition)
        transform.position = Vector3.Lerp(transform.position, targetPos.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPos.rotation, Time.deltaTime * moveSpeed);
    }

    // ฟังก์ชันสำหรับผูกกับปุ่มกด Shop (กดสลับไป-มา)
    public void ToggleShop()
    {
        if (isInShop)
        {
            GoToGame();
        }
        else
        {
            GoToShop();
        }
    }

    // สั่งย้ายกล้องไปที่ร้านค้า
    public void GoToShop()
    {
        if (shopCameraPos == null) return;

        targetPos = shopCameraPos;
        isInShop = true;

        // สั่ง ShopManager ให้สร้างการ์ดสุ่ม 8 ใบเมื่อเปิดเข้า Shop (ถ้าต้องการ)
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.GenerateShopItems();
        }
    }

    // สั่งย้ายกล้องกลับไปที่เกม
    public void GoToGame()
    {
        if (gameCameraPos == null) return;

        targetPos = gameCameraPos;
        isInShop = false;
    }

    // เช็กสถานะปัจจุบัน
    public bool IsInShop() => isInShop;
}