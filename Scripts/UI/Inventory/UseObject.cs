using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseObject : MonoBehaviour
{
    [SerializeField]
    private InventorySlot[] playerSlot = new InventorySlot[4];
    [SerializeField]
    private Transform Offset;
    [SerializeField]
    private GameObject[] ITEM;

    [SerializeField]
    int currentEquiped = -1;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (playerSlot[0].Item == null) return;

            if (currentEquiped != 0)
            {
                InGameEvents.current.Equiped(0);
                InGameEvents.current.UnEquiped(currentEquiped);

                currentEquiped = 0;

                Equiped(0);
                Unequiped();
            }
            else
            {
                currentEquiped = -1;

                InGameEvents.current.UnEquiped(0);
                Unequiped();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (playerSlot[1].Item == null) return;

            if (currentEquiped != 1)
            {
                InGameEvents.current.Equiped(1);
                InGameEvents.current.UnEquiped(currentEquiped);

                currentEquiped = 1;

                Equiped(1);
                Unequiped();
            }
            else
            {
                currentEquiped = -1;

                InGameEvents.current.UnEquiped(1);
                Unequiped();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (playerSlot[2].Item == null) return;

            if (currentEquiped != 2)
            {
                InGameEvents.current.Equiped(0);
                InGameEvents.current.UnEquiped(currentEquiped);

                currentEquiped = 2;

                Equiped(2);
                Unequiped();
            }
            else
            {
                currentEquiped = -1;

                InGameEvents.current.UnEquiped(2);
                Unequiped();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (playerSlot[3].Item == null) return;

            if (currentEquiped != 3)
            {
                InGameEvents.current.Equiped(3);
                InGameEvents.current.UnEquiped(currentEquiped);

                currentEquiped = 3;

                Equiped(3);
                Unequiped();
            }
            else
            {
                currentEquiped = -1;

                InGameEvents.current.UnEquiped(3);
                Unequiped();
            }

            return;
        }
    }

    GameObject item, InstantiedItem;

    void Equiped(int slot) //FIX THIS SHIT
    {
        item = ITEM[playerSlot[slot].Item.ID];

        Offset.localPosition = item.GetComponent<EquipableItem>().offsetPos;

        InstantiedItem = Instantiate(item, Vector3.zero, Quaternion.Euler(item.GetComponent<EquipableItem>().ItemRot), Offset);
    }

    void Unequiped()
    {
        Destroy(InstantiedItem);
    }
}
