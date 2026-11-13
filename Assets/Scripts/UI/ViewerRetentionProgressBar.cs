using Unity.VisualScripting;
using UnityEngine;

public class ViewerRetentionProgressBar : MonoBehaviour
{
    //Copied and pasted old version of this script into Google AI wanting to add a better transition between bars, 
    //The result ended up being better than my previous solution :/
    //Cannot in good faith say this is my script anymore so this one goes to Google...
    //I did read through this script and I understand how it works compared to my old version

    [Header("Fill Segments (In Order)")]
    [SerializeField] private RectTransform[] _fillSegments;
    
    [Header("Animation Settings")]
    [SerializeField] private float _smoothSpeed = 2f;

    private float _maxOffset;
    private float _targetRetention;
    private float _displayedRetention;

    // Clamps and steps the displayed value towards the target
    private float DisplayedRetention
    {
        get => _displayedRetention;
        set
        {
            float moved = Mathf.MoveTowards(_displayedRetention, value, _smoothSpeed * Time.deltaTime);
            _displayedRetention = Mathf.Clamp(moved, 0f, _fillSegments.Length);
        }
    }

    private void OnEnable()
    {
        GameController.OnViewerRetentionChanged += HandleRetentionChanged;
    }

    private void OnDisable()
    {
        GameController.OnViewerRetentionChanged -= HandleRetentionChanged;
    }

    private void Start()
    {
        _maxOffset = GetComponent<RectTransform>().offsetMax.y * 2f;
        UpdateAllBars(0f);
    }

    private void HandleRetentionChanged(float newRetention)
    {
        _targetRetention = Mathf.Clamp(newRetention, 0f, _fillSegments.Length);
    }

    private void Update()
    {
        // Avoid unnecessary UI updates if the bar has already reached the target
        if (Mathf.Approximately(_displayedRetention, _targetRetention)) return;

        DisplayedRetention = _targetRetention;
        UpdateAllBars(DisplayedRetention);
    }

    private void UpdateAllBars(float retentionValue)
    {
        for (int i = 0; i < _fillSegments.Length; i++)
        {
            // Isolates the 0.0 to 1.0 fill amount for this specific bar:
            // - Values below 'i' clamp to 0 (completely empty)
            // - Values above 'i + 1' clamp to 1 (completely full)
            float segmentFill = Mathf.Clamp01(retentionValue - i);
            
            float currentOffset = (1f - segmentFill) * _maxOffset;
            _fillSegments[i].offsetMax = new Vector2(_fillSegments[i].offsetMax.x, -currentOffset);
        }
    }
}
