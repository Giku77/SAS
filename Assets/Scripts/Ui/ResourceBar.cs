using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private string sliderTag;
    private float maxValue;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindSlider();
    }

    private void RebindSlider()
    {
        GameObject sliderObj = GameObject.FindGameObjectWithTag(sliderTag);
        if (sliderObj != null)
        {
            slider = sliderObj.GetComponent<Slider>();
            Debug.Log($"[ResourceBar] Slider re-bound by tag!");

            if (maxValue > 0 && slider != null)
            {
                slider.maxValue = maxValue;
                slider.value = 0;
            }
        }
    }
    public void InitSlider(float max)
    {
        Debug.Log($"[ResourceBar] InitSlider called - max: {max}");
        slider.value = 0;
        maxValue = max;
        slider.maxValue = max;
        //slider.value = max;
    }
    public void UpdateSlider(float cur)
    {
        if(Input.GetKey(KeyCode.K))
        {
            slider.gameObject.SetActive(false);
        }
        Debug.Log($"[ResourceBar] UpdateSlider called - cur: {slider.value}, clamped: {Mathf.Clamp(cur, 0f, slider.maxValue)}");
        slider.value = cur;
    }
}