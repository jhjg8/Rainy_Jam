using UnityEngine;

public class QuitGameManager : MonoBehaviour
{
    /// <summary>
    /// ฟังก์ชั่นนี้ผูกกับปุ่ม QUIT ใน Main Menu
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game Pressed!");

        // ถ้าเล่นทดสอบอยู่ใน Unity Editor -> สั่งให้หลุดออกจากโหมด Play
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ถ้าเป็นเกมที่ Build ออกมาแล้ว (.exe / .apk / .app) -> สั่งปิดโปรแกรมเกม
        Application.Quit();
#endif
    }
}
