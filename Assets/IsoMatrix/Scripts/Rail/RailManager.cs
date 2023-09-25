using UnityEngine;

namespace IsoMatrix.Scripts.Rail
{
    public enum RailOption
    {
        edge,
        angle
    }
    public class RailManager : MonoBehaviour
    {
        public RailType railType;
        public RailOption railOption;
        public bool isFix;
    }
}