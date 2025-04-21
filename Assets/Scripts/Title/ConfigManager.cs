using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    public SaveManager saveManager; // SaveManagerの参照

    public List<Image> volumeButtons; // VolumeボタンのImageコンポーネントリスト (0~9)
    public Sprite volumeOnSprite; // VolumeボタンがOnのときの画像
    public Sprite volumeOffSprite; // VolumeボタンがOffのときの画像

    public List<Image> difficultButtons; // 難易度ボタンのImageコンポーネントリスト
    public Sprite difficultOnSprite; // 難易度ボタンがOnのときの画像
    public Sprite difficultOffSprite; // 難易度ボタンがOffのときの画像

    public List<Image> cheatModeButtons; // CheatModeボタンのImageコンポーネントリスト (0: Off, 1: On)
    public Sprite cheatModeOnSprite; // CheatModeがOnのときの画像
    public Sprite cheatModeOffSprite; // CheatModeがOffのときの画像

    public Image muteButton; // MuteボタンのImageコンポーネント
    public Sprite muteOnSprite; // MuteがOnのときの画像
    public Sprite muteOffSprite; // MuteがOffのときの画像

    private int difficult = 0; // 初期難易度 (0: Easy, 1: Normal, 2: Hard)
    private bool isCheatMode = false; // 初期CheatMode状態
    private bool isMute = false; // 初期Mute状態
    private int volume = 5; // 初期Volume値 (0~9)

    void Start()
    {
        // 初期化時にセーブデータをロード
        Debug.Log("ConfigManager Start: セーブデータをロードします。");
        LoadConfigFromSave();

        UpdateDifficultButtons();
        UpdateCheatModeButtons();
        UpdateMuteButton();
        UpdateVolumeButtons();
    }

    void OnEnable()
    {
        Debug.Log("ConfigManager OnEnable: セーブデータをロードします。");
        // アクティブ化時にセーブデータをロード
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
            Debug.LogError("SaveManagerが設定されていません。");
        }
    }

    public void SetDifficult(int value)
    {
        difficult = Mathf.Clamp(value, 0, 2); // 0~2の範囲に制限
        UpdateDifficultButtons();

        // SaveManagerに保存
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

        // SaveManagerに保存
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

        // SaveManagerに保存
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
        volume = Mathf.Clamp(value, 0, 9); // 0~9の範囲に制限
        UpdateVolumeButtons();

        // SaveManagerに保存
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
