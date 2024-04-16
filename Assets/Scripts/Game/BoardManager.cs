using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardManager
    {
        [SerializeField] private List<Board> boards;
        [SerializeField] private List<Field> allFields = new();

        public List<Board> Boards { get => boards; }
        public List<Field> AllFields { get => allFields; }

        public void Setup()
        {
            this.boards = new List<Board>() {
                new Board(0, 50.0f, 50.0f),
                new Board(1, 25.0f, 25.0f),
                new Board(2, 25.0f, 25.0f)
            };
            BoardInitializator.InitBoard();
        }

        public void AddFields(List<Field> fields)
        {
            this.allFields.AddRange(fields);
        }

        public Field GetFieldById(int id)
        {
            return this.allFields.Where(field => field.Index == id).FirstOrDefault();
        }

        public List<Field> GetTowerFields()
        {
            return allFields.Where(field => field.IsTower).ToList();
        }

        public List<Field> GetFieldsByIds(List<int> indexes)
        {
            return this.allFields.Where(field => indexes.Contains(field.Index)).ToList();
        }

        /// <summary>
        /// Player can end his move on fields:
        /// - that are towers (he can go to the board id = 2)
        /// - that are mission fields
        /// - according to the dice thrown number
        /// Player can't appear on the same field twice during one move
        /// </summary>
        /// <returns></returns>
        public void ShowPossibleMoves()
        {
            Field myField = this.allFields.Where(field => field.Index == GameManager.GetMyPlayer().LastFieldId).FirstOrDefault();

            myField.Neighbors.ForEach(delegate (Field neighbor)
            {
                FindFieldsToHighlightRecursive(new List<Field>() { myField, neighbor }, 1, false);
            });
        }

        public void HighlightAllFields()
        {
            boards[0].Fields.ForEach(delegate (Field field)
            {
                field.Highlight();
            });
            boards[1].Fields.ForEach(delegate (Field field)
            {
                field.Highlight();
            });
            boards[2].Fields.ForEach(delegate (Field field)
            {
                field.Highlight();
            });
        }

        public void UnhighlightAllFields()
        {
            RequestBroker.requests.Add(new UnhighlightFieldRequest(null));

            boards[0].Fields.ForEach(delegate (Field field)
            {
                field.Unhighlight();
            });
            boards[1].Fields.ForEach(delegate (Field field)
            {
                field.Unhighlight();
            });
            boards[2].Fields.ForEach(delegate (Field field)
            {
                field.Unhighlight();
            });
        }

        private void FindFieldsToHighlightRecursive(List<Field> path, int steps, bool fiuuFieldsHighlighted)
        {
            Field currentField = path.Last();

            if (steps == GameManager.CurrentDiceThrownNumber)
            {
                currentField.Highlight();
            }
            else if (steps < GameManager.CurrentDiceThrownNumber)
            {
                if (currentField.IsTower)
                {
                    currentField.Highlight();
                    if (!fiuuFieldsHighlighted)
                    {
                        boards[2].Fields.Where(field => field.IsFiuuField).ToList().ForEach(delegate (Field fiuuField)
                        {
                            fiuuField.Highlight();
                        });
                        fiuuFieldsHighlighted = true;
                    }
                }
                else if (currentField.IsMissionField)
                {
                    currentField.Highlight();
                }

                // go to neighbors of current field
                currentField.Neighbors.ForEach(delegate (Field neighbor)
                {
                    if (!path.Contains(neighbor))
                    {
                        path.Add(neighbor);
                        FindFieldsToHighlightRecursive(path, steps+1, fiuuFieldsHighlighted);
                        path.Remove(neighbor);
                    }
                });
            }

            // teleport by portalField to portalField
            if (currentField.PortalField != null)
            {
                if (!path.Contains(currentField.PortalField))
                {
                    path.Add(currentField.PortalField);
                    FindFieldsToHighlightRecursive(path, steps, fiuuFieldsHighlighted);
                    path.Remove(currentField.PortalField);
                }
            }
        }
    }
}
