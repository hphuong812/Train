using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace IsoMatrix.Scripts.UI
{
    public class SwitchToggle : MonoBehaviour {
        [SerializeField] RectTransform uiHandleRectTransform ;
        [SerializeField] Color backgroundActiveColor ;
        [SerializeField] Color handleActiveColor ;
        [SerializeField] private Sprite deactiveSprite;

        private Sprite activeSprite;
        Image backgroundImage, handleImage ;

        Color backgroundDefaultColor, handleDefaultColor ;

        Toggle toggle ;

        Vector2 handlePosition ;

        void Awake ( ) {
            toggle = GetComponent <Toggle> ( ) ;

            handlePosition = uiHandleRectTransform.anchoredPosition ;

            backgroundImage = uiHandleRectTransform.parent.GetComponent <Image> ( ) ;
            handleImage = uiHandleRectTransform.GetComponent <Image> ( ) ;

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

            //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
            backgroundImage.DOColor (on ? backgroundActiveColor : backgroundDefaultColor, .6f) ;

            //handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
            handleImage.DOColor (on ? handleActiveColor : handleDefaultColor, .4f) ;
            if (!on)
            {
                handleImage.sprite = activeSprite;
            }
            else
            {
                handleImage.sprite = deactiveSprite;
            }
        }

        void OnDestroy ( ) {
            toggle.onValueChanged.RemoveListener (OnSwitch) ;
        }
    }
}