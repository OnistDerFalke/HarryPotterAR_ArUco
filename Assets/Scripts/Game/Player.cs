using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using Scripts;
using System;

namespace Game
{  
    public class Player
    {
        private int index;
        private Character character;
        private SpecialPower specialPower;
        private bool isDuringMove;
        private bool myPlayer;
        private int lastFieldId;

        public int Index { get => index; }
        public Character Character { get => character; }
        public SpecialPower SpecialPower { get => specialPower; }
        public bool IsDuringMove { get => isDuringMove; set => isDuringMove = value; }
        public bool IsMyPlayer { get => myPlayer; }
        public int LastFieldId { get => lastFieldId; set => lastFieldId = value; }

        public Player(int index, Character character)
        {
            this.index = index;
            this.character = character;
            myPlayer = this.index == 0;
            isDuringMove = false;
            lastFieldId = 0;
            this.specialPower = GetSpecialPower();
        }

        public string GetCurrentFieldName()
        {
            if (lastFieldId >= 0)
                return GameManager.BoardManager.GetFieldById(lastFieldId).Name;
            return "Zczytaj pozycję gracza";
        }

        public string GetCurrentFieldActions()
        {
            if (lastFieldId >= 0)
                return GameManager.BoardManager.GetFieldById(lastFieldId).GetActionsInfo();
            return "Zczytaj pozycję gracza poprzez nakierowanie kamery telefonu na znacznik gracza. " +
                    "Zrób to tak, aby w obrębie ekranu był widoczny przynajmniej jeden znacznik planszy";
        }

        public List<Instruction.InstructionInfo> GetCurrentInstructionParts()
        {
            return GameManager.BoardManager.GetFieldById(lastFieldId).InstructionParts;
        }

        public static Character CharacterFromInt(int id)
        {
            switch (id)
            {
                case 0:
                    return Character.Harry;
                case 1:
                    return Character.Hermiona;
                case 2:
                    return Character.Ron;
                case 3:
                    return Character.Draco;
                case 4:
                    return Character.Luna;
                case 5:
                    return Character.Ginny;
                case 6:
                    return Character.Neville;
                case 7:
                    return Character.Cedrik;
                case 8:
                    return Character.Peter;
                default:
                    return Character.None;
            }
        }

        private SpecialPower GetSpecialPower()
        {
            switch (this.character)
            {
                case Character.Harry:
                    return SpecialPower.GetOneMoreSpell;
                case Character.Hermiona:
                    return SpecialPower.GetOneMoreBook;
                case Character.Luna:
                    return SpecialPower.GetOneMoreBook;
                case Character.Peter:
                    return SpecialPower.AddTwoToMove;
                case Character.Draco:
                    return SpecialPower.GetOneMoreElixir;
                case Character.Ron:
                    return SpecialPower.AddThreeToMove;
                case Character.Cedrik:
                    return SpecialPower.GetAdditionalLive;
                case Character.Neville:
                    return SpecialPower.GetOneMoreElixir;
                case Character.Ginny:
                    return SpecialPower.AddTwoToMove;
                default:
                    return SpecialPower.None;
            }
        }
    }
}
