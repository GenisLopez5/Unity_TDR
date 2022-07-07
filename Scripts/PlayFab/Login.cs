using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class Login : MonoBehaviour
{
    public TMP_InputField UsernameInput, PasswordInput;

    public void LoginClicked()
    {
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
        request.Username = UsernameInput.text;
        request.Password = PasswordInput.text;
        request.TitleId = "27662";

        PlayFabClientAPI.LoginWithPlayFab(request, LoginSuccess, LoginFailure);
    }

    public void LoginSuccess(LoginResult result)
    {
        PhotonNetwork.NickName = UsernameInput.text;
        SceneManager.UnloadSceneAsync(gameObject.scene);

        LoginRegisterEvents.current.LoggedIn(UsernameInput.text);
    }

    public void LoginFailure(PlayFabError error)
    {
        Alerts a = new Alerts();
        StartCoroutine(a.NewAlert(new string[3] { error.ErrorMessage, error.ErrorMessage, error.ErrorMessage }, new string[3] { "ERROR", "ERROR", "ERROR" }));
    }
}
