using UnityEngine;

// ���� CameraController �࣬�̳��� MonoBehaviour
public class CameraController : MonoBehaviour
{
    public Transform target; // ����Ŀ�� Transform�����ڸ���Ŀ��
    public Vector3 offset; // ���������ƫ����
    public float smooth; // ����ƽ��ϵ��
    private Vector3 velocity; // �����ٶ�����������ƽ���˶�
    public Vector2 xRange; // ���� x ����ƶ���Χ
    public Vector2 yRange; // ���� y ����ƶ���Χ

    // Start �����ڽű���һ��ʵ����ʱ����
    void Start()
    {
        // �˴�û�г�ʼ������
    }

    // Update ������ÿ֡����
    void Update()
    {
        if (target != null) // ���Ŀ���Ƿ����
        {
            // ��������µ�λ�ã�ʹ�� Vector3.SmoothDamp ����ʵ��ƽ������
            Vector3 pos = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, Time.deltaTime * smooth);
            SetPosition(pos); // ���� SetPosition �����������λ��
        }
    }

    // SetPosition ���������������λ�ã������з�Χ����
    private void SetPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, xRange.x, xRange.y); // ���� x �᷶Χ
        pos.y = Mathf.Clamp(pos.y, yRange.x, yRange.y); // ���� y �᷶Χ
        pos.z -= 10; // ���� z ��λ�ã������������
        transform.position = pos; // �������λ��
    }
}
