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
                    return "Wype≥nianie misji";
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
                "Gracz ma moøliwoúÊ wybrania warunkÛw zwyciÍstwa:\n\n" +
                "1. szybka rozgrywka: wykonanie 1 misji i zdobycie 210 punktÛw\n\n" +
                "2. klasyczna rozgrywka: wykonanie 2 misji i zdobycie 260 punktÛw\n\n" +
                "3. d≥uga rozgrywka: wykonanie 3 misji i zdobycie 320 punktÛw";

            instructions[InstructionInfo.GamePreparation] =
                "1. Umieszczenie przedmiotÛw (≥πcznie 30) na polach z gwiazdkπ.\n\n" +
                "2. Kaødy gracz wybiera 1 z 9 planszetek z dostÍpnymi postaciami.\n\n" +
                "3. Kaødy gracz dostaje 2 øetony proszka fiuu, 1 úwistoklik, odpowiedniπ liczbÍ øyÊ oraz karty okreúlone na planszetce.\n\n" +
                "4. Kaødy losuje 5 kart misji (jeøeli w grze bierze udzia≥ wiÍcej niø 6 graczy, losujπ po 3).\n\n" +
                "5. Wszyscy gracze zaczynajπ na polu \"Start\".";

            instructions[InstructionInfo.PlayerMove] =
                "Gracz, przed kaødym rzutem koúÊmi, powinien zczytaÊ pozycjÍ gracza, " +
                "a nastÍpnie wybraÊ jedno z podúwietlonych pÛl planszy i przesunπÊ na nie znacznik. \n\n" + 
                "Dodatkowo gracz powinien zatrzymaÊ siÍ na polu, na ktÛrym stoi inny gracz, " +
                "gdy mija go w swoim ruchu (nie obowiπzuje go wtedy akcja powiπzana z danym polem).\n\n" + 
                "Kaøde pole powiπzane jest z pewnπ akcjπ, ktÛrπ gracz powinien wykonaÊ. " + 
                "NiezbÍdne informacje o polach bÍdπ dostÍpne po naklikniÍciu na podúwietlenia. " + 
                "Dodatkowo wyúwietlana bÍdzie informacja o aktualnym polu, na ktÛrym znajduje siÍ gracz.";

            instructions[InstructionInfo.MissionCompleting] =
                "Gracz moøe sprÛbowaÊ wykonaÊ misjÍ, tylko gdy spe≥nia warunki widoczne na karcie misji:\n" +
                "1) znajduje siÍ w odpowiednim miejscu na planszy\n" + 
                "2) posiada odpowiednie øetony lub karty (okreúlone dla wariantu ìNiepe≥noletni czarodziejî)\n\n" +
                "Punkty przypisane do misji otrzymuje dopiero wtedy, gdy pokona przeciwnika okreúlonego na karcie misji. " + 
                "Po wykorzystaniu przedmiotÛw przy misji, naleøy umieúciÊ je na dowolnym polu z gwiazdkπ na planszy 2 " + 
                "(z niezapomnianymi miejscami ze úwiata HP).";

            instructions[InstructionInfo.SpecialItems] =
                "Po ich wykorzystaniu po≥Ûø je na 1 z 3 gwiazdek na ulicy Pokπtnej. Sπ to\n\n" +
                "1) Czarna rÛødøka: +1 koúÊ ataku w czasie walki\n\n" +
                "2) KamieÒ wskrzeszenia: +6PØ podczas walki\n\n" +
                "3) Peleryna niewidka: ucieknij z miejsca walki na odleg≥oúÊ 5 pÛl\n\n" +
                "4) Miecz Gryffindora: +1 koúÊ ataku w czasie walki\n\n" +
                "5) Zmieniacz czasu: rozegraj dodatkowπ turÍ\n\n" +
                "6) HardodziÛb: udaj siÍ na dowolne pole na planszy 1 albo 2, a nastÍpnie wrÛÊ";

            instructions[InstructionInfo.FightWithPlayer] =
                "Kaødy gracz rzuca 1 koúciπ - ten kto ma mniejsza liczbÍ oczek, traci 1 PØ.\n" +
                "Podczas pojedynku nie moøna uøyÊ ani zaklÍcia ani eliksiru.\n" +
                "\nPrzegrana: utrata 2 PØ\n" +
                "Wygrana: zabranie przegranemu øetonu przedmiotu, úwistoklika, proszka Fiuu lub zakrytej karty";

            instructions[InstructionInfo.FightOnFightField] =
                "Gracz nie moøe graÊ z postaciπ, ktÛrπ sam jest - powinien wybraÊ innπ kartÍ. " +
                "Na karcie pojedynku okreúlona jest liczba øyÊ przeciwnika oraz jego sposÛb obrony.\n" +
                "\nWalczyÊ moøna na 2 sposoby:\n" +
                "1) koúÊmi ataku (ich liczba widnieje na planszetce gracza): " + 
                "kaøda koúÊ z wynikiem wiÍkszym lub rÛwnym odpornoúci przeciwnika, zabiera mu 1 PØ\n" +
                "2) kartami zaklÍÊ i eliksirÛw (moøna wykorzystaÊ w dowolnym momencie ruchu): uøycie karty obrony pozbawia przeciwnika ruchu\n" +
                "\nPrzegrana (pozosta≥ 1 PØ): gracz odrzuca 3 dowolne karty\n" +
                "Wygrana: gracz dobiera 1 zaklÍcie, 1 eliksir i 1 ksiÍgÍ";

            instructions[InstructionInfo.MissionFight] =
                "Kaøda karta misji wskazuje postaÊ, z ktÛrπ naleøy stoczyÊ pojedynek. Karta przeciwnika posiada informacje o nim.\n" +
                "\nGracz ma moøliwoúÊ wykorzystania:\n" +
                " - karty obrony (powoduje brak kontrataku)\n" + 
                " - karty ataku\n" +
                " - koúci ataku: liczba oczek nie mniejsza od odpornoúci przeciwnika, odbiera mu 1 PØ\n" + 
                "Gracz podczas jednej walki moøe wykorzystaÊ maksymalnie 5 kart zaklÍÊ i eliksirÛw\n" +
                "Po úmierci przeciwnika, atakuje on gracza jeszcze raz swojπ zdolnoúciπ specjalnπ\n" +
                "\nWygrana (przeciwnik nie ma øyÊ): misja wype≥niona, zdobycie okreúlonej na karcie misji liczby punktÛw domu\n" +
                "Przegrana (gracz ma 1 Pè): gracz idzie do \"Dziurawego Kot≥a\", zdobywa 6Pè i odk≥ada wszystkie karty eliksirÛw i zaklÍÊ.";

            instructions[InstructionInfo.QuidditchPreparation] =
                "Na poczπtku naleøy wybraÊ druøynÍ oraz rozstawiÊ graczy:\n" + 
                "- 1 szukajπcy na linii startowej (wokÛ≥ boiska)\n" + 
                "- 1 obroÒca na 1 z 3 pÛl pola bramkowego\n" + 
                "- 2 pa≥karzy na 2 z 5 pÛl linii wokÛ≥ pola bramkowego\n" + 
                "- 3 úcigajπcych na 3 z 10 pÛl dwÛch kolejnych linii\n" + 
                "\nKafel umieszcza siÍ na úrodku boiska, a t≥uczki po jednym na kaødej po≥owie\n" + 
                "Mecz rozpoczyna gracz z wiÍkszπ liczbπ oczek po rzucie 2 koúÊmi\n" + 
                "Sk≥ada siÍ on z dwÛch rund: uzyskania 2 goli oraz zdobycia z≥otego znicza\n\n" + 
                "Wygrana: 40 punktÛw domu";

            instructions[InstructionInfo.QuidditchRound1] =
                "Gdy wszyscy úcigajπcy danego gracza sπ wyeliminowani, przegrywa on automatycznie rundÍ 1.\n\n" + 
                "Gracze nie mogπ wchodziÊ na to samo pole co inni gracze, z wyjπtkiem sytuacji z przejÍciem kafla.\n\n" + 
                "ObroÒcy nie mogπ opuúciÊ pÛl bramkowych.\n\n" +
                "Kaødy gracz na poczπtku ruchu rzuca 2 koúÊmi, ktÛre okreúlajπ liczbÍ akcji.";

            instructions[InstructionInfo.QuidditchRound1_Actions1] =
                "Ruchy kosztujπce 1 akcjÍ:\n" +
                "\n1) Ruch wzd≥uø bia≥ych kropkowanych linii.\n" +
                "\n2) Rzut trzymanym kaflem o 1 pole.\n" +
                "\n3) PrzejÍcie kafla (úcigajπcy oraz kafel muszπ byÊ na tym samym polu).\n" +
                "\n4) strzelenie gola przez úcigajπcego znajdujπcego siÍ na linii wokÛ≥ pola bramkowego. " +
                "Strza≥ odbywa siÍ za pomocπ rzutu koúciπ - jeúli rzucajπcy ma wiÍcej oczek niø obroÒca, " +
                "nastÍpuje gol i zdobycie 1 karty quidditcha. Kafel trafia do obroÒcy zarÛwno po prÛbie, " +
                "jak i po golu. Jeúli obroÒca zosta≥ wyeliminowany, kafel zostaje na losowym polu na linii wokÛ≥ pola bramkowego.";

            instructions[InstructionInfo.QuidditchRound1_Actions2] =
                "Ruchy bez øadnego kosztu:\n" +
                "\n1) Podniesienie kafla/t≥uczka odpowiednio przez úciagajπcego/pa≥karza, bÍdπcego na tym samym polu, co pi≥ka.\n" +
                "\n2) Rzut trzymanym t≥uczkiem przez pa≥karza na odleg≥oúÊ okreúlonπ przez dodatkowy rzut koúciπ " +
                "- moøna zrobiÊ unik, ale jeúli wyrzucona liczba oczek przez przeciwnika wynosi mniej niø 4, postaÊ zostaje wyeliminowana.";

            instructions[InstructionInfo.QuidditchRound2] =
                "Wygrywa ten, kto pierwszy okrπøy boisko.\n\n" +
                "Szukajπcy rozpoczynajπ na polu startowym linii wokÛ≥ boiska.\n\n" +
                "Rozpoczyna gracz, ktÛry wygra≥ rundÍ 1.\n\n" +
                "Kaødy gracz rzuca w swojej turze 2 koúÊmi oraz moøe uøyÊ maksymalnie 1 karty quidditcha.";
        }
    }
}
