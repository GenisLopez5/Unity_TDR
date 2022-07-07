using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public int ObjMaxAmmount;
    public int objAmount;
    public int ID = -1;
    public bool equipable;
    public PickableObject.Type type;

    public InventorySlot itSlot;

    private void Start()
    {
        itSlot = GetComponent<InventorySlot>();

        if (type == PickableObject.Type.tool || type == PickableObject.Type.food) equipable = true;
    }

    public void SetVariables(PickableObject pickedObject)
    {
        objAmount = pickedObject.ammount;
        ObjMaxAmmount = pickedObject.maxAmmount;
        ID = pickedObject.ID;
        type = pickedObject.type;
        equipable = pickedObject.equipable;
    }

    public void SetVariablesUI(int a, int ma, int id, PickableObject.Type _type, bool _equipable)
    {
        ObjMaxAmmount = ma;
        objAmount = a;
        ID = id;
        type = _type;
        equipable = _equipable;
    }
}
