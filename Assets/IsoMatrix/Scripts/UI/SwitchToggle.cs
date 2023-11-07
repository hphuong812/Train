using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IsoMatrix.Scripts.UI
{
    public class SwitchToggle : MonoBehaviour {
        [SerializeField] RectTransform uiHandleRectTransform ;
        [SerializeField] RectTransform uiNumrailTransform ;
        [SerializeField] Color backgroundActiveColor ;
        [SerializeField] Color handleActiveColor ;
        [SerializeField] private Sprite deactiveSprite;

        private TextMeshProUGUI textNumrail;
        private Sprite activeSprite;
        Image backgroundImage, handleImage ;

        Color backgroundDefaultColor, handleDefaultColor ;

        Toggle toggle ;

        Vector2 handlePosition;
        Vector2 handleNumrailPosition;

        void Awake ( ) {
            toggle = GetComponent <Toggle> ( ) ;

            handlePosition = uiHandleRectTransform.anchoredPosition ;
            handleNumrailPosition = uiNumrailTransform.anchoredPosition;
            backgroundImage = uiHandleRectTransform.parent.GetComponent <Image> ( ) ;
            handleImage = uiHandleRectTransform.GetComponent <Image> ( ) ;

            textNumrail = uiNumrailTransform.gameObject.GetComponent<TextMeshProUGUI>();
            backgroundDefaultColor = backgroundImage.color ;
            handleDefaultColor = handleImage.color ;
            activeSprite = handleImage.sprite;

            toggle.onValueChanged.AddListener (OnSwitch) ;

            if (toggle.isOn)
                OnSwitch (true) ;
        }

        void OnSwitch (bool on) {
            //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
            uiHandleRectTransform.DOAnchorPos (on ? handlePosition * -1 : handlePosition, .4f).SetEase (Ease.InOutBack) ;
            uiNumrailTransform.DOAnchorPos (on ? handleNumrailPosition * -1 : handleNumrailPosition, .4f).SetEase (Ease.InOutBack) ;
            //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
            backgroundImage.DOColor (on ? backgroundActiveColor : backgroundDefaultColor, .6f) ;

            //handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
            handleImage.DOColor (on ? handleActiveColor : handleDefaultColor, .4f) ;
            if (!on)
            {
                handleImage.sprite = activeSprite;
                textNumrail.color = Color.white;
            }
            else
            {
                handleImage.sprite = deactiveSprite;
                textNumrail.color = new Color32(254, 113, 113, 255);
            }
        }

        void OnDestroy ( ) {
            toggle.onValueChanged.RemoveListener (OnSwitch) ;
        }
    }
}