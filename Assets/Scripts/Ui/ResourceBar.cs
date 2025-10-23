using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float maxValue;
    public void InitSlider(float max)
    {
        maxValue = max;
        slider.maxValue = max;
        slider.value = max;
    }
    public void UpdateSlider(float cur)
    {
        slider.value = Mathf.Clamp(cur, 0f, slider.maxValue);
    }
}