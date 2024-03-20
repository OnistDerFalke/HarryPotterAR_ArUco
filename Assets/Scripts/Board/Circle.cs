using UnityEngine;

namespace Assets.Scripts
{
    public class Circle : IFigure
    {
        [SerializeField] private Vector2 center;
        [SerializeField] private float radius;

        public Vector2 CenterPosition { get => center; }
        public float Radius { get => radius; }

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool ContainsPosition(Vector2 pos)
        {
            return Vector2.Distance(center, pos) <= radius;
        }
    }
}