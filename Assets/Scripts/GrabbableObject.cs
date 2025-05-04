using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class GrabbableObject : MonoBehaviour
{
    private Vector3 initialPosition;

    private float fixedY;
    private Vector3 grabOffset;
    private bool hasSnapped = false;
    private Collider objectCollider;

    public static event Action<GrabbableObject> OnReleased;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (hasSnapped) return;

        fixedY = transform.position.y;

        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            grabOffset = transform.position - hitPoint;
        }

        if (SnapManager.Instance != null)
        {
            var currentData = SnapManager.Instance.CurrentCameraData;
            if (!currentData.isRunning)
            {
                SnapManager.Instance.StartTimer();
            }
        }
    }

    private void OnMouseDrag()
    {
        if (hasSnapped) return;

        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + grabOffset;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            int direction = scroll > 0 ? 1 : -1;

            transform.position += Vector3.up * direction;

            fixedY = transform.position.y;
        }


    }

    private void OnMouseUp()
    {
        if (hasSnapped) return;
        OnReleased?.Invoke(this);
    }

    public void SnapTo(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        hasSnapped = true;

        if (objectCollider != null)
            objectCollider.enabled = false;

        if (SnapManager.Instance != null)
            SnapManager.Instance.CheckForCompletion();
    }

    public bool IsSnapped()
    {
        return hasSnapped;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        hasSnapped = false;
        if (objectCollider != null)
            objectCollider.enabled = true;
    }
}
