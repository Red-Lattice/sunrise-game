using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float Xsensitivity;
    [SerializeField] private float Ysensitivity;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform playerTransform;

    private float xRotation;
    private float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Xsensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Ysensitivity;
    
        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        transform.position = playerTransform.position;
    }

    public float getXRot()
    {
        return xRotation;
    }
    public float getYRot()
    {
        return yRotation;
    }
}
