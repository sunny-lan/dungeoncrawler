using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] Transform playerBody = null;
    [SerializeField] Transform camera = null;
    [SerializeField] bool invertY = true;
    float pitch = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch += (invertY)? -mouseY:mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        camera.localRotation = Quaternion.Euler(Vector3.right * pitch);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
