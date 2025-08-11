using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    // 얼마나 자주 검출할 지
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask interactLayerMask;

    // 캐싱
    public GameObject curInteractGO;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxCheckDistance, interactLayerMask))
            {
                if(hit.collider.gameObject != curInteractGO)
                {
                    curInteractGO = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGO = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }


    public void OnInteraction(InputAction.CallbackContext context)
    {
        if(curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractable = null;
            curInteractGO = null;
            Destroy(curInteractGO);
        }
    }
}
