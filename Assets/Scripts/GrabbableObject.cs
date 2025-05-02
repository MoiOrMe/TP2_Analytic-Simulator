using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class GrabbableObject : MonoBehaviour
{
    private float fixedY;
    private Vector3 grabOffset;
    private bool hasSnapped = false;
    private Collider objectCollider;

    public static event Action<GrabbableObject> OnReleased;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
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

        // Démarrer le timer dès qu'on appuie sur le cube
        if (SnapManager.Instance != null && !SnapManager.Instance.IsTimerRunning())
        {
            SnapManager.Instance.StartTimer();
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
    }

    public bool IsSnapped()
    {
        return hasSnapped;
    }
}
