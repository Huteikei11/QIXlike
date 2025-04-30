using System.Collections.Generic;
using UnityEngine;

public class BackCGViewSingleSprite : MonoBehaviour
{
    public List<CharacterSpriteSet> characterSprites; // キャラクターごとのスプライトリスト
    private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class CharacterSpriteSet
    {
        public string characterName; // キャラクター名 (任意)
        public Sprite sprite; // キャラクターに対応するスプライト
    }

    void Awake()
    {
        int charaIndex = 0; // デフォルト値

        try
        {
            // SaveManagerからキャラクターのインデックスを取得
            charaIndex = Mathf.Clamp(SaveManager.Instance.GetCharacter(), 0, characterSprites.Count - 1);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SaveManagerがシーンに存在しないため、デフォルト値を使用します: {ex.Message}");
        }

        // SpriteRendererを取得し、スプライトを設定
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && characterSprites.Count > 0)
        {
            spriteRenderer.sprite = characterSprites[charaIndex].sprite;
        }
        else
        {
            Debug.LogError("SpriteRendererまたはスプライトリストが正しく設定されていません。");
        }
    }
}
