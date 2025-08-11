using System;
using UnityEngine;

[Serializable]
public class Condition
{
    private string observerKey;
    Observable<float> curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue; // 주기적으로 변하는 값

    public void Init(string key)
    {
        observerKey = key;
        curValue = new Observable<float>(observerKey, startValue);
    }
    public float GetPercent()
    {
        return curValue.Value / maxValue;
    }
    public void Add(float value)
    {
        // 최대 값을 넘지 않도록 제한
        curValue.Value = Mathf.Min(curValue.Value + value, maxValue);
    }

    public void Substract(float value)
    {
        // 최소 값보다 작아지지 않도록 제한
        curValue.Value = Mathf.Max(curValue.Value - value, 0);
    }

    public float CurValue()
    {
        return curValue.Value;
    }
}
