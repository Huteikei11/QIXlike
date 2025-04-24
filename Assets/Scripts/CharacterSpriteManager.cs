using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterSpriteSet
{
    public string characterName; // �L�����N�^�[�� (�C��)
    public Sprite[] sprites; // �L�����N�^�[���Ƃ̃X�v���C�g�z��
}

public class CharacterSpriteManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // �X�v���C�g��\������SpriteRenderer
    [SerializeField] private CharacterSpriteSet[] characterSpriteSets; // �L�����N�^�[���Ƃ̃X�v���C�g�Z�b�g

    public int currentCharacterIndex = -1; // ���݂̃L�����N�^�[�C���f�b�N�X
    public int currentSpriteIndex = 0; // ���݂̃X�v���C�g�C���f�b�N�X

    private void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer���ݒ肳��Ă��܂���B");
            return;
        }

        if (characterSpriteSets == null || characterSpriteSets.Length == 0)
        {
            Debug.LogError("�L�����N�^�[�X�v���C�g�Z�b�g���ݒ肳��Ă��܂���B");
            return;
        }

        LoadCharacterAndSpriteIndex();
        UpdateSprite();
    }

    private void LoadCharacterAndSpriteIndex()
    {
        // SaveManager����L�����N�^�[�C���f�b�N�X���擾
        currentCharacterIndex = SaveManager.Instance.GetCharacter();

        // �L�����N�^�[�C���f�b�N�X�������ȏꍇ�̓f�t�H���g�l��ݒ�
        if (currentCharacterIndex < 0 || currentCharacterIndex >= characterSpriteSets.Length)
        {
            Debug.LogWarning("�����ȃL�����N�^�[�C���f�b�N�X�ł��B�f�t�H���g�l���g�p���܂��B");
            currentCharacterIndex = 0;
        }

        // �K�v�ɉ����ăX�v���C�g�C���f�b�N�X�����[�h (�����ł͏����l��0�ɐݒ�)
        currentSpriteIndex = 0;
    }

    private void UpdateSprite()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            if (currentSpriteIndex >= 0 && currentSpriteIndex < spriteSet.sprites.Length)
            {
                spriteRenderer.sprite = spriteSet.sprites[currentSpriteIndex];
                Debug.Log($"�L�����N�^�[ {currentCharacterIndex} �̃X�v���C�g {currentSpriteIndex} ��\�����܂����B");
            }
            else
            {
                Debug.LogWarning("�����ȃX�v���C�g�C���f�b�N�X�ł��B�f�t�H���g�̃X�v���C�g���g�p���܂��B");
                spriteRenderer.sprite = null; // �K�v�ɉ����ăf�t�H���g�X�v���C�g��ݒ�
            }
        }
        else
        {
            Debug.LogError("�����ȃL�����N�^�[�C���f�b�N�X�ł��B");
        }
    }

    public void SetSpriteIndex(int spriteIndex)
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            // �X�v���C�g�C���f�b�N�X���X�v���C�g���͈͓̔��ɐ���
            if (spriteIndex >= 0 && spriteIndex < spriteSet.sprites.Length)
            {
                currentSpriteIndex = spriteIndex;
                UpdateSprite();
                Debug.Log($"�X�v���C�g�C���f�b�N�X�� {spriteIndex} �ɕύX���܂����B");
            }
            else
            {
                Debug.LogError($"�����ȃX�v���C�g�C���f�b�N�X�ł�: {spriteIndex} (�ő�l: {spriteSet.sprites.Length - 1})");
            }
        }
        else
        {
            Debug.LogError("�����ȃL�����N�^�[�C���f�b�N�X�ł��B");
        }
    }

    public void SetCharacterIndex(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterSpriteSets.Length)
        {
            currentCharacterIndex = characterIndex;
            currentSpriteIndex = 0; // �L�����N�^�[�ύX���ɃX�v���C�g�C���f�b�N�X�����Z�b�g
            UpdateSprite();
            Debug.Log($"�L�����N�^�[�C���f�b�N�X�� {characterIndex} �ɕύX���܂����B");
        }
        else
        {
            Debug.LogError("�����ȃL�����N�^�[�C���f�b�N�X�ł�: " + characterIndex);
        }
    }

    // �X�v���C�g�C���f�b�N�X��+1����
    public void IncrementSpriteIndex()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            var spriteSet = characterSpriteSets[currentCharacterIndex];

            if (currentSpriteIndex + 1 < spriteSet.sprites.Length)
            {
                currentSpriteIndex++;
                UpdateSprite();
                Debug.Log($"�X�v���C�g�C���f�b�N�X��+1���܂���: {currentSpriteIndex}");
            }
            else
            {
                Debug.LogWarning("�X�v���C�g�C���f�b�N�X�͂���ȏ㑝�₹�܂���B");
            }
        }
        else
        {
            Debug.LogError("�����ȃL�����N�^�[�C���f�b�N�X�ł��B");
        }
    }

    // �X�v���C�g�C���f�b�N�X��-1����
    public void DecrementSpriteIndex()
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterSpriteSets.Length)
        {
            if (currentSpriteIndex - 1 >= 0)
            {
                currentSpriteIndex--;
                UpdateSprite();
                Debug.Log($"�X�v���C�g�C���f�b�N�X��-1���܂���: {currentSpriteIndex}");
            }
            else
            {
                Debug.LogWarning("�X�v���C�g�C���f�b�N�X�͂���ȏ㌸�点�܂���B");
            }
        }
        else
        {
            Debug.LogError("�����ȃL�����N�^�[�C���f�b�N�X�ł��B");
        }
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
