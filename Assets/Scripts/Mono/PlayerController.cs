using Unity.Entities;
using UnityEngine;

// ���� PlayerState ö�����ͣ���ʾ���״̬
public enum PlayerState
{
    Idle, // ����״̬
    Move  // �ƶ�״̬
}

// ���� PlayerController �࣬�̳��� MonoBehaviour
public class PlayerController : MonoBehaviour
{
    public Animator animator; // ���嶯�����
    public float moveSpeed; // �����ƶ��ٶ�
    public Vector2 moveRangeX; // ���� x ����ƶ���Χ
    public Vector2 moveRangeY; // ���� y ����ƶ���Χ
    public Transform gunRoot; // ����ǹ�ĸ� Transform
    public int lv = 1; // �����ʼ�ȼ�

    // �����ӵ��������ԣ����صȼ�ֵ
    public int bulletQuantity { get => lv; }

    // ���幥����ȴʱ�����ԣ����ػ��ڵȼ��������ȴʱ��
    public float attackCD { get => Mathf.Clamp(1f / lv * 1.5F, 0.1F, 1F); }

    // ����ȼ�����
    public int LV
    {
        get => lv;
        set
        {
            lv = value;
            // ���ڵȼ������������ɼ��������
            SharedData.GameSharedInfo.Data.spawnInterval = 10f / lv * spawnMonsterIntervaLMultiply;
            SharedData.GameSharedInfo.Data.spawnCount = (int)(lv * 5 * spawnMonsterQuantityLMultiply);
        }
    }

    public float spawnMonsterIntervaLMultiply = 1; // �������ɹ���������
    public float spawnMonsterQuantityLMultiply = 1; // �������ɹ�����������
    public PlayerState playerState; // �������״̬

    // �������״̬���ԣ�����״̬ʱ�л�����
    public PlayerState PlayerState
    {
        get => playerState;
        set
        {
            playerState = value;
            switch (playerState)
            {
                case PlayerState.Idle:
                    PlayAnimation("Idle");
                    break;
                case PlayerState.Move:
                    PlayAnimation("Move");
                    break;
            }
        }
    }

    // Awake �����ڽű�ʵ����ʱ����
    private void Awake()
    {
        CheckPositionRange(); // ���λ�÷�Χ
        LV = lv; // ��ʼ���ȼ�
    }

    // Start �����ڵ�һ�ε��� Update ����ǰ����
    private void Start()
    {
        PlayerState = PlayerState.Idle; // ���ó�ʼ״̬Ϊ Idle
    }

    // Update ������ÿ֡����
    private void Update()
    {
        CheckAttack(); // ��鹥������

        float h = Input.GetAxis("Horizontal"); // ��ȡˮƽ����
        float v = Input.GetAxis("Vertical"); // ��ȡ��ֱ����

        switch (playerState)
        {
            case PlayerState.Idle:
                if (h != 0 || v != 0) PlayerState = PlayerState.Move; // ��������룬�л����ƶ�״̬
                break;
            case PlayerState.Move:
                if (h == 0 && v == 0)
                {
                    PlayerState = PlayerState.Idle; // ���û�����룬�л�������״̬
                    return;
                }
                transform.Translate(moveSpeed * Time.deltaTime * new Vector3(h, v, 0)); // ���������ƶ����
                CheckPositionRange(); // ���λ�÷�Χ

                // ����ˮƽ���������ҳ���
                if (h > 0) transform.localScale = Vector3.one;
                else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }

    // ���Ŷ�������
    public void PlayAnimation(string animationName)
    {
        animator.CrossFadeInFixedTime(animationName, 0); // ʹ�ö����������ָ������
    }

    // ���λ�÷�Χ����
    private void CheckPositionRange()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, moveRangeX.x, moveRangeX.y); // ���� x �᷶Χ
        pos.y = Mathf.Clamp(pos.y, moveRangeY.x, moveRangeY.y); // ���� y �᷶Χ
        pos.z = pos.y; // ���� z ���� y ����ͬ
        transform.position = pos; // �������λ��
        SharedData.playerPos.Data = (Vector2)transform.position; // ���¹��������е����λ��
    }

    private float attackCDTimer; // ���幥����ȴ��ʱ��

    // ��鹥������
    private void CheckAttack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // ��ȡ���λ��
        gunRoot.up = (Vector2)mousePos - (Vector2)transform.position; // ����ǹ�ĳ���

        attackCDTimer -= Time.deltaTime; // ���¹�����ȴ��ʱ��
        if (attackCDTimer <= 0 && Input.GetMouseButton(0))
        {
            Attack(); // ���й���
            attackCDTimer = attackCD; // ������ȴ��ʱ��
        }
    }

    // ��������
    private void Attack()
    {
        AudioManager.instance.PlayShootAudio(); // ���������Ч

        // �����ӵ���Ϣ
        DynamicBuffer<BulletCreateInfo> buffer = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<BulletCreateInfo>(SharedData.singtonEntity.Data);
        buffer.Add(new BulletCreateInfo()
        {
            position = gunRoot.position, // �����ӵ�λ��
            rotation = gunRoot.rotation, // �����ӵ���ת
        });

        // �����ӵ��ĽǶȲ��������ɶ���ӵ�
        float angleStep = Mathf.Clamp(360 / bulletQuantity, 0, 5f);
        for (int i = 1; i < bulletQuantity / 2; i++)
        {
            buffer.Add(new BulletCreateInfo()
            {
                position = gunRoot.position,
                rotation = gunRoot.rotation * Quaternion.Euler(0, 0, angleStep * i),
            });
            buffer.Add(new BulletCreateInfo()
            {
                position = gunRoot.position,
                rotation = gunRoot.rotation * Quaternion.Euler(0, 0, -angleStep * i),
            });
        }
    }
}
