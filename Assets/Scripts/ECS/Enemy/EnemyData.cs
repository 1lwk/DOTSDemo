using Unity.Entities;

// ���� EnemyData �ṹ�壬ʵ�� IComponentData �� IEnableableComponent �ӿ�
public struct EnemyData : IComponentData, IEnableableComponent
{
    public bool die; // ���幫���������� die����ʾ�����Ƿ�����
}
