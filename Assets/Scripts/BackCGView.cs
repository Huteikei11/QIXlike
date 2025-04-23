using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureBoundaryDetector;

public class BackCGView : MonoBehaviour
{
    public List<CharacterTextureSet> characterTextures; // キャラクターごとの
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    [System.Serializable]
    public class CharacterTextureSet
    {
        public string characterName; // キャラクター名 (任意)
        public List<Texture2D> textures; // キャラクターに対応するテクスチャリスト
    }
    void Awake()
    {
        int charaIndex = 0; // デフォルト値
        int levelIndex = 0; // デフォルト値

        try
        {
            charaIndex = Mathf.Clamp(SaveManager.Instance.GetCharacter(), 0, characterTextures.Count - 1);
            levelIndex = Mathf.Clamp(SaveManager.Instance.GetLevel(), 0, characterTextures[charaIndex].textures.Count - 1);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManagerがシーンに存在しないため、デフォルト値を使用します: {ex.Message}");
        }

        texture = characterTextures[charaIndex].textures[levelIndex+1];//裏のCGなので+1

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
