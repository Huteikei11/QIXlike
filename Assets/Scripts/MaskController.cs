using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MaskController : MonoBehaviour
{
    public Texture2D sourceTexture;  // 元の画像
    private Texture2D maskTexture;   // マスク後の画像
    private List<PolygonCollider2D> polygonColliders = new List<PolygonCollider2D>();
    public TextureBoundaryDetector textureBoundaryDetector;

    private Color[] maskPixels;
    private int width, height;

    void Start()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("sourceTexture が設定されていません！Inspector で設定してください。");
            return;
        }
        InitializeMask();
    }

    void InitializeMask()
    {
        width = sourceTexture.width;
        height = sourceTexture.height;

        // 元の画像をコピー
        maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        maskPixels = sourceTexture.GetPixels();
        maskTexture.SetPixels(maskPixels);
        maskTexture.Apply();

        // SpriteRenderer に適用
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(maskTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    public void ApplyMask(PolygonCollider2D polyCollider)
    {
        /*
        if (polyCollider == null || polyCollider.points == null || polyCollider.points.Length < 3)
        {
            Debug.LogError("PolygonCollider2D のポイントが3つ未満のため、マスク処理を実行できません！");
            return;
        }
        */

        polygonColliders.Add(polyCollider);

        // **ワールド座標をテクスチャ座標に変換**
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        Vector2 minBounds = bounds.min;
        Vector2 maxBounds = bounds.max;

        Vector2[] worldPoints = new Vector2[polyCollider.points.Length];
        for (int i = 0; i < polyCollider.points.Length; i++)
        {
            worldPoints[i] = polyCollider.transform.TransformPoint(polyCollider.points[i]);
        }

        // **ポリゴンのバウンディングボックスを求める**
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (Vector2 p in worldPoints)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        // **バウンディングボックスをテクスチャ座標に変換**
        int startX = Mathf.Clamp(Mathf.RoundToInt((minX - minBounds.x) / (maxBounds.x - minBounds.x) * width), 0, width - 1);
        int endX = Mathf.Clamp(Mathf.RoundToInt((maxX - minBounds.x) / (maxBounds.x - minBounds.x) * width), 0, width - 1);
        int startY = Mathf.Clamp(Mathf.RoundToInt((minY - minBounds.y) / (maxBounds.y - minBounds.y) * height), 0, height - 1);
        int endY = Mathf.Clamp(Mathf.RoundToInt((maxY - minBounds.y) / (maxBounds.y - minBounds.y) * height), 0, height - 1);

        // **対象範囲のピクセルのみ処理**
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                // **テクスチャ座標をワールド座標へ変換**
                Vector2 pixelWorldPoint = new Vector2(
                    Mathf.Lerp(minBounds.x, maxBounds.x, (float)x / width),
                    Mathf.Lerp(minBounds.y, maxBounds.y, (float)y / height)
                );

                if (IsPointInPolygon(pixelWorldPoint, worldPoints))
                {
                    int pixelIndex = y * width + x;
                    maskPixels[pixelIndex].a = 0; // 透明化
                }
            }
        }

        maskTexture.SetPixels(maskPixels);
        maskTexture.Apply();
        //画像を置き換える
        textureBoundaryDetector.ReTexture(maskTexture);
    }

    private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int j = polygon.Length - 1;
        bool inside = false;
        for (int i = 0; i < polygon.Length; j = i++)
        {
            if ((polygon[i].y > point.y) != (polygon[j].y > point.y) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }
}
