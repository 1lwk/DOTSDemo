using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Skill:IBufferElementData
{
    public int id;
}

public readonly partial struct MonsterAspect:IAspect
{
    public readonly RefRW<MonsterData> monsterData;
    public readonly RefRW<LocalTransform> loaclTransform;
    public readonly MonsterConfig monsterConfig;
    public readonly DynamicBuffer<Skill> skills;
}

public readonly partial struct MoveAspect : IAspect
{
    public readonly RefRW<MoveData> moveData;
    public readonly RefRW<LocalTransform> loaclTransform;
    public readonly RefRO<MoveTag> tag;
}

public struct MonsterData:IComponentData,IEnableableComponent
{
    public float hp;
    public float createBulletTimer;
}

public struct MonsterConfig: ISharedComponentData
{
    public float createbulletInterval;
}

public struct GameConfig : IComponentData
{
    public Entity bulletPrototype;
}

public struct MoveData:IComponentData
{
    public float moveSpeed;
}

public struct MoveTag : IComponentData
{ }

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MonsterSystem:ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //Entity monster = state.EntityManager.CreateEntity(typeof(MonsterData),typeof(LocalTransform));
        //state.EntityManager.SetComponentData(monster, new MonsterData()
        //{
        //    hp = 100
        //}) ;
        state.RequireForUpdate<GameConfig>();//如果没有这个组件就不运行UpDate
    }

    public void OnUpdate(ref SystemState state) 
    {
        //SystemAPI 只能在System中使用 否则会报错
        //筛选 遍历 
        //foreach ((RefRW<MonsterData> monsterData,RefRW<LocalTransform> localTransform) in SystemAPI.Query<RefRW<MonsterData>, RefRW<LocalTransform>>().WithAny())
        //{
        //    localTransform.ValueRW.Position += dir * SystemAPI.Time.DeltaTime;
        //    monsterData.ValueRW.hp -= SystemAPI.Time.DeltaTime;
        //}

        GameConfig gameConfig = SystemAPI.GetSingleton<GameConfig>();
        foreach (MonsterAspect monster in SystemAPI.Query<MonsterAspect>())
        {
            monster.monsterData.ValueRW.hp -= SystemAPI.Time.DeltaTime;
            monster.monsterData.ValueRW.createBulletTimer -= SystemAPI.Time.DeltaTime;
            if(monster.monsterData.ValueRW.createBulletTimer<=0)
            {
                monster.monsterData.ValueRW.createBulletTimer = monster.monsterConfig.createbulletInterval;
                Entity bullet=state.EntityManager.Instantiate(gameConfig.bulletPrototype);
                state.EntityManager.SetComponentData(bullet, new LocalTransform()
                {
                    Position = monster.loaclTransform.ValueRO.Position,
                    Scale = 0.5f
                }) ;

                UnityEngine.Debug.Log(monster.skills.Length);
            }
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MoveSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        float3 dir = new float3(0, 0, 1);
        foreach (MoveAspect move in SystemAPI.Query<MoveAspect>())
        {
            move.loaclTransform.ValueRW.Position += SystemAPI.Time.DeltaTime * move.moveData.ValueRW.moveSpeed * dir;
        }
    }
}