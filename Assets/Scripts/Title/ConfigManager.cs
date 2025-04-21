using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    public SaveManager saveManager; // SaveManager�̎Q��

    public List<Image> volumeButtons; // Volume�{�^����Image�R���|�[�l���g���X�g (0~9)
    public Sprite volumeOnSprite; // Volume�{�^����On�̂Ƃ��̉摜
    public Sprite volumeOffSprite; // Volume�{�^����Off�̂Ƃ��̉摜

    public List<Image> difficultButtons; // ��Փx�{�^����Image�R���|�[�l���g���X�g
    public Sprite difficultOnSprite; // ��Փx�{�^����On�̂Ƃ��̉摜
    public Sprite difficultOffSprite; // ��Փx�{�^����Off�̂Ƃ��̉摜

    public List<Image> cheatModeButtons; // CheatMode�{�^����Image�R���|�[�l���g���X�g (0: Off, 1: On)
    public Sprite cheatModeOnSprite; // CheatMode��On�̂Ƃ��̉摜
    public Sprite cheatModeOffSprite; // CheatMode��Off�̂Ƃ��̉摜

    public Image muteButton; // Mute�{�^����Image�R���|�[�l���g
    public Sprite muteOnSprite; // Mute��On�̂Ƃ��̉摜
    public Sprite muteOffSprite; // Mute��Off�̂Ƃ��̉摜

    private int difficult = 0; // ������Փx (0: Easy, 1: Normal, 2: Hard)
    private bool isCheatMode = false; // ����CheatMode���
    private bool isMute = false; // ����Mute���
    private int volume = 5; // ����Volume�l (0~9)

    void Start()
    {
        // ���������ɃZ�[�u�f�[�^�����[�h
        Debug.Log("ConfigManager Start: �Z�[�u�f�[�^�����[�h���܂��B");
        LoadConfigFromSave();

        UpdateDifficultButtons();
        UpdateCheatModeButtons();
        UpdateMuteButton();
        UpdateVolumeButtons();
    }

    void OnEnable()
    {
        Debug.Log("ConfigManager OnEnable: �Z�[�u�f�[�^�����[�h���܂��B");
        // �A�N�e�B�u�����ɃZ�[�u�f�[�^�����[�h
        LoadConfigFromSave();

        UpdateDifficultButtons();
        UpdateCheatModeButtons();
        UpdateMuteButton();
        UpdateVolumeButtons();
    }

    private void LoadConfigFromSave()
    {
        if (saveManager != null)
        {
            var saveData = saveManager.GetSaveData();
            difficult = saveData.difficult;
            isCheatMode = saveData.cheatMode;
            isMute = saveData.mute;
            volume = saveData.volume;
        }
        else
        {
            Debug.LogError("SaveManager���ݒ肳��Ă��܂���B");
        }
    }

    public void SetDifficult(int value)
    {
        difficult = Mathf.Clamp(value, 0, 2); // 0~2�͈̔͂ɐ���
        UpdateDifficultButtons();

        // SaveManager�ɕۑ�
        if (saveManager != null)
        {
            saveManager.SetDifficult(difficult);
        }
    }

    private void UpdateDifficultButtons()
    {
        for (int i = 0; i < difficultButtons.Count; i++)
        {
            if (difficultButtons[i] != null)
            {
                difficultButtons[i].sprite = i == difficult ? difficultOnSprite : difficultOffSprite;
            }
        }
    }

    public void ToggleCheatMode()
    {
        isCheatMode = !isCheatMode;
        UpdateCheatModeButtons();

        // SaveManager�ɕۑ�
        if (saveManager != null)
        {
            saveManager.SetCheatMode(isCheatMode);
        }
    }

    private void UpdateCheatModeButtons()
    {
        if (cheatModeButtons.Count >= 2)
        {
            if (cheatModeButtons[0] != null)
                cheatModeButtons[0].sprite = isCheatMode ? cheatModeOffSprite : cheatModeOnSprite;

            if (cheatModeButtons[1] != null)
                cheatModeButtons[1].sprite = isCheatMode ? cheatModeOnSprite : cheatModeOffSprite;
        }
    }

    public void ToggleMute()
    {
        isMute = !isMute;
        UpdateMuteButton();

        // SaveManager�ɕۑ�
        if (saveManager != null)
        {
            saveManager.SetMute(isMute);
        }
    }

    private void UpdateMuteButton()
    {
        if (muteButton != null)
        {
            muteButton.sprite = isMute ? muteOnSprite : muteOffSprite;
        }
    }

    public void SetVolume(int value)
    {
        volume = Mathf.Clamp(value, 0, 9); // 0~9�͈̔͂ɐ���
        UpdateVolumeButtons();

        // SaveManager�ɕۑ�
        if (saveManager != null)
        {
            saveManager.SetVolume(volume);
        }
    }

    private void UpdateVolumeButtons()
    {
        for (int i = 0; i < volumeButtons.Count; i++)
        {
            if (volumeButtons[i] != null)
            {
                volumeButtons[i].sprite = i <= volume ? volumeOnSprite : volumeOffSprite;
            }
        }
    }
}
