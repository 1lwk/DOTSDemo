using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct BulletSystem : ISystem // 定义部分结构体 BulletSystem，实现 ISystem 接口
{
    // 定义静态只读共享静态变量 createBulletCount
    public readonly static SharedStatic<int> createBulletCount = SharedStatic<int>.GetOrCreate<BulletSystem>();

    // 定义系统创建时的初始化方法
    public void OnCreate(ref SystemState state)
    {
        createBulletCount.Data = 0; // 初始化 createBulletCount 为 0
        SharedData.singtonEntity.Data = state.EntityManager.CreateEntity(typeof(BulletCreateInfo)); // 创建 BulletCreateInfo 类型的实体
    }

    // 定义系统更新方法
    public void OnUpdate(ref SystemState state)
    {
        // 获取并行命令缓冲区
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // 获取 BulletCreateInfo 缓冲区
        DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer = SystemAPI.GetSingletonBuffer<BulletCreateInfo>();
        createBulletCount.Data = bulletCreateInfoBuffer.Length; // 更新子弹创建数量

        // 调度并行执行 BulletJob
        new BulletJob()
        {
            enemyLayerMask = 1 << 6, // 敌人层掩码
            ecb = ecb, // 并行命令缓冲区
            deltaTime = SystemAPI.Time.DeltaTime, // 每帧时间增量
            bulletCreateInfoBuffer = bulletCreateInfoBuffer, // 子弹创建信息缓冲区
            collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld, // 碰撞世界
        }.ScheduleParallel();
        state.CompleteDependency(); // 完成依赖

        // 如果需要创建的子弹数量大于 0
        if (createBulletCount.Data > 0)
        {
            // 创建新的子弹实体数组
            NativeArray<Entity> newBullets = new NativeArray<Entity>(createBulletCount.Data, Allocator.Temp);
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets); // 实例化新的子弹

            // 初始化每个新子弹的 Transform 组件
            for (int i = 0; i < newBullets.Length; i++)
            {
                BulletCreateInfo info = bulletCreateInfoBuffer[i];
                ecb.SetComponent<LocalTransform>(newBullets[i].Index, newBullets[i], new LocalTransform()
                {
                    Position = info.position,
                    Rotation = info.rotation,
                    Scale = 1
                });
            }
            newBullets.Dispose(); // 释放新子弹数组
        }

        bulletCreateInfoBuffer.Clear(); // 清空子弹创建信息缓冲区
    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)] // 忽略组件启用状态的实体查询选项
    [BurstCompile] // 使用 Burst 编译
    public partial struct BulletJob : IJobEntity // 定义部分结构体 BulletJob，实现 IJobEntity 接口
    {
        public uint enemyLayerMask; // 敌人层掩码
        public EntityCommandBuffer.ParallelWriter ecb; // 并行命令缓冲区
        public float deltaTime; // 每帧时间增量
        [ReadOnly] public DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer; // 只读的子弹创建信息缓冲区
        [ReadOnly] public CollisionWorld collisionWorld; // 只读的碰撞世界

        public void Execute(
            EnabledRefRW<BulletData> bulletEnableState,
            EnabledRefRW<RendererSortTag> sortEnableState,
            ref BulletData bulletData,
            in BulletSharedData bulletSharedData,
            in Entity entity,
            ref LocalTransform localTransform)
        {
            // 如果当前子弹是非激活状态，同时需要创建子弹
            if (bulletEnableState.ValueRO == false)
            {
                if (createBulletCount.Data > 0)
                {
                    int index = createBulletCount.Data -= 1; // 更新子弹创建数量
                    bulletEnableState.ValueRW = true; // 启用子弹
                    sortEnableState.ValueRW = true; // 启用渲染排序标签
                    localTransform.Position = bulletCreateInfoBuffer[index].position; // 设置子弹位置
                    localTransform.Rotation = bulletCreateInfoBuffer[index].rotation; // 设置子弹旋转
                    localTransform.Scale = 1; // 设置子弹缩放
                    bulletData.destroyTimer = bulletSharedData.destroyTimer; // 初始化销毁计时器
                }
                return;
            }

            // 子弹位置移动
            localTransform.Position += bulletSharedData.moveSpeed * deltaTime * localTransform.Up();

            // 更新销毁计时器
            bulletData.destroyTimer -= deltaTime;
            if (bulletData.destroyTimer <= 0)
            {
                bulletEnableState.ValueRW = false; // 禁用子弹
                sortEnableState.ValueRW = false; // 禁用渲染排序标签
                localTransform.Scale = 0; // 将缩放设置为 0
                return;
            }

            // 进行伤害检测
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            CollisionFilter filter = new CollisionFilter()
            {
                BelongsTo = ~0u, // 属于所有层
                CollidesWith = enemyLayerMask, // 与敌人层碰撞
                GroupIndex = 0
            };

            // 检查是否与敌人碰撞
            if (collisionWorld.OverlapBox(localTransform.Position, localTransform.Rotation, bulletSharedData.colliderHalfExtents, ref hits, filter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity temp = hits[i].Entity; // 获取碰撞实体
                    bulletData.destroyTimer = 0; // 将销毁计时器设置为 0
                    ecb.SetComponent<EnemyData>(temp.Index, temp, new EnemyData()
                    {
                        die = true, // 设置敌人死亡标志
                    });
                }
            }
            hits.Dispose(); // 释放命中列表
        }
    }
}
