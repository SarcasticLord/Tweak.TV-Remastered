using UnityEngine;

public class ProgressBarScript : MonoBehaviour
{
    [SerializeField] private RectTransform _fillImage;
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _maxOffset = 150f; //Match this to ProgressBarBackground width 

    public void UpdateProgressBar(float currentValue)
    {
        currentValue = Mathf.Clamp(currentValue, 0, _maxValue);

        //Calc percentage 
        float fillPercentage = currentValue / _maxValue;

        float currentOffset = (1f - fillPercentage) * _maxOffset;

        _fillImage.offsetMax = new Vector2(-currentOffset, _fillImage.offsetMax.y);

    }
}
