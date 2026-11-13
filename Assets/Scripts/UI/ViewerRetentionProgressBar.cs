using UnityEngine;

public class ViewerRetentionProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform _fill0to1;
    [SerializeField] private RectTransform _fill1to2;
    [SerializeField] private RectTransform _fill2to3;
    [SerializeField] private RectTransform _fill3to4;
    [SerializeField] private RectTransform _fill4to5;

    private float _maxFillValue = 1f;
    private float _maxOffset;
    private float _maxOffsetWidth;

    void OnEnable()
    {
        GameController.OnViewerRetentionChanged += UpdateViewerRetentionBar;
    }
    void OnDisable()
    {
        GameController.OnViewerRetentionChanged -= UpdateViewerRetentionBar;
    }

    void Start()
    {
        _maxOffset = GetComponent<RectTransform>().offsetMax.y;
        _maxOffsetWidth = GetComponent<RectTransform>().offsetMax.x;
    }

    private void UpdateViewerRetentionBar(float newRetention)
    {
        float currentOffset;

        switch(newRetention)
        {
            case < 0:
                _fill0to1.offsetMax = new Vector2(_fill0to1.offsetMax.x,_maxOffset);
                break;
            case <= 1:
                currentOffset = newRetention /_maxFillValue * _maxOffset;
                _fill0to1.offsetMax = new Vector2(_maxOffsetWidth, currentOffset);
                break;
            case <= 2:
                currentOffset = (newRetention - 1)/_maxFillValue * _maxOffset;
                _fill1to2.offsetMax = new Vector2(_maxOffsetWidth, currentOffset);
                break;
            case <= 3:
                currentOffset = (newRetention - 2)/_maxFillValue * _maxOffset;
                _fill2to3.offsetMax = new Vector2(_maxOffsetWidth, currentOffset);
                break;
            case <= 4:
                currentOffset = (newRetention - 3)/_maxFillValue * _maxOffset;
                _fill3to4.offsetMax = new Vector2(_maxOffsetWidth, currentOffset);
                break;
            case <= 5:
                currentOffset = (newRetention - 4)/_maxFillValue * _maxOffset;
                _fill4to5.offsetMax = new Vector2(_maxOffsetWidth, currentOffset);
                break;


        }
    }
}
