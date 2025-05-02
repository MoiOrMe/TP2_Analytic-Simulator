using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorText; // Ajoute ça dans l’inspecteur Unity

    private float timer = 0f;
    private bool isRunning = false;
    private bool hasStarted = false;
    private int errorCount = 0; // Compteur d’erreurs

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

        if (errorText != null)
        {
            errorText.text = $"Erreurs : {errorCount}";
        }
    }

    private void OnObjectReleased(GrabbableObject obj)
    {
        StartCoroutine(DelayedSnapCheck(obj));
    }

    private IEnumerator DelayedSnapCheck(GrabbableObject obj)
    {
        yield return null; // Attendre un frame

        if (!obj.IsSnapped())
        {
            RegisterError();
        }

        CheckForCompletion();
    }

    private void RegisterError()
    {
        errorCount++;
        Debug.Log($"Erreur détectée ! Total erreurs : {errorCount}");
    }

    public void StartTimer()
    {
        isRunning = true;
        hasStarted = true;
        timer = 0f;
        errorCount = 0; // Réinitialisation du compteur
        Debug.Log("Timer started.");
    }

    private void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped at {timer:F2} seconds.");
    }

    public void CheckForCompletion()
    {
        foreach (var cube in taggedCubes)
        {
            if (!cube.IsSnapped())
                return;
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

    public int GetErrorCount()
    {
        return errorCount;
    }
}
