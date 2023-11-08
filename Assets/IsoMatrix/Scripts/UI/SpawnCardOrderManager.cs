using UnityEngine;

public class SpawnCardOrderManager : MonoBehaviour
{
    [SerializeField] private GameObject cardMissionPrefab;
    public CardOrderManager SpawnCard()
    {
        var cardGO = Instantiate(cardMissionPrefab, transform);
       CardOrderManager cardOrderManager = cardGO.GetComponent<CardOrderManager>();
       return cardOrderManager;
    }

}
