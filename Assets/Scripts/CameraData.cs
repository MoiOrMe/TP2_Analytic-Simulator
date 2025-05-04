using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraData
{
    public Camera camera;
    public string tag;
    public float timer = 0f;
    public int errorCount = 0;
    public int clickCount = 0;
    public bool isRunning = false;
    public List<GrabbableObject> taggedCubes = new List<GrabbableObject>();
}
