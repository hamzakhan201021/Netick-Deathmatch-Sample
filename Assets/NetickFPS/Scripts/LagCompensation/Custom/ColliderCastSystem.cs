using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PG.LagCompensation
{

    /// <summary>
    /// Used to simulate moving all HitCollider components to a position in the past and cast a ray at them
    /// </summary>
    public class ColliderCastSystem
    {

        //public static List<HitCollider> SimulationObjects = new List<HitCollider>();
        //public static List<int> Framekeys = new List<int>();


        public static List<HitColliderCollection> SimulationObjects = new List<HitColliderCollection>();




        #region New

        /// <summary>
        /// Check current transform. Cast against all HitColliders in the scene
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="range"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool ColliderCastTransform(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit, out HitColliderCollection collection, out int hitColliderIndex)
        {
            hit = ColliderCastHit.Zero;
            collection = null;
            hitColliderIndex = -1;

            for (int i = 0; i < SimulationObjects.Count; i++)
            {

                if (SimulationObjects[i].CheckBoundingSphereTransform(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                {
                    if (SimulationObjects[i].CheckBoundingSphereDistanceTransform(origin, direction, range))
                    {
                        if (SimulationObjects[i].ColliderCastTransform(origin, direction, range, out ColliderCastHit newHit, out int newHitColliderIndex))
                        {
                            if (newHit.entryDistance < hit.entryDistance)
							{
                                collection = SimulationObjects[i];
                                hit = newHit;
                                hitColliderIndex = newHitColliderIndex;
                            }
                                
                        }
                    }
                }

            }


            return hit.entryDistance != Mathf.Infinity;
        }

        /// <summary>
        /// Check collision against cached transforms. Cast against all HitColliders in the scene
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="range"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool ColliderCastTransformWithExclusion(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit, out HitColliderCollection collection, out int hitColliderIndex, HitColliderCollection exclude, bool useCachedTransforms)
        {
            hit = ColliderCastHit.Zero;
            collection = null;
            hitColliderIndex = -1;

            if (useCachedTransforms)
			{
                for (int i = 0; i < SimulationObjects.Count; i++)
                {
                    if (SimulationObjects[i] == exclude) // skip this one
                        continue;

                    if (SimulationObjects[i].CheckBoundingSphereCached(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                    {
                        if (SimulationObjects[i].CheckBoundingSphereDistanceCached(origin, direction, range))
                        {
                            // New function call.
                            //SimulationObjects[i].ORIGINALSimulateFully(); // cache the locations/rotations of all managed hitColliders (if it hasn't been done already)
                            SimulationObjects[i].SimulateFully();

                            if (SimulationObjects[i].ColliderCastInterpolatedFrameData(origin, direction, range, out ColliderCastHit newHit, out int newHitColliderIndex))
                            {
                                if (newHit.entryDistance < hit.entryDistance)
                                {
                                    collection = SimulationObjects[i];
                                    hit = newHit;
                                    hitColliderIndex = newHitColliderIndex;
                                }
                            }
                        }
                    }

                }
            }
            else
			{
                for (int i = 0; i < SimulationObjects.Count; i++)
                {
                    if (SimulationObjects[i] == exclude) // skip this one
                        continue;

                    if (SimulationObjects[i].CheckBoundingSphereTransform(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                    {
                        if (SimulationObjects[i].CheckBoundingSphereDistanceTransform(origin, direction, range))
                        {
                            if (SimulationObjects[i].ColliderCastTransform(origin, direction, range, out ColliderCastHit newHit, out int newHitColliderIndex))
                            {
                                if (newHit.entryDistance < hit.entryDistance)
                                {
                                    collection = SimulationObjects[i];
                                    hit = newHit;
                                    hitColliderIndex = newHitColliderIndex;
                                }

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
        public static bool ColliderCastInterpolatedFrameData(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit)
        {
            hit = ColliderCastHit.Zero;

            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                if (SimulationObjects[i].CheckBoundingSphereCached(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                {
                    if (SimulationObjects[i].CheckBoundingSphereDistanceCached(origin, direction, range))
                    {
                        // New function
                        //SimulationObjects[i].ORIGINALSimulateFully(); // cache the locations/rotations of all managed hitColliders (if it hasn't been done already)
                        SimulationObjects[i].SimulateFully();

                        if (SimulationObjects[i].ColliderCastInterpolatedFrameData(origin, direction, range, out ColliderCastHit newHit, out int newHitColliderIndex))
                        {
                            if (newHit.entryDistance < hit.entryDistance)
							{
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
        /// <returns></returns>
        public static bool ColliderCastFromCachedData(Vector3 origin, Vector3 direction, float range, out ColliderCastHit hit, out HitColliderCollection hitCollection, out int hitColliderIndex)
        {
            hit = ColliderCastHit.Zero;
            hitColliderIndex = -1;
            hitCollection = null;

            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                if (SimulationObjects[i].CheckBoundingSphereCached(origin, direction)) // CheckBoundingSphere   //CheckBoundingSphereAtTestPosition
                {
                    if (SimulationObjects[i].CheckBoundingSphereDistanceCached(origin, direction, range))
                    {
                        // New function
                        //SimulationObjects[i].ORIGINALSimulateFully(); // cache the locations/rotations of all managed hitColliders (if it hasn't been done already)
                        SimulationObjects[i].SimulateFully();

                        if (SimulationObjects[i].ColliderCastInterpolatedFrameData(origin, direction, range, out ColliderCastHit newHit, out int newHitColliderIndex))
                        {
                            if (newHit.entryDistance < hit.entryDistance)
                            {
                                hit = newHit;

                                // Send this info too
                                hitColliderIndex = newHitColliderIndex;
                                hitCollection = SimulationObjects[i];
                            }
                        }
                    }
                }

            }


            return hit.entryDistance != Mathf.Infinity;
        }


        /// <summary>
        /// At first only simulate the collection (which is a large sphere collider acting as the bounding sphere for all managed colliders)
        /// </summary>
        /// <param name="simulationTime"></param>
        public static void ORIGINALSimulate(double simulationTime)
        {
            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].SetSimulationTime = simulationTime;
                SimulationObjects[i].ORIGINALCacheInterpolationPositionRotation(simulationTime);
            }

        }

        /// <summary>
        /// At first only simulate the collection (which is a large sphere collider acting as the bounding sphere for all managed colliders)
        /// </summary>
        /// <param name="tick"></param>
        public static void Simulate(int tick)
        {
            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].SetSimulationTick = tick;
                SimulationObjects[i].CachePositionRotation(tick);
            }
        }

        /// <summary>
        /// Draw the colliders at their current positions
        /// </summary>
        public static void DebugDrawColliders()
        {
            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].DebugDrawAllColliders();
            }

        }

        #endregion





    }


}