using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement2 : MonoBehaviour
{
    public Texture2D backgroundTexture;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 5f;
    public LineRenderer lineRenderer;

    private HashSet<Vector2Int> boundaryPixels = new HashSet<Vector2Int>();
    private List<Vector2Int> trail = new List<Vector2Int>();
    private Vector2Int playerPixelPos;
    private Vector2Int startExitPixelPos;
    private bool isOutsideBoundary = false;

    void Start()
    {
        ExtractBoundaryPixels();
        playerPixelPos = FindStartingPixelPosition();
        transform.position = PixelToWorld(playerPixelPos);

        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        Vector2Int moveDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2Int.right;

        Vector2Int nextPixelPos = playerPixelPos + moveDirection;
        Debug.Log($"{boundaryPixels.Contains(playerPixelPos)}");
        if (boundaryPixels.Contains(nextPixelPos))
        {
            if (isOutsideBoundary)
            {
                ApplyTransparency();
                trail.Clear();
                isOutsideBoundary = false;
                lineRenderer.positionCount = 0;
            }
            playerPixelPos = nextPixelPos;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            if (!isOutsideBoundary)
            {
                startExitPixelPos = playerPixelPos;
                isOutsideBoundary = true;
            }
            trail.Add(nextPixelPos);
            playerPixelPos = nextPixelPos;
            UpdateLineRenderer();
        }
        else if (isOutsideBoundary)
        {
            playerPixelPos = startExitPixelPos;
            isOutsideBoundary = false;
            trail.Clear();
            lineRenderer.positionCount = 0;
        }

        transform.position = PixelToWorld(playerPixelPos);
    }

    void ApplyTransparency()
    {
        foreach (Vector2Int pos in trail)
        {
            backgroundTexture.SetPixel(pos.x, pos.y, new Color(0, 0, 0, 0));
        }
        backgroundTexture.Apply();
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = trail.Count;
        for (int i = 0; i < trail.Count; i++)
        {
            lineRenderer.SetPosition(i, PixelToWorld(trail[i]));
        }
    }

    void ExtractBoundaryPixels()
    {
        int width = backgroundTexture.width;
        int height = backgroundTexture.height;

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                Color pixel = backgroundTexture.GetPixel(x, y);
                if (pixel.a == 0)
                {
                    bool isBoundary = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            Color neighbor = backgroundTexture.GetPixel(x + dx, y + dy);
                            if (neighbor.a > 0)
                            {
                                isBoundary = true;
                                break;
                            }
                        }
                        if (isBoundary) break;
                    }

                    if (isBoundary)
                    {
                        boundaryPixels.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    Vector2Int FindStartingPixelPosition()
    {
        foreach (Vector2Int pos in boundaryPixels)
        {
            return pos;
        }
        return Vector2Int.zero;
    }

    Vector3 PixelToWorld(Vector2Int pixelPos)
    {
        float ppu = spriteRenderer.sprite.pixelsPerUnit;
        Vector2 spriteSize = new Vector2(backgroundTexture.width / ppu, backgroundTexture.height / ppu);
        Vector2 worldBottomLeft = (Vector2)spriteRenderer.transform.position - (spriteSize / 2);

        return worldBottomLeft + ((Vector2)pixelPos / ppu);
    }

    Vector2Int WorldToPixel(Vector2 worldPos)
    {
        float ppu = spriteRenderer.sprite.pixelsPerUnit;
        Vector2 spriteSize = new Vector2(backgroundTexture.width / ppu, backgroundTexture.height / ppu);
        Vector2 worldBottomLeft = (Vector2)spriteRenderer.transform.position - (spriteSize / 2);

        Vector2 pixelPos = (worldPos - worldBottomLeft) * ppu;
        return new Vector2Int(Mathf.RoundToInt(pixelPos.x), Mathf.RoundToInt(pixelPos.y));
    }
}
