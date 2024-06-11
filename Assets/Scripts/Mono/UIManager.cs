using UnityEngine;
using UnityEngine.UI;

// ���� UIManager �࣬�̳��� MonoBehaviour
public class UIManager : MonoBehaviour
{
    private int score; // ����˽�б��� score�����ڴ洢��ǰ�÷�
    public Text socreText; // ���б����������� Unity �༭����ָ�� UI Text ���

    // Start �����ڵ�һ�ε��� Update ����ǰ����
    void Start()
    {
        // ��ʼ���߼����Է������ĿǰΪ��
    }

    // Update ������ÿ֡����
    void Update()
    {
        // �����ǰ�÷��빲�������е����������������
        if (score != SharedData.GameSharedInfo.Data.deadCounter)
        {
            // ���µ÷�
            score = SharedData.GameSharedInfo.Data.deadCounter;
            // ���� UI �ı���ʾ�÷�
            socreText.text = score.ToString();
        }
    }
}
