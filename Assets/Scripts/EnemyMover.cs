using Unity.VisualScripting;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2.0f;
    public float cooldownDuration = 2.0f; // �N�[���_�E���̒����i�b�j
    public TextureBoundaryDetector textureBoundaryDetector;
    public PlayerMovementpix playerMovementpix;

    private Vector2 direction;
    private bool enemyOnLineCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        // ���������F45�x�i�E��j
        float angle = 45f * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 nextPosition = currentPosition + direction * speed * Time.deltaTime;

        // �N�[���_�E������
        if (!enemyOnLineCooldown)
        {
            if (playerMovementpix.IsEnemyOnLine(nextPosition))
            {
                enemyOnLineCooldown = true;
                cooldownTimer = cooldownDuration;
                Debug.Log("Enemy touched line! Starting cooldown.");
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                enemyOnLineCooldown = false;
                Debug.Log("Cooldown ended.");
            }
        }

        // �ʍs�\�Ȃ�ړ�
        if (!textureBoundaryDetector.IsInTransparentArea(nextPosition) && IsWithinScreenBounds(nextPosition))
        {
            transform.position = nextPosition;
        }
        else
        {
            // 90�x���v���ɉ�]
            direction = new Vector2(direction.y, -direction.x);
        }
    }

    // ��ʓ��ɂ��邩���`�F�b�N�i�J�����r���[��j
    private bool IsWithinScreenBounds(Vector2 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 &&
               screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    public void CheckArea()
    {
        //�����G���A�ɂ��邩
        if (textureBoundaryDetector.IsInTransparentArea(new Vector2(transform.position.x, transform.position.y)))
        {
            Debug.Log("���ꂽ�[");
            Destroy(this.gameObject);
        }
    }
}
