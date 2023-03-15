using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Components;

namespace Systems
{
    public class DanceSystem : IEcsInitSystem, IEcsRunSystem
    {
        const int threadGroupSize = 1024;
        const int numViewDirections = 100;

        EcsWorld world;
        EcsFilter filter;

        GameData gameData;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<Boid>().End();

            gameData = systems.GetShared<GameData>();
        }

        public void Run(IEcsSystems systems)
        {
            var spawnPrefabPool = world.GetPool<Boid>();

            int numBoids = filter.GetEntitiesCount();

            foreach (var entity in filter)
            {
                ref var boid = ref spawnPrefabPool.Get(entity);

                int numFlockmates = 0;
                Vector3 flockHeading = Vector3.zero;
                Vector3 flockCentre = Vector3.zero;
                Vector3 separationHeading = Vector3.zero;

                foreach (var entityB in filter)
                {
                    ref var boidB = ref spawnPrefabPool.Get(entityB);
                    if (boidB.transform != boid.transform)
                    {
                        Vector3 offset = boidB.position - boid.position;
                        float sqrDst = offset.x * offset.x + offset.y * offset.y + offset.z * offset.z;

                        if (sqrDst < gameData.perceptionRadius * gameData.perceptionRadius)
                        {
                            numFlockmates += 1;
                            flockHeading += boidB.forward;
                            flockCentre += boidB.position;

                            if (sqrDst < gameData.avoidanceRadius * gameData.avoidanceRadius)
                            {
                                separationHeading -= offset / sqrDst;
                            }
                        }
                    }
                }

                boid.avgFlockHeading = flockHeading;
                boid.centreOfFlockmates = flockCentre;
                boid.avgAvoidanceHeading = separationHeading;
                boid.numPerceivedFlockmates = numFlockmates;

                UpdateBoid(ref boid);
            }
        }

        public void UpdateBoid(ref Boid boid)
        {
            Vector3 acceleration = Vector3.zero;

            if (boid.target != null)
            {
                Vector3 offsetToTarget = (boid.target.position - boid.position);
                acceleration = SteerTowards(ref boid, offsetToTarget) * gameData.targetWeight;
            }

            if (boid.numPerceivedFlockmates != 0)
            {
                boid.centreOfFlockmates /= boid.numPerceivedFlockmates;

                Vector3 offsetToFlockmatesCentre = (boid.centreOfFlockmates - boid.position);

                var alignmentForce = SteerTowards(ref boid, boid.avgFlockHeading) * gameData.alignWeight;
                var cohesionForce = SteerTowards(ref boid, offsetToFlockmatesCentre) * gameData.cohesionWeight;
                var seperationForce = SteerTowards(ref boid, boid.avgAvoidanceHeading) * gameData.seperateWeight;

                acceleration += alignmentForce;
                acceleration += cohesionForce;
                acceleration += seperationForce;
            }

            if (IsHeadingForCollision(ref boid))
            {
                Vector3 collisionAvoidDir = ObstacleRays(ref boid);
                Vector3 collisionAvoidForce = SteerTowards(ref boid, collisionAvoidDir) * gameData.avoidCollisionWeight;
                acceleration += collisionAvoidForce;
            }

            boid.velocity += acceleration * Time.deltaTime;
            float speed = boid.velocity.magnitude;
            Vector3 dir = boid.velocity / speed;
            speed = Mathf.Clamp(speed, gameData.minSpeed, gameData.maxSpeed);
            boid.velocity = dir * speed;

            boid.transform.position += boid.velocity * Time.deltaTime;
            boid.transform.forward = dir;
            boid.position = boid.transform.position;
            boid.forward = dir;
        }

        bool IsHeadingForCollision(ref Boid boid)
        {
            RaycastHit hit;
            if (Physics.SphereCast(boid.position, gameData.boundsRadius, boid.forward, out hit, gameData.collisionAvoidDst, gameData.obstacleMask))
            {
                return true;
            }
            else { }
            return false;
        }

        Vector3 ObstacleRays(ref Boid boid)
        {
            Vector3[] rayDirections = BoidHelper();
            for (int i = 0; i < rayDirections.Length; i++)
            {
                Vector3 dir = boid.transform.TransformDirection(rayDirections[i]);
                Ray ray = new Ray(boid.position, dir);
                if (!Physics.SphereCast(ray, gameData.boundsRadius, gameData.collisionAvoidDst, gameData.obstacleMask))
                {
                    return dir;
                }
            }

            return boid.forward;
        }

        Vector3 SteerTowards(ref Boid boid, Vector3 vector)
        {
            Vector3 v = vector.normalized * gameData.maxSpeed - boid.velocity;
            return Vector3.ClampMagnitude(v, gameData.maxSteerForce);
        }

        static Vector3[] BoidHelper()
        {
            Vector3[] directions = new Vector3[numViewDirections];

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < numViewDirections; i++)
            {
                float t = (float)i / numViewDirections;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                directions[i] = new Vector3(x, y, z);
            }

            return directions;
        }

        public struct BoidData
        {
            public Vector3 position;
            public Vector3 direction;

            public Vector3 flockHeading;
            public Vector3 flockCentre;
            public Vector3 avoidanceHeading;
            public int numFlockmates;

            public static int Size
            {
                get
                {
                    return sizeof(float) * 3 * 5 + sizeof(int);
                }
            }
        }
    }
}