using System;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [Header("Stat Settings")]
    public Condition health;
    public Condition hunger;
    public Condition stamina;
    public float noHungerHealthDecay;

    public event Action onTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        health.Init(EventBusKeys.ChangeHealthKey);
        hunger.Init(EventBusKeys.ChangeHungerKey);
        stamina.Init(EventBusKeys.ChangeStaminaKey);
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Substract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.CurValue() == 0f)
        {
            health.Substract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.CurValue() == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount >= 0) health.Add(amount);
        else health.Substract(-amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Rest(float amount)
    {
        stamina.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.CurValue() - amount < 0f)
        {
            return false;
        }
        stamina.Substract(amount);
        return true;
    }


    public void Die()
    {
        Debug.Log("ав╬З╢ы!");
    }

    public void TakeDamage(int damage)
    {
        health.Substract(damage);
        onTakeDamage?.Invoke();
    }
}
