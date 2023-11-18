using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : MonoBehaviour, IInteractable
{
    public string interactText;
    public Item item;

    public void Interact()
    {
        //Debug.Log("Interact");
        Inventory.instance.AddItem(item);
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
