using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockMarker : MonoBehaviour
{
    [SerializeField] private CameraController camCon;
    [SerializeField] private Canvas thisCanvas;
    private Camera cam;
    private Image thisImage;

    void Start() {
        thisImage = GetComponent<Image>();
        if (camCon == null) {Debug.LogError("Camera Controller not set in the LockMarker script! Cannot place UI."); return;} 
        if (thisCanvas == null) {Debug.LogError("Canvas not set in the LockMarker script! Cannot place UI"); return;}
        cam = camCon.gameObject.GetComponent<Camera>();
    }

    void Update() {
        if (camCon == null || thisCanvas == null) {return;}

        if (!thisImage.IsActive() && camCon.hasTarget) {thisImage.enabled = true;}
        if (thisImage.IsActive() && !camCon.hasTarget) {thisImage.enabled = false; return;}
        if (!thisImage.IsActive()) {return;}

        UpdatePosition();
    }

    void UpdatePosition() {
        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = cam.WorldToScreenPoint(camCon.point);
        
        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(thisImage.rectTransform, screenPoint, null, out canvasPos);
        
        // Set
        thisImage.rectTransform.position = screenPoint;
    }
}
