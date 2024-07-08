using UnityEngine;

public class SpawnTrainControlManager : MonoBehaviour
{
    [SerializeField] private GameObject trainControlPrefab;
    public TrainControlManager SpawnTrainControl()
    {
        var buttonGO = Instantiate(trainControlPrefab, transform);
        TrainControlManager trainControlManager = buttonGO.GetComponent<TrainControlManager>();
       return trainControlManager;
    }

}
