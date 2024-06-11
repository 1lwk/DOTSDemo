using Unity.Entities;
using Unity.Mathematics;

// ���� BulletCreateInfo �ṹ�壬ʵ�� IBufferElementData �ӿ�
public struct BulletCreateInfo : IBufferElementData
{
    public float3 position; // ���幫������ position������Ϊ float3����ʾ�ӵ���λ��
    public quaternion rotation; // ���幫������ rotation������Ϊ quaternion����ʾ�ӵ�����ת
}
