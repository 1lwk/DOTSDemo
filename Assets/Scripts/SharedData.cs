using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

// 定义一个静态类 SharedData 用于存储共享数据
public static class SharedData
{
    // 定义一个共享静态变量 singtonEntity，用于存储一个全局唯一的实体
    public static readonly SharedStatic<Entity> singtonEntity = SharedStatic<Entity>.GetOrCreate<KeyClass1>();

    // 定义一个共享静态变量 GameSharedInfo，用于存储全局游戏共享信息
    public static readonly SharedStatic<GameSharedInfo> GameSharedInfo = SharedStatic<GameSharedInfo>.GetOrCreate<GameSharedInfo>();

    // 定义一个共享静态变量 playerPos，用于存储玩家位置
    public static readonly SharedStatic<float2> playerPos = SharedStatic<float2>.GetOrCreate<KeyClass2>();

    // 定义一个空的结构体 KeyClass1，用于标识 singtonEntity 的共享静态变量
    public struct KeyClass1 { }

    // 定义一个空的结构体 KeyClass2，用于标识 playerPos 的共享静态变量
    public struct KeyClass2 { }
}

// 定义一个结构体 GameSharedInfo，用于存储全局游戏共享信息
public struct GameSharedInfo
{
    public int deadCounter; // 记录死亡计数
    public float spawnInterval; // 记录生成敌人的间隔时间
    public int spawnCount; // 记录生成敌人的数量
    public bool playHitAudio; // 标识是否播放击中音效
    public double playHitAudioTime; // 记录播放击中音效的时间
}
