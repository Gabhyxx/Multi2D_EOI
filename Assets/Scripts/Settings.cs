using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject soundPanel;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionsDropdown;

    Resolution[] resolutions;
    FullScreenMode[] fullScreenModes;
    private void Start()
    {
        settingsPanel.SetActive(false);
        soundPanel.SetActive(false);
        FindResolutions();
    }
    #region Settings and Sound Panel
    public void EnableSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }
    public void DisableSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
    public void EnableSoundPanel()
    {
        soundPanel.SetActive(true);
    }
    public void DisableSoundPanel()
    {
        soundPanel.SetActive(false);
    }
    #endregion
    #region Resolution
    void FindResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        for (int i = resolutions.Length - 1; i >= 0; i--) 
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = 0;
        resolutionsDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        int index = resolutions.Length - 1;
        index -= resolutionIndex;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }
    #endregion
    #region Fullscreen Mode
    public void SetScreenMode(int fsmIndex)
    {
        Debug.Log(fsmIndex);
        if (fsmIndex == 0)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (fsmIndex == 1)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if ( fsmIndex == 2)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }
    public void FullscreenMode()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }
    public void Windowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    public void Borderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    #endregion
    #region Volume
    public void SetMaster(float master)
    {
        audioMixer.SetFloat("Master", master);
    }
    public void SetMusic(float music)
    {
        audioMixer.SetFloat("Music", music);
    }
    #endregion
}
