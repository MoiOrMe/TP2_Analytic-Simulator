using UnityEngine;

public class DropZone : MonoBehaviour
{
    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        if (!zoneCollider.isTrigger)
        {
            Debug.LogWarning("DropZone requires the Collider to be a trigger.");
        }

        GrabbableObject.OnReleased += OnObjectReleased;
    }

    private void OnDestroy()
    {
        GrabbableObject.OnReleased -= OnObjectReleased;
    }

    private void OnObjectReleased(GrabbableObject obj)
    {
        if (obj == null) return;

        if (IsObjectInZone(obj))
        {
            obj.SnapTo(GetSnapPosition(obj));
        }
    }

    private bool IsObjectInZone(GrabbableObject obj)
    {
        Bounds zoneBounds = zoneCollider.bounds;
        Bounds objectBounds = obj.GetComponent<Collider>().bounds;

        bool insideX = objectBounds.min.x >= zoneBounds.min.x && objectBounds.max.x <= zoneBounds.max.x;
        bool insideZ = objectBounds.min.z >= zoneBounds.min.z && objectBounds.max.z <= zoneBounds.max.z;

        return insideX && insideZ;
    }

    private Vector3 GetSnapPosition(GrabbableObject obj)
    {
        Bounds zoneBounds = zoneCollider.bounds;
        Bounds objectBounds = obj.GetComponent<Collider>().bounds;

        float objectHalfHeight = objectBounds.extents.y;
        float dropZoneTopY = zoneBounds.max.y;
        float targetY = dropZoneTopY + objectHalfHeight;

        return new Vector3(zoneBounds.center.x, targetY, zoneBounds.center.z);
    }
}
