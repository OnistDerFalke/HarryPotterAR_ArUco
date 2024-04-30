namespace Assets.Scripts
{
    public enum Character
    {
        None,
        Harry,
        Hermiona,
        Ron,
        Draco,
        Luna,
        Ginny,
        Neville,
        Cedrik,
        Peter
    }

    public static class CharacterSpecialPower
    {
        public static SpecialPower GetSpecialPower(Character character)
        {
            switch (character)
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

        public static string GetSpecialPowerText(Character character)
        {
            switch (GetSpecialPower(character))
            {
                case SpecialPower.GetOneMoreSpell:
                    return "Dodatkowe zaklęcie na polu zaklęcia";
                case SpecialPower.GetOneMoreBook:
                    return "Dodatkowa księga na polu księgi";
                case SpecialPower.AddTwoToMove:
                    return "Wydłużenie ruchu o 2";
                case SpecialPower.GetOneMoreElixir:
                    return "Dodatkowy eliksir na polu eliksiru";
                case SpecialPower.AddThreeToMove:
                    return "Wydłużenie ruchu o 3";
                case SpecialPower.GetAdditionalLive:
                    return "Dodatkowe życie na początku ruchu";
                default:
                    return "";
            }
        }
    }
}
