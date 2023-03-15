using UnityEngine;

namespace Components
{
    public struct Boid
    {
        public GameObject prefab;
        public Transform transform;

		public Material material;
        public Transform target;

        public Vector3 position;
        public Vector3 forward;
        public Vector3 velocity;

        public Vector3 acceleration;
        public Vector3 avgFlockHeading;
        public Vector3 avgAvoidanceHeading;
        public Vector3 centreOfFlockmates;
        public int numPerceivedFlockmates;
    }
}