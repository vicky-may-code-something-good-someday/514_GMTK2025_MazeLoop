using TMPro;
using UnityEngine;

public class Player_Interact : MonoBehaviour
{
    Camera cmra;
    float hitRange = 2f;
    InteractableObj targetInteractable;


    [SerializeField] TMP_Text text_PressToInteract;

    void Start()
    {
        cmra = Camera.main;

        if (text_PressToInteract == null)
        {
            Debug.LogError("Text_PressToInteract is not assigned in the inspector.");
        }
        else
        {
            text_PressToInteract.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (CheckForInteractable())
        {
            text_PressToInteract.gameObject.SetActive(true); // Enable UI only if an interactable is in sight
            //InteractableLight.intensity = LightIntensity;


            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }

        }
        else if (text_PressToInteract.gameObject.activeSelf)
        {
            text_PressToInteract.gameObject.SetActive(false); // Disable UI if no interactable is in sight
            //InteractableLight.intensity = 0f;
        }
        else if (targetInteractable != null)
        {
            targetInteractable = null; // Reset target if no interactable is in sight
        }
    }

    private bool CheckForInteractable()
    {
        if (Physics.Raycast(cmra.transform.position, cmra.transform.forward, out RaycastHit hitinfo, hitRange, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide))
        {
            hitinfo.collider.TryGetComponent<InteractableObj>(out InteractableObj interactableObj);
            if (interactableObj != null)
            {
                targetInteractable = interactableObj;
                return true;
            }
        }
        return false;
    }

    void Interact()
    {
        if (targetInteractable != null)
        {
            targetInteractable.GetInteracted();
        }

    }
}
