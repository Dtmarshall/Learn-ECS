using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Components;

namespace Systems
{
    class SpawnSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var gameData = systems.GetShared<GameData>();

            for (int i = 0; i < gameData.boidCount; i++)
            {
                int entity = world.NewEntity();

                var prefab = world.GetPool<Boid>();
                prefab.Add(entity);
                ref Boid prefabComponent = ref prefab.Get(entity);

                var spawnedPrefab = GameObject.Instantiate(gameData.prefabInitData.prefab, Random.insideUnitSphere * 2, Quaternion.identity);
                prefabComponent.prefab = spawnedPrefab;
                prefabComponent.transform = spawnedPrefab.transform;

                prefabComponent.position = spawnedPrefab.transform.position;
                prefabComponent.forward = spawnedPrefab.transform.forward;
                prefabComponent.target = null;

                float startSpeed = (gameData.minSpeed + gameData.maxSpeed) / 2;
                prefabComponent.velocity = spawnedPrefab.transform.forward * startSpeed;
            }
        }
    }
}