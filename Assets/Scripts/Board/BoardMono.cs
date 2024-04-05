using Assets.Scripts;
using Game;
using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMono : MonoBehaviour
{
    public CoordinatesConverter coordinatesConverter;
    public BoardVisulalizer boardVisualiser;
    //TODO2: change to ArUco
    public MarkerDetector charactersHandler;
    public int id;

    public Board Board { get;  set; }
    public bool IsTracked => isTracked;
    public List<int> CurrentTrackedBoardMarks => currentTrackedBoardMarks;

    public bool isTracked = false;
    private List<int> currentTrackedBoardMarks = new();
    private List<Character> managedCharacters = new();


    public Field GetOccupiedField(Vector3 pos)
    {
        Vector2 boardSpacePos = coordinatesConverter.WorldToBoard(pos);
        //Debug.Log("Board Position: " + boardSpacePos);
        foreach (Field f in Board.Fields)
        {
            if (f.Figure.ContainsPosition(boardSpacePos))
            {
                // assuming that fields don't overlap
                return f;
            }
        }
        return null;
    }

    private void OnMarkerDetected(int id)
    {
        if (coordinatesConverter.BoardMarkIds.Contains(id))
        {
            if (currentTrackedBoardMarks.Count == 0)
            {
                EventBroadcaster.InvokeOnBoardDetected(Board.Id);
            }
            currentTrackedBoardMarks.Add(id);
        }
    }

    private void OnMarkerLost(int id)
    {
        if (currentTrackedBoardMarks.Contains(id))
        {
            currentTrackedBoardMarks.Remove(id);
            if (currentTrackedBoardMarks.Count == 0)
            {
                EventBroadcaster.InvokeOnBoardLost(Board.Id);
            }
        }
    }

    private void OnBoardDetected(int id)
    {
        if(id == Board.Id)
        {
            isTracked = true;
            coordinatesConverter.enabled = true;
            boardVisualiser.enabled = true;
            boardVisualiser.ShowVisuals();
        }
    }

    private void OnBoardLost(int id)
    {
        if (id == Board.Id)
        {
            isTracked = false;
            coordinatesConverter.enabled = false;
            boardVisualiser.HideVisuals();
            boardVisualiser.enabled = false;
        }
    }

    private void Awake()
    {
        Board = GameManager.BoardManager.Boards[id];
    }

    private void OnEnable()
    {
        EventBroadcaster.OnMarkDetected += OnMarkerDetected;
        EventBroadcaster.OnMarkLost += OnMarkerLost;
        EventBroadcaster.OnBoardDetected += OnBoardDetected;
        EventBroadcaster.OnBoardLost += OnBoardLost;
    }

    private void OnDisable()
    {
        EventBroadcaster.OnMarkDetected -= OnMarkerDetected;
        EventBroadcaster.OnMarkLost -= OnMarkerLost;
        EventBroadcaster.OnBoardDetected -= OnBoardDetected;
        EventBroadcaster.OnBoardLost -= OnBoardLost;
    }

    private void PlacePlayers()
    {
        managedCharacters.Clear();
        //TODO2: change to ArUco
        foreach (var marker in GameManager.CurrentTrackedObjects)
        {
            Character character = Player.CharacterFromInt(marker.Key);
            if (character != Character.None && !managedCharacters.Contains(character))
            {
                managedCharacters.Add(character);
                Vector3 characterPos = charactersHandler.FindModelById(marker.Key).transform.position;
                //Vector3 offset = charactersHandler.transform.position - marker.Value;

                //Field f = GetOccupiedField(characterPos + offset);
                Field f = GetOccupiedField(characterPos);
                Player player = GameManager.Players.Find((e) => e.Character == character);
                if (f != null && player != null)
                {
                    if (!player.IsDuringMove && player.LastFieldId != f.Index)
                    {
                        player.LastFieldId = f.Index;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (isTracked)
        {
            PlacePlayers();
        }
    }
}
