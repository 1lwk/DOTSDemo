using UnityEngine;

// 定义 CameraController 类，继承自 MonoBehaviour
public class CameraController : MonoBehaviour
{
    public Transform target; // 定义目标 Transform，用于跟随目标
    public Vector3 offset; // 定义相机的偏移量
    public float smooth; // 定义平滑系数
    private Vector3 velocity; // 定义速度向量，用于平滑运动
    public Vector2 xRange; // 定义 x 轴的移动范围
    public Vector2 yRange; // 定义 y 轴的移动范围

    // Start 方法在脚本第一次实例化时调用
    void Start()
    {
        // 此处没有初始化代码
    }

    // Update 方法在每帧调用
    void Update()
    {
        if (target != null) // 检查目标是否存在
        {
            // 计算相机新的位置，使用 Vector3.SmoothDamp 方法实现平滑跟随
            Vector3 pos = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, Time.deltaTime * smooth);
            SetPosition(pos); // 调用 SetPosition 方法设置相机位置
        }
    }

    // SetPosition 方法用于设置相机位置，并进行范围限制
    private void SetPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, xRange.x, xRange.y); // 限制 x 轴范围
        pos.y = Mathf.Clamp(pos.y, yRange.x, yRange.y); // 限制 y 轴范围
        pos.z -= 10; // 调整 z 轴位置，保持相机距离
        transform.position = pos; // 设置相机位置
    }
}
