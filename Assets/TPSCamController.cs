using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamController : MonoBehaviour {

    public float rotationSpeed = 1;
    public Transform target, player;

    float mouseX, mouseY;

    private void LateUpdate() {
        
    }

    void CamControl() {
        mouseX += Input.GetAxis("Horizontal") * rotationSpeed;
        mouseY -= Input.GetAxis("Vertical") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(target);
        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        player.rotation = Quaternion.Euler(0, mouseX, 0);

    }
}
