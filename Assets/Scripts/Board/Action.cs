using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum Action
    {
        //Obowi¹zkowe akcje
        COMMON_ROOM_CHOICE,             //jedna dowolna karta lub wymiana jednego przedmiotu
        FIGHT_FIELD,                    //wszystkie pola z pojedynkiem
        GET_ONE_BOOK,                   //dzia³ ksi¹g zakazanych, wszystkie pola z ksi¹¿k¹
        GET_ONE_ELIXIR,                 //wszystkie pola z eliksirem
        GET_ONE_EVENT,                  //wszystkie pola z wydarzeniem
        GET_ONE_EXERCISE,               //wszystkie pola ze sprawowaniem
        GET_ONE_SPELL,                  //klasy: "zaklêcia i uroki", "transmutacja", "obrona przed czarn¹ magi¹", wszystkie pola z zaklêciem
        GET_ONE_SPELL_AND_ONE_ELIXIR,   //chatka Hagrida        
        GET_TWO_BOOKS,                  //Esy i floresy
        GET_TWO_ELIXIRS,                //klasy: "eliksiry", "zielarstwo", "opieka nad magicznymi stworzeniami", gabinet Snape'a, biblioteka
        GET_TWO_SPELLS,                 //gabinet Dumbledore'a, Sklep z kot³ami Madame Potage
        GET_QUIDDITCH_CARD,             //Markowy sprzêt do Quidditcha

        //Warunkowe i nieobowi¹zkowe 
        ADD_TWO_LIVES,                  //dziurawy kocio³ 
        ADD_THREE_LIVES,                //kuchnia 
        ADD_FOUR_LIVES,                 //pola +4¿ (ko³o zielarstwa, ko³o jeziora) 
        ADD_FIVE_LIVES,                 //pola +5¿ (ko³o jeziora)
        ADD_SIX_LIVES,                  //skrzyd³o szpitalne
        CAN_MAKE_MISSION,               //wszystkie pola z misj¹
        CAN_PASS_EXAMS,                 //klasy
        CAN_USE_FIUU,                   //wie¿a zamkowa
        CAN_USE_PORTKEY,                //boisko do quidditcha

        //Mo¿liwe, ale nieobowi¹zkowe akcje
        CAN_GET_THING,                  //wszystkie pola z gwiazdk¹
        CAN_START_QUIDDITCH,            //boisko do Quidditch
        CAN_TELEPORT                    //wszystkie pola portalowe
    }

    public static class ActionText
    {
        public static string GetActionText(Action action, List<int> missionNumbers)
        {
            SpecialPower power = GameManager.GetMyPlayer().SpecialPower;

            switch (action)
            {
                case Action.COMMON_ROOM_CHOICE:
                    return "WeŸ jedn¹ dowoln¹ kartê lub zaproponuj wymianê przedmiotu pozosta³ym graczom.";
                case Action.FIGHT_FIELD:
                    return "Pobierz kartê z pojedynkiem i przeprowadŸ walkê.";
                case Action.GET_ONE_BOOK:
                    if (power == SpecialPower.GetOneMoreBook)
                        return "WeŸ dwie karty ksiêgi.";
                    else
                        return "WeŸ jedn¹ kartê ksiêgi.";
                case Action.GET_ONE_ELIXIR:
                    if (power == SpecialPower.GetOneMoreElixir)
                        return "WeŸ dwie karty eliksiru.";
                    else
                        return "WeŸ jedn¹ kartê eliksiru.";
                case Action.GET_ONE_EVENT:
                    return "WeŸ jedn¹ kartê wydarzenia.";
                case Action.GET_ONE_EXERCISE:
                    return "WeŸ jedn¹ kartê sprawowania.";
                case Action.GET_ONE_SPELL:
                    if (power == SpecialPower.GetOneMoreSpell)
                        return "WeŸ dwie karty zaklêcia.";
                    else
                        return "WeŸ jedn¹ kartê zaklêcia.";
                case Action.GET_TWO_BOOKS:
                    return "WeŸ dwie karty ksiêgi.";
                case Action.GET_TWO_ELIXIRS:
                    return "WeŸ dwie karty eliksiru.";
                case Action.GET_TWO_SPELLS:
                    return "WeŸ dwie karty zaklêcia.";
                case Action.GET_QUIDDITCH_CARD:
                    return "WeŸ jedn¹ kartê quidditcha.";
                case Action.ADD_TWO_LIVES: 
                    return "Jeœli masz miejsce, dobierz 2 ¿ycia.";
                case Action.ADD_THREE_LIVES:
                    return "Jeœli masz miejsce, dobierz 3 ¿ycia.";
                case Action.ADD_FOUR_LIVES:
                    return "Jeœli masz miejsce, dobierz 4 ¿ycia.";
                case Action.ADD_FIVE_LIVES:
                    return "Jeœli masz miejsce, dobierz 5 ¿yæ.";
                case Action.ADD_SIX_LIVES:
                    return "Jeœli masz miejsce, dobierz 6 ¿yæ.";
                case Action.CAN_MAKE_MISSION:
                    string result = "Je¿eli spe³niasz wszystkie warunki odpowiedniej misji (nr ";
                    for (int i = 0; i < missionNumbers.Count; i++)
                    {
                        result += $"{missionNumbers[i]}";
                        if (i < missionNumbers.Count - 1) result += ", ";
                    }
                    result += "), mo¿esz j¹ wykonaæ.";
                    return result;
                case Action.CAN_PASS_EXAMS:
                    return "Mo¿esz zdaæ egzaminy, jeœli posiadasz karty ksiêgi powi¹zane z t¹ klas¹.";
                case Action.CAN_USE_FIUU:
                    return "Mo¿esz transportowaæ siê za pomoc¹ proszka fiuu na 1 z 7 pól na planszy 2 (z wyj¹tkiem \"Cmentarza w Little Hangleton\"). " +
                        "Na koniec tury wróæ do miejsca, z którego siê przenios³eœ.";
                case Action.CAN_USE_PORTKEY:
                    return "Mo¿esz za pomoc¹ œwistoklika przenieœæ siê na pole \"Cmentarz w Little Hangleton\".";
                case Action.CAN_GET_THING:
                    return "Je¿eli na polu znajduje siê przedmiot, mo¿esz go podnieœæ. " +
                        "Mo¿na posiadaæ maksymalnie 6 przedmiotów, a podczas jednego ruchu mo¿na podnieœæ tylko 2 przedmioty. " + 
                        "Gracz ma mo¿liwoœæ wymiany przedmiotu.";
                case Action.CAN_START_QUIDDITCH:
                    return "Mo¿esz wyzwaæ dowolnego gracza do meczu Quidditcha.";
                case Action.CAN_TELEPORT:
                    return "Z tego pola mo¿esz siê teleportowaæ (za darmo) na odpowiednio po³¹czone z nim pole.";
            }

            return "";
        }

        public static string GetActionShortText(Action action, List<int> missionNumbers)
        {
            SpecialPower power = GameManager.GetMyPlayer().SpecialPower;

            switch (action)
            {
                case Action.COMMON_ROOM_CHOICE:
                    return "WeŸ jedn¹ dowoln¹ kartê lub zaproponuj wymianê przedmiotu pozosta³ym graczom.";
                case Action.FIGHT_FIELD:
                    return "Pobierz kartê z pojedynkiem i przeprowadŸ walkê.";
                case Action.GET_ONE_BOOK:
                    if (power == SpecialPower.GetOneMoreBook)
                        return "WeŸ dwie karty ksiêgi.";
                    else
                        return "WeŸ jedn¹ kartê ksiêgi.";
                case Action.GET_ONE_ELIXIR:
                    if (power == SpecialPower.GetOneMoreElixir)
                        return "WeŸ dwie karty eliksiru.";
                    else
                        return "WeŸ jedn¹ kartê eliksiru.";
                case Action.GET_ONE_EVENT:
                    return "WeŸ jedn¹ kartê wydarzenia.";
                case Action.GET_ONE_EXERCISE:
                    return "WeŸ jedn¹ kartê sprawowania.";
                case Action.GET_ONE_SPELL:
                    if (power == SpecialPower.GetOneMoreSpell)
                        return "WeŸ dwie karty zaklêcia.";
                    else
                        return "WeŸ jedn¹ kartê zaklêcia.";
                case Action.GET_TWO_BOOKS:
                    return "WeŸ dwie karty ksiêgi.";
                case Action.GET_TWO_ELIXIRS:
                    return "WeŸ dwie karty eliksiru.";
                case Action.GET_TWO_SPELLS:
                    return "WeŸ dwie karty zaklêcia.";
                case Action.GET_QUIDDITCH_CARD:
                    return "WeŸ jedn¹ kartê quidditcha.";
                case Action.ADD_TWO_LIVES:
                    return "Jeœli masz miejsce, dobierz 2 ¿ycia.";
                case Action.ADD_THREE_LIVES:
                    return "Jeœli masz miejsce, dobierz 3 ¿ycia.";
                case Action.ADD_FOUR_LIVES:
                    return "Jeœli masz miejsce, dobierz 4 ¿ycia.";
                case Action.ADD_FIVE_LIVES:
                    return "Jeœli masz miejsce, dobierz 5 ¿yæ.";
                case Action.ADD_SIX_LIVES:
                    return "Jeœli masz miejsce, dobierz 6 ¿yæ.";
                case Action.CAN_MAKE_MISSION:
                    string result = "Je¿eli spe³niasz wszystkie warunki odpowiedniej misji (nr ";
                    for (int i = 0; i < missionNumbers.Count; i++)
                    {
                        result += $"{missionNumbers[i]}";
                        if (i < missionNumbers.Count - 1) result += ", ";
                    }
                    result += "), mo¿esz j¹ wykonaæ.";
                    return result;
                case Action.CAN_PASS_EXAMS:
                    return "Mo¿esz zdaæ egzaminy, jeœli posiadasz karty ksiêgi powi¹zane z t¹ klas¹.";
                case Action.CAN_USE_FIUU:
                    return "Mo¿esz transportowaæ siê za pomoc¹ proszka fiuu planszê 2.";
                case Action.CAN_USE_PORTKEY:
                    return "Mo¿esz za pomoc¹ œwistoklika przenieœæ siê na pole \"Cmentarz w Little Hangleton\".";
                case Action.CAN_GET_THING:
                    return "Mo¿esz podnieœæ przedmiot, który znajduje siê na tym polu.";
                case Action.CAN_START_QUIDDITCH:
                    return "Mo¿esz wyzwaæ dowolnego gracza do meczu Quidditcha.";
                case Action.CAN_TELEPORT:
                    return "Z tego pola mo¿esz siê teleportowaæ (za darmo) na odpowiednio po³¹czone z nim pole.";
            }

            return "";
        }
    }
}