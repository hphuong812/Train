using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardOrderManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private Image iconImage;
    private int countItem;

    public UnityEvent<CardOrderManager> MissionCompleted;

    public void SetUpMission(string name, int index)
    {
        Addressables.LoadAssetAsync<Sprite>(name).Completed += handle =>
        {
            iconImage.sprite = handle.Result;
        };
        indexText.text = index.ToString();
        countItem = index;
    }
}
