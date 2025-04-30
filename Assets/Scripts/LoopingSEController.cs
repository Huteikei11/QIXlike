using UnityEngine;

public class LoopingSEController : MonoBehaviour
{
    public AudioSource audioSource;
    public PlayerMovementpix player; // PlayerMovementpixの参照
    public AudioClip outSound;       // 再生する音声クリップ

    private bool wasOut = false; // 前回のisOutの状態を記録

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSourceがアタッチされていません。");
            return;
        }

        if (player == null)
        {
            Debug.LogError("PlayerMovementpixの参照が設定されていません。");
            return;
        }

        UpdateAudioSettings(); // ミュートと音量を設定
    }

    private void Update()
    {
        if (outSound == null)
        {
            Debug.LogWarning("再生するAudioClipが設定されていません。");
            return;
        }

        // isOutの状態が変化した場合に処理を実行
        if (player.isOut && !wasOut)
        {
            PlayLoopingSE(outSound); // 音声を再生
        }
        else if (!player.isOut && wasOut)
        {
            StopSE(); // 音声を停止
        }

        // 現在のisOutの状態を記録
        wasOut = player.isOut;
    }

    public void PlayLoopingSE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("指定されたSEクリップがありません。");
            return;
        }

        UpdateAudioSettings(); // 再生前に最新の設定を適用
        audioSource.clip = clip;
        audioSource.loop = true; // ループ再生を有効化
        audioSource.Play();
        Debug.Log($"ループ再生を開始: {clip.name}");
    }

    public void StopSE()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("SEの再生を停止しました。");
        }
        else
        {
            Debug.LogWarning("再生中のSEがありません。");
        }
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
