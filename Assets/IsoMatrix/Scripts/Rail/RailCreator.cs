using System;
using System.Collections.Generic;
using System.Linq;
using ADN.Meta.Core;
using Cinemachine;
using IsoMatrix.Scripts.Data;
using IsoMatrix.Scripts.Train;
using IsoMatrix.Scripts.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IsoMatrix.Scripts.Rail
{
    public class RailCreator : MonoBehaviour, IEventListener<TrainActionEvent>
    {
        private TrainPath pathTrain_1;
        public TrainPath StartRail;
        public GameObject PathContainer;
        public TrainController train;
        public TrainManager TrainManager;
        private  List<TrainPath.Waypoint> generatedWaypoints = new List<TrainPath.Waypoint>();
        private TrainPath.Waypoint[] _defaultWaypoint = new TrainPath.Waypoint[]{};
        private List<TrainPath> _listRails = new List<TrainPath>();
        private TrainPath ChechWaypoint;
        private float _defaultSpeed;
        private int indexCheck = 1;
        private bool canRun;
        private int maxPoint = 150;

        private void Start()
        {
            EventManager.Subscribe(this);
            pathTrain_1 = gameObject.GetComponent<TrainPath>();
            _defaultWaypoint = pathTrain_1.m_Waypoints;
            _defaultSpeed = train.m_Speed;
            StartPoint();
            UpdateRail();
            // TrainManager = train.gameObject.GetComponent<TrainManager>();

            // CinemachinePath.Waypoint wp = pathTrain_1.m_Waypoints[1];
            // var pos1= pathTrain_1.transform.localRotation * wp.position + pathTrain_1.transform.localPosition;
            // var tangent1 = pathTrain_1.transform.localRotation * wp.tangent;
            //
            // CinemachinePath.Waypoint wp2 = StartRail.m_Waypoints[0];
            // var pos2= StartRail.transform.localRotation * wp2.position + StartRail.transform.localPosition;
            // var tangent2 = StartRail.transform.localRotation * wp2.tangent;
            //
            // Debug.Log(pos1+ "||"+pos2);
            // Debug.Log(tangent1+ "||"+tangent2);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(this);
        }

        public void RespawnTrain()
        {
            if (TrainManager.transform.parent != TrainManager.DefaultParent)
            {
                TrainManager.transform.parent = TrainManager.DefaultParent;
            }
            train.RespawnDefault();
            train.m_Speed = _defaultSpeed;
            StartPoint();
            train.UpdateStateArrowIndicator();
            UpdateRail();
        }

        private void StartPoint()
        {
            indexCheck = 1;
            generatedWaypoints.Clear();
            AddWaypoint(StartRail, 0, true);
            AddWaypoint(StartRail, 1);
            ChechWaypoint = StartRail;
        }

        public void DragStopped()
        {
            GetAllRail();
            TrainWayFindind();
            Invoke(nameof(Run), 0f);
        }

        public void UpdateRail()
        {
            GetAllRail();
            TrainWayFindind();
            train.UpdateStateArrowIndicator();
        }

        public void GetAllRail()
        {
            _listRails.Clear();
            foreach (Transform child in PathContainer.transform)
            {
                TrainPath trainPath = child.GetComponent<TrainPath>();
                if (trainPath)
                {
                    _listRails.Add(trainPath);
                }
            }
        }

        public void TrainWayFindind()
        {
            bool oldStage = train.canRun;
            train.canRun = false;
            TrainPath currentCinePath = gameObject.GetComponent<TrainPath>();
            if (currentCinePath)
            {
                TrainPath.Waypoint[] oldWaypoints = currentCinePath.m_Waypoints;
                float pos = currentCinePath.GetBoundingIndices(train.m_Position, out var indexA, out var indexB);
                GetRailCheck(oldWaypoints[indexA].position, oldWaypoints[indexB].position);
                generatedWaypoints.Clear();
                generatedWaypoints.AddRange(oldWaypoints.Take(indexB+1));
                // AddWaypoint(ChechWaypoint, indexCheck);
            }
            RailCheck();
            if (generatedWaypoints.Count>2)
            {
                canRun = true;
            }

            TrainPath.Waypoint[] cine = new TrainPath.Waypoint[generatedWaypoints.Count];
            cine = generatedWaypoints.ToArray();
            train.canRun = oldStage;
            if (!currentCinePath)
            {
                pathTrain_1 = gameObject.AddComponent<TrainPath>();
                pathTrain_1.m_Waypoints =cine;
                train.m_Path = pathTrain_1;
            }
            else
            {
                TrainPath.Waypoint[] oldWaypoints = currentCinePath.m_Waypoints;
                if (oldWaypoints.SequenceEqual(cine))
                {
                    return;
                }

                
                currentCinePath.m_Waypoints =cine;
                currentCinePath.InvalidateDistanceCache();
                // float positionClosestPoint = currentCinePath.FindClosestPoint(train.transform.position, 10, -1, 10);
                // train.m_Position = positionClosestPoint;
            }
        }

        public void Run()
        {
            if (canRun)
            {
                train.canRun = true;
                canRun = false;
                TrainManager.StartRun();
            }
        }

        public void GetRailCheck(Vector3 posA , Vector3 posB)
        {
            foreach (var rail in _listRails)
            {
                TrainPath.Waypoint startWP = rail.m_Waypoints[0];
                var pos2= rail.transform.localRotation * startWP.position + rail.transform.localPosition;
                TrainPath.Waypoint endWP = rail.m_Waypoints[1];
                var pos3= rail.transform.localRotation * endWP.position + rail.transform.localPosition;
                Vector3 pos4 = Vector3.zero;
                Vector3 pos5 = Vector3.zero;
                Vector3 pos6 = Vector3.zero;

                if (rail.m_Waypoints.Length>2)
                {
                    TrainPath.Waypoint moreWP = rail.m_Waypoints[2];
                    pos4= rail.transform.localRotation * moreWP.position + rail.transform.localPosition;
                    TrainPath.Waypoint moreWP2 = rail.m_Waypoints[4];
                    pos5= rail.transform.localRotation * moreWP2.position + rail.transform.localPosition;
                    TrainPath.Waypoint moreWP3 = rail.m_Waypoints[3];
                    pos6= rail.transform.localRotation * moreWP3.position + rail.transform.localPosition;
                    if (pos4 == posA && pos3 == posB)
                    {
                        ChechWaypoint = rail;
                        indexCheck = 1;
                        return;
                    }else  if (pos2 == posA && pos3 == posB)
                    {
                        ChechWaypoint = rail;
                        indexCheck = 1;
                        return;
                    }else  if (pos5 == posA && pos6 == posB)
                    {
                        ChechWaypoint = rail;
                        indexCheck = 3;
                        return;
                    }
                }
                else
                {
                    if (pos2 == posA && pos3 == posB)
                    {
                        ChechWaypoint = rail;
                        indexCheck = 1;
                        return;
                    }
                    if (pos3 == posA && pos2 == posB)
                    {
                        ChechWaypoint = rail;
                        indexCheck = 0;
                        return;
                    }
                }
            }
        } 

        public void RailCheck()
        {
            if ( generatedWaypoints.Count < maxPoint)
            {
                if (indexCheck >= ChechWaypoint.m_Waypoints.Length)
                {
                    return;
                }
                TrainPath.Waypoint wp = ChechWaypoint.m_Waypoints[indexCheck];
                var pos1= ChechWaypoint.transform.localRotation * wp.position + ChechWaypoint.transform.localPosition;
                foreach (var rail in _listRails)
                {
                    TrainPath.Waypoint startWP = rail.m_Waypoints[0];
                    var pos2= rail.transform.localRotation * startWP.position + rail.transform.localPosition;
                    TrainPath.Waypoint endWP = rail.m_Waypoints[1];
                    var pos3= rail.transform.localRotation * endWP.position + rail.transform.localPosition;
                    Vector3 pos4 = Vector3.zero;
                    Vector3 pos5 = Vector3.zero;
                    Vector3 pos6 = Vector3.zero;
                    if (ChechWaypoint.transform.localPosition !=  rail.transform.localPosition)
                    {
                        if (rail.m_Waypoints.Length>2)
                        {
                            TrainPath.Waypoint moreWP = rail.m_Waypoints[2];
                            pos4= rail.transform.localRotation * moreWP.position + rail.transform.localPosition;
                            TrainPath.Waypoint moreWP2 = rail.m_Waypoints[4];
                            pos5= rail.transform.localRotation * moreWP2.position + rail.transform.localPosition;
                            if (pos4 == pos1)
                            {
                                AddWaypoint(rail, 1);
                                // _listRails.Add(CurrentChechWaypoint);
                                // CurrentChechWaypoint = ChechWaypoint;
                                ChechWaypoint = rail;
                                indexCheck = 1;
                                RailCheck();
                                return;
                            }else  if (pos2 == pos1)
                            {
                                AddWaypoint(rail, 1);
                                ChechWaypoint = rail;
                                indexCheck = 1;
                                RailCheck();
                                return;
                            }else  if (pos5 == pos1)
                            {
                                AddWaypoint(rail, 3);
                                ChechWaypoint = rail;
                                indexCheck = 3;
                                RailCheck();
                                return;
                            }
                        }
                        else
                        {
                            if (pos2 == pos1)
                            {
                                AddWaypoint(rail, 1);
                                ChechWaypoint = rail;
                                indexCheck = 1;
                                RailCheck();
                                return;
                            }else if (pos3 == pos1)
                            {
                                AddWaypoint(rail, 0);
                                ChechWaypoint = rail;
                                indexCheck = 0;
                                RailCheck();
                                return;
                            }
                        }
                    }
                }
            }
        }

        void AutoMove(TrainPath rail, Vector3 pos1)
        {
            Vector2Int indexPoint = GameConstant.TOP_POINT;
            TrainPath.Waypoint moreWP1 = rail.m_Waypoints[indexPoint.x];
            Vector3 pos4= moreWP1.position + rail.transform.localPosition;
            
            TrainPath.Waypoint moreWP2 = rail.m_Waypoints[indexPoint.y];
            Vector3 pos5= moreWP2.position + rail.transform.localPosition;
            if (pos4 == pos1)
            {
                AddWaypoint(rail, indexPoint.y);
                ChechWaypoint = rail;
                indexCheck = indexPoint.y;
                RailCheck();
                return;
            }else if (pos5 == pos1)
            {
                AddWaypoint(rail, indexPoint.x);
                ChechWaypoint = rail;
                indexCheck = indexPoint.x;
                RailCheck();
                return;
            }
            indexPoint = GameConstant.RIGHT_POINT;
            moreWP1 = rail.m_Waypoints[indexPoint.x];
            pos4= moreWP1.position + rail.transform.localPosition;
        
            moreWP2 = rail.m_Waypoints[indexPoint.y];
            pos5= moreWP2.position + rail.transform.localPosition;
            if (pos4 == pos1)
            {
                AddWaypoint(rail, indexPoint.y);
                ChechWaypoint = rail;
                indexCheck = indexPoint.y;
                RailCheck();
                return;
            }else if (pos5 == pos1)
            {
                AddWaypoint(rail, indexPoint.x);
                ChechWaypoint = rail;
                indexCheck = indexPoint.x;
                RailCheck();
                return;
            }
        }
        void AddWaypoint(TrainPath child, int idx, bool start = false)
        {
            if(!child.GetComponent<TrainPath>()) return;
            TrainPath childTrainPath = child.GetComponent<TrainPath>();
            if (idx >= childTrainPath.m_Waypoints.Length)
            {
                return;
            }
            TrainPath.Waypoint wp = childTrainPath.m_Waypoints[idx];
            TrainPath.Waypoint targetWP = new TrainPath.Waypoint();
            targetWP.position = child.transform.localRotation * wp.position + child.transform.localPosition;
            var changeVar = 1;
            if (!start)
            {
                if (idx == 0 || idx == 3)
                {
                    changeVar = -1;
                }
            }
            targetWP.tangent = child.transform.localRotation * wp.tangent * changeVar;
            targetWP.roll = wp.roll;
            generatedWaypoints.Add(targetWP);
        }

        public void OnEventTriggered(TrainActionEvent e)
        {
            if (e.type == TrainActionEventType.Run)
            {
                train.canRun = true;
                TrainManager.StartRun();
            }
            if (e.type == TrainActionEventType.Update)
            {
                UpdateRail();
            }

            if (e.type == TrainActionEventType.Reset)
            {
                // train.canRun = !train.canRun;
                RespawnTrain();
            }
        }
    }
}
