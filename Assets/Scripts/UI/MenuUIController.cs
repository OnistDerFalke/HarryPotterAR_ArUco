using System;
using System.Linq;
using Assets.Scripts;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts
{
    public class MenuUIController : MonoBehaviour
    {
        [SerializeField] private GameObject[] menuContext = new GameObject[4];

        [Header("Character Context")]
        [SerializeField] private GameObject characterBox;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button nextCharacterButton;
        [SerializeField] private Button previousCharacterButton;
        [SerializeField] private Sprite[] markerSprites = new Sprite[9];
        [SerializeField] private Sprite markerSpriteNotSet;
        [SerializeField] private Text characterName;
        [SerializeField] private Text specialPowerText;
        [SerializeField] private Image markerImage;

        [Header("Instruction Context")]
        [SerializeField] private Button nextInstructionButton;
        [SerializeField] private Button previousInstructionButton;
        [SerializeField] private Text instructionTitle;
        [SerializeField] private Text instructionText;

        [Header("Start Instruction Context")]
        [SerializeField] private Text startInstructionTitle;
        [SerializeField] private Text startInstructionText;

        private enum Contexts
        {
            StartContext,
            CharactersContext,
            InstructionContext,
            StartInstructionContext
        }

        private enum SwitchDirection
        {
            Left,
            Right
        }

        // character context
        private int playersNumber;
        private int chosenPlayerIndex;
        private int charactersNumber = 9;
        private int chosenCharacterIndex;
        private Character[] chosenCharacters;
        private bool[] takenCharacters;

        // instruction context
        private Instruction instruction;
        private int currentInstructionPartId;
        private int instructionPartNumber;

        private void Start()
        {
            ChangeContext(Contexts.StartContext);
            GameManager.Setup();
            playersNumber = 1;
            chosenPlayerIndex = 0;
            instruction = new();
            instructionPartNumber = instruction.instructions.Count;
        }

        private void Update()
        {
            startGameButton.interactable = chosenCharacterIndex != 0;
        }

        private bool IfAllCharactersChosen()
        {
            return chosenCharacters.All(character => character != Character.None);
        }
                
        private void SwitchCharacter(SwitchDirection direction)
        {
            do
            {
                switch (direction)
                {
                    case SwitchDirection.Left:
                        chosenCharacterIndex =
                            chosenCharacterIndex == 0 ? charactersNumber : chosenCharacterIndex - 1;
                        break;
                    case SwitchDirection.Right:
                        chosenCharacterIndex = (chosenCharacterIndex + 1) % (charactersNumber + 1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            } while (takenCharacters[chosenCharacterIndex]);

            UpdateCharactersContext();
        }

        private void UpdateCharactersContext()
        {
            characterName.text = chosenCharacterIndex == 0 ? 
                "Nie wybrano" : ((Character)Enum.ToObject(typeof(Character), chosenCharacterIndex)).ToString();
            specialPowerText.text = chosenCharacterIndex == 0 ?
                "Moc specjalna:\nBrak" : "Moc specjalna:\n" + CharacterSpecialPower.GetSpecialPowerText((Character)Enum.ToObject(typeof(Character), chosenCharacterIndex));
            if (chosenCharacterIndex <= charactersNumber)
            {
                markerImage.sprite = chosenCharacterIndex == 0
                    ? markerSpriteNotSet : markerSprites[chosenCharacterIndex - 1];
            }
        }

        private void UpdateInstructionContext()
        {
            var info = (Instruction.InstructionInfo)currentInstructionPartId;
            instructionTitle.text = instruction.GetInstructionInfoString(info);
            instructionText.text = instruction.instructions[info];
        }

        private void ChangeContext(Contexts context)
        {
            foreach (var c in menuContext)
                c.SetActive(false);
            menuContext[(int)context].SetActive(true);
        }

        private void LoadDataToGameManager()
        {
            GameManager.Setup();
            GameManager.PlayerNumber = playersNumber;
            for(var i = 0; i < playersNumber; i++)
                GameManager.Players.Add(new Player(i, chosenCharacters[i]));
        }

        public void OnStartContextPlayButtonClick()
        {
            ChangeContext(Contexts.CharactersContext);
            chosenCharacters = new Character[playersNumber];
            takenCharacters = new bool[charactersNumber + 1];
            chosenCharacterIndex = 0;

            startGameButton.gameObject.SetActive(true);
            //characterBox.SetActive(true);
            nextCharacterButton.gameObject.SetActive(true);
            previousCharacterButton.gameObject.SetActive(true);

            UpdateCharactersContext();
        }

        public void OnBackToMenuButtonClick()
        {
            ChangeContext(Contexts.StartContext);
        }

        public void OnBackToCharactersButtonClick()
        {
            ChangeContext(Contexts.CharactersContext);
        }

        public void OnStartInstructionButtonClick()
        {
            chosenCharacters[chosenPlayerIndex] = (Character)Enum.ToObject(typeof(Character), chosenCharacterIndex);
            takenCharacters[chosenCharacterIndex] = true;

            LoadDataToGameManager();
            startInstructionTitle.text = instruction.GetInstructionInfoString(Instruction.InstructionInfo.GamePreparation);
            startInstructionText.text = instruction.instructions[Instruction.InstructionInfo.GamePreparation];
            ChangeContext(Contexts.StartInstructionContext);
        }

        public void OnStartGameButtonClick()
        {
            SceneManager.LoadScene("Scenes/Test", LoadSceneMode.Single);
        }

        public void OnNextCharacterButtonClick()
        {
            SwitchCharacter(SwitchDirection.Right);
        }

        public void OnPreviousCharacterButtonClick()
        {
            SwitchCharacter(SwitchDirection.Left);
        }

        public void OnInstructionContextButtonClick()
        {
            ChangeContext(Contexts.InstructionContext);
            currentInstructionPartId = 0;
            UpdateInstructionContext();
        }

        public void OnNextInstructionButtonClick()
        {
            currentInstructionPartId = (currentInstructionPartId + 1) % instructionPartNumber;
            UpdateInstructionContext();
        }

        public void OnPreviousInstructionButtonClick()
        {
            currentInstructionPartId = (currentInstructionPartId - 1) < 0 ? instructionPartNumber - 1 : currentInstructionPartId - 1;
            UpdateInstructionContext();
        }
    }
}
