using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public static class BoardInitializator
    {
        public static void InitBoard()
        {
            InitFirstBoard();
            InitSecondBoard();
            InitThirdBoard();

            GameManager.BoardManager.AddFields(GameManager.BoardManager.Boards[0].Fields);
            GameManager.BoardManager.AddFields(GameManager.BoardManager.Boards[1].Fields);
            GameManager.BoardManager.AddFields(GameManager.BoardManager.Boards[2].Fields);

            AddNeighborsToFieldOnFirstBoard();
            AddNeighborsToFieldOnSecondBoard();
            AddNeighborsToFieldOnThirdBoard();

            AddPortalFieldsToFieldOnFirstBoard();
            AddPortalFieldsToFieldOnSecondBoard();
            AddPortalFieldsToFieldOnThirdBoard();
            Debug.Log("send event");
            RequestBroker.requests.Add(new BoardInitializedRequest());
        }

        private static void InitFirstBoard()
        {
            int boardId = 0;

            // Start
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(3.7f, 14.4f), new Vector2(9.0f, 14.4f), new Vector2(9.0f, 11.0f), new Vector2(3.7f, 11.0f)), 0, 
                "Start", new List<Action>() { }));
            // Ręce
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(3.4f, 17.5f), new Vector2(5.9f, 17.5f), new Vector2(5.9f, 14.9f), new Vector2(3.4f, 14.9f)), 1,
                "Pole ze znakiem rąk", new List<Action>() { }));
            // Flaga
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(6.2f, 17.5f), new Vector2(8.9f, 17.5f), new Vector2(8.9f, 14.9f), new Vector2(6.2f, 14.9f)), 2,
                "Pole Hogwartu", new List<Action>() { }));
            // Wieża zegarowa
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.2f, 17.3f), new Vector2(11.8f, 17.3f), new Vector2(11.8f, 14.8f), new Vector2(9.2f, 14.8f)), 3,
                "Wieża zegarowa", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_USE_FIUU }, 
                isTower: true, isMissionField: true, missionNumbers: new List<int>() { 28, 29 } ));
            // Skrzydło szpitalne
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(13.3f, 17.0f), new Vector2(15.8f, 17.0f), new Vector2(15.8f, 15.2f), new Vector2(13.3f, 15.2f)), 4,
                "Skrzydło szpitalne", new List<Action>() { Action.ADD_SIX_LIVES }));
            // Posąg jednookiej wiedźmy
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(16.5f, 19.8f), new Vector2(20.0f, 19.8f), new Vector2(20.0f, 16.0f), new Vector2(16.5f, 16.0f)), 5,
                "Posąg jednookiej wiedźmy", new List<Action>() { Action.CAN_TELEPORT }));
            // Księga nad wieżą zegarową
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.4f, 19.5f), new Vector2(11.7f, 19.5f), new Vector2(11.7f, 17.8f), new Vector2(9.4f, 17.8f)), 6,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK }));
            // Pokój wspólny Slytherinu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(10.4f, 22.0f), 1.5f), 7,
                "Pokój wspólny Slytherinu", new List<Action>() { Action.COMMON_ROOM_CHOICE, Action.CAN_USE_FIUU }, isTower: true));
            // Gwiazdka obok pokoju wspólnego Slytherinu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(12.2f, 23.0f), new Vector2(14.5f, 23.0f), new Vector2(14.5f, 20.3f), new Vector2(12.2f, 20.3f)), 8,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Zaklęcie obok gwiazki obok pokoju wspólnego Slytherinu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(14.9f, 23.0f), new Vector2(17.2f, 23.0f), new Vector2(17.2f, 20.3f), new Vector2(14.9f, 20.3f)), 9,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Księga obok pokoju wspólnego Ravenclaw
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(17.6f, 22.9f), new Vector2(19.6f, 22.9f), new Vector2(19.6f, 20.4f), new Vector2(17.6f, 20.4f)), 10,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK }));
            // Pokój wspólny Ravenclaw
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(21.75f, 21.7f), 1.7f), 11,
                "Pokój wspólny Ravenclaw", new List<Action>() { Action.COMMON_ROOM_CHOICE, Action.CAN_USE_FIUU }, isTower: true));
            // Wydarzenie pod pokojem wspólnym Ravenclaw
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(20.3f, 19.8f), new Vector2(22.9f, 19.8f), new Vector2(22.9f, 17.7f), new Vector2(20.3f, 17.7f)), 12,
                "Pole z wydarzeniem", new List<Action>() { Action.GET_ONE_EVENT }));
            // Gwiazdka pod eliksirem pod pokojem wspólnym Ravenclaw
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(20.4f, 17.5f), new Vector2(23.0f, 17.5f), new Vector2(23.0f, 15.0f), new Vector2(20.4f, 15.0f)), 13,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Zaklęcie nad pokojem wspólnym Hufflepuff
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(20.4f, 14.5f), new Vector2(22.9f, 14.5f), new Vector2(22.9f, 12.5f), new Vector2(20.4f, 12.5f)), 14,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Pokój wspólny Hufflepuff
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(22.6f, 9.7f), 2.0f), 15,
                "Pokój wspólny Hufflepuff", new List<Action>() { Action.COMMON_ROOM_CHOICE, Action.CAN_USE_FIUU }, isTower: true));
            // Gabinet Dumbledore'a
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(20.6f, 6.8f), 1.5f), 16,
                "Gabinet Dumledore'a", new List<Action>() { Action.GET_TWO_SPELLS, Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 21 } ));
            // Sprawowanie obok pokoju wspólnego Hufflepuff
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(17.6f, 11.9f), new Vector2(20.0f, 11.9f), new Vector2(19.7f, 8.9f), new Vector2(17.7f, 8.9f)), 17,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE }));
            // Pojedynek obok sprawowania obok pokoju wspólnego Hufflepuff
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(14.8f, 11.8f), new Vector2(17.4f, 11.8f), new Vector2(17.3f, 8.9f), new Vector2(14.8f, 8.9f)), 18,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD }));
            // Gwiazdka obok pokoju wspólnego Gryffindoru
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(12.1f, 11.8f), new Vector2(14.6f, 11.8f), new Vector2(14.6f, 9.0f), new Vector2(12.1f, 9.0f)), 19,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Pokój wspólny Gryffindoru
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(10.5f, 10.3f), 1.6f), 20,
                "Pokój wspólny Gryffindoru", new List<Action>() { Action.COMMON_ROOM_CHOICE, Action.CAN_USE_FIUU }, isTower: true));
            // Eliksir nad pokojem wspólnym Gryffindoru
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.3f, 14.3f), new Vector2(11.8f, 14.3f), new Vector2(11.8f, 12.0f), new Vector2(9.3f, 12.4f)), 21,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Most obok pokoju wspólnego Hufflepuff
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(25.6f, 8.9f), new Vector2(28.6f, 8.2f), new Vector2(29.1f, 6.5f), new Vector2(24.9f, 7.5f)), 22,
                "Most", new List<Action>() { }));

            // Eliksir po lewej stronie wielkiej sali
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(29.2f, 8.6f), new Vector2(31.4f, 9.2f), new Vector2(32.1f, 6.5f), new Vector2(30.0f, 5.85f)), 23,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Gwiazdka nad eliksirem po lewej stronie wielkiej sali
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(28.4f, 11.1f), new Vector2(30.5f, 11.9f), new Vector2(31.2f, 9.5f), new Vector2(29.1f, 8.8f)), 24,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING }, missionNumbers: new List<int>() { 18 }));
            // Wydarzenie nad wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(31.0f, 12.0f), new Vector2(33.0f, 12.7f), new Vector2(33.9f, 10.3f), new Vector2(31.6f, 9.7f)), 25,
                "Pole z wydarzeniem", new List<Action>() { Action.GET_ONE_EVENT, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Gwiazdka między wydarzeniem i księgą nad wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(33.4f, 12.8f), new Vector2(35.4f, 13.5f), new Vector2(36.4f, 11.15f), new Vector2(34.3f, 10.5f)), 26,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING }, missionNumbers: new List<int>() { 18 }));
            // Księga nad wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(36.1f, 13.6f), new Vector2(38.3f, 14.3f), new Vector2(38.9f, 11.9f), new Vector2(37.0f, 11.2f)), 27,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Pojedynek obok księgi nad wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(38.6f, 14.4f), new Vector2(41.3f, 15.15f), new Vector2(42.0f, 12.9f), new Vector2(39.3f, 12.0f)), 28,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Gwiazdka nad kuchnią obok wielkiej sali
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.5f, 15.3f), new Vector2(43.9f, 16.1f), new Vector2(44.4f, 13.8f), new Vector2(42.3f, 13.0f)), 29,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Kuchnia obok wielkiej sali
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(42.5f, 12.8f), new Vector2(44.5f, 13.6f), new Vector2(45.25f, 11.0f), new Vector2(43.1f, 10.3f)), 30,
                "Kuchnia", new List<Action>() { Action.ADD_THREE_LIVES, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Eliksir pod kuchnią obok wielkiej sali
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(43.2f, 10.0f), new Vector2(45.4f, 10.8f), new Vector2(46.1f, 8.45f), new Vector2(43.9f, 7.7f)), 31,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Sprawowanie obok gwiazdki pod wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(40.6f, 9.3f), new Vector2(42.8f, 9.9f), new Vector2(43.4f, 7.6f), new Vector2(41.4f, 6.9f)), 32,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Gwiazdka obok sprawowania pod wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(38.2f, 8.4f), new Vector2(40.3f, 9.1f), new Vector2(40.9f, 6.75f), new Vector2(39.0f, 6.2f)), 33,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING }, missionNumbers: new List<int>() { 18 }));
            // Zaklęcie pod wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(35.5f, 7.6f), new Vector2(37.7f, 8.3f), new Vector2(38.4f, 6.0f), new Vector2(36.2f, 5.3f)), 34,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Sprawowanie obok zaklęcia pod wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(32.6f, 6.6f), new Vector2(35.1f, 7.5f), new Vector2(35.9f, 5.2f), new Vector2(33.2f, 4.5f)), 35,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Pojedynek obok sprawowania pod wielką salą
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(30.1f, 5.6f), new Vector2(32.2f, 6.3f), new Vector2(33.1f, 4.3f), new Vector2(30.55f, 3.6f)), 36,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD, Action.CAN_MAKE_MISSION }, missionNumbers: new List<int>() { 18 }));
            // Most nad wielką salą do części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(36.5f, 18.5f), new Vector2(38.2f, 18.5f), new Vector2(38.1f, 14.5f), new Vector2(36.8f, 14.4f)), 38,
                "Most", new List<Action>() { }));

            // Zaklęcie w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(36.3f, 21.3f), new Vector2(38.7f, 21.3f), new Vector2(38.7f, 19.0f), new Vector2(36.3f, 19.0f)), 39,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Gwiazdka obok gabinetu Snape'a w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(39.25f, 21.3f), new Vector2(41.2f, 21.3f), new Vector2(41.2f, 19.0f), new Vector2(39.25f, 19.0f)), 40,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Gabinet Snape'a w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(43.05f, 20.15f), 1.5f), 41,
                "Gabinet Snape'a", new List<Action>() { Action.GET_TWO_ELIXIRS, Action.CAN_USE_FIUU }, isTower: true));
            // Gwiazdka nad gabinetem Snape'a w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.8f, 24.0f), new Vector2(44.2f, 24.0f), new Vector2(44.2f, 21.8f), new Vector2(41.8f, 21.9f)), 42,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Czwarta wieża z pojedynkiem w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(43.05f, 25.7f), 1.5f), 43,
                "Wieża z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD, Action.CAN_USE_FIUU }, isTower: true));
            // Wydarzenie w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(39.1f, 26.9f), new Vector2(41.0f, 26.9f), new Vector2(41.4f, 24.4f), new Vector2(39.1f, 24.4f)), 44,
                "Pole z wydarzeniem", new List<Action>() { Action.GET_ONE_EVENT }));
            // Gwizdka obok wydarzenia w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(35.9f, 26.85f), new Vector2(38.5f, 26.85f), new Vector2(38.5f, 24.4f), new Vector2(35.9f, 24.4f)), 45,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Sprawowanie w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(33.0f, 26.85f), new Vector2(35.6f, 26.85f), new Vector2(35.6f, 24.4f), new Vector2(33.0f, 24.4f)), 46,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE }));
            // Gwizdka obok zachodniego skrzydła 3. piętro w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(30.6f, 26.95f), new Vector2(32.7f, 26.95f), new Vector2(32.7f, 24.4f), new Vector2(30.6f, 24.6f)), 47,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Zachodnie skrzydło 3. piętro w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(28.85f, 25.85f), 1.5f), 48,
                "Zachodnie skrzydło 3. piętro", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_USE_FIUU }, 
                isTower: true, isMissionField: true, missionNumbers: new List<int>() { 1, 2, 3 } ));
            // Eliksir pod zachodnim skrzydłem 3. piętro w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(27.4f, 24.3f), new Vector2(30.0f, 24.3f), new Vector2(30.0f, 21.75f), new Vector2(27.4f, 21.75f)), 49,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Eliksiry - sala w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(28.85f, 20.15f), 1.5f), 50,
                "Eliksiry (klasa)", new List<Action>() { Action.GET_TWO_ELIXIRS, Action.CAN_PASS_EXAMS, Action.CAN_USE_FIUU }, isTower: true));
            // Księga obok eliksirów w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(30.5f, 21.25f), new Vector2(32.7f, 21.25f), new Vector2(32.7f, 18.9f), new Vector2(30.5f, 18.9f)), 51,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK }));
            // Gwiazdka obok księgi w części z nauką Eliksirów
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(33.1f, 21.3f), new Vector2(35.7f, 21.3f), new Vector2(35.7f, 18.9f), new Vector2(33.1f, 18.9f)), 52,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Kamienny most
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(23.4f, 23.3f), new Vector2(27.2f, 23.5f), new Vector2(27.3f, 22.4f), new Vector2(23.9f, 22.2f)), 54,
                "Kamienny most", new List<Action>() { }));
            // Most między częścią z nauką Eliksirów a wieżą centralną
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(36.5f, 29.7f), new Vector2(38.0f, 29.7f), new Vector2(38.0f, 27.0f), new Vector2(36.5f, 27.0f)), 55,
                "Most", new List<Action>() { }));

            // Wieża centralna
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(37.4f, 31.7f), 1.6f), 56,
                "Wieża centralna", new List<Action>() { Action.CAN_USE_FIUU }, isTower: true));
            // Zaklęcie obok wieży centralnej
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(39.3f, 32.4f), new Vector2(41.3f, 32.45f), new Vector2(41.4f, 29.9f), new Vector2(38.6f, 29.8f)), 57,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Transmutacja
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(43.1f, 31.1f), 1.5f), 58,
                "Transmutacja (klasa)", new List<Action>() { Action.GET_ONE_SPELL, Action.CAN_PASS_EXAMS, Action.CAN_USE_FIUU}, isTower: true));
            // Pojedynek nad transmutacją
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.5f, 35.3f), new Vector2(44.2f, 35.3f), new Vector2(44.1f, 33.0f), new Vector2(41.55f, 32.8f)), 59,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD }));
            // Gwiazdka nad pojedynkiem nad transmutacją
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.55f, 37.9f), new Vector2(44.1f, 37.9f), new Vector2(44.1f, 35.7f), new Vector2(41.55f, 35.7f)), 60,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Wydarzenie obok dzwonnicy z zaklęciem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.6f, 39.4f), new Vector2(44.2f, 40.7f), new Vector2(44.2f, 38.4f), new Vector2(41.6f, 38.4f)), 61,
                "Pole z wydarzeniem", new List<Action>() { Action.GET_ONE_EVENT }));
            // Dzwonnica z zaklęciem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(42.0f, 41.5f), 1.7f), 62,
                "Dzwonnica z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL, Action.CAN_USE_FIUU }, isTower: true));
            // Sprawowanie obok dzwonnicy z zaklęciem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(41.7f, 46.4f), new Vector2(44.2f, 46.4f), new Vector2(44.2f, 44.1f), new Vector2(41.7f, 44.1f)), 63,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE }));
            // Zielarstwo
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(44.5f, 46.4f), new Vector2(46.7f, 46.4f), new Vector2(46.7f, 43.9f), new Vector2(44.5f, 43.9f)), 64,
                "Zielarstwo (klasa)", new List<Action>() { Action.GET_TWO_ELIXIRS, Action.CAN_PASS_EXAMS }));
            // +4PŻ obok zielarstwa
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(44.5f, 43.5f), new Vector2(46.8f, 43.5f), new Vector2(46.8f, 41.5f), new Vector2(44.5f, 41.5f)), 65,
                "Pole z +4PŻ", new List<Action>() { Action.ADD_FOUR_LIVES }));
            // Eliksir pod +4PŹ (ślepy zaułek)
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(44.5f, 41.2f), new Vector2(46.8f, 41.2f), new Vector2(46.8f, 38.6f), new Vector2(44.5f, 38.6f)), 66,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Gwiazdka między dzwonnicami
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(35.3f, 43.85f), new Vector2(39.3f, 43.85f), new Vector2(39.3f, 41.4f), new Vector2(35.3f, 41.4f)), 67,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Dzwonnica z eliksirem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(32.8f, 41.6f), 1.7f), 68,
                "Dzwonnica z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR, Action.CAN_USE_FIUU }, isTower: true));
            // Wierzba bijąca
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(27.8f, 46.9f), new Vector2(33.3f, 46.9f), new Vector2(33.3f, 44.2f), new Vector2(27.8f, 44.2f)), 69,
                "Wierzba bijąca", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_TELEPORT }, 
                isMissionField: true, missionNumbers: new List<int>() { 8 } ));
            // Puste pole między dzwonnicą i zaklęciami i urokami
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(30.75f, 40.8f), new Vector2(33.0f, 39.5f), new Vector2(33.0f, 38.4f), new Vector2(30.7f, 39.0f)), 130,
                "Pole", new List<Action>() { }));
            // Zaklęcia i uroki
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(31.5f, 37.1f), 1.5f), 70,
                "Zaklęcia i uroki (klasa)", new List<Action>() { Action.GET_ONE_SPELL, Action.CAN_PASS_EXAMS, Action.CAN_USE_FIUU }, isTower: true));
            // Zaklęcie pod zaklęciami i urokami
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(30.35f, 35.3f), new Vector2(33.0f, 35.3f), new Vector2(33.0f, 33.0f), new Vector2(30.35f, 33.0f)), 71,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Wieża ze sprawowaniem obok pokoju życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(31.7f, 31.3f), 1.5f), 72,
                "Wieża ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE, Action.CAN_USE_FIUU }, isTower: true));
            // Eliksir obok wieży centralnej
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(33.45f, 32.5f), new Vector2(35.4f, 32.55f), new Vector2(36.0f, 29.8f), new Vector2(33.6f, 29.8f)), 73,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));

            // Księga pod pokojem życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(27.3f, 32.45f), new Vector2(30.0f, 32.4f), new Vector2(30.0f, 30.0f), new Vector2(27.5f, 30.0f)), 74,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK }));
            // Gwizdka obok księgi pod pokojem życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(24.7f, 32.45f), new Vector2(27.05f, 32.45f), new Vector2(27.05f, 29.95f), new Vector2(24.7f, 29.95f)), 75,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Pojedynek obok obrony przed czarną magią
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(21.9f, 32.5f), new Vector2(24.3f, 32.5f), new Vector2(24.3f, 29.9f), new Vector2(21.9f, 29.9f)), 76,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD }));
            // Obrona przed czarną magią
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(19.7f, 30.9f), 2.0f), 77,
                "Obrona przed czarną magią (klasa)", new List<Action>() { Action.GET_ONE_SPELL, Action.CAN_MAKE_MISSION, Action.CAN_PASS_EXAMS, Action.CAN_USE_FIUU }, 
                isTower: true, isMissionField: true, missionNumbers: new List<int>() { 17 } ));
            // Most wiszący
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(18.4f, 28.45f), new Vector2(20.6f, 28.3f), new Vector2(20.0f, 23.4f), new Vector2(18.25f, 23.4f)), 78,
                "Most wiszący", new List<Action>() { Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 30 } ));
            // Gwiazdka obok Komnaty Tajemnic
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(19.0f, 35.4f), new Vector2(21.5f, 35.6f), new Vector2(21.45f, 32.9f), new Vector2(19.0f, 32.85f)), 79,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Komnata Tajemnic
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(15.8f, 35.5f), new Vector2(18.65f, 35.4f), new Vector2(18.65f, 33.0f), new Vector2(15.9f, 33.0f)), 80,
                "Komnata Tajemnic", new List<Action>() { Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 5, 6, 25 } ));
            // Pokój życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(21.8f, 35.6f), new Vector2(30.1f, 35.6f), new Vector2(30.2f, 32.6f), new Vector2(21.8f, 32.6f)), 81,
                "Pokój życzeń", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_TELEPORT }, 
                isMissionField: true, missionNumbers: new List<int>() { 26 } ));
            // Wieża z eliksirem obok pokoju życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(20.1f, 37.1f), 1.5f), 82,
                "Wieża z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR, Action.CAN_USE_FIUU }, isTower: true));
            // Sprawowanie nad pokojem życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(21.9f, 38.2f), new Vector2(24.2f, 38.2f), new Vector2(24.1f, 35.75f), new Vector2(21.7f, 35.75f)), 83,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE }));
            // Gwizadka nad pokojem życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(24.5f, 38.15f), new Vector2(27.1f, 38.15f), new Vector2(27.1f, 35.85f), new Vector2(24.5f, 35.85f)), 84,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));
            // Zaklęcie nad pokojem życzeń
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(27.4f, 38.1f), new Vector2(30.0f, 38.1f), new Vector2(30.0f, 35.9f), new Vector2(27.6f, 35.9f)), 85,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));

            // Pojedynek pod biblioteką
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(19.0f, 40.7f), new Vector2(21.4f, 40.8f), new Vector2(21.3f, 38.7f), new Vector2(19.0f, 38.9f)), 86,
                "Pole z pojedynkiem", new List<Action>() { Action.FIGHT_FIELD }));
            // Biblioteka
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(20.1f, 42.6f), 1.8f), 87,
                "Biblioteka", new List<Action>() { Action.GET_TWO_ELIXIRS, Action.CAN_USE_FIUU }, isTower: true));
            // Dział ksiąg zakazanych
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(18.9f, 46.3f), new Vector2(21.45f, 46.3f), new Vector2(21.4f, 44.5f), new Vector2(18.9f, 44.4f)), 88,
                "Dział ksiąg zakazanych", new List<Action>() { Action.GET_ONE_BOOK }));
            // Eliksir obok biblioteki
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(22.0f, 43.8f), new Vector2(24.2f, 43.8f), new Vector2(24.2f, 41.3f), new Vector2(22.0f, 41.3f)), 89,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Wydarzenie obok eliksiru obok biblioteki
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(24.6f, 43.75f), new Vector2(26.9f, 43.75f), new Vector2(26.9f, 41.3f), new Vector2(24.6f, 41.3f)), 90,
                "Pole z wydarzeniem", new List<Action>() { Action.GET_ONE_EVENT }));
            // Gwiazdka obok dzwonnicy z eliksirem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(27.3f, 43.8f), new Vector2(30.1f, 43.8f), new Vector2(30.1f, 41.3f), new Vector2(27.3f, 41.3f)), 91,
                "Pole z gwiazdką", new List<Action>() { Action.CAN_GET_THING }));

            // Zakazany las
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(15.6f, 39.9f), 1.5f), 92,
                "Zakazany las", new List<Action>() { Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 4, 27 } ));
            // Boisko do quidditcha
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(15.2f, 45.4f), 1.8f), 93,
                "Boisko do quidditcha", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_USE_PORTKEY, Action.CAN_START_QUIDDITCH }, 
                isMissionField: true, isQuidditchPitch: true, missionNumbers: new List<int>() { 7, 13, 16 } ));
            // Eliksir obok zakazanego lasu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(11.4f, 39.6f), 1.5f), 94,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Chatka Hagrida
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(10.7f, 43.1f), 1.7f), 95,
                "Chatka Hagrida", new List<Action>() { Action.CAN_MAKE_MISSION, Action.GET_ONE_SPELL_AND_ONE_ELIXIR }, 
                isMissionField: true, missionNumbers: new List<int>() { 10 } ));
            // Smocza Arena
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(5.9f, 45.0f), 2.2f), 96,
                "Smocza Arena", new List<Action>() { Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 11 } ));
            // Sprawowanie obok opieki nad magicznymi stworzeniami
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(9.7f, 35.8f), 1.1f), 97,
                "Pole ze sprawowaniem", new List<Action>() { Action.GET_ONE_EXERCISE }));
            // Opieka nad magicznymi stworzeniami
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(6.7f, 37.3f), 1.6f), 98,
                "Opieka nad magicznymi stworzeniami (klasa)", new List<Action>() { Action.GET_TWO_ELIXIRS, Action.CAN_PASS_EXAMS }));
            // +4PŹ niedaleko Jeziora
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(8.3f, 32.3f), 1.3f), 99,
                "Pole z +4PŻ", new List<Action>() { Action.ADD_FOUR_LIVES }));
            // +5PŹ obok Jeziora
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(6.5f, 28.2f), 1.3f), 100,
                "Pole z +5PŻ", new List<Action>() { Action.ADD_FIVE_LIVES }));
            // Jezioro
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.6f, 28.9f), new Vector2(16.4f, 29.8f), new Vector2(17.4f, 25.3f), new Vector2(8.7f, 25.3f)), 101,
                "Jezioro", new List<Action>() { Action.CAN_MAKE_MISSION }, 
                isMissionField: true, missionNumbers: new List<int>() { 9, 12 } ));
        }

        private static void InitSecondBoard()
        {
            int boardId = 1;

            // Dziurawy Kocioł
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(19.3f, 14.6f), 2.0f), 102,
                "Dziurawy Kocioł", new List<Action>() { Action.ADD_TWO_LIVES }));
            // Sklep z kotłami Madame Potage
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(12.9f, 16.5f), 1.5f), 103,
                "Sklep z kotłami Madame Potage", new List<Action>() { Action.GET_TWO_SPELLS }));
            // Markowy sprzęt do quidditcha
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(17.5f, 18.0f), 1.6f), 104,
                "Markowy sprzęt do quidditcha", new List<Action>() { Action.GET_QUIDDITCH_CARD }));
            // Esy i floresy
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(14.0f, 20.4f), 2.0f), 105,
                "Esy i floresy", new List<Action>() { Action.GET_TWO_BOOKS }));
            // Bank Gringotta
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(8.9f, 20.7f), 2.2f), 106,
                "Bank Gringotta", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_TELEPORT }, 
                isMissionField: true, missionNumbers: new List<int>() { 24 } ));
            // Zaklęcie na ulicy śmiertelnego Nokturnu obok +3PŻ
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(12.2f, 13.1f), 1.3f), 107,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // +3PŻ na ulicy śmiertelnego Nokturnu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(8.9f, 13.0f), 1.5f), 108,
                "Pole z +3PŻ", new List<Action>() { Action.ADD_THREE_LIVES }));
            // +2PŻ na ulicy śmiertelnego Nokturnu
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(11.4f, 8.0f), 1.6f), 109,
                "Pole z +2PŻ", new List<Action>() { Action.ADD_TWO_LIVES }));
            // Zaklęcie na ulicy śmiertelnego Nokturnu obok +2PŻ
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(15.6f, 6.3f), 1.5f), 110,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Borgin & Burkes
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(18.3f, 3.8f), 2.0f), 111,
                "Borgin & Burkes", new List<Action>() { Action.CAN_GET_THING }));
            // Magiczna Menażeria
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(4.1f, 18.1f), 1.8f), 112,
                "Magiczna Menażeria", new List<Action>() { Action.CAN_GET_THING }));
            // Czarodziejskie niespodzianki Gambola i Japesa
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(3.4f, 13.0f), 2.0f), 113,
                "Czarodziejskie niespodzianki Gambola i Japesa", new List<Action>() { Action.ADD_TWO_LIVES }));
            // Zaklęcie obok czarodziejskich niespodzianek Gambola i Japesa
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(2.0f, 9.4f), 1.3f), 114,
                "Pole z zaklęciem", new List<Action>() { Action.GET_ONE_SPELL }));
            // Księga obok zaklęcia obok czarodziejskich niespodzianek Gambola i Japesa
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(9.1f, 9.9f), 1.3f), 115,
                "Pole z księgą", new List<Action>() { Action.GET_ONE_BOOK }));
            // Eliksir obok sklepu Ollivandera
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(9.2f, 3.6f), 1.3f), 116,
                "Pole z eliksirem", new List<Action>() { Action.GET_ONE_ELIXIR }));
            // Sklep Ollivandera
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(4.7f, 5.2f), 2.0f), 117,
                "Sklep Ollivandera", new List<Action>() { Action.GET_TWO_SPELLS, Action.CAN_GET_THING }));
        }

        private static void InitThirdBoard()
        {
            int boardId = 2;

            // Miodowe Królestwo
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(23.0f, 3.5f), 1.3f), 118,
                "Miodowe Królestwo", new List<Action>() { Action.CAN_TELEPORT }));
            // Sklep Zonka
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(18.7f, 3.8f), 1.3f), 119,
                "Sklep Zonka", new List<Action>() { Action.CAN_GET_THING }));
            // Wrzeszcząca Chata
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(21.3f, 6.6f), 1.4f), 120,
                "Wrzeszcząca Chata", new List<Action>() { Action.CAN_TELEPORT }));
            // Pod świńskim łbem
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Circle(new Vector2(17.7f, 7.3f), 1.3f), 121,
                "Pod świńskim łbem", new List<Action>() { Action.CAN_TELEPORT }));

            // Bank Gringotta
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.1f, 8.3f), new Vector2(16.0f, 8.3f), new Vector2(16.0f, 1.4f), new Vector2(9.1f, 1.4f)), 122,
                "Bank Gringotta", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING, Action.CAN_TELEPORT }, 
                isMissionField: true, isFiuuField: true, missionNumbers: new List<int>() { 24 } ));
            // Ministerstwo Magii
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(1.1f, 8.3f), new Vector2(8.0f, 8.3f), new Vector2(8.0f, 1.4f), new Vector2(1.1f, 1.4f)), 123,
                "Ministerstwo Magii", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING, Action.CAN_TELEPORT }, 
                isMissionField: true, isFiuuField: true, missionNumbers: new List<int>() { 22 } ));
            // Grimmauld Place 12
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(17.2f, 16.3f), new Vector2(24.1f, 16.3f), new Vector2(24.1f, 9.4f), new Vector2(17.2f, 9.4f)), 124,
                "Grimmauld Place 12", new List<Action>() { Action.CAN_GET_THING }, isFiuuField: true));
            // Nora
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.1f, 16.3f), new Vector2(16.0f, 16.3f), new Vector2(16.0f, 9.4f), new Vector2(9.1f, 9.4f)), 125,
                "Nora", new List<Action>() { Action.CAN_GET_THING }, isFiuuField: true));
            // Sala przepowiedni
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(1.1f, 16.3f), new Vector2(8.0f, 16.3f), new Vector2(8.0f, 9.4f), new Vector2(1.1f, 9.4f)), 126,
                "Sala przepowiedni", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING, Action.CAN_TELEPORT }, 
                isMissionField: true, isFiuuField: true, missionNumbers: new List<int>() { 19, 20 } ));
            // Forest of dean
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(17.2f, 24.2f), new Vector2(24.1f, 24.2f), new Vector2(24.1f, 17.3f), new Vector2(17.2f, 17.3f)), 127,
                "Forest of dean", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING }, 
                isMissionField: true, isFiuuField: true, missionNumbers: new List<int>() { 23 } ));
            // Dwór Malfoya
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(9.1f, 24.2f), new Vector2(16.0f, 24.2f), new Vector2(16.0f, 17.3f), new Vector2(9.1f, 17.3f)), 128,
                "Dwór Malfoya", new List<Action>() { Action.CAN_GET_THING }, isFiuuField: true));
            // Cmentarz w Little Hangleton
            GameManager.BoardManager.Boards[boardId].Fields.Add(new Field(boardId, new Quadrangle(
                new Vector2(1.1f, 24.2f), new Vector2(8.0f, 24.2f), new Vector2(8.0f, 17.3f), new Vector2(1.1f, 17.3f)), 129,
                "Cmentarz w Little Hangleton", new List<Action>() { Action.CAN_MAKE_MISSION, Action.CAN_GET_THING }, 
                isMissionField: true, isPortkeyField: true, missionNumbers: new List<int>() { 14, 15 } ));
        }

        private static void AddNeighborsToFieldOnFirstBoard()
        {
            GameManager.BoardManager.GetFieldById(0).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 1 }));
            GameManager.BoardManager.GetFieldById(1).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 0, 2, 102 }));
            GameManager.BoardManager.GetFieldById(2).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 1, 3 }));
            GameManager.BoardManager.GetFieldById(3).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 2, 4, 6, 21 }));
            GameManager.BoardManager.GetFieldById(4).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 3, 5 }));
            GameManager.BoardManager.GetFieldById(5).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 4 }));
            GameManager.BoardManager.GetFieldById(6).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 3, 7 }));
            GameManager.BoardManager.GetFieldById(7).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 6, 8 }));
            GameManager.BoardManager.GetFieldById(8).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 7, 9 }));
            GameManager.BoardManager.GetFieldById(9).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 8, 10 }));
            GameManager.BoardManager.GetFieldById(10).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 9, 11, 78 }));
            GameManager.BoardManager.GetFieldById(11).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 10, 12 }));
            GameManager.BoardManager.GetFieldById(12).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 11, 13 }));
            GameManager.BoardManager.GetFieldById(13).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 12, 14 }));
            GameManager.BoardManager.GetFieldById(14).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 13, 15 }));
            GameManager.BoardManager.GetFieldById(15).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 14, 16, 17, 22 }));
            GameManager.BoardManager.GetFieldById(16).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 15 }));
            GameManager.BoardManager.GetFieldById(17).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 15, 18 }));
            GameManager.BoardManager.GetFieldById(18).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 17, 19 }));
            GameManager.BoardManager.GetFieldById(19).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 18, 20 }));
            GameManager.BoardManager.GetFieldById(20).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 19, 21 }));
            GameManager.BoardManager.GetFieldById(21).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 3, 20 }));
            GameManager.BoardManager.GetFieldById(22).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 15, 23 }));
            GameManager.BoardManager.GetFieldById(23).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 22, 24, 36 }));
            GameManager.BoardManager.GetFieldById(24).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 23, 25 }));
            GameManager.BoardManager.GetFieldById(25).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 24, 26 }));
            GameManager.BoardManager.GetFieldById(26).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 25, 27 }));
            GameManager.BoardManager.GetFieldById(27).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 26, 28, 38 }));
            GameManager.BoardManager.GetFieldById(28).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 27, 29 }));
            GameManager.BoardManager.GetFieldById(29).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 28, 30 }));
            GameManager.BoardManager.GetFieldById(30).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 29, 31 }));
            GameManager.BoardManager.GetFieldById(31).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 30, 32 }));
            GameManager.BoardManager.GetFieldById(32).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 31, 33 }));
            GameManager.BoardManager.GetFieldById(33).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 32, 34 }));
            GameManager.BoardManager.GetFieldById(34).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 33, 35 }));
            GameManager.BoardManager.GetFieldById(35).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 34, 36 }));
            GameManager.BoardManager.GetFieldById(36).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 35, 23 }));
            GameManager.BoardManager.GetFieldById(38).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 27, 39 }));
            GameManager.BoardManager.GetFieldById(39).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 38, 40, 52 }));
            GameManager.BoardManager.GetFieldById(40).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 39, 41 }));
            GameManager.BoardManager.GetFieldById(41).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 40, 42 }));
            GameManager.BoardManager.GetFieldById(42).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 41, 43 }));
            GameManager.BoardManager.GetFieldById(43).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 42, 44 }));
            GameManager.BoardManager.GetFieldById(44).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 43, 45 }));
            GameManager.BoardManager.GetFieldById(45).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 44, 46, 55 }));
            GameManager.BoardManager.GetFieldById(46).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 45, 47 }));
            GameManager.BoardManager.GetFieldById(47).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 46, 48 }));
            GameManager.BoardManager.GetFieldById(48).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 47, 49 }));
            GameManager.BoardManager.GetFieldById(49).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 48, 50, 54 }));
            GameManager.BoardManager.GetFieldById(50).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 49, 51 }));
            GameManager.BoardManager.GetFieldById(51).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 50, 52 }));
            GameManager.BoardManager.GetFieldById(52).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 39, 51 }));
            GameManager.BoardManager.GetFieldById(54).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 11, 49 }));
            GameManager.BoardManager.GetFieldById(55).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 45, 56 }));
            GameManager.BoardManager.GetFieldById(56).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 55, 57, 73 }));
            GameManager.BoardManager.GetFieldById(57).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 56, 58 }));
            GameManager.BoardManager.GetFieldById(58).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 57, 59 }));
            GameManager.BoardManager.GetFieldById(59).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 58, 60 }));
            GameManager.BoardManager.GetFieldById(60).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 59, 61 }));
            GameManager.BoardManager.GetFieldById(61).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 60, 62 }));
            GameManager.BoardManager.GetFieldById(62).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 61, 63, 67 }));
            GameManager.BoardManager.GetFieldById(63).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 62, 64 }));
            GameManager.BoardManager.GetFieldById(64).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 63, 65 }));
            GameManager.BoardManager.GetFieldById(65).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 64, 66 }));
            GameManager.BoardManager.GetFieldById(66).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 65 }));
            GameManager.BoardManager.GetFieldById(67).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 62, 68 }));
            GameManager.BoardManager.GetFieldById(68).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 67, 69, 70, 91 }));
            GameManager.BoardManager.GetFieldById(69).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 68 }));
            GameManager.BoardManager.GetFieldById(70).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 68, 71, 85 }));
            GameManager.BoardManager.GetFieldById(71).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 70, 72 }));
            GameManager.BoardManager.GetFieldById(72).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 71, 73, 74 }));
            GameManager.BoardManager.GetFieldById(73).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 72, 56 }));
            GameManager.BoardManager.GetFieldById(74).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 72, 75 }));
            GameManager.BoardManager.GetFieldById(75).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 74, 76 }));
            GameManager.BoardManager.GetFieldById(76).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 75, 77 }));
            GameManager.BoardManager.GetFieldById(77).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 76, 78, 79 }));
            GameManager.BoardManager.GetFieldById(78).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 10, 77 }));
            GameManager.BoardManager.GetFieldById(79).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 77, 80, 81, 82 }));
            GameManager.BoardManager.GetFieldById(80).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 79 }));
            GameManager.BoardManager.GetFieldById(81).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 79 }));
            GameManager.BoardManager.GetFieldById(82).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 79, 83, 86 }));
            GameManager.BoardManager.GetFieldById(83).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 82, 84 }));
            GameManager.BoardManager.GetFieldById(84).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 83, 85 }));
            GameManager.BoardManager.GetFieldById(85).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 84, 70 }));
            GameManager.BoardManager.GetFieldById(86).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 82, 87, 92 }));
            GameManager.BoardManager.GetFieldById(87).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 86, 88, 89 }));
            GameManager.BoardManager.GetFieldById(88).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 87 }));
            GameManager.BoardManager.GetFieldById(89).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 87, 90 }));
            GameManager.BoardManager.GetFieldById(90).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 89, 91 }));
            GameManager.BoardManager.GetFieldById(91).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 68, 90 }));
            GameManager.BoardManager.GetFieldById(92).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 86, 93, 94 }));
            GameManager.BoardManager.GetFieldById(93).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 92 }));
            GameManager.BoardManager.GetFieldById(94).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 92, 95, 96, 97 }));
            GameManager.BoardManager.GetFieldById(95).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 94, 96 }));
            GameManager.BoardManager.GetFieldById(96).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 95, 94 }));
            GameManager.BoardManager.GetFieldById(97).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 94, 98, 99 }));
            GameManager.BoardManager.GetFieldById(98).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 97 }));
            GameManager.BoardManager.GetFieldById(99).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 97, 100 }));
            GameManager.BoardManager.GetFieldById(100).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 99, 101, 118 }));
            GameManager.BoardManager.GetFieldById(101).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 100 }));
        }

        private static void AddNeighborsToFieldOnSecondBoard()
        {
            GameManager.BoardManager.GetFieldById(102).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 1, 103 }));
            GameManager.BoardManager.GetFieldById(103).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 102, 104 }));
            GameManager.BoardManager.GetFieldById(104).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 103, 105 }));
            GameManager.BoardManager.GetFieldById(105).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 104, 106 }));
            GameManager.BoardManager.GetFieldById(106).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 105, 107, 112, 113, 114 }));
            GameManager.BoardManager.GetFieldById(107).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 106, 108, 109, 112, 113, 114 }));
            GameManager.BoardManager.GetFieldById(108).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 107 }));
            GameManager.BoardManager.GetFieldById(109).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 107, 110 }));
            GameManager.BoardManager.GetFieldById(110).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 109, 111 }));
            GameManager.BoardManager.GetFieldById(111).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 110 }));
            GameManager.BoardManager.GetFieldById(112).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 106, 107, 113, 114 }));
            GameManager.BoardManager.GetFieldById(113).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 106, 107, 112, 114 }));
            GameManager.BoardManager.GetFieldById(114).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 106, 107, 112, 113, 115 }));
            GameManager.BoardManager.GetFieldById(115).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 114, 116 }));
            GameManager.BoardManager.GetFieldById(116).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 115, 117 }));
            GameManager.BoardManager.GetFieldById(117).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 116 }));
        }

        private static void AddNeighborsToFieldOnThirdBoard()
        {
            GameManager.BoardManager.GetFieldById(118).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 100, 119, 120 }));
            GameManager.BoardManager.GetFieldById(119).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 118, 120 }));
            GameManager.BoardManager.GetFieldById(120).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 118, 119, 121 }));
            GameManager.BoardManager.GetFieldById(121).AddNeighbors(GameManager.BoardManager.GetFieldsByIds(new List<int>() { 120 }));
        }

        private static void AddPortalFieldsToFieldOnFirstBoard()
        {
            GameManager.BoardManager.GetFieldById(5).SetPortalField(GameManager.BoardManager.GetFieldById(118));
            GameManager.BoardManager.GetFieldById(69).SetPortalField(GameManager.BoardManager.GetFieldById(120));
            GameManager.BoardManager.GetFieldById(81).SetPortalField(GameManager.BoardManager.GetFieldById(121));
        }

        private static void AddPortalFieldsToFieldOnSecondBoard()
        {
            GameManager.BoardManager.GetFieldById(106).SetPortalField(GameManager.BoardManager.GetFieldById(122));
        }

        private static void AddPortalFieldsToFieldOnThirdBoard()
        {
            GameManager.BoardManager.GetFieldById(118).SetPortalField(GameManager.BoardManager.GetFieldById(5));
            GameManager.BoardManager.GetFieldById(120).SetPortalField(GameManager.BoardManager.GetFieldById(69));
            GameManager.BoardManager.GetFieldById(121).SetPortalField(GameManager.BoardManager.GetFieldById(81));
            GameManager.BoardManager.GetFieldById(122).SetPortalField(GameManager.BoardManager.GetFieldById(106));
            GameManager.BoardManager.GetFieldById(123).SetPortalField(GameManager.BoardManager.GetFieldById(126));
            GameManager.BoardManager.GetFieldById(126).SetPortalField(GameManager.BoardManager.GetFieldById(123));
        }
    }
}
