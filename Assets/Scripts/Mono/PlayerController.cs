using Unity.Entities;
using UnityEngine;

// 定义 PlayerState 枚举类型，表示玩家状态
public enum PlayerState
{
    Idle, // 空闲状态
    Move  // 移动状态
}

// 定义 PlayerController 类，继承自 MonoBehaviour
public class PlayerController : MonoBehaviour
{
    public Animator animator; // 定义动画组件
    public float moveSpeed; // 定义移动速度
    public Vector2 moveRangeX; // 定义 x 轴的移动范围
    public Vector2 moveRangeY; // 定义 y 轴的移动范围
    public Transform gunRoot; // 定义枪的根 Transform
    public int lv = 1; // 定义初始等级

    // 定义子弹数量属性，返回等级值
    public int bulletQuantity { get => lv; }

    // 定义攻击冷却时间属性，返回基于等级计算的冷却时间
    public float attackCD { get => Mathf.Clamp(1f / lv * 1.5F, 0.1F, 1F); }

    // 定义等级属性
    public int LV
    {
        get => lv;
        set
        {
            lv = value;
            // 基于等级调整怪物生成间隔和数量
            SharedData.GameSharedInfo.Data.spawnInterval = 10f / lv * spawnMonsterIntervaLMultiply;
            SharedData.GameSharedInfo.Data.spawnCount = (int)(lv * 5 * spawnMonsterQuantityLMultiply);
        }
    }

    public float spawnMonsterIntervaLMultiply = 1; // 定义生成怪物间隔乘数
    public float spawnMonsterQuantityLMultiply = 1; // 定义生成怪物数量乘数
    public PlayerState playerState; // 定义玩家状态

    // 定义玩家状态属性，设置状态时切换动画
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

    // Awake 方法在脚本实例化时调用
    private void Awake()
    {
        CheckPositionRange(); // 检查位置范围
        LV = lv; // 初始化等级
    }

    // Start 方法在第一次调用 Update 方法前调用
    private void Start()
    {
        PlayerState = PlayerState.Idle; // 设置初始状态为 Idle
    }

    // Update 方法在每帧调用
    private void Update()
    {
        CheckAttack(); // 检查攻击输入

        float h = Input.GetAxis("Horizontal"); // 获取水平输入
        float v = Input.GetAxis("Vertical"); // 获取垂直输入

        switch (playerState)
        {
            case PlayerState.Idle:
                if (h != 0 || v != 0) PlayerState = PlayerState.Move; // 如果有输入，切换到移动状态
                break;
            case PlayerState.Move:
                if (h == 0 && v == 0)
                {
                    PlayerState = PlayerState.Idle; // 如果没有输入，切换到空闲状态
                    return;
                }
                transform.Translate(moveSpeed * Time.deltaTime * new Vector3(h, v, 0)); // 根据输入移动玩家
                CheckPositionRange(); // 检查位置范围

                // 根据水平输入调整玩家朝向
                if (h > 0) transform.localScale = Vector3.one;
                else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }

    // 播放动画方法
    public void PlayAnimation(string animationName)
    {
        animator.CrossFadeInFixedTime(animationName, 0); // 使用动画组件播放指定动画
    }

    // 检查位置范围方法
    private void CheckPositionRange()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, moveRangeX.x, moveRangeX.y); // 限制 x 轴范围
        pos.y = Mathf.Clamp(pos.y, moveRangeY.x, moveRangeY.y); // 限制 y 轴范围
        pos.z = pos.y; // 设置 z 轴与 y 轴相同
        transform.position = pos; // 更新玩家位置
        SharedData.playerPos.Data = (Vector2)transform.position; // 更新共享数据中的玩家位置
    }

    private float attackCDTimer; // 定义攻击冷却计时器

    // 检查攻击方法
    private void CheckAttack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 获取鼠标位置
        gunRoot.up = (Vector2)mousePos - (Vector2)transform.position; // 设置枪的朝向

        attackCDTimer -= Time.deltaTime; // 更新攻击冷却计时器
        if (attackCDTimer <= 0 && Input.GetMouseButton(0))
        {
            Attack(); // 进行攻击
            attackCDTimer = attackCD; // 重置冷却计时器
        }
    }

    // 攻击方法
    private void Attack()
    {
        AudioManager.instance.PlayShootAudio(); // 播放射击音效

        // 生成子弹信息
        DynamicBuffer<BulletCreateInfo> buffer = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<BulletCreateInfo>(SharedData.singtonEntity.Data);
        buffer.Add(new BulletCreateInfo()
        {
            position = gunRoot.position, // 设置子弹位置
            rotation = gunRoot.rotation, // 设置子弹旋转
        });

        // 计算子弹的角度步长并生成多个子弹
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
