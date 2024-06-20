using UnityEngine;
using Assets.Scripts;
using Game;

public class PossibleMovesTest : MonoBehaviour
{
    [SerializeField] public int position = 0;
    [SerializeField] public int diceNumber = 1;

    void Start()
    {
        GameManager.Setup();
        GameManager.PlayerNumber = 1;
        GameManager.Players.Add(new Player(0, Character.Harry));

        UpdatePosition(position);
        ThrowDice(diceNumber);
        ClearFields();
    }

    private void UpdatePosition(int fieldId)
    {
        GameManager.GetMyPlayer().LastFieldId = fieldId;
    }

    private void ThrowDice(int number)
    {
        GameManager.CurrentDiceThrownNumber = number;
        GameManager.GetMyPlayer().IsDuringMove = true;
        GameManager.BoardManager.ShowPossibleMoves();

        CheckFields("Player position: " + GameManager.GetMyPlayer().LastFieldId + "   Move: " + number + "\n");
    }

    private void ClearFields()
    {
        GameManager.GetMyPlayer().IsDuringMove = false;
        GameManager.GetMyPlayer().LastFieldId = -1;
        GameManager.BoardManager.UnhighlightAllFields();

        CheckFields("After move\n");
    }

    private void CheckFields(string text)
    {
        foreach (var field in GameManager.BoardManager.AllFields)
        {
            if (field.IsHighlighted)
                text += field.Index + ". " + field.Name + "\n";
        }
        Debug.Log(text);
    }
}
