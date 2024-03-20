using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Board
    {
        private int id;
        private Vector2 size;
        private List<Field> fields = new();

        public int Id { get => id; }
        public float Width { get => size.x; }
        public float Heigth { get => size.y; }
        public Vector2 Size { get => size; }
        public List<Field> Fields { get => fields; }

        public Board(int id, float width, float height)
        {
            this.id = id;
            size = new Vector2(width, height);
        }
    }
}