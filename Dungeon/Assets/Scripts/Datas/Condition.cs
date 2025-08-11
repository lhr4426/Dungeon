using System;
using UnityEngine;

[Serializable]
public class Condition
{
    private string observerKey;
    Observable<float> curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue; // �ֱ������� ���ϴ� ��

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
        // �ִ� ���� ���� �ʵ��� ����
        curValue.Value = Mathf.Min(curValue.Value + value, maxValue);
    }

    public void Substract(float value)
    {
        // �ּ� ������ �۾����� �ʵ��� ����
        curValue.Value = Mathf.Max(curValue.Value - value, 0);
    }

    public float CurValue()
    {
        return curValue.Value;
    }
}
