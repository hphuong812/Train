using UnityEngine;

public class LayerMarkChecker
{
    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        if(((1 << layer) & layerMask) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}