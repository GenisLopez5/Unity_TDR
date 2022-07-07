using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem Item;

    [SerializeField]
    private Image ItemSprite;
    [SerializeField]
    private TextMeshProUGUI Amount;

    public bool isPlayerSlot;
    [SerializeField]
    private bool isIINV = false;
    public byte num;

    bool equiped = false;

    private void Start()
    {
        UpdateAmountText();
        UpdateImage();
    }

    private void OnEnable()
    {
        UpdateAmountText();
        UpdateImage();
    }

    public void Initialize()
    {
        if (isPlayerSlot && !isIINV)
        {
            Debug.Log("test1");
            InGameEvents.current.OnEquiped += Equiped;
            InGameEvents.current.OnUnEquiped += Unequiped;

            if (Item == null) gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (!isPlayerSlot || !isIINV) return;
        InGameEvents.current.OnEquiped -= Equiped;
        InGameEvents.current.OnUnEquiped -= Unequiped;
    }

    public void UpdateAmountText()
    {
        if (Item != null)
        {
            if (Item.objAmount == 1) Amount.text = "";
            else Amount.text = Item.objAmount.ToString();
        }
        else
        {
            Amount.text = "";
        }
    }

    public void UpdateImage()
    {
        if (isPlayerSlot && !isIINV)
        {
            if (Item != null)
            {
                ItemSprite.sprite = InventoryUiControl.SpriteList[Item.ID];
                if (equiped) ItemSprite.color = new Color(1, 1, 1, 1);
                else ItemSprite.color = new Color(1, 1, 1, 0.33f);
            }
            else
            {
                ItemSprite.sprite = null;
                ItemSprite.color = new Color(1, 1, 1, 0);
            }

            return;
        }

        if (Item != null)
        {
            ItemSprite.sprite = InventoryUiControl.SpriteList[Item.ID];
            ItemSprite.color = new Color(1, 1, 1, 1);
        }
        else
        {
            ItemSprite.sprite = null;
            ItemSprite.color = new Color(1, 1, 1, 0);
        }
    }

    public void RemoveItem()
    {
        Destroy(Item);
        Item = null;

        UpdateImage();
        UpdateAmountText();
    }

    public void AddItem(InventoryItem newItem, int am)
    {
        Item = gameObject.AddComponent<InventoryItem>();
        Item.SetVariablesUI(am, newItem.ObjMaxAmmount, newItem.ID, newItem.type, newItem.equipable);

        UpdateImage();
        UpdateAmountText();
    }

    public void AddItemThroughPickable(PickableObject obj, int am)
    {
        Item = gameObject.AddComponent<InventoryItem>();
        Item.SetVariablesUI(am, obj.maxAmmount, obj.ID, obj.type, obj.equipable);

        UpdateImage();
        UpdateAmountText();
    }

    void Equiped(int slot)
    {
        if (slot == num)
        {
            equiped = true;
            UpdateImage();
        }
    }

    void Unequiped(int slot)
    {
        if (slot == num)
        {
            if (!equiped) return;

            equiped = false;
            UpdateImage();
        }
    }
}
