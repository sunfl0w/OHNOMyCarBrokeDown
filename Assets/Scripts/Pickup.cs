using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Pickup : MonoBehaviour
{
    public Item item;
    public GameObject pickupDialog;
    public TextMeshProUGUI pickupText;

    public FirstPersonController firstPersonController;

    private bool canRotate = false;


    void Update()
    {
        if (canRotate)
        {
            gameObject.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {

            ShowPickupDialog();
            //Inventory.instance.AddItem(item);
            //Destroy(gameObject);
        }
    }

    void ShowPickupDialog()
    {

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;


        // Set the position of the item in front of the camera
        gameObject.transform.position = cameraPosition + cameraForward * 3f; // Adjust the distance as needed
        gameObject.transform.LookAt(cameraPosition);

        firstPersonController.canMove = false;
        canRotate = true;

        pickupText.text = "I found a " + item.name + ".";
        pickupDialog.SetActive(true);

        StartCoroutine(DisableDialog());



    }

    IEnumerator DisableDialog()
    {
        yield return new WaitForSeconds(2f);
        pickupDialog.SetActive(false);
        Inventory.instance.AddItem(item);
        Destroy(gameObject);

        firstPersonController.canMove = true;

    }




}
