using Unity.Entities;
using UnityEngine;

/// <summary>
/// 定义 GameManagerAuthoring 类，继承自 MonoBehaviour
/// </summary>
public class GameManagerAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab; // 定义公共变量 bulletPrefab，类型为 GameObject
    public GameObject enemyPrefab; // 定义公共变量 enemyPrefab，类型为 GameObject

    /// <summary>
    /// 定义嵌套类 GameManagerBaker，继承自 Baker<GameManagerAuthoring>
    /// </summary>
    public class GameManagerBaker : Baker<GameManagerAuthoring> 
    {
        public override void Bake(GameManagerAuthoring authoring) // 重写 Bake 方法，参数为 GameManagerAuthoring
        {
            Entity entity = GetEntity(TransformUsageFlags.None); // 获取实体，不使用变换标志
            GameConfigData configData = new GameConfigData(); // 创建新的 GameConfigData 实例
            configData.bulletPortotype = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic); // 获取 bulletPrefab 对应的实体，使用动态变换标志
            configData.enemyPortotype = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic); // 获取 enemyPrefab 对应的实体，使用动态变换标志
            AddComponent<GameConfigData>(entity, configData); // 为实体添加 GameConfigData 组件
        }
    }
}
