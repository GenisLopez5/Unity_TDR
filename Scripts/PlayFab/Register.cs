using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class Register : MonoBehaviour
{
    public TMP_InputField UserNameText, Pass1Text, Pass2Text, MailText;

    public void ClickedRegister()
    {
        if (UserNameText.text.Length < 4)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "Username lenght must be 4 or more characters", "El nombre de usuario debe contener 4 o más carácteres", "El nom d'usuari ha de ser de 4 o més caràcters"}, new string[3] { "WARNING", "ALERTA", "ALERTA" }));
            return;
        }
        if (Pass1Text.text.Length < 6 || Pass2Text.text.Length < 6)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "Passwords lenght must be 6 or more characters", "La contraseña debe ser de 6 o más carácteres", "La contrasenya ha de ser de 6 o més caràcters"}, new string[3] { "WARNING", "ALERTA", "ALERTA" }));
            return;
        }
        if (Pass1Text.text != Pass2Text.text)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "Passwords don't coincide", "Las contraseñas no coinciden", "Les contrasenyes no coincideixen"}, new string[3] { "WARNING", "ALERTA", "ALERTA" }));
            return;
        }
        if (Pass1Text.text == UserNameText.text || Pass2Text.text == UserNameText.text)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "The password can't be the same as the name", "La contraseña no puede ser lo mismo que el nombre", "La contrasenya no pot ser el mateix que el nom"}, new string[3] { "WARNING", "ALERTA", "ALERTA" }));
            return;
        }

        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
        request.RequireBothUsernameAndEmail = true;
        request.TitleId = "27662";
        request.Username = UserNameText.text;
        request.Password = Pass1Text.text;
        request.Email = MailText.text;
        request.DisplayName = UserNameText.text;

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSucces, RegisterError);
    }


    public void RegisterSucces(RegisterPlayFabUserResult result)
    {
        PhotonNetwork.NickName = UserNameText.text;

        LoginRegisterEvents.current.LoggedIn(UserNameText.text);

        Alerts a = new Alerts();
        StartCoroutine(a.NewAlert(new string[3] { "You have been succesfuly registered as " + result.Username, "Has sido exitosamente registrado como " + result.Username, "Has estat exitosament registrat com " + result.Username}, new string[3] {"SUCCESS", "ÉXITO", "ÈXIT"}));

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    public void RegisterError(PlayFabError error)
    {
        Alerts a = new Alerts();
        StartCoroutine(a.NewAlert(new string[3] { error.ErrorMessage , error.ErrorMessage , error.ErrorMessage }, new string[3] {"ERROR","ERROR","ERROR"}));
    }
}