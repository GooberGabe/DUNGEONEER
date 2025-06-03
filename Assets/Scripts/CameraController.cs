using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f; // Speed at which the camera pans
    public float zoomSpeed = 5f; // Speed at which the camera zooms
    public float minZoom = 5f; // Minimum zoom distance
    public float maxZoom = 20f; // Maximum zoom distance
    public float damping = 5f; // Damping factor for smooth panning
    public Vector2 panBoundsMin; // Minimum bounds for camera panning
    public Vector2 panBoundsMax; // Maximum bounds for camera panning
    private Vector3 targetPosition; // Target position for the camera to move towards
    private Vector3 velocity = Vector3.zero; // Velocity for damping
    private float currentZoom; // Current zoom level
    private Vector3 dragOrigin; // Origin point for the drag operation
    public GameObject canvas;
    private GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Start()
    {
        targetPosition = transform.position; // Set initial target position to the camera's current position
        currentZoom = Camera.main.orthographicSize; // Get the initial orthographic size (zoom level)
        raycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        // Handle camera panning
        if (Input.GetMouseButtonDown(2)) // Middle mouse button down
        {
            dragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2)) // Middle mouse button held
        {
            // Calculate zoom factor - higher zoom (smaller orthographic size) means less sensitive panning
            float zoomFactor = currentZoom / minZoom;

            Vector3 delta = new Vector3(
                -(Input.mousePosition.x - dragOrigin.x) * panSpeed * Time.deltaTime * zoomFactor,
                (Input.mousePosition.y - dragOrigin.y) * panSpeed * Time.deltaTime * zoomFactor,
                0f
            );

            Vector3 originalVector = new Vector3(delta.y, 0, delta.x);
            // Create a rotation quaternion around the Y-axis
            Quaternion rotation = Quaternion.AngleAxis(45, Vector3.up);
            // Apply the rotation to the original vector
            Vector3 rotatedVector = rotation * originalVector;

            targetPosition += rotatedVector;
            targetPosition.x = Mathf.Clamp(targetPosition.x, panBoundsMin.x, panBoundsMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, panBoundsMin.y, panBoundsMax.y);

            dragOrigin = Input.mousePosition;
        }

        // Handle camera zooming

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
            

        // Interpolate camera position and zoom
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, damping);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, currentZoom, Time.deltaTime * zoomSpeed);
    }
}