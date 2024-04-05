using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using Scripts;

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

        public static Character CharacterFromString(string s)
        {
            switch (s)
            {
                case "harr":
                    return Character.Harry;
                case "herm":
                    return Character.Hermiona;
                case "luna":
                    return Character.Luna;
                case "pete":
                    return Character.Peter;
                case "drac":
                    return Character.Draco;
                case "rono":
                    return Character.Ron;
                case "cedr":
                    return Character.Cedrik;
                case "nevi":
                    return Character.Neville;
                case "ginn":
                    return Character.Ginny;
                default:
                    return Character.None;
            }
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
                    return Character.Luna;
                case 3:
                    return Character.Peter;
                case 4:
                    return Character.Draco;
                case 5:
                    return Character.Ron;
                case 6:
                    return Character.Cedrik;
                case 7:
                    return Character.Neville;
                case 8:
                    return Character.Ginny;
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
