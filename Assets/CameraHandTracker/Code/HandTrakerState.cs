using CameraHandTracker;
using UnityEngine;

public class HandTrakerState : MonoBehaviour
{
    public static HandTrakerState Instance;
    
    public UnityEngine.UI.Image tipImage;
    
    [SerializeField] private WebcamRgbSource webcamSource;
    [SerializeField] private CameraHandJointTracker handTracker;

    private void Awake()
    {
        Instance = this;
        
        SetState(TrackType.Error);
        
    }

    private void OnEnable()
    {
        if (!handTracker) handTracker = FindObjectOfType<CameraHandJointTracker>();
        
        if (handTracker && handTracker.webcamSource) webcamSource = handTracker.webcamSource;
        if (!webcamSource) webcamSource = FindObjectOfType<WebcamRgbSource>();
        
        if (webcamSource) webcamSource.OnInitialized += RefreshState;
        if (handTracker) handTracker.OnHandsUpdated += OnHandsUpdated;
        RefreshState();
    }

    private void OnDisable()
    {
        if (webcamSource) webcamSource.OnInitialized -= RefreshState;
        if (handTracker) handTracker.OnHandsUpdated -= OnHandsUpdated;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        // Also reflect camera/tracker shutdown, which does not emit a hand result.
        RefreshState();
    }

    private void OnHandsUpdated(CameraHandJointTracker.Result result)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        bool initialized = webcamSource && webcamSource.isActiveAndEnabled && webcamSource.IsInitialized;
            
        if (!initialized)
        {
            SetState(TrackType.Error);
            return;
        }

        bool streaming = webcamSource.IsStreaming;
        bool isTracking = streaming && handTracker &&
            handTracker.isActiveAndEnabled && handTracker.TrackedHandCount > 0;
        SetState(isTracking ? TrackType.Playing : TrackType.Waiting);
    }

    private void SetState(TrackType type)
    {
        if (!tipImage) return;

        switch (type)
        {
            case TrackType.Error:
                tipImage.color = Color.red;
                break;
            case TrackType.Waiting:
                tipImage.color = Color.yellow;
                break;
            case TrackType.Playing:
                tipImage.color = Color.green;
                break;
        }
        
    }
}

public enum TrackType
{
    Error,
    Waiting,
    Playing
}
