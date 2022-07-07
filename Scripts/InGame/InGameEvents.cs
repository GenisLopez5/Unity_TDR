using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InGameEvents : MonoBehaviour
{
    public static InGameEvents current { get; private set; }

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this);
            return;
        }

        current = this;

        ConfigEvents.current.GameLoaded();
    }

    public event Action OnInventoryOpened;
    public void InventoryOpened()
    {
        OnInventoryOpened?.Invoke();
    }

    public event Action<bool> OnInventoryClosed;
    public void InventoryClosed(bool WithEsc)
    {
        OnInventoryClosed?.Invoke(WithEsc);
    }

    public event Action<PickableObject> OnPickableObjectInteracted;
    public void PickableObjectInteracted(PickableObject obj)
    {
        OnPickableObjectInteracted?.Invoke(obj);
    }

    public event Action OnGamePaused;
    public void GamePaused()
    {
        OnGamePaused?.Invoke();
    }

    public event Action OnGameResumed;
    public void GameResumed()
    {
        OnGameResumed?.Invoke();
    }

    public event Action<int> OnEquiped;
    public void Equiped(int PlayerSlot)
    {
        OnEquiped?.Invoke(PlayerSlot);
    }

    public event Action<int> OnUnEquiped;
    public void UnEquiped(int PlayerSlot)
    {
        OnUnEquiped?.Invoke(PlayerSlot);
    }
}
