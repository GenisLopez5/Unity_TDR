using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockManagement : MonoBehaviour
{
    private void Start()
    {
        LockCursor();
    }

    private void Awake()
    {
        InGameEvents.current.OnGamePaused += UnlockCursor;
        InGameEvents.current.OnInventoryOpened += UnlockCursor;

        InGameEvents.current.OnGameResumed += LockCursor;
        InGameEvents.current.OnInventoryClosed += InventoryClosed;
    }

    private void OnDestroy()
    {
        InGameEvents.current.OnGamePaused -= UnlockCursor;
        InGameEvents.current.OnInventoryOpened -= UnlockCursor;

        InGameEvents.current.OnGameResumed -= LockCursor;
        InGameEvents.current.OnInventoryClosed -= InventoryClosed;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void InventoryClosed(bool withEsc)
    {
        if (!withEsc) LockCursor();
    }
}
