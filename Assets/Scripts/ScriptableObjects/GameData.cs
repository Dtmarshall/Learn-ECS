using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

[CreateAssetMenu(fileName = "Game Data")]
public class GameData : ScriptableObject
{
    public PrefabInitData prefabInitData;

    [Header("Boid")]
    public ComputeShader compute;
    public int boidCount;

    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float maxSteerForce = 3;

    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float seperateWeight = 1;

    public float targetWeight = 1;

    [Header("Boid Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;

    // public static GameData LoadFromAssets() => Resources.Load("Data/Game Data") as GameData;
}