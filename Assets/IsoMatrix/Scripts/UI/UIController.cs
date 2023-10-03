using TMPro;
using UnityEngine;

namespace IsoMatrix.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textNumRail;

        public void UpdateCountRail(int cout)
        {
            _textNumRail.text = cout.ToString();
        }
    }
}
