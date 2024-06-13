using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct BulletSystem : ISystem // 定义部分结构体 BulletSystem，实现 ISystem 接口
{
    // 定义静态只读共享静态变量 createBulletCount使用GetOrCreate可以避免多次创建 
    // readonly static 静态只读 GetOrCreate<BulletSystem>()创建或获取一个 "SharedStatic"的实例 与BulletSystem相关 保证了BulletSystem共享同一个静态数据
    public readonly static SharedStatic<int> createBulletCount = SharedStatic<int>.GetOrCreate<BulletSystem>();

    // 定义系统创建时的初始化方法 SystemState系统运行的状态以及相关数据
    public void OnCreate(ref SystemState state)
    {
        // Data 属性用于访问或修改 SharedStatic<int> 实例中的实际数据值
        createBulletCount.Data = 0; // 初始化 createBulletCount 为 0
        SharedData.singtonEntity.Data = state.EntityManager.CreateEntity(typeof(BulletCreateInfo)); // 创建 BulletCreateInfo 类型的实体
    }

    // 定义系统更新方法
    public void OnUpdate(ref SystemState state)
    {
        // 获取并行命令缓冲区 可以实例和删除实体 
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // 获取 BulletCreateInfo 缓冲区 DynamicBuffer是用于处理动态数组的一个结构 允许为每个实体存储一个动态大小的缓存区
        DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer = SystemAPI.GetSingletonBuffer<BulletCreateInfo>();
        createBulletCount.Data = bulletCreateInfoBuffer.Length; // 更新子弹创建数量

        // 调度并行执行 BulletJob
        new BulletJob()
        {
            enemyLayerMask = 1 << 6, // 敌人层掩码
            ecb = ecb, // 并行命令缓冲区
            deltaTime = SystemAPI.Time.DeltaTime, // 每帧时间增量 即Time.DeltaTime
            bulletCreateInfoBuffer = bulletCreateInfoBuffer, // 子弹创建信息缓冲区
            collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld, // 碰撞世界 一般可以用于实体之间检测碰撞 确定他们是否在接触或者相互影响 处理物理交互使用
        }.ScheduleParallel();//自动使用当前系统的依赖来管理job的调度
        state.CompleteDependency(); // 完成依赖 用于显式地完成当前系统的所有依赖工作(jobs）

        // 如果需要创建的子弹数量大于 0
        if (createBulletCount.Data > 0)
        {
            // 创建新的子弹实体数组
            NativeArray<Entity> newBullets = new NativeArray<Entity>(createBulletCount.Data, Allocator.Temp); //参数1数组的长度 参数2如何分配内存这里指的是临时分配内存 只在当前帧有效 然后同一帧释放
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets); // 实例化新的子弹 第一个参数SortKey 指定命令的排序顺序 越小越先执行

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
                    bulletEnableState.ValueRW = true; // 启用子弹倒计时销毁时间
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
            // 物理命名空间下的一个结构体 定义物理对象的碰撞过滤规则的
            CollisionFilter filter = new CollisionFilter()
            {
                BelongsTo = ~0u, // 物体所属的层
                CollidesWith = enemyLayerMask, // 物体可以与哪些层发生碰撞
                GroupIndex = 0 // 0表示没有特殊的碰撞规则
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
