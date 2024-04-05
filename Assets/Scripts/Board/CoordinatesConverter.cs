using System;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Demo;

namespace Assets.Scripts
{
    public class CoordinatesConverter : MonoBehaviour
    {
        // marks that identify the position and orientation of a board, sticked to the board
        [SerializeField] private List<Vector2> boardMarkPositions;
        [SerializeField] private int boardMarkStartId;
        private List<int> boardMarkIds;
        //TODO2: change to ArUco
        [SerializeField] private MarkerDetector arucoMarkHandler;

        [SerializeField] private float scale = 1/1.75f;

        public List<int> BoardMarkIds => boardMarkIds;
        public float Scale => scale;

        private Dictionary<int, Vector2> boardMarks;
        private (int id, GameObject marker) referenceMarker;
        private BoardMono board;

        /// <summary>
        /// Use this method to make sure that the result of converting coordinates is valid
        /// </summary>
        /// <returns>true if at least one board marker is tracked, false otherwise</returns>
        public bool IsTrackingBoard()
        {
            return referenceMarker.marker != null;
        }

        /// <summary>
        /// Use this method to obtain scene coordinates of a point on the board plane
        /// </summary>
        /// <param name="boardCoordinates">the point to map to world space</param>
        /// <returns>scene coordinates of a given point or (0,0,0) if the board is not tracked</returns>
        public Vector3 ConvertCoordinates(Vector2 boardCoordinates)
        {
            if (IsTrackingBoard())
            {
                var newPosiiton = new Vector3(0, 0, 0);
                //TODO3: change to ArUco
                if (GameManager.CurrentTrackedObjects.ContainsKey(referenceMarker.id))
                {
                    var position = GameManager.CurrentTrackedObjects[referenceMarker.id];
                    newPosiiton = position - arucoMarkHandler.transform.position;
                }

                return newPosiiton + referenceMarker.marker.transform.position +
                    (boardCoordinates.x - boardMarks[referenceMarker.id].x) * referenceMarker.marker.transform.right.normalized * scale +
                    (boardCoordinates.y - boardMarks[referenceMarker.id].y) * referenceMarker.marker.transform.forward.normalized * scale;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public Vector3 ConvertCoordinates(Vector2 boardCoordinates, int referenceMarkerId)
        {
            //TODO2: change to ArUco
            Debug.Log("ConvertCoordinates - FindModelById: " + referenceMarkerId);
            GameObject marker = arucoMarkHandler.FindModelById(referenceMarkerId);
            (int id, GameObject marker) referenceMarker = (referenceMarkerId, marker);

            return referenceMarker.marker.transform.position +
                    (boardCoordinates.x - boardMarks[referenceMarker.id].x) * referenceMarker.marker.transform.right.normalized * scale +
                    (boardCoordinates.y - boardMarks[referenceMarker.id].y) * referenceMarker.marker.transform.forward.normalized * scale;
        }

        public Quaternion ReferenceRotation()
        {
            if(IsTrackingBoard())
            {
                return referenceMarker.marker.transform.rotation;
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public Vector2 WorldToBoard(Vector3 worldPos)
        {
            if(referenceMarker.marker == null)
            {
                return Vector2.one * -1;
            }

            //var newPosition = new Vector3(0, 0, 0);
            var position = new Vector3(0, 0, 0);
            //TODO2: change to ArUco
            if (GameManager.CurrentTrackedObjects.ContainsKey(referenceMarker.id))
            {
                position = GameManager.CurrentTrackedObjects[referenceMarker.id];
                //newPosition = position - arucoMarkHandler.transform.position;
            }

            //Vector3 referencePosition = referenceMarker.marker.transform.position - newPosition;
            Vector3 referencePosition = referenceMarker.marker.transform.position - position;

            // direction down the pawn
            Vector3 normalizedDirection = -1 * referenceMarker.marker.transform.up.normalized;

            // line from given position to reference marker in 3d space
            Vector3 lineToBoard = referencePosition - worldPos;

            // distance of the pawn from the board
            float projection = Vector3.Dot(lineToBoard, normalizedDirection);
            projection /= scale;

            // pawn projected onto the board
            Vector3 intersection = worldPos + projection * normalizedDirection;

            // offset from reference transform to projected pawn position
            Vector3 offset = intersection - referencePosition;
            float y_dist_board = Vector3.Dot(offset, referenceMarker.marker.transform.forward);
            float x_dist_board = Vector3.Dot(offset, referenceMarker.marker.transform.right);
            Vector2 boardPos = boardMarks[referenceMarker.id]
                - Vector2.right * x_dist_board / scale
                - Vector2.up * y_dist_board / scale;

            return boardPos;
        }

        private float CalculateErrorRate(int markerId)
        {
            float err = 0f;
            foreach(var otherId in board.CurrentTrackedBoardMarks)
            {
                if(otherId == markerId)
                {
                    continue;
                }
                Vector3 expectedPosition = ConvertCoordinates(boardMarks[otherId], markerId);
                //TODO2: change to ArUco
                Vector3 actualPosition = arucoMarkHandler.FindModelById(otherId).transform.position;
                err += Vector3.SqrMagnitude(expectedPosition - actualPosition);
            }
            return err;
        }

        private (int, float) ChooseReferenceMarker()
        {
            int bestId = -1;
            float minErrorRate = float.MaxValue;

            foreach (var markerId in board.CurrentTrackedBoardMarks)
            {
                float err = CalculateErrorRate(markerId);
                if (err < minErrorRate)
                {
                    minErrorRate = err;
                    bestId = markerId;
                }
            }
            return (bestId, minErrorRate);
        }

        private void Awake()
        {
            boardMarks = new Dictionary<int, Vector2>();
            boardMarkIds = new List<int>();

            for (int i = 0; i < boardMarkPositions.Count; i++)
            {
                boardMarkIds.Add(boardMarkStartId + i);
                boardMarks[boardMarkIds[i]] = boardMarkPositions[i];
            }

            board = GetComponentInParent<BoardMono>();
        }

        private void Update()
        {
            var bestMarker = ChooseReferenceMarker();
            Debug.Log("BestMarker: " + bestMarker.Item1);
            if (bestMarker.Item1 != referenceMarker.id)
            {
                if (referenceMarker.marker != null && board.CurrentTrackedBoardMarks.Contains(referenceMarker.id))
                {
                    float referenceError = CalculateErrorRate(referenceMarker.id);
                    if (Math.Abs(bestMarker.Item2 - referenceError) < 0.00005f)
                        return;
                }

                Debug.Log("Podmianka");
                //TODO2: change to ArUco
                GameObject marker = arucoMarkHandler.FindModelById(bestMarker.Item1);
                if (marker != null)
                {
                    referenceMarker = (bestMarker.Item1, marker);
                }
            }
        }
    }
}