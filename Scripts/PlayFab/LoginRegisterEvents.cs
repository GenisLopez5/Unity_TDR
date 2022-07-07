using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoginRegisterEvents : MonoBehaviour
{
    public static LoginRegisterEvents current { get; private set; }

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this);
            return;
        }

        current = this;
    }

    public event Action<string> OnLoggedIn;
    public void LoggedIn(string name)
    {
        OnLoggedIn?.Invoke(name);
    }

    public event Action OnConfigLoaded;
    public void ConfigLoaded()
    {
        OnConfigLoaded?.Invoke();
    }
}
