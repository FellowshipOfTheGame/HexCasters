using UnityEngine;

public class JobsDoneController : MonoBehaviour {
    public Camera cam;
    public RectTransform endTurnButton;
    public Transform jobsDone;

    void LateUpdate() {
        var center = endTurnButton.position;
        center -= new Vector3(endTurnButton.rect.width, endTurnButton.rect.height) / 2;
        var pos = cam.ScreenToWorldPoint(center);
        pos.z = 0;
        jobsDone.position = pos;
    }
}