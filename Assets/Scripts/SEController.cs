using UnityEngine;

public class SEController : MonoBehaviour
{
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
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("指定されたSEクリップがありません。");
            return;
        }

        UpdateAudioSettings(); // 再生前に最新の設定を適用
        audioSource.PlayOneShot(clip);
        Debug.Log($"SEを再生中: {clip.name}");
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
