using UnityEngine;
using UnityEngine.UI;
using System;

public class CoffeeMeter : MonoBehaviour
{
    public Slider slider;
    [SerializeField] public int maxValue = 100;
    [SerializeField] public int minValue = 0;
    [SerializeField] public int currentValue = 50;

    public Action OnMaxReached;
    public Action OnMinReached;

    void Start()
    {
        if (slider != null)
        {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = currentValue;
        }
    }

    public void Increase(int amount = 10)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        UpdateSlider();

        if (currentValue >= maxValue)
            OnMaxReached?.Invoke();
    }

    public void Decrease(int amount = 10)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        UpdateSlider();

        if (currentValue <= minValue)
            OnMinReached?.Invoke();
    }

    private void UpdateSlider()
    {
        if (slider != null)
            slider.value = currentValue;
    }
}
