using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    private Slider slider;
    private int maxValue;

    void InitSlider(Slider go, int max)
    {
        maxValue = max;
        go.maxValue = max;
        go.value = max;
    }
    void UpdateSlider(int cur)
    {
        slider.value = cur;
    }
}