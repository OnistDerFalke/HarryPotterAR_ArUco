using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Instruction
    {
        public Dictionary<InstructionInfo, string> instructions;

        public enum InstructionInfo
        {
            GameDuration,                       //wszystkie pola
            GamePreparation,
            PlayerMove,                         //wszystkie pola
            MissionCompleting,                  //misje
            SpecialItems,                       //wszystkie pola
            FightWithPlayer,                    //wszystkie pola
            FightOnFightField,                  //pole walki
            MissionFight,                       //misje
            QuidditchPreparation,               //boisko do quidditcha
            QuidditchRound1, 
            QuidditchRound1_Actions1, 
            QuidditchRound1_Actions2, 
            QuidditchRound2
        }

        public Instruction()
        {
            InitializeInstruction();
        }

        public string GetInstructionInfoString(InstructionInfo info)
        {
            switch (info)
            {
                case InstructionInfo.GameDuration:
                    return "Czas trwania gry";
                case InstructionInfo.GamePreparation:
                    return "Przygotowanie gry";
                case InstructionInfo.PlayerMove:
                    return "Ruch gracza";
                case InstructionInfo.MissionCompleting:
                    return "Wypełnianie misji";
                case InstructionInfo.SpecialItems:
                    return "Przedmioty specjalne";
                case InstructionInfo.FightWithPlayer:
                    return "Pojedynek\nz innym graczem";
                case InstructionInfo.FightOnFightField:
                    return "Pojedynek\nna polu pojedynku";
                case InstructionInfo.MissionFight:
                    return "Pojedynek\npodczas misji";
                case InstructionInfo.QuidditchPreparation:
                    return "Przygotowanie\nmeczu quidditcha";
                case InstructionInfo.QuidditchRound1:
                    return "Runda 1\nmeczu quidditcha";
                case InstructionInfo.QuidditchRound1_Actions1:
                    return "Runda 1\nAkcje (1)";
                case InstructionInfo.QuidditchRound1_Actions2:
                    return "Runda 1\nAkcje (2)";
                case InstructionInfo.QuidditchRound2:
                    return "Runda 2\nmeczu quidditcha";
                default:
                    return "";
            }
        }

        private void InitializeInstruction()
        {
            instructions = new();

            instructions[InstructionInfo.GameDuration] = 
                "Gracz ma możliwość wybrania warunków zwycięstwa:\n\n" +
                "1. szybka rozgrywka: wykonanie 1 misji i zdobycie 210 punktów\n\n" +
                "2. klasyczna rozgrywka: wykonanie 2 misji i zdobycie 260 punktów\n\n" +
                "3. długa rozgrywka: wykonanie 3 misji i zdobycie 320 punktów";

            instructions[InstructionInfo.GamePreparation] =
                "1. Umieszczenie przedmiotów (łącznie 30) na polach z gwiazdką.\n\n" +
                "2. Każdy gracz wybiera 1 z 9 planszetek z dostępnymi postaciami.\n\n" +
                "3. Każdy gracz dostaje 2 żetony proszka fiuu, 1 świstoklik, odpowiednią liczbę żyć oraz karty określone na planszetce.\n\n" +
                "4. Każdy losuje 5 kart misji (jeżeli w grze bierze udział więcej niż 6 graczy, losują po 3).\n\n" +
                "5. Wszyscy gracze zaczynają na polu \"Start\".";

            instructions[InstructionInfo.PlayerMove] =
                "Gracz, przed każdym rzutem kośćmi, powinien zczytać pozycję gracza, " +
                "a następnie wybrać jedno z podświetlonych pól planszy i przesunąć na nie znacznik. \n\n" + 
                "Dodatkowo gracz powinien zatrzymać się na polu, na którym stoi inny gracz, " +
                "gdy mija go w swoim ruchu (nie obowiązuje go wtedy akcja powiązana z danym polem).\n\n" + 
                "Każde pole powiązane jest z pewną akcją, którą gracz powinien wykonać. " + 
                "Niezbędne informacje o polach będą dostępne po nakliknięciu na podświetlenia. " + 
                "Dodatkowo wyświetlana będzie informacja o aktualnym polu, na którym znajduje się gracz.";

            instructions[InstructionInfo.MissionCompleting] =
                "Gracz może spróbować wykonać misję, tylko gdy spełnia warunki widoczne na karcie misji:\n" +
                "1) znajduje się w odpowiednim miejscu na planszy\n" + 
                "2) posiada odpowiednie żetony lub karty (określone dla wariantu “Niepełnoletni czarodziej”)\n\n" +
                "Punkty przypisane do misji otrzymuje dopiero wtedy, gdy pokona przeciwnika określonego na karcie misji. " + 
                "Po wykorzystaniu przedmiotów przy misji, należy umieścić je na dowolnym polu z gwiazdką na planszy 2 " + 
                "(z niezapomnianymi miejscami ze świata HP).";

            instructions[InstructionInfo.SpecialItems] =
                "Po ich wykorzystaniu połóż je na 1 z 3 gwiazdek na ulicy Pokątnej. Są to\n\n" +
                "1) Czarna różdżka: +1 kość ataku w czasie walki\n\n" +
                "2) Kamień wskrzeszenia: +6PŻ podczas walki\n\n" +
                "3) Peleryna niewidka: ucieknij z miejsca walki na odległość 5 pól\n\n" +
                "4) Miecz Gryffindora: +1 kość ataku w czasie walki\n\n" +
                "5) Zmieniacz czasu: rozegraj dodatkową turę\n\n" +
                "6) Hardodziób: udaj się na dowolne pole na planszy 1 albo 2, a następnie wróć";

            instructions[InstructionInfo.FightWithPlayer] =
                "Każdy gracz rzuca 1 kością - ten kto ma mniejsza liczbę oczek, traci 1 PŻ.\n" +
                "Podczas pojedynku nie można użyć ani zaklęcia ani eliksiru.\n" +
                "\nPrzegrana: utrata 2 PŻ\n" +
                "Wygrana: zabranie przegranemu żetonu przedmiotu, świstoklika, proszka Fiuu lub zakrytej karty";

            instructions[InstructionInfo.FightOnFightField] =
                "Gracz nie może grać z postacią, którą sam jest - powinien wybrać inną kartę. " +
                "Na karcie pojedynku określona jest liczba żyć przeciwnika oraz jego sposób obrony.\n" +
                "\nWalczyć można na 2 sposoby:\n" +
                "1) kośćmi ataku (ich liczba widnieje na planszetce gracza): " + 
                "każda kość z wynikiem większym lub równym odporności przeciwnika, zabiera mu 1 PŻ\n" +
                "2) kartami zaklęć i eliksirów (można wykorzystać w dowolnym momencie ruchu): użycie karty obrony pozbawia przeciwnika ruchu\n" +
                "\nPrzegrana (pozostał 1 PŻ): gracz odrzuca 3 dowolne karty\n" +
                "Wygrana: gracz dobiera 1 zaklęcie, 1 eliksir i 1 księgę";

            instructions[InstructionInfo.MissionFight] =
                "Każda karta misji wskazuje postać, z którą należy stoczyć pojedynek. Karta przeciwnika posiada informacje o nim.\n" +
                "\nGracz ma możliwość wykorzystania:\n" +
                " - karty obrony (powoduje brak kontrataku)\n" + 
                " - karty ataku\n" +
                " - kości ataku: liczba oczek nie mniejsza od odporności przeciwnika, odbiera mu 1 PŻ\n" + 
                "Gracz podczas jednej walki może wykorzystać maksymalnie 5 kart zaklęć i eliksirów\n" +
                "Po śmierci przeciwnika, atakuje on gracza jeszcze raz swoją zdolnością specjalną\n" +
                "\nWygrana (przeciwnik nie ma żyć): misja wypełniona, zdobycie określonej na karcie misji liczby punktów domu\n" +
                "Przegrana (gracz ma 1 PŹ): gracz idzie do \"Dziurawego Kotła\", zdobywa 6PŹ i odkłada wszystkie karty eliksirów i zaklęć.";

            instructions[InstructionInfo.QuidditchPreparation] =
                "Na początku należy wybrać drużynę oraz rozstawić graczy:\n" + 
                "- 1 szukający na linii startowej (wokół boiska)\n" + 
                "- 1 obrońca na 1 z 3 pól pola bramkowego\n" + 
                "- 2 pałkarzy na 2 z 5 pól linii wokół pola bramkowego\n" + 
                "- 3 ścigających na 3 z 10 pól dwóch kolejnych linii\n" + 
                "\nKafel umieszcza się na środku boiska, a tłuczki po jednym na każdej połowie\n" + 
                "Mecz rozpoczyna gracz z większą liczbą oczek po rzucie 2 kośćmi\n" + 
                "Składa się on z dwóch rund: uzyskania 2 goli oraz zdobycia złotego znicza\n\n" + 
                "Wygrana: 40 punktów domu";

            instructions[InstructionInfo.QuidditchRound1] =
                "Gdy wszyscy ścigający danego gracza są wyeliminowani, przegrywa on automatycznie rundę 1.\n\n" + 
                "Gracze nie mogą wchodzić na to samo pole co inni gracze, z wyjątkiem sytuacji z przejęciem kafla.\n\n" + 
                "Obrońcy nie mogą opuścić pól bramkowych.\n\n" +
                "Każdy gracz na początku ruchu rzuca 2 kośćmi, które określają liczbę akcji.";

            instructions[InstructionInfo.QuidditchRound1_Actions1] =
                "Ruchy kosztujące 1 akcję:\n" +
                "\n1) Ruch wzdłuż białych kropkowanych linii.\n" +
                "\n2) Rzut trzymanym kaflem o 1 pole.\n" +
                "\n3) Przejęcie kafla (ścigający oraz kafel muszą być na tym samym polu).\n" +
                "\n4) strzelenie gola przez ścigającego znajdującego się na linii wokół pola bramkowego. " +
                "Strzał odbywa się za pomocą rzutu kością - jeśli rzucający ma więcej oczek niż obrońca, " +
                "następuje gol i zdobycie 1 karty quidditcha. Kafel trafia do obrońcy zarówno po próbie, " +
                "jak i po golu. Jeśli obrońca został wyeliminowany, kafel zostaje na losowym polu na linii wokół pola bramkowego.";

            instructions[InstructionInfo.QuidditchRound1_Actions2] =
                "Ruchy bez żadnego kosztu:\n" +
                "\n1) Podniesienie kafla/tłuczka odpowiednio przez ściagającego/pałkarza, będącego na tym samym polu, co piłka.\n" +
                "\n2) Rzut trzymanym tłuczkiem przez pałkarza na odległość określoną przez dodatkowy rzut kością " +
                "- można zrobić unik, ale jeśli wyrzucona liczba oczek przez przeciwnika wynosi mniej niż 4, postać zostaje wyeliminowana.";

            instructions[InstructionInfo.QuidditchRound2] =
                "Wygrywa ten, kto pierwszy okrąży boisko.\n\n" +
                "Szukający rozpoczynają na polu startowym linii wokół boiska.\n\n" +
                "Rozpoczyna gracz, który wygrał rundę 1.\n\n" +
                "Każdy gracz rzuca w swojej turze 2 kośćmi oraz może użyć maksymalnie 1 karty quidditcha.";
        }
    }
}
