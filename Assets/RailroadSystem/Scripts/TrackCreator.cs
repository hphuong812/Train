using UnityEngine;
using Cinemachine;
using IsoMatrix.Scripts.Utilities;

public class TrackCreator : MonoBehaviour
{
    [SerializeField] TrainPath track;
    [SerializeField] bool loopedTrack = false;

    private  TrainPath.Waypoint[] generatedWaypoints;
    private int waypointCount;
    int currentWaypointIndex = 0;


    void Start()
    {
        // GenerateTrack();

    }

    public void GenerateTrack()
    {
        if(!track) Debug.Log("No track assigned.");

        currentWaypointIndex = 0;

        waypointCount = loopedTrack ? track.transform.childCount : track.transform.childCount + 1;

        generatedWaypoints = new TrainPath.Waypoint[waypointCount];

        for (int i = 0; i < track.transform.childCount; i++)
        {
            Transform currentChild = track.transform.GetChild(i);

            if (i == 0 || loopedTrack)
            {
                AddWaypoint(currentChild, 0);
            }

            if (!loopedTrack)
            {
                AddWaypoint(currentChild, 1);
            }

        }
        track.m_Waypoints = generatedWaypoints;
        track.m_Looped = loopedTrack;
    }

    void AddWaypoint(Transform child, int idx)
            {
                if(!child.GetComponent<TrainPath>()) return;
                TrainPath childTrainPath = child.GetComponent<TrainPath>();
                TrainPath.Waypoint wp = childTrainPath.m_Waypoints[idx];
                TrainPath.Waypoint targetWP = new TrainPath.Waypoint();
                targetWP.position = child.localRotation * wp.position + child.localPosition;
                targetWP.tangent = child.localRotation * wp.tangent;
                targetWP.roll = wp.roll;
                generatedWaypoints[currentWaypointIndex] = targetWP;
                currentWaypointIndex++;
            }
}
