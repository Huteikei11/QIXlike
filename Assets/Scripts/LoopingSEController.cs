using UnityEngine;

public class LoopingSEController : MonoBehaviour
{
    public AudioSource audioSource;
    public PlayerMovementpix player; // PlayerMovementpix�̎Q��
    public AudioClip outSound;       // �Đ����鉹���N���b�v

    private bool wasOut = false; // �O���isOut�̏�Ԃ��L�^

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource���A�^�b�`����Ă��܂���B");
            return;
        }

        if (player == null)
        {
            Debug.LogError("PlayerMovementpix�̎Q�Ƃ��ݒ肳��Ă��܂���B");
            return;
        }

        UpdateAudioSettings(); // �~���[�g�Ɖ��ʂ�ݒ�
    }

    private void Update()
    {
        if (outSound == null)
        {
            Debug.LogWarning("�Đ�����AudioClip���ݒ肳��Ă��܂���B");
            return;
        }

        // isOut�̏�Ԃ��ω������ꍇ�ɏ��������s
        if (player.isOut && !wasOut)
        {
            PlayLoopingSE(outSound); // �������Đ�
        }
        else if (!player.isOut && wasOut)
        {
            StopSE(); // �������~
        }

        // ���݂�isOut�̏�Ԃ��L�^
        wasOut = player.isOut;
    }

    public void PlayLoopingSE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("�w�肳�ꂽSE�N���b�v������܂���B");
            return;
        }

        UpdateAudioSettings(); // �Đ��O�ɍŐV�̐ݒ��K�p
        audioSource.clip = clip;
        audioSource.loop = true; // ���[�v�Đ���L����
        audioSource.Play();
        Debug.Log($"���[�v�Đ����J�n: {clip.name}");
    }

    public void StopSE()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("SE�̍Đ����~���܂����B");
        }
        else
        {
            Debug.LogWarning("�Đ�����SE������܂���B");
        }
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
