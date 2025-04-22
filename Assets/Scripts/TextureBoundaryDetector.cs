using UnityEngine;
using System.Collections.Generic;
using static MaskController;

public class TextureBoundaryDetector : MonoBehaviour
{
    public List<CharacterTextureSet> characterTextures; // �L�����N�^�[���Ƃ�
    public Texture2D sourceTexture;
    private Texture2D processedTexture;
    private SpriteRenderer spriteRenderer;

    private HashSet<Vector2Int> boundaryPixels = new HashSet<Vector2Int>(); // ���E�f�[�^


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

        boundaryPixels.Clear(); // ���E�f�[�^��������

        HashSet<Vector2Int> tempBoundary = new HashSet<Vector2Int>();

        // **1�s�N�Z���̋��E�����o**
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

        // **���E�̌����� 4 �s�N�Z���Ɋg��**
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

        // **���E�s�N�Z���𔽉f**
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

    // **�v���C���[�������̈���ɂ��邩�m�F���郁�\�b�h**
    public bool IsInTransparentArea(Vector2 playerPosition)
    {
        // �v���C���[�̈ʒu���s�N�Z�����W�ɕϊ�
        Vector2Int pixelPos = WorldToPixel(playerPosition);

        // �v���C���[�ʒu�̃s�N�Z�����������ǂ������`�F�b�N
        Color32 pixelColor = processedTexture.GetPixel(pixelPos.x, pixelPos.y);

        // �����s�N�Z���̔���i�A���t�@�l��0�̃s�N�Z���j
        return pixelColor.a == 0;
    }

    // �s�N�Z�����W -> ���[���h���W�ϊ�
    Vector3 PixelToWorld(Vector2Int pixelPos)
    {
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float x = ((float)pixelPos.x / sourceTexture.width) * spriteSize.x - spriteSize.x / 2;
        float y = ((float)pixelPos.y / sourceTexture.height) * spriteSize.y - spriteSize.y / 2;

        return spriteRenderer.transform.TransformPoint(new Vector3(x, y, 0));
    }

    // ���[���h���W -> �s�N�Z�����W�ϊ�
    Vector2Int WorldToPixel(Vector2 worldPos)
    {
        Vector3 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        int px = Mathf.RoundToInt((localPos.x + spriteSize.x / 2) / spriteSize.x * sourceTexture.width);
        int py = Mathf.RoundToInt((localPos.y + spriteSize.y / 2) / spriteSize.y * sourceTexture.height);
        return new Vector2Int(px, py);
    }
}
