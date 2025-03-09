using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementColi : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private MaskController maskController;

    private bool isOutsideMyArea = false;
    private List<Vector2> pathPoints = new List<Vector2>();
    private Vector2 lastDirection;

    public float MaxY;
    public float MinY;
    public float MaxX;
    public float MinX;

    public LineRenderer lineRenderer;  // LineRenderer

    // �ڐG���Ă���I�u�W�F�N�g���Ǘ����郊�X�g
    private List<Collider2D> currentColliders = new List<Collider2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maskController = FindObjectOfType<MaskController>(); // MaskController���擾
        lineRenderer.positionCount = 0;  // �ŏ��͐���`�悵�Ȃ�

        if (maskController == null)
        {
            Debug.LogError("MaskController ���V�[�����Ɍ�����܂���I");
        }
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 && moveY != 0)
        {
            moveY = 0; // �΂߈ړ��֎~
        }

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection != Vector2.zero && moveDirection != lastDirection)
        {
            RecordPoint();
            lastDirection = moveDirection;
        }
        // LineRenderer�Ōo�H��`��
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));
    }

    void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;

        if (MaxX < transform.position.x)
        {
            transform.position = new Vector2(MaxX, transform.position.y);
            Debug.Log("�E");
            StopPlayer();
        }
        if (MinX > transform.position.x)
        {
            Debug.Log("��");
            transform.position = new Vector2(MinX, transform.position.y);
            StopPlayer();
        }
        if (MaxY < transform.position.y)
        {
            Debug.Log("��");
            transform.position = new Vector2(transform.position.x, MaxY);
            StopPlayer();
        }
        if (MinY > transform.position.y)
        {
            Debug.Log("��");
            transform.position = new Vector2(transform.position.x, MinY);
            StopPlayer();
        }
    }
    void StopPlayer()
    {
        rb.velocity = new Vector2(0, 0);
    }

    void RecordPoint()
    {
        pathPoints.Add(transform.position);
    }

    private Vector3[] ConvertToVector3Array(List<Vector2> pathPoints)
    {
        pathPoints.Add(transform.position);
        Vector3[] positions = new Vector3[pathPoints.Count];
        for (int i = 0; i < pathPoints.Count; i++)
        {
            positions[i] = new Vector3(pathPoints[i].x, pathPoints[i].y, 0);  // z����0�ɐݒ�
        }
        return positions;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter");
        Collider2D other = collision.collider;
        if (!currentColliders.Contains(other))
        {
            currentColliders.Add(other);
        }

        if (other.CompareTag("myarea"))
        {
            StopPlayer();
            if (isOutsideMyArea)
            {
                pathPoints.Add(transform.position); // �ŏI�_
                GeneratePolygonCollider();
            }
            isOutsideMyArea = false;
            pathPoints.Clear();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Exit");
        Collider2D other = collision.collider;
        if (currentColliders.Contains(other))
        {
            currentColliders.Remove(other);
        }

        bool hasAreaTag = currentColliders.Any(collider => collider.CompareTag("myarea"));
        if (!hasAreaTag && other.CompareTag("myarea"))
        {
            isOutsideMyArea = true;
            RecordPoint(); // �ŏ��̓_���L�^
        }
    }

    void GeneratePolygonCollider()
    {
        if (pathPoints.Count < 3)
        {
            Debug.LogWarning("���p�`���������܂���B�����_���ȕ����ŏC�������݂܂��B");
            AdjustPathForPolygon();
        }
        if (pathPoints.Count % 2 == 1)//��Ȃ�_�𑫂�
        {
            if (pathPoints.Count == 3)
            {
                AddPointToEvenNumber(pathPoints[0], pathPoints[1], pathPoints[2]);
            }
            else
            {
                Vector2 pointB = CalculateRightAnglePoint(pathPoints[0], pathPoints[pathPoints.Count - 1]);
                pathPoints.Add(pointB);
            }
        }

        GameObject newPoly = new GameObject("GeneratedPolygon");
        PolygonCollider2D polyCollider = newPoly.AddComponent<PolygonCollider2D>();

        polyCollider.points = pathPoints.ToArray();
        newPoly.tag = "myarea";

        Debug.Log($"�������ꂽ PolygonCollider2D �̓_�̐�: {polyCollider.points.Length}");
        foreach (Vector2 point in polyCollider.points)
        {
            Debug.Log($"�_: {point}");
        }

        if (maskController != null)
        {
            maskController.ApplyMask(polyCollider);
        }
    }

    void AddPointToEvenNumber(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        // ���p�O�p�`�̒��p��p2�ɂ���̂ŁA���p���`������2�ӂ���Ɏc��̓_���v�Z
        // p1��p3�����p�O�p�`�̕ӂŁAp2�����p
        Vector2 p4 = p1 + p3 - p2;
        pathPoints.Add(p4);
    }

    // �_A�Ɠ_C���璼�p�O�p�`�̒��p�̓_B���v�Z
    Vector2 CalculateRightAnglePoint(Vector2 A, Vector2 C)
    {
        // ����AC�̃x�N�g�������߂�
        Vector2 AC = C - A;

        // AC�̕����x�N�g���ɐ����ȃx�N�g�����v�Z
        // 2D�x�N�g���̐����x�N�g���� (x, y) -> (-y, x) �܂��� (y, -x)
        Vector2 perpendicular = new Vector2(-AC.y, AC.x);

        // �_C�̏ی����m�F
        Vector2 pointB = Vector2.zero;
        if (C.x > 0 && C.y > 0)  // ���ی�
        {
            pointB = C + perpendicular;  // �E��
        }
        else if (C.x < 0 && C.y > 0)  // ���ی�
        {
            pointB = C + perpendicular;  // ����
        }
        else if (C.x < 0 && C.y < 0)  // ��O�ی�
        {
            pointB = C - perpendicular;  // ����
        }
        else if (C.x > 0 && C.y < 0)  // ��l�ی�
        {
            pointB = C - perpendicular;  // �E��
        }

        return pointB;
    }



    void AdjustPathForPolygon()
    {
        if (pathPoints.Count < 2) return;

        Vector2 lastPoint = pathPoints[pathPoints.Count - 1];
        Vector2 firstPoint = pathPoints[0];

        Vector2 midPoint = (lastPoint + firstPoint) / 2;
        pathPoints.Insert(1, midPoint);
    }
}
