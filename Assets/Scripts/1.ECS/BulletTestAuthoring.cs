using Unity.Entities;
using UnityEngine;

public class BulletTestAuthoring : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotationSpeed = 1;

    public class BulletAuthoringBaker : Baker<BulletTestAuthoring>
    {
        public override void Bake(BulletTestAuthoring authoring)
        {
            Entity bulletentity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveData>(bulletentity, new MoveData()
            {
                moveSpeed = authoring.moveSpeed
            });

            AddComponent<MoveTag>(bulletentity);
        }
    }
}
