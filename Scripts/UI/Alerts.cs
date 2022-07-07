using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Alerts
{
    public IEnumerator NewAlert(string[] AlertMsg, string[] AlertTitle)
    {
        yield return SceneManager.LoadSceneAsync("Alerts", LoadSceneMode.Additive);
        GameObject.Find("AlertText").GetComponent<TextMeshProUGUI>().SetText(AlertMsg[ConfigurationUIManager.current.language]);
        GameObject.Find("AlertTitleText").GetComponent<TextMeshProUGUI>().SetText(AlertTitle[ConfigurationUIManager.current.language]);
    }
}
