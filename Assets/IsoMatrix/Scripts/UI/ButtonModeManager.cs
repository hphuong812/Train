using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonModeManager : MonoBehaviour
{
    [SerializeField] private RectTransform groupIcon;
    public UnityEvent LeftAction;
    public UnityEvent RightAction;
    private bool checker;

    public void ResetDefault()
    {
        RectTransform targetAnchor = gameObject.GetComponent<RectTransform>();
        float targetRotation = Mathf.Atan((targetAnchor.position.z - groupIcon.position.y)/(targetAnchor.position.x - groupIcon.position.x));
        groupIcon.rotation = Quaternion.Euler(0,0, targetRotation);
    }
    public void OnClick()
    {
        if (!checker)
        {
            checker = true;
            LeftAction?.Invoke();
        }
        else
        {
            checker = false;
            RightAction?.Invoke();
        }
    }
}
