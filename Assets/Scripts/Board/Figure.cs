using UnityEngine;

namespace Assets.Scripts
{
    public interface IFigure
    {
        Vector2 CenterPosition { get; }
        bool ContainsPosition(Vector2 pos);
    }
}