using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementpix : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float moveSpeed_tmp = 5f;
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
    public BoundaryChecker boundarychecker;
    public TextureBoundaryDetector textureboundarydetector;

    // �ڐG���Ă���I�u�W�F�N�g���Ǘ����郊�X�g
    private List<Collider2D> currentColliders = new List<Collider2D>();

    private bool allowSpace = true;

    public float threshold = 0.2f; // ����Ɏg�������̂������l

    void Start()
    {
        moveSpeed_tmp = moveSpeed;
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

        moveSpeed = moveSpeed_tmp;
        Vector2 nextPosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * moveSpeed * Time.fixedDeltaTime;


            // �n���Ƃ����� Vector3 �ɕϊ��iz = 1�j
            if (textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
            {
                moveDirection = new Vector2(moveX, moveY).normalized;
            }
            else
            {
                //�p���Ȃ���Ƃ��͑��x�𗎂Ƃ��Ȃ��Ƃ��܂������Ȃ�
                //�X�s�[�h�𗎂Ƃ��čĔ���
                moveSpeed = 0.5f;
                nextPosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * moveSpeed * Time.fixedDeltaTime;

                // �n���Ƃ����� Vector3 �ɕϊ��iz = 1�j
                if (textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
                {
                    moveDirection = new Vector2(moveX, moveY).normalized;
                }
                else
                {
                    moveDirection = Vector2.zero; // �ړ��s�Ȃ��~
                }
            }
            //�X�y�[�X�L�[��������Ă���Ƃ�
           if (Input.GetKey(KeyCode.Space)&&allowSpace)
           {
            moveSpeed = moveSpeed_tmp;
            moveDirection = new Vector2(moveX, moveY).normalized;

               //�̈������̈�O�ɂł�
               if (boundarychecker.isBoundary && !textureboundarydetector.IsOnBoundary(boundarychecker.WorldToPixel(new Vector3(nextPosition2D.x, nextPosition2D.y, 1))))
               {
                Vector2 prePosition2D = (Vector2)transform.position + new Vector2(moveX, moveY) * (-1*moveSpeed) * Time.fixedDeltaTime;
                ExitArea(prePosition2D);
               }
               //�̈�O����̈���ɂ��ǂ�
               else if (!boundarychecker.isBoundary && textureboundarydetector.IsInTransparentArea(nextPosition2D))
               {
                   EnterArea(nextPosition2D);
                   boundarychecker.WarpToClosestBoundary();
                   moveDirection = Vector2.zero;
                   allowSpace = false;
               }
           //�O�ɂ���Ƃ��Ɉ�
           if (!boundarychecker.isBoundary&& IsPointOnLine(nextPosition2D)&&(new Vector2(moveX,moveY) != Vector2.zero))
           {
                Debug.Log("�����_��ʂ�܂���");
                //�ŏ��̓_�ɖ߂�
                Vector2 firstPoint = pathPoints[0];
                transform.position = firstPoint;
                pathPoints.Clear();
            }

                    // LineRenderer�Ōo�H��`��
                    lineRenderer.positionCount = pathPoints.Count;
                    lineRenderer.SetPositions(ConvertToVector3Array(pathPoints));

           }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!boundarychecker.isBoundary)
            {
                //�ŏ��̓_�ɖ߂�
                Vector2 firstPoint = pathPoints[0];
                transform.position = firstPoint;
            }
            pathPoints.Clear();
            allowSpace = true;
        }


        if (moveDirection != Vector2.zero && moveDirection != lastDirection)
        {
            RecordPoint();
            lastDirection = moveDirection;
        }

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

    bool IsPointOnLineSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        float lengthAB = Vector2.Distance(a, b);
        float lengthAP = Vector2.Distance(a, p);
        float lengthPB = Vector2.Distance(p, b);

        // ������ɂ��邩����i�덷�l���j
        return Mathf.Approximately(lengthAB, lengthAP + lengthPB);
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

    void EnterArea(Vector2 next)
    {
        Debug.Log("�̈�ɖ߂�܂���");
        StopPlayer();
        if (isOutsideMyArea)
        {
            pathPoints.Add(next); // �ŏI�_
            GeneratePolygonCollider();
        }
        isOutsideMyArea = false;
        pathPoints.Clear();
    }

    void ExitArea(Vector2 pre)
    {
        pathPoints.Clear();
        Debug.Log("�̈悩��o�܂���");
        isOutsideMyArea = true;
        pathPoints.Add(pre); // �ŏ��̓_���L�^

    }


    bool IsPointOnLine(Vector2 point)
    {
        int count = lineRenderer.positionCount;

        // �ŐV�̃Z�O�����g�͏��O���邽�߁Acount - 1 �܂Ń��[�v
        for (int i = 0; i < count - 100; i++)
        {
            Vector2 lineStart = lineRenderer.GetPosition(i);
            Vector2 lineEnd = lineRenderer.GetPosition(i + 1);

            if (IsPointNearLineSegment(lineStart, lineEnd, point))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPointNearLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        // �����Ɠ_�Ƃ̋������v�Z
        float distance = DistanceFromPointToLineSegment(lineStart, lineEnd, point);

        // �������������l�ȉ��Ȃ�d�Ȃ��Ă���Ƃ݂Ȃ�
        return distance < threshold;
    }

    float DistanceFromPointToLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        float lineLength = (lineEnd - lineStart).magnitude;

        if (lineLength == 0)
            return Vector2.Distance(point, lineStart); // �n�_�ƏI�_�������Ȃ�n�_�܂ł̋���

        float projection = Vector2.Dot(point - lineStart, lineEnd - lineStart) / lineLength;

        // �v���W�F�N�V������������Ɏ��܂邩�ǂ������m�F
        if (projection < 0)
            return Vector2.Distance(point, lineStart);
        if (projection > lineLength)
            return Vector2.Distance(point, lineEnd);

        // ������̍ŒZ����
        Vector2 closestPoint = lineStart + projection * (lineEnd - lineStart).normalized;
        return Vector2.Distance(point, closestPoint);
    }




    void GeneratePolygonCollider()
    {
        //��������
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
                //Vector2 pointB = CalculateRightAnglePoint(pathPoints[0], pathPoints[pathPoints.Count - 1]);
                //pathPoints.Add(pointB);
            }

        }
        //�����܂ł��Ԃ񂢂�Ȃ�

        GameObject newPoly = new GameObject("GeneratedPolygon");
        PolygonCollider2D polyCollider = newPoly.AddComponent<PolygonCollider2D>();

        polyCollider.points = pathPoints.ToArray();
        newPoly.tag = "myarea";

        // **�f�o�b�O�p���O**
        Debug.Log($"�������ꂽ PolygonCollider2D �̓_�̐�: {polyCollider.points.Length}");
        foreach (Vector2 point in polyCollider.points)
        {
            Debug.Log($"�_: {point}");
        }

        // **�}�X�N������K�p**
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