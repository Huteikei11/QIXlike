using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("BGM Clips")]
    public AudioClip bgmCharacter1; // キャラクター1用BGM
    public AudioClip bgmCharacter2; // キャラクター2用BGM
    public AudioClip bgmCharacter3; // キャラクター3用BGM
    public AudioClip bgmDefault;    // デフォルトBGM (キャラクター未選択時)

    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSourceがアタッチされていません。");
            return;
        }

        UpdateAudioSettings(); // ミュートと音量を設定
        PlayBGMBasedOnCharacter();
    }

    private void PlayBGMBasedOnCharacter()
    {
        int selectedCharacter = SaveManager.Instance.GetCharacter(); // 選択されたキャラクターを取得

        switch (selectedCharacter)
        {
            case 0: // 霊夢
                PlayBGM(bgmCharacter1);
                break;
            case 1: // アリス
                PlayBGM(bgmCharacter1);
                break;
            case 2: // うどんげ
                PlayBGM(bgmCharacter2);
                break;
            case 3: // 咲夜
                PlayBGM(bgmCharacter2);
                break;
            case 4: // 妖夢
                PlayBGM(bgmCharacter2);
                break;
            case 5: // 魔理沙
                PlayBGM(bgmCharacter1);
                break;
            case 6: // ぬえ
                PlayBGM(bgmCharacter3);
                break;
            default:
                PlayBGM(bgmDefault); // キャラクター未選択時
                break;
        }
    }

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("指定されたBGMクリップがありません。");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log($"BGMを再生中: {clip.name}");
    }

    private void UpdateAudioSettings()
    {
        // SaveManagerからミュート状態と音量を取得
        bool isMute = SaveManager.Instance.GetMute();
        int volumeLevel = SaveManager.Instance.GetVolume();

        // AudioSourceの設定を更新
        audioSource.mute = isMute;
        audioSource.volume = Mathf.Clamp01(volumeLevel / 9f); // 0~9を0.0~1.0に変換

        Debug.Log($"Audio設定を更新: Mute={isMute}, Volume={audioSource.volume}");
    }
}