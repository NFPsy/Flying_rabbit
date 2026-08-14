using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string gameSceneName = "Round1";
    [SerializeField] private Toggle soundToggle;

    private const string SoundPrefKey = "SoundOn";

    private void Awake()
    {
        bool soundOn = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;
        AudioListener.volume = soundOn ? 1f : 0f;
        if (soundToggle != null)
            soundToggle.SetIsOnWithoutNotify(soundOn);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnSoundToggleChanged(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
        PlayerPrefs.SetInt(SoundPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
