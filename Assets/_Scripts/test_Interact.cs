using TMPro;
using UnityEngine;

public class test_Interact : MonoBehaviour
{
    Camera cmra;
    float hitRange = 2f;
    LayerMask nonPlayerLayer;
    LayerMask pickUpLayer;

    [SerializeField] TMP_Text text_PressToInteract;
    bool canInteract = true;

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
    }

    private bool CheckForInteractable()
    {
        if (Physics.Raycast(cmra.transform.position, cmra.transform.forward, out RaycastHit hitinfo, hitRange, LayerMask.GetMask("Interactable")))
        {
            return hitinfo.collider.CompareTag("Collectible") ||
                   hitinfo.collider.CompareTag("Interactable");
        }
        return false;
    }

    void Interact()
    {
        if (Physics.Raycast(cmra.transform.position, cmra.transform.forward, out RaycastHit hitinfo, hitRange, LayerMask.GetMask("Interactable")))
        {
            if (hitinfo.collider.CompareTag("Collectible"))
            {
                //collect Item and add to timebank
            }
            if (hitinfo.collider.CompareTag("Interactable"))
            {
                // focus camera on the interactable object
                // activate the interactable set up
                // deactivate player movement

            }
        }
    }
}
