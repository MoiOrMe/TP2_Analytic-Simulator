using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI clickText;

    public List<CameraData> cameraDataList = new List<CameraData>();
    private int currentCameraIndex = 0;

    public CameraData CurrentCameraData => cameraDataList[currentCameraIndex];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var camData in cameraDataList)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(camData.tag);
            camData.taggedCubes.Clear();

            foreach (GameObject obj in taggedObjects)
            {
                GrabbableObject grabbable = obj.GetComponent<GrabbableObject>();
                if (grabbable != null)
                    camData.taggedCubes.Add(grabbable);
            }
        }

        ActivateCamera(currentCameraIndex);
    }

    private void Update()
    {
        var currentData = cameraDataList[currentCameraIndex];

        if (currentData.isRunning)
        {
            currentData.timer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
                currentData.clickCount++;
        }

        UpdateUI(currentData);
    }

    private void UpdateUI(CameraData data)
    {
        if (timerText != null)
            timerText.text = FormatTime(data.timer);

        if (errorText != null)
            errorText.text = $"Erreurs : {data.errorCount}";

        if (clickText != null)
            clickText.text = $"Clicks : {data.clickCount}";
    }

    public void StartTimer()
    {
        var currentData = cameraDataList[currentCameraIndex];
        currentData.timer = 0f;
        currentData.errorCount = 0;
        currentData.clickCount = 0;
        currentData.isRunning = true;
    }

    private void StopTimer()
    {
        cameraDataList[currentCameraIndex].isRunning = false;
    }

    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameraDataList.Count; i++)
        {
            cameraDataList[i].camera.enabled = (i == index);
        }

        Debug.Log($"Caméra activée : {cameraDataList[index].camera.name}");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100);
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }

    private void OnEnable()
    {
        GrabbableObject.OnReleased += OnObjectReleased;
    }

    private void OnDisable()
    {
        GrabbableObject.OnReleased -= OnObjectReleased;
    }

    private void OnObjectReleased(GrabbableObject obj)
    {
        StartCoroutine(DelayedSnapCheck(obj));
    }

    private IEnumerator DelayedSnapCheck(GrabbableObject obj)
    {
        yield return null;

        if (!obj.IsSnapped())
        {
            cameraDataList[currentCameraIndex].errorCount++;
        }

        CheckForCompletion();
    }

    public void CheckForCompletion()
    {
        var currentData = cameraDataList[currentCameraIndex];

        foreach (var cube in currentData.taggedCubes)
        {
            if (!cube.IsSnapped())
                return;
        }

        StopTimer();
    }

    public bool IsTimerRunning()
    {
        return cameraDataList[currentCameraIndex].isRunning;
    }

    public void SwitchToCamera(int index)
    {
        if (index < 0 || index >= cameraDataList.Count) return;

        cameraDataList[currentCameraIndex].camera.enabled = false;
        currentCameraIndex = index;
        ActivateCamera(currentCameraIndex);
    }

    public void ResetStatsAndObjects()
    {
        foreach (var cameraData in cameraDataList)
        {
            cameraData.timer = 0f;
            cameraData.errorCount = 0;
            cameraData.clickCount = 0;
            cameraData.isRunning = false;

            foreach (var grabbable in cameraData.taggedCubes)
            {
                grabbable.ResetPosition();
            }
        }

        UpdateUI(CurrentCameraData);
    }

}
