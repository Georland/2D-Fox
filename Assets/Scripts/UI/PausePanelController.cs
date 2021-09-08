using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PausePanelController : MonoBehaviour
{

    public AudioMixer mixer;

    // Start is called before the first frame update
    public void PanelEnable()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PanelDisable()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SetVolume(float volumeValue)
    {
        mixer.SetFloat("MainVolume", volumeValue);
    }
}
