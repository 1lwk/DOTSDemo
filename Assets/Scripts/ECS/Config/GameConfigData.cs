using Unity.Entities;

// ���� GameConfigData �ṹ�壬ʵ�� IComponentData �ӿ�
public struct GameConfigData : IComponentData
{
    public Entity bulletPortotype; // ���幫������ bulletPortotype������Ϊ Entity����ʾ�ӵ���ԭ��ʵ��
    public Entity enemyPortotype; // ���幫������ enemyPortotype������Ϊ Entity����ʾ���˵�ԭ��ʵ��
}
