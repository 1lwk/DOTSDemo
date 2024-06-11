using Unity.Entities;
using Unity.Mathematics;

// ���� BulletSharedData �ṹ�壬ʵ�� ISharedComponentData �ӿ�
public struct BulletSharedData : ISharedComponentData
{
    public float moveSpeed; // ���幫��������� moveSpeed����ʾ�ӵ����ƶ��ٶ�
    public float destroyTimer; // ���幫��������� destroyTimer����ʾ�ӵ����ٵļ�ʱ��
    public float2 colloderOffset; // ���幫������ colloderOffset������Ϊ float2����ʾ��ײ����ƫ��
    public float3 colliderHalfExtents; // ���幫������ colliderHalfExtents������Ϊ float3����ʾ��ײ���İ�ߴ�
}
