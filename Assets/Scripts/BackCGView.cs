using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureBoundaryDetector;

public class BackCGView : MonoBehaviour
{
    public List<CharacterTextureSet> characterTextures; // �L�����N�^�[���Ƃ�
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    [System.Serializable]
    public class CharacterTextureSet
    {
        public string characterName; // �L�����N�^�[�� (�C��)
        public List<Texture2D> textures; // �L�����N�^�[�ɑΉ�����e�N�X�`�����X�g
    }
    void Awake()
    {
        int charaIndex = 0; // �f�t�H���g�l
        int levelIndex = 0; // �f�t�H���g�l

        try
        {
            charaIndex = Mathf.Clamp(SaveManager.Instance.GetCharacter(), 0, characterTextures.Count - 1);
            levelIndex = Mathf.Clamp(SaveManager.Instance.GetLevel(), 0, characterTextures[charaIndex].textures.Count - 1);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManager���V�[���ɑ��݂��Ȃ����߁A�f�t�H���g�l���g�p���܂�: {ex.Message}");
        }

        texture = characterTextures[charaIndex].textures[levelIndex+1];//����CG�Ȃ̂�+1

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
