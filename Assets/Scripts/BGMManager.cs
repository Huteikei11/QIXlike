using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("BGM Clips")]
    public AudioClip bgmCharacter1; // �L�����N�^�[1�pBGM
    public AudioClip bgmCharacter2; // �L�����N�^�[2�pBGM
    public AudioClip bgmCharacter3; // �L�����N�^�[3�pBGM
    public AudioClip bgmDefault;    // �f�t�H���gBGM (�L�����N�^�[���I����)

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
        PlayBGMBasedOnCharacter();
    }

    private void PlayBGMBasedOnCharacter()
    {
        int selectedCharacter = SaveManager.Instance.GetCharacter(); // �I�����ꂽ�L�����N�^�[���擾

        switch (selectedCharacter)
        {
            case 0: // �얲
                PlayBGM(bgmCharacter1);
                break;
            case 1: // �A���X
                PlayBGM(bgmCharacter1);
                break;
            case 2: // ���ǂ�
                PlayBGM(bgmCharacter2);
                break;
            case 3: // ���
                PlayBGM(bgmCharacter2);
                break;
            case 4: // �d��
                PlayBGM(bgmCharacter2);
                break;
            case 5: // ������
                PlayBGM(bgmCharacter1);
                break;
            case 6: // �ʂ�
                PlayBGM(bgmCharacter3);
                break;
            default:
                PlayBGM(bgmDefault); // �L�����N�^�[���I����
                break;
        }
    }

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("�w�肳�ꂽBGM�N���b�v������܂���B");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log($"BGM���Đ���: {clip.name}");
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