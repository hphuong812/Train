using System.Collections.Generic;
using Cinemachine;
using IsoMatrix.Scripts.Train;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IsoMatrix.Scripts.Rail
{
    public class RailCreator : MonoBehaviour
    {
        private CinemachinePath pathTrain_1;
        public CinemachinePath StartRail;
        public GameObject PathContainer;
        public TrainController train;
        private  List<CinemachinePath.Waypoint> generatedWaypoints;
        private List<CinemachinePath> _listRails = new List<CinemachinePath>();
        private CinemachinePath ChechWaypoint;
        private CinemachinePath CurrentChechWaypoint;
        private  int currentWaypointIndex = 0;
        private int indexCheck = 1;
        private bool canRun;
        private bool lockCheckGroup;
        private int maxPoint = 100;

        private void Start()
        {
            generatedWaypoints = new List<CinemachinePath.Waypoint>();
            AddWaypoint(StartRail, 0, true);
            AddWaypoint(StartRail, 1);
            ChechWaypoint = StartRail;
            CurrentChechWaypoint = ChechWaypoint;

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

        public void DragStopped(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Canceled)
            {
                GetAllRail();
                TrainWayFindind();
                Invoke(nameof(Run), 0f);
            }
        }

        public void GetAllRail()
        {
            _listRails.Clear();
            foreach (Transform child in PathContainer.transform)
            {
                CinemachinePath cinemachinePath = child.GetComponent<CinemachinePath>();
                if (cinemachinePath)
                {
                    _listRails.Add(cinemachinePath);
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

            CinemachinePath.Waypoint[] cine = new CinemachinePath.Waypoint[generatedWaypoints.Count];
            cine = generatedWaypoints.ToArray();
            CinemachinePath currentCinePath = gameObject.GetComponent<CinemachinePath>();
            if (!currentCinePath)
            {
                pathTrain_1 = gameObject.AddComponent<CinemachinePath>();
                pathTrain_1.m_Waypoints =cine;
                train.m_Path = pathTrain_1;
            }
        }

        public void Run()
        {
            if (canRun)
            {
                train.canRun = true;
                canRun = false;
            }
        }

        public void RailCheck()
        {
            restart:
            if ( generatedWaypoints.Count < maxPoint)
            {
                foreach (var rail in _listRails)
                {
                    CinemachinePath.Waypoint wp = ChechWaypoint.m_Waypoints[indexCheck];
                    var pos1= ChechWaypoint.transform.localRotation * wp.position + ChechWaypoint.transform.localPosition;

                    CinemachinePath.Waypoint startWP = rail.m_Waypoints[0];
                    var pos2= rail.transform.localRotation * startWP.position + rail.transform.localPosition;
                    CinemachinePath.Waypoint endWP = rail.m_Waypoints[1];
                    var pos3= rail.transform.localRotation * endWP.position + rail.transform.localPosition;
                    Vector3 pos4 = Vector3.zero;
                    if (ChechWaypoint.transform.localPosition !=  rail.transform.localPosition)
                    {
                        if (rail.m_Waypoints.Length>2)
                        {
                            CinemachinePath.Waypoint moreWP = rail.m_Waypoints[2];
                            pos4= rail.transform.localRotation * moreWP.position + rail.transform.localPosition;
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
                            AddWaypoint(rail, 1);
                            // _listRails.Add(CurrentChechWaypoint);
                            // CurrentChechWaypoint = ChechWaypoint;
                            ChechWaypoint = rail;
                            indexCheck = 1;
                            // _listRails.Remove(rail);
                            goto restart;
                        }
                    }
                }
            }
        }

        void AddWaypoint(CinemachinePath child, int idx, bool start = false)
        {
            if(!child.GetComponent<CinemachinePath>()) return;
            CinemachinePath childCinemachinePath = child.GetComponent<CinemachinePath>();
            CinemachinePath.Waypoint wp = childCinemachinePath.m_Waypoints[idx];
            CinemachinePath.Waypoint targetWP = new CinemachinePath.Waypoint();
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
    }
}
