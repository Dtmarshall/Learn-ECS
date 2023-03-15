using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Systems;

class Startup : MonoBehaviour
{
    public GameData gameData;

    EcsWorld _world;
    IEcsSystems _systems;

    void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world, gameData)
        .Add(new SpawnSystem())
        .Add(new DanceSystem());

        _systems.Init();
    }

    void FixedUpdate()
    {
        _systems?.Run();
    }

    void OnDestroy()
    {
        if (_systems != null)
        {
            _systems.Destroy();
            _systems = null;
        }
        
        if (_world != null)
        {
            _world.Destroy();
            _world = null;
        }
    }
}
