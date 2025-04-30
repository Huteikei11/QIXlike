using System.Collections.Generic;
using UnityEngine;

public class BackCGViewSingleSprite : MonoBehaviour
{
    public List<CharacterSpriteSet> characterSprites; // �L�����N�^�[���Ƃ̃X�v���C�g���X�g
    private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class CharacterSpriteSet
    {
        public string characterName; // �L�����N�^�[�� (�C��)
        public Sprite sprite; // �L�����N�^�[�ɑΉ�����X�v���C�g
    }

    void Awake()
    {
        int charaIndex = 0; // �f�t�H���g�l

        try
        {
            // SaveManager����L�����N�^�[�̃C���f�b�N�X���擾
            charaIndex = Mathf.Clamp(SaveManager.Instance.GetCharacter(), 0, characterSprites.Count - 1);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManager���V�[���ɑ��݂��Ȃ����߁A�f�t�H���g�l���g�p���܂�: {ex.Message}");
        }

        // SpriteRenderer���擾���A�X�v���C�g��ݒ�
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && characterSprites.Count > 0)
        {
            spriteRenderer.sprite = characterSprites[charaIndex].sprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer�܂��̓X�v���C�g���X�g���������ݒ肳��Ă��܂���B");
        }
    }
}
