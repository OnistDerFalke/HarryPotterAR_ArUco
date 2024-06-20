using System.Collections.Generic;

namespace Assets.Scripts
{
    public enum Action
    {
        //Obowiązkowe akcje
        COMMON_ROOM_CHOICE,             //jedna dowolna karta lub wymiana jednego przedmiotu
        FIGHT_FIELD,                    //wszystkie pola z pojedynkiem
        GET_ONE_BOOK,                   //dział ksiąg zakazanych, wszystkie pola z książką
        GET_ONE_ELIXIR,                 //wszystkie pola z eliksirem
        GET_ONE_EVENT,                  //wszystkie pola z wydarzeniem
        GET_ONE_EXERCISE,               //wszystkie pola ze sprawowaniem
        GET_ONE_SPELL,                  //klasy: "zaklęcia i uroki", "transmutacja", "obrona przed czarną magią", wszystkie pola z zaklęciem
        GET_ONE_SPELL_AND_ONE_ELIXIR,   //chatka Hagrida        
        GET_TWO_BOOKS,                  //Esy i floresy
        GET_TWO_ELIXIRS,                //klasy: "eliksiry", "zielarstwo", "opieka nad magicznymi stworzeniami", gabinet Snape'a, biblioteka
        GET_TWO_SPELLS,                 //gabinet Dumbledore'a, Sklep z kotłami Madame Potage
        GET_QUIDDITCH_CARD,             //Markowy sprzęt do Quidditcha

        //Warunkowe i nieobowiązkowe 
        ADD_TWO_LIVES,                  //dziurawy kocioł 
        ADD_THREE_LIVES,                //kuchnia 
        ADD_FOUR_LIVES,                 //pola +4ż (koło zielarstwa, koło jeziora) 
        ADD_FIVE_LIVES,                 //pola +5ż (koło jeziora)
        ADD_SIX_LIVES,                  //skrzydło szpitalne
        CAN_MAKE_MISSION,               //wszystkie pola z misją
        CAN_PASS_EXAMS,                 //klasy
        CAN_USE_FIUU,                   //wieża zamkowa
        CAN_USE_PORTKEY,                //boisko do quidditcha

        //Możliwe, ale nieobowiązkowe akcje
        CAN_GET_THING,                  //wszystkie pola z gwiazdką
        CAN_START_QUIDDITCH,            //boisko do Quidditch
        CAN_TELEPORT                    //wszystkie pola portalowe
    }

    public static class ActionText
    {
        public static string GetActionText(Action action, List<int> missionNumbers, string portalName = "")
        {
            SpecialPower power = GameManager.GetMyPlayer().SpecialPower;

            switch (action)
            {
                case Action.COMMON_ROOM_CHOICE:
                    return "Weź jedną dowolną kartę lub zaproponuj wymianę przedmiotu pozostałym graczom.";
                case Action.FIGHT_FIELD:
                    return "Pobierz kartę z pojedynkiem i przeprowadź walkę.";
                case Action.GET_ONE_BOOK:
                    if (power == SpecialPower.GetOneMoreBook)
                        return "Weź dwie karty księgi.";
                    else
                        return "Weź jedną kartę księgi.";
                case Action.GET_ONE_ELIXIR:
                    if (power == SpecialPower.GetOneMoreElixir)
                        return "Weź dwie karty eliksiru.";
                    else
                        return "Weź jedną kartę eliksiru.";
                case Action.GET_ONE_EVENT:
                    return "Weź jedną kartę wydarzenia.";
                case Action.GET_ONE_EXERCISE:
                    return "Weź jedną kartę sprawowania.";
                case Action.GET_ONE_SPELL:
                    if (power == SpecialPower.GetOneMoreSpell)
                        return "Weź dwie karty zaklęcia.";
                    else
                        return "Weź jedną kartę zaklęcia.";
                case Action.GET_TWO_BOOKS:
                    return "Weź dwie karty księgi.";
                case Action.GET_TWO_ELIXIRS:
                    return "Weź dwie karty eliksiru.";
                case Action.GET_TWO_SPELLS:
                    return "Weź dwie karty zaklęcia.";
                case Action.GET_QUIDDITCH_CARD:
                    return "Weź jedną kartę quidditcha.";
                case Action.ADD_TWO_LIVES: 
                    return "Jeśli masz miejsce, dobierz 2 życia.";
                case Action.ADD_THREE_LIVES:
                    return "Jeśli masz miejsce, dobierz 3 życia.";
                case Action.ADD_FOUR_LIVES:
                    return "Jeśli masz miejsce, dobierz 4 życia.";
                case Action.ADD_FIVE_LIVES:
                    return "Jeśli masz miejsce, dobierz 5 żyć.";
                case Action.ADD_SIX_LIVES:
                    return "Jeśli masz miejsce, dobierz 6 żyć.";
                case Action.CAN_MAKE_MISSION:
                    string result = "Jeżeli spełniasz wszystkie warunki odpowiedniej misji (nr ";
                    for (int i = 0; i < missionNumbers.Count; i++)
                    {
                        result += $"{missionNumbers[i]}";
                        if (i < missionNumbers.Count - 1) result += ", ";
                    }
                    result += "), możesz ją wykonać.";
                    return result;
                case Action.CAN_PASS_EXAMS:
                    return "Możesz zdać egzaminy, jeśli posiadasz karty księgi powiązane z tą klasą.";
                case Action.CAN_USE_FIUU:
                    return "Możesz transportować się za pomocą proszka fiuu na 1 z 7 pól na planszy 2 (z wyjątkiem \"Cmentarza w Little Hangleton\"). " +
                        "Na koniec tury wróć do miejsca, z którego się przeniosłeś.";
                case Action.CAN_USE_PORTKEY:
                    return "Możesz za pomocą świstoklika przenieść się na pole \"Cmentarz w Little Hangleton\".";
                case Action.CAN_GET_THING:
                    return "Jeżeli na polu znajduje się przedmiot, możesz go podnieść. " +
                        "Można posiadać maksymalnie 6 przedmiotów, a podczas jednego ruchu można podnieść tylko 2 przedmioty. " + 
                        "Gracz ma możliwość wymiany przedmiotu.";
                case Action.CAN_START_QUIDDITCH:
                    return "Możesz wyzwać dowolnego gracza do meczu Quidditcha.";
                case Action.CAN_TELEPORT:
                    return $"Z tego pola możesz się teleportować (za darmo) na połączone z nim pole: {portalName}.";
            }

            return "";
        }

