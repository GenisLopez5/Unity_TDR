using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;

public class LoginRegisterUIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI LoggedText;
    [SerializeField]
    GameObject LogOffBtt, LoginBtt, RegisterBtt;
    [SerializeField]
    Transform all;

    private string NickName = null;

    private async void Awake()
    {
        SceneManager.LoadSceneAsync("OptionsMenu", LoadSceneMode.Additive);

        while (!SceneManager.GetSceneByName("optionsMenu").IsValid())
        {
            await Task.Delay(10);
        }

        while (!FindObjectOfType<ConfigurationUIManager>())
        {
            await Task.Delay(10);
        }

        LoginRegisterEvents.current.OnLoggedIn += LoggedIn;
        ConfigEvents.current.OnLanguageChanged += UpdateText;
        ConfigEvents.current.OnConfigClosed += Show;

        UpdateText(ConfigurationUIManager.current.language);
    }

    private void OnDestroy()
    {
        LoginRegisterEvents.current.OnLoggedIn -= LoggedIn;
        ConfigEvents.current.OnLanguageChanged -= UpdateText;
        ConfigEvents.current.OnConfigClosed -= Show;
    }

    public void Login()
    {
        if (SceneManager.GetSceneByName("Login").IsValid()) return;

        SceneManager.LoadSceneAsync("Login", LoadSceneMode.Additive);

        CloseRLScenes();
    }

    public void Register()
    {
        if (SceneManager.GetSceneByName("Register").IsValid()) return;

        SceneManager.LoadSceneAsync("Register", LoadSceneMode.Additive);

        CloseRLScenes();
    }

    void CloseRLScenes()
    {
        if (SceneManager.GetSceneByName("Login").IsValid())
        {
            SceneManager.UnloadSceneAsync("Login");
        }

        if (SceneManager.GetSceneByName("Register").IsValid())
        {
            SceneManager.UnloadSceneAsync("Register");
        }
    }

    public void Play()
    {
        if (NickName == null)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "You need to be logged in to play, please login or register", "Necesitas haber iniciado sesion para jugar, por favor inicia sesión o registrate", "Necesites haver iniciat sessió per jugar, si us plau inicia sessió o registra't"}, new string[3] {"WARNING", "ALERTA", "ALERTA"}));
            return;
        }

        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Master", LoadSceneMode.Additive);
    }

    public void Options()
    {
        ConfigurationUIManager.current.Show();
        Hide();
    }

    public void Close()
    {
        Application.Quit();
    }

    public void LogOut()
    {
        NickName = null;
        PhotonNetwork.NickName = "Guest";
        PlayFabClientAPI.ForgetAllCredentials();

        LogOffBtt.SetActive(false);
        LoginBtt.SetActive(true);
        RegisterBtt.SetActive(true);

        UpdateText(ConfigurationUIManager.current.language);
    }

    void LoggedIn(string name)
    {
        NickName = name;

        UpdateText(ConfigurationUIManager.current.language);

        LogOffBtt.SetActive(true);
        LoginBtt.SetActive(false);
        RegisterBtt.SetActive(false);
    }

    async void Show()
    {
        float v = 0, nx = -1920;

        while (nx < -0.01f)
        {
            nx = Mathf.SmoothDamp(nx, 0, ref v, 0.25f);
            all.localPosition = new Vector3(nx, 0, 0);

            await Task.Yield();
        }
    }

    async void Hide()
    {
        float v = 0, nx = 0;

        while (nx > -1919.9f)
        {
            nx = Mathf.SmoothDamp(nx, -1920, ref v, 0.25f);
            all.localPosition = new Vector3(nx, 0, 0);

            await Task.Yield();
        }
    }

    void UpdateText(int language)
    {
        if (NickName == null)
        {
            switch (language)
            {
                case 0:
                    try { LoggedText.text = "Your are not logged in, please login or register to play"; }
                    catch { }
                    break;
                case 1:
                    try { LoggedText.text = "No has iniciado sesión, por favor inicia sesión o registrate para poder jugar"; }
                    catch { }
                    break;
                case 2:
                    try { LoggedText.text = "No has iniciat sessió, si us plau inicia sessió o registra't per a poder jugar"; }
                    catch { }
                    break;
            }
        }
        else
        {
            switch (language)
            {
                case 0:
                    LoggedText.text = "Logged in as: " + NickName;
                    break;
                case 1:
                    LoggedText.text = "Sesión iniciada como: " + NickName;
                    break;
                case 2:
                    LoggedText.text = "Sessió iniciada com: " + NickName;
                    break;
            }
        }
    }
}
