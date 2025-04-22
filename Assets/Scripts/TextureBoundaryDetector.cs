using UnityEngine;
using System.Collections.Generic;
using static MaskController;

public class TextureBoundaryDetector : MonoBehaviour
{
    public List<CharacterTextureSet> characterTextures; // キャラクターごとの
    public Texture2D sourceTexture;
    private Texture2D processedTexture;
    private SpriteRenderer spriteRenderer;

    private HashSet<Vector2Int> boundaryPixels = new HashSet<Vector2Int>(); // 境界データ


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

        sourceTexture = characterTextures[charaIndex].textures[levelIndex];

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sourceTexture != null)
        {
            processedTexture = GenerateBoundaryTexture(sourceTexture);
            ApplyTexture(processedTexture);
        }
    }

    Texture2D GenerateBoundaryTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D newTexture = new Texture2D(width, height);

        Color32[] pixels = texture.GetPixels32();
        Color32[] newPixels = new Color32[pixels.Length];

        boundaryPixels.Clear(); // 境界データを初期化

        HashSet<Vector2Int> tempBoundary = new HashSet<Vector2Int>();

        // **1ピクセルの境界を検出**
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int index = y * width + x;
                if (pixels[index].a == 0 && HasOpaqueNeighbor(pixels, x, y, width, height, 1))
                {
                    tempBoundary.Add(new Vector2Int(x, y));
                }
            }
        }

        // **境界の厚さを 4 ピクセルに拡張**
        for (int thickness = 2; thickness <= 3; thickness++)
        {
            HashSet<Vector2Int> newBoundary = new HashSet<Vector2Int>();
            foreach (Vector2Int pos in tempBoundary)
            {
                if (HasOpaqueNeighbor(pixels, pos.x, pos.y, width, height, thickness))
                {
                    newBoundary.Add(pos);
                }
            }
            tempBoundary.UnionWith(newBoundary);
        }

        // **境界ピクセルを反映**
        for (int i = 0; i < pixels.Length; i++)
        {
            newPixels[i] = pixels[i];
        }
        foreach (Vector2Int pos in tempBoundary)
        {
            int index = pos.y * width + pos.x;
            newPixels[index] = Color.red;
            boundaryPixels.Add(pos);
        }

        newTexture.SetPixels32(newPixels);
        newTexture.Apply();
        return newTexture;
    }

    bool HasOpaqueNeighbor(Color32[] pixels, int x, int y, int width, int height, int range)
    {
        for (int dy = -range; dy <= range; dy++)
        {
            for (int dx = -range; dx <= range; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;

                int index = ny * width + nx;
                if (pixels[index].a > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ApplyTexture(Texture2D texture)
    {
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public bool IsOnBoundary(Vector2Int pixelPos)
    {
        return boundaryPixels.Contains(pixelPos);
    }

    public void ReTexture(Texture2D texture)
    {
        processedTexture = GenerateBoundaryTexture(texture);
        ApplyTexture(processedTexture);
    }

    // **プレイヤーが透明領域内にいるか確認するメソッド**
    public bool IsInTransparentArea(Vector2 playerPosition)
    {
        // プレイヤーの位置をピクセル座標に変換
        Vector2Int pixelPos = WorldToPixel(playerPosition);

        // プレイヤー位置のピクセルが透明かどうかをチェック
        Color32 pixelColor = processedTexture.GetPixel(pixelPos.x, pixelPos.y);

        // 透明ピクセルの判定（アルファ値が0のピクセル）
        return pixelColor.a == 0;
    }

    // ピクセル座標 -> ワールド座標変換
    Vector3 PixelToWorld(Vector2Int pixelPos)
    {
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float x = ((float)pixelPos.x / sourceTexture.width) * spriteSize.x - spriteSize.x / 2;
        float y = ((float)pixelPos.y / sourceTexture.height) * spriteSize.y - spriteSize.y / 2;

        return spriteRenderer.transform.TransformPoint(new Vector3(x, y, 0));
    }

    // ワールド座標 -> ピクセル座標変換
    Vector2Int WorldToPixel(Vector2 worldPos)
    {
        Vector3 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        int px = Mathf.RoundToInt((localPos.x + spriteSize.x / 2) / spriteSize.x * sourceTexture.width);
        int py = Mathf.RoundToInt((localPos.y + spriteSize.y / 2) / spriteSize.y * sourceTexture.height);
        return new Vector2Int(px, py);
    }
}