        public static string GetActionShortText(Action action, List<int> missionNumbers, string portalName = "")
        {
            SpecialPower power = GameManager.GetMyPlayer().SpecialPower;

            switch (action)
            {
                case Action.COMMON_ROOM_CHOICE:
                    return "Weź jedną dowolną kartę lub zaproponuj wymianę przedmiotu pozostałym graczom.";
                case Action.FIGHT_FIELD:
                    return "Pobierz kartę z pojedynkiem i przeprowadź walkę.";
                case Action.GET_ONE_BOOK:
                    if (power == SpecialPower.GetOneMoreBook)
                        return "Weź dwie karty księgi.";
                    else
                        return "Weź jedną kartę księgi.";
                case Action.GET_ONE_ELIXIR:
                    if (power == SpecialPower.GetOneMoreElixir)
                        return "Weź dwie karty eliksiru.";
                    else
                        return "Weź jedną kartę eliksiru.";
                case Action.GET_ONE_EVENT:
                    return "Weź jedną kartę wydarzenia.";
                case Action.GET_ONE_EXERCISE:
                    return "Weź jedną kartę sprawowania.";
                case Action.GET_ONE_SPELL:
                    if (power == SpecialPower.GetOneMoreSpell)
                        return "Weź dwie karty zaklęcia.";
                    else
                        return "Weź jedną kartę zaklęcia.";
                case Action.GET_TWO_BOOKS:
                    return "Weź dwie karty księgi.";
                case Action.GET_TWO_ELIXIRS:
                    return "Weź dwie karty eliksiru.";
                case Action.GET_TWO_SPELLS:
                    return "Weź dwie karty zaklęcia.";
                case Action.GET_QUIDDITCH_CARD:
                    return "Weź jedną kartę quidditcha.";
                case Action.ADD_TWO_LIVES:
                    return "Jeśli masz miejsce, dobierz 2 życia.";
                case Action.ADD_THREE_LIVES:
                    return "Jeśli masz miejsce, dobierz 3 życia.";
                case Action.ADD_FOUR_LIVES:
                    return "Jeśli masz miejsce, dobierz 4 życia.";
                case Action.ADD_FIVE_LIVES:
                    return "Jeśli masz miejsce, dobierz 5 żyć.";
                case Action.ADD_SIX_LIVES:
                    return "Jeśli masz miejsce, dobierz 6 żyć.";
                case Action.CAN_MAKE_MISSION:
                    string result = "Jeżeli spełniasz wszystkie warunki odpowiedniej misji (nr ";
                    for (int i = 0; i < missionNumbers.Count; i++)
                    {
                        result += $"{missionNumbers[i]}";
                        if (i < missionNumbers.Count - 1) result += ", ";
                    }
                    result += "), możesz ją wykonać.";
                    return result;
                case Action.CAN_PASS_EXAMS:
                    return "Możesz zdać egzaminy, jeśli posiadasz karty księgi powiązane z tą klasą.";
                case Action.CAN_USE_FIUU:
                    return "Możesz transportować się za pomocą proszka fiuu planszę 2.";
                case Action.CAN_USE_PORTKEY:
                    return "Możesz za pomocą świstoklika przenieść się na pole \"Cmentarz w Little Hangleton\".";
                case Action.CAN_GET_THING:
                    return "Możesz podnieść przedmiot, który znajduje się na tym polu.";
                case Action.CAN_START_QUIDDITCH:
                    return "Możesz wyzwać dowolnego gracza do meczu Quidditcha.";
                case Action.CAN_TELEPORT:
                    return $"Z tego pola możesz się teleportować (za darmo) na połączone z nim pole: {portalName}.";
            }

            return "";
        }
    }
}