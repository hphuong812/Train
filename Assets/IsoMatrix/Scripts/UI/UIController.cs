using IsoMatrix.Scripts.Data;
using TMPro;
using UnityEngine;

namespace IsoMatrix.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textNumRail;
        [SerializeField] private TextMeshProUGUI _textLevel;


        public void UpdateCountRail(int cout)
        {
            _textNumRail.text = cout.ToString();
        }

        public void UpdateLevel()
        {
            int currentLevel = GameConfig.Instance.CurrentLevel + 1;
            _textLevel.text = "LV " + currentLevel.ToString();
        }
    }
}
