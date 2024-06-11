using Unity.Entities;

// 定义 RendererSortTag 结构体，实现 IComponentData 和 IEnableableComponent 接口
public struct RendererSortTag : IComponentData, IEnableableComponent
{
    // 该结构体没有任何字段，仅用于标记实体，可以通过启用或禁用组件来控制渲染排序逻辑
}
