using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterAuthoring : MonoBehaviour
{
    public float hp=100;
    public float moveSpeed = 1;
    public float createbulletInterval = 1;

    
    public List<int> skills=new List<int>();

    public class MonsterBaker:Baker<MonsterAuthoring>
    {
        public override void Bake(MonsterAuthoring authoring)
        {
            Entity monsterentity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MonsterData>(monsterentity, new MonsterData()
            {
                hp = authoring.hp,
            });
            SetComponentEnabled<MonsterData>(monsterentity, true);

            AddSharedComponent<MonsterConfig>(monsterentity, new MonsterConfig()
            {
                createbulletInterval = authoring.createbulletInterval
            });
            AddComponent<MoveData>(monsterentity, new MoveData()
            {
                moveSpeed = authoring.moveSpeed
            });
            AddBuffer<Skill>(monsterentity);
            for (int i = 0; i < authoring.skills.Count; i++)
            {
                AppendToBuffer<Skill>(monsterentity, new Skill() { id = authoring.skills[i] }) ;
            }
        }
    }
}
