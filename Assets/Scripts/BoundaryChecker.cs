using System.Collections.Generic;
using UnityEngine;

public class BoundaryChecker : MonoBehaviour
{
    public Transform player;
    private TextureBoundaryDetector boundaryDetector;
    private SpriteRenderer spriteRenderer;
    private MaskController maskController;
    public  bool isBoundary;
    void Start()
    {
        maskController = FindObjectOfType<MaskController>(); // MaskControllerを取得
        boundaryDetector = FindObjectOfType<TextureBoundaryDetector>();

        CreateInitialRectangleCollider();

        if (boundaryDetector == null)
        {
            Debug.LogError("[BoundaryChecker] TextureBoundaryDetector がシーン内に見つかりません。");
            return;
        }
        Debug.Log("[BoundaryChecker] TextureBoundaryDetector を取得しました: " + boundaryDetector.gameObject.name);

        spriteRenderer = boundaryDetector.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("[BoundaryChecker] SpriteRenderer が見つからないか、スプライトが設定されていません。");
            return;
        }
        Debug.Log("[BoundaryChecker] スプライトが設定されています: " + spriteRenderer.sprite.name);

        // プレイヤーの初期位置を境界内に設定
        SetPlayerInsideBoundary();
    }

    void Update()
    {
        if (boundaryDetector == null || spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        Vector2Int pixelPos = WorldToPixel(player.position);
        //Debug.Log("[BoundaryChecker] ワールド座標: " + player.position + " → ピクセル座標: " + pixelPos);

        if (pixelPos.x < 0 || pixelPos.x >= boundaryDetector.sourceTexture.width ||
            pixelPos.y < 0 || pixelPos.y >= boundaryDetector.sourceTexture.height)
        {
            Debug.LogWarning("[BoundaryChecker] プレイヤーのピクセル座標がテクスチャ範囲外: " + pixelPos);
            return;
        }

        if (boundaryDetector.IsOnBoundary(pixelPos))
        {
            //Debug.Log("[BoundaryChecker] プレイヤーは境界部分にいる！ ピクセル座標: " + pixelPos);
            isBoundary = true;

        }
        else
        {
            Debug.Log("[BoundaryChecker] プレイヤーは境界部分にいない。");
            isBoundary = false;
        }
    }

    public void CheckMoveBoundary()
    {
        if (!isBoundary)
        {
            WarpToClosestBoundary();// プレイヤーを最寄りの境界にワープ
        }
    }

    void SetPlayerInsideBoundary()
    {
        Vector2Int textureCenter = new Vector2Int(boundaryDetector.sourceTexture.width / 2, boundaryDetector.sourceTexture.height / 2);
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(textureCenter);
        visited.Add(textureCenter);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 境界に到達した場合
            if (boundaryDetector.IsOnBoundary(current))
            {
                player.position = PixelToWorld(current);
                Debug.Log("[BoundaryChecker] プレイヤーの初期位置を境界内に設定しました: " + player.position);
                return;
            }

            // 周囲のピクセルを探索
            foreach (Vector2Int neighbor in GetNeighbors(current, boundaryDetector.sourceTexture.width, boundaryDetector.sourceTexture.height))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // 境界が見つからなかった場合
        Debug.LogError("[BoundaryChecker] 境界が見つからないため、初期位置を設定できません。");
    }

    IEnumerable<Vector2Int> GetNeighbors(Vector2Int current, int width, int height)
    {
        // 上下左右の隣接ピクセルを返す
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1),  // 上
        new Vector2Int(0, -1), // 下
        new Vector2Int(1, 0),  // 右
        new Vector2Int(-1, 0)  // 左
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                yield return neighbor;
            }
        }
    }

    public Vector2Int WorldToPixel(Vector3 worldPos)
    {
        Vector3 localPos = boundaryDetector.transform.InverseTransformPoint(worldPos);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        if (spriteSize.x == 0 || spriteSize.y == 0)
        {
            Debug.LogError("[BoundaryChecker] スプライトのサイズが無効です。");
            return Vector2Int.zero;
        }

        int px = Mathf.RoundToInt((localPos.x + spriteSize.x / 2) / spriteSize.x * boundaryDetector.sourceTexture.width);
        int py = Mathf.RoundToInt((localPos.y + spriteSize.y / 2) / spriteSize.y * boundaryDetector.sourceTexture.height);
        return new Vector2Int(px, py);
    }

    Vector3 PixelToWorld(Vector2Int pixelPos)
    {
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float x = ((float)pixelPos.x / boundaryDetector.sourceTexture.width) * spriteSize.x - spriteSize.x / 2;
        float y = ((float)pixelPos.y / boundaryDetector.sourceTexture.height) * spriteSize.y - spriteSize.y / 2;

        return boundaryDetector.transform.TransformPoint(new Vector3(x, y, 0));
    }

    // プレイヤーに最も近い境界の点を探してその位置にワープさせる
    public void WarpToClosestBoundary()
    {
        if (boundaryDetector == null) return;

        Vector2 closestPoint = FindClosestBoundaryPoint(player.position);
        if (closestPoint != Vector2.zero)
        {
            player.position = closestPoint;
            Debug.Log("[BoundaryChecker] プレイヤーを最寄りの境界にワープさせました: " + closestPoint);
        }
        else
        {
            Debug.LogWarning("[BoundaryChecker] 境界が見つかりませんでした。");
        }
    }

    // プレイヤーの縦横方向の直線範囲内で最も近い境界の点を探す
    Vector2 FindClosestBoundaryPoint(Vector3 playerPosition)
    {
        float closestDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;

        // プレイヤーの位置をピクセル座標に変換
        Vector2Int playerPixelPos = WorldToPixel(playerPosition);
        int width = boundaryDetector.sourceTexture.width;
        int height = boundaryDetector.sourceTexture.height;

        // 縦横方向に範囲を制限して探索
        for (int y = 0; y < height; y++)
        {
            if (boundaryDetector.IsOnBoundary(new Vector2Int(playerPixelPos.x, y)))
            {
                Vector3 worldPos = PixelToWorld(new Vector2Int(playerPixelPos.x, y));
                float distance = Vector3.Distance(playerPosition, worldPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = worldPos;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            if (boundaryDetector.IsOnBoundary(new Vector2Int(x, playerPixelPos.y)))
            {
                Vector3 worldPos = PixelToWorld(new Vector2Int(x, playerPixelPos.y));
                float distance = Vector3.Distance(playerPosition, worldPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = worldPos;
                }
            }
        }

        return closestPoint;
    }

    void CreateInitialRectangleCollider()
    {
        // ランダムな範囲を指定（必要に応じて調整可能）
        float minX = -9f, maxX = 6f;
        float minY = -4f, maxY = 4f;

        // ランダムな中心位置を生成
        Vector2 center = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        // サイズを指定（必要に応じて調整可能）
        float width = 2f;
        float height = 1f;

        // 四角形の頂点を時計回りで設定
        Vector2[] rectanglePoints = new Vector2[]
        {
        new Vector2(center.x - width / 2, center.y - height / 2), // 左下
        new Vector2(center.x - width / 2, center.y + height / 2), // 左上
        new Vector2(center.x + width / 2, center.y + height / 2), // 右上
        new Vector2(center.x + width / 2, center.y - height / 2)  // 右下
        };

        // GameObject と PolygonCollider2D を作成
        GameObject initialPoly = new GameObject("InitialPolygon");
        PolygonCollider2D polyCollider = initialPoly.AddComponent<PolygonCollider2D>();
        initialPoly.tag = "myarea";

        polyCollider.points = rectanglePoints;

        // オブジェクトをワールドの中心に配置
        initialPoly.transform.position = Vector3.zero;

        if (maskController != null)
        {
            maskController.ApplyMask(polyCollider);
        }

        // デバッグログ
        Debug.Log($"初期の四角いPolygonCollider2Dを生成しました。中心: {center}, 幅: {width}, 高さ: {height}");
    }

}
