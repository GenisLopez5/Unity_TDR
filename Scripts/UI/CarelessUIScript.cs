using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CarelessUIScript : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImg;
    [SerializeField]
    private Transform BttPanel;
    [SerializeField]
    private Button[] btt;

    public static bool paused = false, optionsOpened = false, transitioning = false;

    private void Awake()
    {
        ConfigEvents.current.OnConfigClosed += ConfigClosed;

        DisableBtt();
    }

    private void OnDestroy()
    {
        ConfigEvents.current.OnConfigClosed -= ConfigClosed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !optionsOpened && !transitioning && !paused)
        {
            Show();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !optionsOpened && !transitioning && paused)
        {
            Hide();
        }
    }

    public async void Hide()
    {
        if (transitioning || !paused) return;

        paused = false;

        transitioning = true;

        DisableBtt();

        float v = 0, ny = 0;

        if (!optionsOpened) InGameEvents.current.GameResumed();

        while (ny < 1919)
        {
            ny = Mathf.SmoothDamp(ny, 1920, ref v, 0.15f);
            BttPanel.localPosition = new Vector3(0, ny, 0);
            backgroundImg.color = new Color(0, 0, 0, 0.5f - (ny / 3840));

            await Task.Yield();
        }

        backgroundImg.gameObject.SetActive(false);

        transitioning = false;
    }

    async void Show()
    {
        if (transitioning || paused) return;

        paused = true;

        transitioning = true;

        backgroundImg.gameObject.SetActive(true);

        float v = 0, ny = 1080;

        if (!optionsOpened) InGameEvents.current.GamePaused();

        while (ny > 1)
        {
            ny = Mathf.SmoothDamp(ny, 0, ref v, 0.15f);

            BttPanel.localPosition = new Vector3(0, ny, 0);
            backgroundImg.color = new Color(0, 0, 0, 0.5f - (ny / 2160));
            await Task.Yield();
        }

        EnableBtt();

        transitioning = false;
    }

    async void ConfigClosed()
    {
        Show();
        await Task.Yield();
        optionsOpened = false;
    }

    public void ShowOptions()
    {
        if (transitioning) return;

        optionsOpened = true;
        Hide();
        ConfigurationUIManager.current.Show();
    }

    void DisableBtt()
    {
        for (int i = 0; i < btt.Length; i++)
        {
            btt[i].enabled = false;
        }
    }

    void EnableBtt()
    {
        for (int i = 0; i < btt.Length; i++)
        {
            btt[i].enabled = true;
        }
    }

    public void QuitGame()
    {

    }
}