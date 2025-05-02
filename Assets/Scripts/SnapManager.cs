using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    public TextMeshProUGUI timerText;

    private float timer = 0f;
    private bool isRunning = false;
    private bool hasStarted = false;

    private List<GrabbableObject> taggedCubes;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("1");
        taggedCubes = new List<GrabbableObject>();

        foreach (GameObject obj in taggedObjects)
        {
            GrabbableObject grabbable = obj.GetComponent<GrabbableObject>();
            if (grabbable != null)
                taggedCubes.Add(grabbable);
        }
    }

    private void OnEnable()
    {
        GrabbableObject.OnReleased += OnObjectReleased;
    }

    private void OnDisable()
    {
        GrabbableObject.OnReleased -= OnObjectReleased;
    }

    private void Update()
    {
        if (isRunning)
            timer += Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = FormatTime(timer);
        }

    }

    private void OnObjectReleased(GrabbableObject obj)
    {
        CheckForCompletion();
    }

    public void StartTimer()
    {
        isRunning = true;
        hasStarted = true;
        timer = 0f;
        Debug.Log("Timer started.");
    }

    private void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped at {timer:F2} seconds.");
    }

    private void CheckForCompletion()
    {
        foreach (var cube in taggedCubes)
        {
            if (!cube.IsSnapped())
                return; // Dès qu’un cube n’est pas snap, on arrête la vérif
        }

        StopTimer();
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100);

        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }

    public bool IsTimerRunning()
    {
        return isRunning;
    }

}
