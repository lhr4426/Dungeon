using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conditions : MonoBehaviour
{
    public Image healthBar;
    public Image hungerBar;
    public Image staminaBar;

    private void OnEnable()
    {
        EventBus.Subscribe(EventBusKeys.ChangeHealthKey, OnHealthChanged);
        EventBus.Subscribe(EventBusKeys.ChangeHungerKey, OnHungerChanged);
        EventBus.Subscribe(EventBusKeys.ChangeStaminaKey, OnStaminaChanged);

    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusKeys.ChangeHealthKey, OnHealthChanged);
        EventBus.Unsubscribe(EventBusKeys.ChangeHungerKey, OnHungerChanged);
        EventBus.Unsubscribe(EventBusKeys.ChangeStaminaKey, OnStaminaChanged);
    }
    
    private void OnHealthChanged(object data)
    {
        healthBar.fillAmount = PlayerManager.Player.condition.health.GetPercent();
    }

    private void OnHungerChanged(object data)
    {
        hungerBar.fillAmount = PlayerManager.Player.condition.hunger.GetPercent();
    }

    private void OnStaminaChanged(object data)
    {
        staminaBar.fillAmount = PlayerManager.Player.condition.stamina.GetPercent();
    }
}
