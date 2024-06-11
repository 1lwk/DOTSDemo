using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterManagerAuthoring : MonoBehaviour
{
    //public float moveSpeed = 1;
    public GameObject bulletPrefab;

    public class MonsterManagerBaker : Baker<MonsterManagerAuthoring>
    {
        public override void Bake(MonsterManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<GameConfig>(entity, new GameConfig()
            {
                bulletPrototype = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
