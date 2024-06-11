using Unity.Entities;
using Unity.Mathematics;

// ���� EnemySharedData �ṹ�壬ʵ�� ISharedComponentData �ӿ�
public struct EnemySharedData : ISharedComponentData
{
    public float moveSpeed; // ���幫��������� moveSpeed����ʾ���˵��ƶ��ٶ�
    public float2 scale; // ���幫������ scale������Ϊ float2����ʾ���˵����ű���
}
