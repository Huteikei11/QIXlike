using UnityEngine;

public class SEController : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource���A�^�b�`����Ă��܂���B");
            return;
        }

        UpdateAudioSettings(); // �~���[�g�Ɖ��ʂ�ݒ�
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("�w�肳�ꂽSE�N���b�v������܂���B");
            return;
        }

        UpdateAudioSettings(); // �Đ��O�ɍŐV�̐ݒ��K�p
        audioSource.PlayOneShot(clip);
        Debug.Log($"SE���Đ���: {clip.name}");
    }

    private void UpdateAudioSettings()
    {
        // SaveManager����~���[�g��ԂƉ��ʂ��擾
        bool isMute = SaveManager.Instance.GetMute();
        int volumeLevel = SaveManager.Instance.GetVolume();

        // AudioSource�̐ݒ���X�V
        audioSource.mute = isMute;
        audioSource.volume = Mathf.Clamp01(volumeLevel / 9f); // 0~9��0.0~1.0�ɕϊ�

        Debug.Log($"Audio�ݒ���X�V: Mute={isMute}, Volume={audioSource.volume}");
    }
}
