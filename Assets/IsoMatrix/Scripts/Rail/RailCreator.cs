using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailCreator : MonoBehaviour
{
    public CinemachinePath pathTrain_1;
    public CinemachinePath StartRail;
    public GameObject PathContainer;
    public TrainController train;
    private  List<CinemachinePath.Waypoint> generatedWaypoints;
    private List<CinemachinePath> _listRails = new List<CinemachinePath>();
    private CinemachinePath ChechWaypoint;
    private  int currentWaypointIndex = 0;
    private bool canRun;

    private void Start()
    {
        generatedWaypoints = new List<CinemachinePath.Waypoint>();
        AddWaypoint(StartRail, 0);
        AddWaypoint(StartRail, 1);
        ChechWaypoint = StartRail;

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
        pathTrain_1 = gameObject.AddComponent<CinemachinePath>();
        pathTrain_1.m_Waypoints =cine;
        train.m_Path = pathTrain_1;
    }

    public void Run()
    {
        if (canRun)
        {
            train.m_Speed = 1;
            train.canRun = true;
            canRun = false;
        }
    }

    public void RailCheck()
    {
        restart:
        foreach (var rail in _listRails)
        {


            CinemachinePath.Waypoint wp = ChechWaypoint.m_Waypoints[1];
            var pos1= ChechWaypoint.transform.localRotation * wp.position + ChechWaypoint.transform.localPosition;

            CinemachinePath.Waypoint startWP = rail.m_Waypoints[0];
            var pos2= rail.transform.localRotation * startWP.position + rail.transform.localPosition;

            if (pos2 == pos1)
            {
                AddWaypoint(rail, 1);
                ChechWaypoint = rail;
                _listRails.Remove(rail);
                goto restart;
            }
        }
    }

    void AddWaypoint(CinemachinePath child, int idx)
    {
        if(!child.GetComponent<CinemachinePath>()) return;
        CinemachinePath childCinemachinePath = child.GetComponent<CinemachinePath>();
        CinemachinePath.Waypoint wp = childCinemachinePath.m_Waypoints[idx];
        CinemachinePath.Waypoint targetWP = new CinemachinePath.Waypoint();
        targetWP.position = child.transform.localRotation * wp.position + child.transform.localPosition;
        targetWP.tangent = child.transform.localRotation * wp.tangent;
        targetWP.roll = wp.roll;
        generatedWaypoints.Add(targetWP);
    }
}
