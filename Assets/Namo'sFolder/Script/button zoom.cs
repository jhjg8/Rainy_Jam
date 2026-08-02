using UnityEngine;

public class buttonzoom : MonoBehaviour
{
public void OnHoverIn()
{
    CameraZoom.Instance.ZoomIn();
}

public void OnHoverOut()
{
    CameraZoom.Instance.ZoomOut();
}
}
