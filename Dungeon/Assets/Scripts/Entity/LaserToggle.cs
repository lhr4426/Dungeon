using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserToggle : MonoBehaviour, IInteractable
{
    public LaserObject laser;
    public string description;
    public Renderer render;

    private void Start()
    {
        ColorChange();
    }

    public string GetInteractPrompt()
    {
        return description;
    }

    public void OnInteract()
    {
        laser.ToggleButton();
        ColorChange();
    }

    void ColorChange()
    {
        if (laser.IsOn)
        {
            render.material.color = Color.red;
        }
        else
        {
            render.material.color = Color.blue;
        }
    }
}
