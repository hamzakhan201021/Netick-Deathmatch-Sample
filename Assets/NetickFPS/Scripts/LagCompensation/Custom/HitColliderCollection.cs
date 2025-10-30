using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PG.LagCompensation
{

    public class HitColliderCollection : HitCollider
    {
        public float radius = 1f;

        /// <summary>
        /// All HitColliders managed by this
        /// </summary>
        [SerializeField]
        [Tooltip("Important: Should not contain this itself!")]
        private List<HitColliderGeneric> hitColliders = new List<HitColliderGeneric>();

        public HitColliderGeneric GetHitColliderAtIndex(int i)
		{
            /*
            if (i >= hitColliders.Count || i < 0)
			{
                Debug.LogError("Index " + i + " out of range for list with count " + hitColliders.Count + " (" + this.name + ")");
                return null;

            }
            */

            return hitColliders[i];
        }

        public List<HitColliderGeneric> GetHitColliderList => hitColliders;

        /// <summary>
        /// Time to simulate the position at
        /// </summary>
        private double simulationTime;

        private int simulationTick;

        /// <summary>
        /// Set 'simulationTime' to value and reset 'simulationTimeActive' to false
        /// </summary>
        public double SetSimulationTime { set { simulationTime = value; simulationTimeActive = false; } }

        public int SetSimulationTick { set { simulationTick = value; simulationTimeActive = false; } }

        // TODO remove this as it doesn't seem to be used.
        /// <summary>
        /// Have the interpoalted hitCollider postions/rotations been cached at time 'simulationTime'? Reset this bool whenever 'simulationTime' changes
        /// </summary>
        private bool simulationTimeActive;


        public override float GetBoundingSphereRadius => radius;
        public override float GetBoundingSphereRadiusSquared => radius * radius;

        // Use networking

        //void Start()
        //{
        //    ColliderCastSystem.SimulationObjects.Add(this);

        //}

        //private void OnDestroy()
        //{
        //    ColliderCastSystem.SimulationObjects.Remove(this);
        //}

        public override void NetworkStart()
        {
            base.NetworkStart();

            ColliderCastSystem.SimulationObjects.Add(this);
        }

        public override void NetworkDestroy()
        {
            ColliderCastSystem.SimulationObjects.Remove(this);
        }

        #region Collection

        [ContextMenu("Get all HitColliders")]
        private void GetAllHitColliders()
        {
            var _hitCols = transform.root.GetComponentsInChildren<HitColliderGeneric>();
            for (int i = 0; i < _hitCols.Length; i++)
            {
                hitColliders.Add(_hitCols[i]);
            }

#if UNITY_EDITOR
            // TODO add this to other context things.
            UnityEditor.EditorUtility.SetDirty(this);

#endif
        }

        /// <summary>
        /// Add postion/rotation with timestamp to list of the collection component as well as all components managed by this. Call this after doing movement updates!
        /// </summary>
        public void ORIGINALAddFrameAll(double _localTime)
        {
            ORIGINALAddFrame(_localTime);
            foreach (HitCollider hitCol in hitColliders)
            {
                hitCol.ORIGINALAddFrame(_localTime);
            }
        }

        /// <summary>
        /// Add pos rot with tick to the list of the collection itself and all managed by it, Call this after all movement updates!
        /// </summary>
        /// <param name="tick"></param>
        public void AddStateAll(int tick)
        {
            AddState(tick);

            foreach (HitCollider hitCol in hitColliders)
            {
                hitCol.AddState(tick);
            }
        }


        /// <summary>
        /// Simulate all managed hitColliders of this collection
        /// </summary>
        public void ORIGINALSimulateFully()
        {
            if (simulationTimeActive)
                return;

            for (int i = 0; i < hitColliders.Count; i++)
            {
                hitColliders[i].ORIGINALCacheInterpolationPositionRotation(simulationTime);
            }
            simulationTimeActive = true;
        }

        /// <summary>
        /// Simulate this collections managed hit colliders
        /// </summary>
        public void SimulateFully()
        {
            for (int i = 0; i < hitColliders.Count; i++)
            {
                hitColliders[i].CachePositionRotation(simulationTick);
            }
        }



        /// <summary>
        /// Cehck current transform. Cast against all HitColliders in the scene
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="range"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public bool ColliderCastTransform(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit, out int hitColliderIndex)
        {
            hit = ColliderCastHit.Zero;
            ColliderCastHit newHit;
            hitColliderIndex = -1;

            for (int i = 0; i < hitColliders.Count; i++)
            {

                if (hitColliders[i].CheckBoundingSphereTransform(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                {
                    if (hitColliders[i].CheckBoundingSphereDistanceTransform(origin, direction, range))
                    {
                        if (hitColliders[i].ColliderCast(origin, direction, range, out newHit))
                        {
                            if (newHit.entryDistance < hit.entryDistance)
							{
                                hitColliderIndex = i;
                                hit = newHit;
                            }
                                
                        }
                    }
                }

            }


            return hit.entryDistance != Mathf.Infinity;
        }


        /// <summary>
        /// Check cached postion/rotation. Cast against all HitColliders in the scene
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="range"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public bool ColliderCastInterpolatedFrameData(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit, out int hitColliderIndex)
        {

            hit = ColliderCastHit.Zero;
            ColliderCastHit newHit;
            hitColliderIndex = -1;

            for (int i = 0; i < hitColliders.Count; i++)
            {

                if (hitColliders[i].CheckBoundingSphereCached(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                {
                    if (hitColliders[i].CheckBoundingSphereDistanceCached(origin, direction, range))
                    {
                        if (hitColliders[i].ColliderCastCached(origin, direction, range, out newHit))
                        {
                            if (newHit.entryDistance < hit.entryDistance)
                            {
                                hitColliderIndex = i;
                                hit = newHit;
                            }
                        }
                    }
                }

            }


            return hit.entryDistance != Mathf.Infinity;
        }


        /// <summary>
        /// Draw the colliders at their cached positions
        /// </summary>
        public void DebugDrawAllColliders()
        {
            // Changed to use new function

            //ORIGINALSimulateFully();
            SimulateFully();

            for (int i = 0; i < hitColliders.Count; i++)
            {
                hitColliders[i].DebugDrawColliderCached(5f, Color.red);

                //hitColliders[i].DebugDrawBoundingSphereAtTestPosition(5f);
            }

        }

        /// <summary>
        /// Set cachedTransforms and debug draw afterwards
        /// </summary>
        /// <param name="_data"></param>
        public void SetCachedAndDebugDraw(TransformFrameData[] _data)
		{
            if (_data.Length != hitColliders.Count)
			{
                Debug.Log("Array Length mismatch: this collections has " + hitColliders.Count + " colliders but the received array has length " + _data.Length);
                return;
            }
                

			for (int i = 0; i < hitColliders.Count; i++)
			{
                hitColliders[i].SetCachedPosRot = _data[i];

                hitColliders[i].DebugDrawColliderCached(5f, Color.green);

                //hitColliders[i].DebugDrawBoundingSphereAtTestPosition(5f);

            }
        }

        #endregion

        #region Raycasting

        public override bool CheckBoundingSphereDistanceTransform(Vector3 o, Vector3 d, float range)
        {
            float closestDistance = GetTValueAlongLine(o, o + d, transform.TransformPoint(center));

            return closestDistance >= -GetBoundingSphereRadius && closestDistance <= range + GetBoundingSphereRadius; // minimum distance larger than negative radius!

        }

        public override bool CheckBoundingSphereDistanceCached(Vector3 o, Vector3 d, float range)
        {
            float closestDistance = GetTValueAlongLine(o, o + d, cachedPosRot.position + cachedPosRot.rotation * center);

            return closestDistance >= -GetBoundingSphereRadius && closestDistance <= range + GetBoundingSphereRadius; // minimum distance larger than negative radius!

        }

        #endregion

        #region Interpolation



        #endregion


        #region Debug Draw





        private void OnDrawGizmosSelected()
        {


            Gizmos.color = Color.yellow;

            DebugDrawSphere(transform.TransformPoint(center), transform.rotation, radius, 1f, Color.white, true);
        }

        #endregion
    }


}