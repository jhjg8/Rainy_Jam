using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance;

    private Camera cam;
    
    [Header("3D Settings (Perspective)")]
    public float zoomFOV = 40f;
    private float defaultFOV;

    [Header("2D Settings (Orthographic)")]
    public float zoomSize = 3f; // ค่าน้อยลง = ซูมเข้า
    private float defaultSize;

    public float zoomSpeed = 8f;
    private bool isZoomed = false;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
        defaultSize = cam.orthographicSize;
    }

    private void Update()
    {
        if (cam.orthographic)
        {
            // สำหรับกล้อง 2D
            float targetSize = isZoomed ? zoomSize : defaultSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // สำหรับกล้อง 3D
            float targetFOV = isZoomed ? zoomFOV : defaultFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }

    public void ZoomIn() => isZoomed = true;
    public void ZoomOut() => isZoomed = false;
}