using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySystem : ISystem // 定义部分结构体 EnemySystem，实现 ISystem 接口
{
    // 定义三个键结构体，用于共享静态变量
    //这三个 key1、key2 和 key3 是用来作为 SharedStatic 静态变量的键的。
    //SharedStatic 是 Unity 的一个特性，用于在多个作业和线程之间共享静态数据。
    //这些键结构体实际上是唯一标识，用于区分不同的 SharedStatic 实例。
    public struct key1 { }
    public struct key2 { }
    public struct key3 { }

    // 定义共享静态变量
    public readonly static SharedStatic<int> createdCount = SharedStatic<int>.GetOrCreate<key1>();
    public readonly static SharedStatic<int> createCount = SharedStatic<int>.GetOrCreate<key2>();
    public readonly static SharedStatic<Random> random = SharedStatic<Random>.GetOrCreate<key3>();

    public float spawnEnemyTimer; // 定义生成敌人的计时器
    public const int maxEnemys = 10000; // 定义最大敌人数

    // 系统创建时的初始化方法
    public void OnCreate(ref SystemState state)
    {
        createdCount.Data = 0; // 初始化已创建敌人数
        createCount.Data = 0; // 初始化待创建敌人数
        random.Data = new Random((uint)System.DateTime.Now.GetHashCode()); // 初始化随机数生成器
        SharedData.GameSharedInfo.Data.deadCounter = 0; // 初始化死亡计数器
    }

    // 系统更新方法
    public void OnUpdate(ref SystemState state)
    {
        spawnEnemyTimer -= SystemAPI.Time.DeltaTime; // 更新生成敌人的计时器
        if (spawnEnemyTimer <= 0)
        {
            spawnEnemyTimer = SharedData.GameSharedInfo.Data.spawnInterval; // 重置生成计时器
            createCount.Data += SharedData.GameSharedInfo.Data.spawnCount; // 增加待创建敌人数
        }

        // 获取并行命令缓冲区
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        float2 playerPos = SharedData.playerPos.Data; // 获取玩家位置

        // 调度并行执行 EnemyJob
        new EnemyJob()
        {
            detaTime = SystemAPI.Time.DeltaTime, // 每帧时间增量
            playerPos = playerPos, // 玩家位置
            ecb = ecb, // 并行命令缓冲区
            time = SystemAPI.Time.ElapsedTime, // 已经过的时间
        }.ScheduleParallel();
        state.CompleteDependency(); // 完成依赖

        // 如果有待创建的敌人且未达到最大敌人数
        if (createCount.Data > 0 && createdCount.Data < maxEnemys)
        {
            NativeArray<Entity> newEnemys = new NativeArray<Entity>(createCount.Data, Allocator.Temp); // 创建新的敌人实体数组
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().enemyPortotype, newEnemys); // 实例化新的敌人

            for (int i = 0; i < newEnemys.Length && createdCount.Data < maxEnemys; i++)
            {
                createdCount.Data += 1; // 更新已创建敌人数
                float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10); // 生成随机偏移量
                ecb.SetComponent<LocalTransform>(newEnemys[i].Index, newEnemys[i], new LocalTransform()
                {
                    Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0), // 设置敌人位置
                    Rotation = quaternion.identity, // 设置敌人旋转
                    Scale = 1, // 设置敌人缩放
                });
            }
            createCount.Data = 0; // 重置待创建敌人数
            newEnemys.Dispose(); // 释放新敌人数组
        }
    }

    // 使用 EntityQueryOptions.IgnoreComponentEnabledState 忽略组件启用状态
    // 使用BurstCompile特性 启用Unity Burst编译器优化代码执行性能
    // Burst编译器会将标记的代码编译成高度优化的本地代码（native code）
    // 以提高运行时的性能 在频繁执行的代码片段中尤为重要
    // 游戏物理计算 数据计算 路径计算
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct EnemyJob : IJobEntity // 定义部分结构体 EnemyJob，实现 IJobEntity 接口
    {
        public float detaTime; // 每帧时间增量
        public double time; // 已经过的时间
        public float2 playerPos; // 玩家位置
        public EntityCommandBuffer.ParallelWriter ecb; // 并行命令缓冲区

        // Execute 方法用于处理每个实体
        private void Execute(
            EnabledRefRW<EnemyData> enableState,
            EnabledRefRW<RendererSortTag> rendererSortEnableState,
            EnabledRefRW<AnimationFrameIndex> aniamtionEnableState,
            ref EnemyData enemyData,
            in EnemySharedData enemySharedData,
            ref LocalTransform localTransform,
            ref LocalToWorld localToWorld)
        {
            // 如果当前敌人是非激活状态，同时需要创建敌人
            if (enableState.ValueRO == false)
            {
                if (createCount.Data > 0)
                {
                    createCount.Data -= 1; // 更新待创建敌人数
                    float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10); // 生成随机偏移量
                    localTransform.Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0); // 设置敌人位置
                    enableState.ValueRW = true; // 启用敌人
                    rendererSortEnableState.ValueRW = true; // 启用渲染排序标签
                    aniamtionEnableState.ValueRW = true; // 启用动画帧索引
                    localTransform.Scale = 1; // 设置敌人缩放
                }
                return;
            }

            // 如果敌人死亡
            if (enemyData.die)
            {
                SharedData.GameSharedInfo.Data.playHitAudio = true; // 播放击中音效
                SharedData.GameSharedInfo.Data.deadCounter += 1; // 更新死亡计数器
                SharedData.GameSharedInfo.Data.playHitAudioTime += time; // 更新击中音效时间
                enemyData.die = false; // 重置敌人死亡标志
                enableState.ValueRW = false; // 禁用敌人
                rendererSortEnableState.ValueRW = false; // 禁用渲染排序标签
                aniamtionEnableState.ValueRW = false; // 禁用动画帧索引
                localTransform.Scale = 0; // 将缩放设置为 0
                localToWorld.Value.c1.y = enemySharedData.scale.y; // 更新 LocalToWorld 的缩放
                return;
            }

            // 敌人移动逻辑
            float2 dir = math.normalize(playerPos - new float2(localTransform.Position.x, localTransform.Position.y)); // 计算方向向量
            localTransform.Position += detaTime * enemySharedData.moveSpeed * new float3(dir.x, dir.y, 0); // 更新敌人位置

            // 更新 LocalToWorld 的缩放
            localToWorld.Value.c0.x = localTransform.Position.x < playerPos.x ? -enemySharedData.scale.x : enemySharedData.scale.x;
            localToWorld.Value.c1.y = enemySharedData.scale.y;
        }
    }
}
