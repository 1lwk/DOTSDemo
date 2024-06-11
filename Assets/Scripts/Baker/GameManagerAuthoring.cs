using Unity.Entities;
using UnityEngine;

/// <summary>
/// ���� GameManagerAuthoring �࣬�̳��� MonoBehaviour
/// </summary>
public class GameManagerAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab; // ���幫������ bulletPrefab������Ϊ GameObject
    public GameObject enemyPrefab; // ���幫������ enemyPrefab������Ϊ GameObject

    /// <summary>
    /// ����Ƕ���� GameManagerBaker���̳��� Baker<GameManagerAuthoring>
    /// </summary>
    public class GameManagerBaker : Baker<GameManagerAuthoring> 
    {
        public override void Bake(GameManagerAuthoring authoring) // ��д Bake ����������Ϊ GameManagerAuthoring
        {
            Entity entity = GetEntity(TransformUsageFlags.None); // ��ȡʵ�壬��ʹ�ñ任��־
            GameConfigData configData = new GameConfigData(); // �����µ� GameConfigData ʵ��
            configData.bulletPortotype = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic); // ��ȡ bulletPrefab ��Ӧ��ʵ�壬ʹ�ö�̬�任��־
            configData.enemyPortotype = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic); // ��ȡ enemyPrefab ��Ӧ��ʵ�壬ʹ�ö�̬�任��־
            AddComponent<GameConfigData>(entity, configData); // Ϊʵ����� GameConfigData ���
        }
    }
}
