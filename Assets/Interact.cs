using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public Transform playerCamera;
    public float distance = 1;
    public LayerMask Interactables;
    public GameObject pressE;
    public GameObject StoreUI;
    public Transform itemHolder;

    public CharacterController_FirstPerson playerLook; // ✅ This is your Movement script

    void Update()
    {
        Debug.DrawRay(playerCamera.position, playerCamera.forward * distance, Color.red);

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, distance, Interactables))
        {
            int hitLayer = hitInfo.collider.gameObject.layer;
          
            if (hitLayer == LayerMask.NameToLayer("pickable"))
            {
                pressE.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUp(hitInfo);

                    // Optional: You could add a check here to ensure only one item can be picked at a time
                }
            }



        }
        else
        {
            pressE.SetActive(false);
            StoreUI.SetActive(false);
           
        }
    }
    public void PickUp(RaycastHit hitInfo)
    {
        GameObject item = hitInfo.collider.gameObject;

        // Parent to item holder
        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero; // Position it at holder's pivot
        item.transform.localRotation = Quaternion.identity; // Reset rotation (optional)

        // Disable physics
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Disable collider (optional: depends on your use case)
        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
   
}
