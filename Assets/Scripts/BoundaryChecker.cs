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
        maskController = FindObjectOfType<MaskController>(); // MaskController���擾
        boundaryDetector = FindObjectOfType<TextureBoundaryDetector>();

        CreateInitialRectangleCollider();

        if (boundaryDetector == null)
        {
            Debug.LogError("[BoundaryChecker] TextureBoundaryDetector ���V�[�����Ɍ�����܂���B");
            return;
        }
        Debug.Log("[BoundaryChecker] TextureBoundaryDetector ���擾���܂���: " + boundaryDetector.gameObject.name);

        spriteRenderer = boundaryDetector.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("[BoundaryChecker] SpriteRenderer ��������Ȃ����A�X�v���C�g���ݒ肳��Ă��܂���B");
            return;
        }
        Debug.Log("[BoundaryChecker] �X�v���C�g���ݒ肳��Ă��܂�: " + spriteRenderer.sprite.name);

        // �v���C���[�̏����ʒu�����E���ɐݒ�
        SetPlayerInsideBoundary();
    }

    void Update()
    {
        if (boundaryDetector == null || spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        Vector2Int pixelPos = WorldToPixel(player.position);
        //Debug.Log("[BoundaryChecker] ���[���h���W: " + player.position + " �� �s�N�Z�����W: " + pixelPos);

        if (pixelPos.x < 0 || pixelPos.x >= boundaryDetector.sourceTexture.width ||
            pixelPos.y < 0 || pixelPos.y >= boundaryDetector.sourceTexture.height)
        {
            Debug.LogWarning("[BoundaryChecker] �v���C���[�̃s�N�Z�����W���e�N�X�`���͈͊O: " + pixelPos);
            return;
        }

        if (boundaryDetector.IsOnBoundary(pixelPos))
        {
            //Debug.Log("[BoundaryChecker] �v���C���[�͋��E�����ɂ���I �s�N�Z�����W: " + pixelPos);
            isBoundary = true;

        }
        else
        {
            Debug.Log("[BoundaryChecker] �v���C���[�͋��E�����ɂ��Ȃ��B");
            isBoundary = false;
        }
    }

    public void CheckMoveBoundary()
    {
        if (!isBoundary)
        {
            WarpToClosestBoundary();// �v���C���[���Ŋ��̋��E�Ƀ��[�v
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

            // ���E�ɓ��B�����ꍇ
            if (boundaryDetector.IsOnBoundary(current))
            {
                player.position = PixelToWorld(current);
                Debug.Log("[BoundaryChecker] �v���C���[�̏����ʒu�����E���ɐݒ肵�܂���: " + player.position);
                return;
            }

            // ���͂̃s�N�Z����T��
            foreach (Vector2Int neighbor in GetNeighbors(current, boundaryDetector.sourceTexture.width, boundaryDetector.sourceTexture.height))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // ���E��������Ȃ������ꍇ
        Debug.LogError("[BoundaryChecker] ���E��������Ȃ����߁A�����ʒu��ݒ�ł��܂���B");
    }

    IEnumerable<Vector2Int> GetNeighbors(Vector2Int current, int width, int height)
    {
        // �㉺���E�̗אڃs�N�Z����Ԃ�
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1),  // ��
        new Vector2Int(0, -1), // ��
        new Vector2Int(1, 0),  // �E
        new Vector2Int(-1, 0)  // ��
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
            Debug.LogError("[BoundaryChecker] �X�v���C�g�̃T�C�Y�������ł��B");
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

    // �v���C���[�ɍł��߂����E�̓_��T���Ă��̈ʒu�Ƀ��[�v������
    public void WarpToClosestBoundary()
    {
        if (boundaryDetector == null) return;

        Vector2 closestPoint = FindClosestBoundaryPoint(player.position);
        if (closestPoint != Vector2.zero)
        {
            player.position = closestPoint;
            Debug.Log("[BoundaryChecker] �v���C���[���Ŋ��̋��E�Ƀ��[�v�����܂���: " + closestPoint);
        }
        else
        {
            Debug.LogWarning("[BoundaryChecker] ���E��������܂���ł����B");
        }
    }

    // �v���C���[�̏c�������̒����͈͓��ōł��߂����E�̓_��T��
    Vector2 FindClosestBoundaryPoint(Vector3 playerPosition)
    {
        float closestDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;

        // �v���C���[�̈ʒu���s�N�Z�����W�ɕϊ�
        Vector2Int playerPixelPos = WorldToPixel(playerPosition);
        int width = boundaryDetector.sourceTexture.width;
        int height = boundaryDetector.sourceTexture.height;

        // �c�������ɔ͈͂𐧌����ĒT��
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
        // �����_���Ȕ͈͂��w��i�K�v�ɉ����Ē����\�j
        float minX = -9f, maxX = 6f;
        float minY = -4f, maxY = 4f;

        // �����_���Ȓ��S�ʒu�𐶐�
        Vector2 center = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        // �T�C�Y���w��i�K�v�ɉ����Ē����\�j
        float width = 2f;
        float height = 1f;

        // �l�p�`�̒��_�����v���Őݒ�
        Vector2[] rectanglePoints = new Vector2[]
        {
        new Vector2(center.x - width / 2, center.y - height / 2), // ����
        new Vector2(center.x - width / 2, center.y + height / 2), // ����
        new Vector2(center.x + width / 2, center.y + height / 2), // �E��
        new Vector2(center.x + width / 2, center.y - height / 2)  // �E��
        };

        // GameObject �� PolygonCollider2D ���쐬
        GameObject initialPoly = new GameObject("InitialPolygon");
        PolygonCollider2D polyCollider = initialPoly.AddComponent<PolygonCollider2D>();
        initialPoly.tag = "myarea";

        polyCollider.points = rectanglePoints;

        // �I�u�W�F�N�g�����[���h�̒��S�ɔz�u
        initialPoly.transform.position = Vector3.zero;

        if (maskController != null)
        {
            maskController.ApplyMask(polyCollider);
        }

        // �f�o�b�O���O
        Debug.Log($"�����̎l�p��PolygonCollider2D�𐶐����܂����B���S: {center}, ��: {width}, ����: {height}");
    }

}
