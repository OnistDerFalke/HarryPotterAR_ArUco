using UnityEngine;

namespace Assets.Scripts
{
    public class Quadrangle : IFigure
    {
        public Vector2 leftUpper;
        public Vector2 rightUpper;
        public Vector2 rightBottom;
        public Vector2 leftBottom;

        public Vector2 CenterPosition { get => (leftUpper + rightUpper + rightBottom + leftBottom) / 4; }

        public Quadrangle(Vector2 leftUpper, Vector2 rightUpper, Vector2 rightBottom, Vector2 leftBottom)
        {
            this.leftUpper = leftUpper;
            this.rightUpper = rightUpper;
            this.rightBottom = rightBottom;
            this.leftBottom = leftBottom;
        }

        private float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return Mathf.Abs((p1.x * (p2.y - p3.y) +
                         p2.x * (p3.y - p1.y) +
                         p3.x * (p1.y - p2.y)) / 2f);
        }

        private bool IsInsideTriangle(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3, float margin = 0.1f)
        {
            float triangleArea = TriangleArea(v1, v2, v3);
            float a1 = TriangleArea(p, v2, v3);
            float a2 = TriangleArea(p, v1, v3);
            float a3 = TriangleArea(p, v1, v2);

            return Mathf.Abs(triangleArea - (a1 + a2 + a3)) <= margin;
        }

        public bool ContainsPosition(Vector2 pos)
        {
            return (IsInsideTriangle(pos, leftUpper, rightUpper, rightBottom) || 
                IsInsideTriangle(pos, rightBottom, leftBottom, leftUpper));
        }
    }
}