using UnityEngine;

namespace Assets.Scripts
{
    public class InteractiveElement : MonoBehaviour
    {
        [SerializeField] private int index;

        public int Index { get => index; set => index = value; }
    }
}
