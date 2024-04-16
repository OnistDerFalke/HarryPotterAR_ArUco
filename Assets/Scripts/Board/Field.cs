using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts;

namespace Assets.Scripts
{
    public class Field
    {
        [SerializeField] private int boardId;
        [SerializeField] private IFigure figure;
        [SerializeField] private int index;
        [SerializeField] private bool isTower;
        [SerializeField] private bool isQuidditchPitch;
        [SerializeField] private bool isMissionField;
        [SerializeField] private bool isFiuuField;
        [SerializeField] private bool isPortkeyField;
        [SerializeField] private List<Field> neighbors;
        [SerializeField] private Field portalField;
        private List<int> missionNumbers;
        private string name;
        private List<Action> actions;

        [SerializeField] private bool isHighlighted = false;

        public int BoardId { get => boardId; }
        public IFigure Figure { get => figure; }
        public int Index { get => index; }
        public bool IsTower { get => isTower; }
        public bool IsQuidditchPitch { get => isQuidditchPitch; }
        //If IsMissionField=true than field should be lighted in different way
        //there are many fields belonging to "Wielka Sala" thats why
        //not every field where the player can fulfill a mission is set to true
        public bool IsMissionField { get => isMissionField; }
        public bool IsFiuuField { get => isFiuuField; }
        public List<Field> Neighbors { get => neighbors; }
        public Field PortalField { get => portalField; }
        public Vector2 Position2D { get => figure.CenterPosition; }
        public bool IsHighlighted { get => isHighlighted; }
        public string Name { get => name; }
        public List<Action> Actions { get => actions; }
        public List<int> MissionNumbers { get => missionNumbers; }
        public List<Instruction.InstructionInfo> InstructionParts;

        public Field(int boardId, IFigure figure, int index, string name, List<Action> actions,
            bool isTower = false, bool isMissionField = false, bool isFiuuField = false,
            bool isPortkeyField = false, bool isQuidditchPitch = false, List<int> missionNumbers = null)
        {
            this.boardId = boardId;
            this.figure = figure;
            this.index = index;
            this.name = name;
            this.actions = actions;
            this.isTower = isTower;
            this.isMissionField = isMissionField;
            this.missionNumbers = missionNumbers;
            this.isFiuuField = isFiuuField;
            this.isPortkeyField = isPortkeyField;
            this.isQuidditchPitch = isQuidditchPitch;
            this.portalField = null;
            this.neighbors = new List<Field>();
            CreateFieldInstruction();
        }

        public string GetActionsInfo()
        {
            string actionText = "";
            for (var i = 0; i < actions.Count; i++)
            {
                actionText += ActionText.GetActionText(actions[i], missionNumbers, portalField.Name);
                if (i < actions.Count - 1) 
                    actionText += "\n\n";
            }
            if (actionText == "") 
                actionText = "Pole nie posiada ¿adnych akcji.";
            return actionText;
        }

        public string GetActionsShortInfo()
        {
            string actionText = "";
            for (var i = 0; i < actions.Count; i++)
            {
                actionText += ActionText.GetActionShortText(actions[i], missionNumbers, portalField.Name);
                if (i < actions.Count - 1)
                    actionText += "\n";
            }
            if (actionText == "")
                actionText = "Pole nie posiada ¿adnych akcji.";
            return actionText;
        }

        public void AddNeighbors(List<Field> neighbors)
        {
            this.neighbors.AddRange(neighbors);
        }

        public void SetPortalField(Field portalField)
        {
            this.portalField = portalField;
        }

        public void Highlight()
        {
            if (!isHighlighted && index != GameManager.GetMyPlayer().LastFieldId)
            {
                RequestBroker.requests.Add(new HighlightFieldRequest(this));
                isHighlighted = true;
            }
        }

        public void Unhighlight()
        {
            if (isHighlighted)
            {
                //RequestBroker.requests.Add(new UnhighlightFieldRequest(this));
                isHighlighted = false;
            }
        }

        public void CreateFieldInstruction()
        {
            Instruction instruction = new();

            InstructionParts = new();

            if (IsQuidditchPitch)
            {
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchPreparation);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1_Actions1);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1_Actions2);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound2);
            }
            if (IsMissionField)
            {
                InstructionParts.Add(Instruction.InstructionInfo.MissionCompleting);
                InstructionParts.Add(Instruction.InstructionInfo.MissionFight);
            }
            if (Actions.Contains(Action.FIGHT_FIELD))
                InstructionParts.Add(Instruction.InstructionInfo.FightOnFightField);

            InstructionParts.Add(Instruction.InstructionInfo.PlayerMove);
            InstructionParts.Add(Instruction.InstructionInfo.SpecialItems);
            InstructionParts.Add(Instruction.InstructionInfo.FightWithPlayer);
            InstructionParts.Add(Instruction.InstructionInfo.GameDuration);

            if (!IsMissionField)
            {
                InstructionParts.Add(Instruction.InstructionInfo.MissionCompleting);
                InstructionParts.Add(Instruction.InstructionInfo.MissionFight);
            }
            if (!Actions.Contains(Action.FIGHT_FIELD))
                InstructionParts.Add(Instruction.InstructionInfo.FightOnFightField);
            if (!IsQuidditchPitch)
            {
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchPreparation);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1_Actions1);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound1_Actions2);
                InstructionParts.Add(Instruction.InstructionInfo.QuidditchRound2);
            }
        }
    }
}
