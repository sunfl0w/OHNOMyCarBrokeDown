using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InventoryGUI : MonoBehaviour
{

    public GameObject InventoryCanvas;
    private bool canvasActivated;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public GameObject itemViewer;

    private int currentItemIndex = 0;
    private ItemData currentItemData;

    private GameObject itemPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && canvasActivated)
        {
            InventoryCanvas.SetActive(false);
            canvasActivated = false;
        }
        else if (Input.GetButtonDown("Inventory") && !canvasActivated)
        {
            InventoryCanvas.SetActive(true);
            canvasActivated = true;
            PlayerInventory.Instance.printItems();
            updateUI();
        }


    }

    void updateUI()
    {
        if (PlayerInventory.Instance.items.Count == 0)
        {
            itemName.text = "No item in inventory.";
            itemDescription.text = "";
            itemViewer.SetActive(false);

        }
        else
        {
            (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
            currentItemData = tuple.item;
            itemName.text = currentItemData.itemName;
            itemDescription.text = currentItemData.interactText;

            if (itemPrefab != null)
            {
                Destroy(itemPrefab.gameObject);
            }
            itemPrefab = Instantiate(currentItemData.prefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
            itemViewer.SetActive(true);
        }
    }




}
