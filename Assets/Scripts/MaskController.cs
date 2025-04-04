using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MaskController : MonoBehaviour
{
    public Texture2D sourceTexture;  // ���̉摜
    private Texture2D maskTexture;   // �}�X�N��̉摜
    private List<PolygonCollider2D> polygonColliders = new List<PolygonCollider2D>();
    public TextureBoundaryDetector textureBoundaryDetector;

    private Color[] maskPixels;
    private int width, height;

    void Start()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("sourceTexture ���ݒ肳��Ă��܂���IInspector �Őݒ肵�Ă��������B");
            return;
        }
        InitializeMask();
    }

    void InitializeMask()
    {
        width = sourceTexture.width;
        height = sourceTexture.height;

        // ���̉摜���R�s�[
        maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        maskPixels = sourceTexture.GetPixels();
        maskTexture.SetPixels(maskPixels);
        maskTexture.Apply();

        // SpriteRenderer �ɓK�p
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(maskTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    public void ApplyMask(PolygonCollider2D polyCollider)
    {
        /*
        if (polyCollider == null || polyCollider.points == null || polyCollider.points.Length < 3)
        {
            Debug.LogError("PolygonCollider2D �̃|�C���g��3�����̂��߁A�}�X�N���������s�ł��܂���I");
            return;
        }
        */

        polygonColliders.Add(polyCollider);

        // **���[���h���W���e�N�X�`�����W�ɕϊ�**
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        Vector2 minBounds = bounds.min;
        Vector2 maxBounds = bounds.max;

        Vector2[] worldPoints = new Vector2[polyCollider.points.Length];
        for (int i = 0; i < polyCollider.points.Length; i++)
        {
            worldPoints[i] = polyCollider.transform.TransformPoint(polyCollider.points[i]);
        }

        // **�|���S���̃o�E���f�B���O�{�b�N�X�����߂�**
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (Vector2 p in worldPoints)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        // **�o�E���f�B���O�{�b�N�X���e�N�X�`�����W�ɕϊ�**
        int startX = Mathf.Clamp(Mathf.RoundToInt((minX - minBounds.x) / (maxBounds.x - minBounds.x) * width), 0, width - 1);
        int endX = Mathf.Clamp(Mathf.RoundToInt((maxX - minBounds.x) / (maxBounds.x - minBounds.x) * width), 0, width - 1);
        int startY = Mathf.Clamp(Mathf.RoundToInt((minY - minBounds.y) / (maxBounds.y - minBounds.y) * height), 0, height - 1);
        int endY = Mathf.Clamp(Mathf.RoundToInt((maxY - minBounds.y) / (maxBounds.y - minBounds.y) * height), 0, height - 1);

        // **�Ώ۔͈͂̃s�N�Z���̂ݏ���**
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                // **�e�N�X�`�����W�����[���h���W�֕ϊ�**
                Vector2 pixelWorldPoint = new Vector2(
                    Mathf.Lerp(minBounds.x, maxBounds.x, (float)x / width),
                    Mathf.Lerp(minBounds.y, maxBounds.y, (float)y / height)
                );

                if (IsPointInPolygon(pixelWorldPoint, worldPoints))
                {
                    int pixelIndex = y * width + x;
                    maskPixels[pixelIndex].a = 0; // ������
                }
            }
        }

        maskTexture.SetPixels(maskPixels);
        maskTexture.Apply();
        //�摜��u��������
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
