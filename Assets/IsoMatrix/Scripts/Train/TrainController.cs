using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using IsoMatrix.Scripts.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace IsoMatrix.Scripts.Train
{
    public class TrainController : MonoBehaviour
    {
        /// <summary>The path to follow</summary>
        [Tooltip("The path to follow")]
        public TrainPath m_Path;

        /// <summary>This enum defines the options available for the update method.</summary>
        public enum UpdateMethod
        {
            /// <summary>Updated in normal MonoBehaviour Update.</summary>
            Update,
            /// <summary>Updated in sync with the Physics module, in FixedUpdate</summary>
            FixedUpdate,
            /// <summary>Updated in normal MonoBehaviour LateUpdate</summary>
            LateUpdate
        };

        /// <summary>When to move the cart, if Velocity is non-zero</summary>
        [Tooltip("When to move the cart, if Velocity is non-zero")]
        public UpdateMethod m_UpdateMethod = UpdateMethod.Update;

        /// <summary>How to interpret the Path Position</summary>
        [Tooltip("How to interpret the Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
        public TrainPathBase.PositionUnits m_PositionUnits = TrainPathBase.PositionUnits.Distance;

        /// <summary>Move the cart with this speed</summary>
        [Tooltip("Move the cart with this speed along the path.  The value is interpreted according to the Position Units setting.")]
        [FormerlySerializedAs("m_Velocity")]
        public float m_Speed;

        /// <summary>The cart's current position on the path, in distance units</summary>
        [Tooltip("The position along the path at which the cart will be placed.  This can be animated directly or, if the velocity is non-zero, will be updated automatically.  The value is interpreted according to the Position Units setting.")]
        [FormerlySerializedAs("m_CurrentDistance")]
        public float m_Position;

        public bool canRun = false;

        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;
        private float _defaultMPosition;
        public GameObject arrow;

        private void Awake()
        {
            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation;
            _defaultMPosition = m_Position;
        }

        void FixedUpdate()
        {
            if (m_UpdateMethod == UpdateMethod.FixedUpdate)
                SetCartPosition(m_Position + m_Speed * Time.deltaTime);
        }

        void Update()
        {
            if (canRun)
            {
                float speed = Application.isPlaying ? m_Speed : 0;
                if (m_UpdateMethod == UpdateMethod.Update)
                    SetCartPosition(m_Position + speed * Time.deltaTime);
            }
        }

        void LateUpdate()
        {
            if (!Application.isPlaying)
                SetCartPosition(m_Position);
            else if (m_UpdateMethod == UpdateMethod.LateUpdate)
            {
                SetCartPosition(m_Position + m_Speed * Time.deltaTime);
                SetArrPosition(m_Position + 1 + m_Speed * Time.deltaTime);
            }

            // if (checkRemove)
            // {
            //     CheckRemoveRail();
            // }
        }

        private void SetArrPosition(float distanceAlongPath)
        {
            if (m_Path != null)
            {
                // Debug.Log(distanceAlongPath);
                m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
               
                // Debug.Log(m_Path.EvaluatePosition(m_Position));
                arrow.transform.localRotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
            }
        }

        void CheckRemoveRail()
        {
            // foreach (TrainPath.Waypoint wp in m_Path.m_Waypoints)
            // {
            //     Debug.Log(wp.position);
            //     
            // }
            Vector3 pointCheck = GetClosestVector(m_Path.m_Waypoints, transform.position);
            // Debug.Log(pointCheck);
            int index = 0;
            for (int i = 0; i < m_Path.m_Waypoints.Length; i++)
            {
                
                TrainPath.Waypoint wp = m_Path.m_Waypoints[i];
                if (wp.position.x == pointCheck.x && wp.position.z == pointCheck.z)
                {
                    if (i>0)
                    {
                        index = i-1;
                        // Debug.Log(index);
                    }
                    
                }
            }

            if (index>0)
            {
                List<TrainPath.Waypoint> waypoint = m_Path.m_Waypoints.ToList();
                waypoint.RemoveRange(0, index);
                m_Path.m_Waypoints = waypoint.ToArray();
                m_Path.InvalidateDistanceCache();
            }
        }
        
        public static Vector3 GetClosestVector(TrainPath.Waypoint[] waypoints, Vector3 posTrain)
        {
            Vector3 closestPoint = new Vector3(waypoints[0].position.x,waypoints[0].position.y, waypoints[0].position.z);
            float closestDistance = Vector3.Distance(closestPoint, posTrain);

            foreach (TrainPath.Waypoint point in waypoints)
            {
                Vector3 pointV3 = new Vector3(point.position.x,point.position.y, point.position.z);
                float distance = Vector3.Distance(pointV3, posTrain);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = pointV3;
                }
            }

            return closestPoint;
        }

        void SetCartPosition(float distanceAlongPath)
        {
            if (m_Path != null)
            {
                // Debug.Log(distanceAlongPath);
                m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
                transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
                // Debug.Log(m_Path.EvaluatePosition(m_Position));
                transform.rotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
            }
        }

        public void RespawnDefault()
        {
            canRun = false;
            // Debug.Log(DefaultTranform.position);
            transform.position = _defaultPosition;
            transform.rotation = _defaultRotation;
            m_Position = _defaultMPosition;
        }
    }
}
