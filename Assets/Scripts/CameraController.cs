using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotationSpeed = 50f;
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("Target")]
    public Transform target;

    private float currentZoom = 15f;
    private float currentRotationX = 45f;
    private float currentRotationY = 45f;

    void Start()
    {
        if (target == null)
        {
            GameObject gridObj = GameObject.Find("GridManager");
            if (gridObj != null)
            {
                target = gridObj.transform;
            }
        }
    }

    void Update()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Rotation
        if (Input.GetMouseButton(1))
        {
            currentRotationY += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentRotationX = Mathf.Clamp(currentRotationX, 10f, 80f);
        }

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * currentZoom);

        transform.position = position;
        transform.LookAt(target.position);
    }
}
