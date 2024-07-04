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
        private List<TrainPath> _listRails = new List<TrainPath>();
        private TrainPath ChechWaypoint;
        private int indexCheck = 1;
        private bool canRun;
        private int maxPoint = 100;

        private void Start()
        {
            EventManager.Subscribe(this);
            pathTrain_1 = gameObject.GetComponent<TrainPath>();
            StartPoint();
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
            indexCheck = 1;
            StartPoint();
            pathTrain_1.m_Waypoints =generatedWaypoints.ToArray();
            pathTrain_1.InvalidateDistanceCache();
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
            StartPoint();
            Invoke(nameof(GetAllRail), 0.1f);
            Invoke(nameof(TrainWayFindind), 0.2f);
            // GetAllRail();
            // TrainWayFindind();
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
            RailCheck();

            if (generatedWaypoints.Count>2)
            {
                canRun = true;
            }

            TrainPath.Waypoint[] cine = new TrainPath.Waypoint[generatedWaypoints.Count];
            cine = generatedWaypoints.ToArray();
            TrainPath currentCinePath = gameObject.GetComponent<TrainPath>();
            if (!currentCinePath)
            {
                pathTrain_1 = gameObject.AddComponent<TrainPath>();
                pathTrain_1.m_Waypoints =cine;
                train.m_Path = pathTrain_1;
            }
            else
            {
                currentCinePath.m_Waypoints =cine;
                currentCinePath.InvalidateDistanceCache();
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

        public void RailCheck()
        {
            restart:
            if ( generatedWaypoints.Count < maxPoint)
            {
                foreach (var rail in _listRails)
                {
                    TrainPath.Waypoint wp = ChechWaypoint.m_Waypoints[indexCheck];
                    var pos1= ChechWaypoint.transform.localRotation * wp.position + ChechWaypoint.transform.localPosition;

                    TrainPath.Waypoint startWP = rail.m_Waypoints[0];
                    var pos2= rail.transform.localRotation * startWP.position + rail.transform.localPosition;
                    TrainPath.Waypoint endWP = rail.m_Waypoints[1];
                    var pos3= rail.transform.localRotation * endWP.position + rail.transform.localPosition;
                    Vector3 pos4 = Vector3.zero;
                    Vector3 pos5 = Vector3.zero;
                    Vector2Int indexPoint = new Vector2Int(0, 1);
                    if (ChechWaypoint.transform.localPosition !=  rail.transform.localPosition)
                    {
                        if (rail.m_Waypoints.Length>2)
                        {
                            
                            RailGroupManager railGroupManager = rail.gameObject.GetComponent<RailGroupManager>();
                            if (railGroupManager)
                            {
                                RailType typeTrainSave = RailType.none;
                                foreach (var dir in railGroupManager.DirTrain)
                                {
                                    if (dir.Key.name == TrainManager.name )
                                    {
                                        typeTrainSave = dir.Value;
                                    }
                                }
                                switch (typeTrainSave)
                                {
                                    case RailType.top_right:
                                        indexPoint = GameConstant.TOP_RIGHT_POINT;
                                        break;
                                    case RailType.top_left:
                                        indexPoint = GameConstant.TOP_LEFT_POINT;
                                        break;
                                    case RailType.bottom_right:
                                        indexPoint = GameConstant.BOTTOM_RIGHT_POINT;
                                        break;
                                    case RailType.bottom_left:
                                        indexPoint = GameConstant.BOTTOM_LEFT_POINT;
                                        break;
                                    case RailType.right:
                                        indexPoint = GameConstant.RIGHT_POINT;
                                        break;
                                    case RailType.top:
                                        indexPoint = GameConstant.TOP_POINT;
                                        break;
                                }
                                TrainPath.Waypoint moreWP1 = rail.m_Waypoints[indexPoint.x];
                                pos4= rail.transform.localRotation * moreWP1.position + rail.transform.localPosition;
                                        
                                TrainPath.Waypoint moreWP2 = rail.m_Waypoints[indexPoint.y];
                                pos5= rail.transform.localRotation * moreWP2.position + rail.transform.localPosition;
                            }
                        }

                        if (pos2 == pos1)
                        {
                            AddWaypoint(rail, 1);
                            // _listRails.Add(CurrentChechWaypoint);
                            // CurrentChechWaypoint = ChechWaypoint;
                            ChechWaypoint = rail;
                            indexCheck = 1;
                            // _listRails.Remove(rail);
                            goto restart;
                        }else if (pos3 == pos1)
                        {
                            AddWaypoint(rail, 0);
                            // _listRails.Add(CurrentChechWaypoint);
                            // CurrentChechWaypoint = ChechWaypoint;
                            ChechWaypoint = rail;
                            indexCheck = 0;
                            // _listRails.Remove(rail);
                            goto restart;
                        }
                        else if (rail.m_Waypoints.Length>2 && pos4 == pos1 && ChechWaypoint != rail)
                        {
                            AddWaypoint(rail, indexPoint.y);
                            // _listRails.Add(CurrentChechWaypoint);
                            // CurrentChechWaypoint = ChechWaypoint;
                            ChechWaypoint = rail;
                            indexCheck = indexPoint.y;
                            // _listRails.Remove(rail);
                            goto restart;
                        }
                        else if (rail.m_Waypoints.Length>2 && pos5 == pos1 && ChechWaypoint != rail)
                        {
                            AddWaypoint(rail, indexPoint.x);
                            // _listRails.Add(CurrentChechWaypoint);
                            // CurrentChechWaypoint = ChechWaypoint;
                            ChechWaypoint = rail;
                            indexCheck = indexPoint.x;
                            // _listRails.Remove(rail);
                            goto restart;
                        }
                    }
                }
            }
        }

        void AddWaypoint(TrainPath child, int idx, bool start = false)
        {
            if(!child.GetComponent<TrainPath>()) return;
            TrainPath childTrainPath = child.GetComponent<TrainPath>();
            TrainPath.Waypoint wp = childTrainPath.m_Waypoints[idx];
            TrainPath.Waypoint targetWP = new TrainPath.Waypoint();
            targetWP.position = child.transform.localRotation * wp.position + child.transform.localPosition;
            var changeVar = 1;
            if (!start)
            {
                if (idx == 0)
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
                DragStopped();
            }
            if (e.type == TrainActionEventType.Update)
            {
                UpdateRail();
            }

            if (e.type == TrainActionEventType.Reset)
            {
                train.canRun = !train.canRun;
                // RespawnTrain();
            }
        }
    }
}
