using System;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Demo;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using UnityEngine.Timeline;

namespace Assets.Scripts
{
    public class CoordinatesConverter : MonoBehaviour
    {
        // marks that identify the position and orientation of a board, sticked to the board
        [SerializeField] private List<Vector2> boardMarkPositions;
        [SerializeField] private int boardMarkStartId;
        private List<int> boardMarkIds;
        [SerializeField] private MarkerDetector arucoMarkHandler;

        [SerializeField] private float scale = 1.2f;

        public List<int> BoardMarkIds => boardMarkIds;
        public float Scale => scale;

        private Dictionary<int, Vector2> boardMarks;
        private BoardMono board;
        [SerializeField] private Camera cam;
        private Vector3 imgPos = Vector3.zero;

        public bool IsTrackingBoard()
        {
            return board.CurrentTrackedBoardMarks.Count > 0;
        }

        /// <summary>
        /// Use this method to obtain scene coordinates of a point on the board plane
        /// </summary>
        /// <param name="boardCoordinates">the point to map to world space</param>
        /// <returns>scene coordinates of a given point or (0,0,0) if the board is not tracked</returns>
        public Vector3 ConvertCoordinates(Vector2 boardCoordinates)
        {
            if (!IsTrackingBoard())
                return Vector3.zero;

            Dictionary<int, float> distanceFromMarkers = new Dictionary<int, float>();
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
                distanceFromMarkers[refMarkerId] = Vector2.Distance(boardMarks[refMarkerId], boardCoordinates);

            int count = board.CurrentTrackedBoardMarks.Count > 4 ? 4 : board.CurrentTrackedBoardMarks.Count;
            var closestMarkerIds = distanceFromMarkers.OrderBy(pair => pair.Value).Take(count).Select(pair => pair.Key).ToList();

            Vector3 result = Vector3.zero;
            foreach (var refMarkerId in closestMarkerIds)
            {
                GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);

                var convS = GetConversionScale(refMarker.transform.localScale);
                var refPos = refMarker.transform.position;
                var xOffset = (boardCoordinates.x - boardMarks[refMarkerId].x) * refMarker.transform.right.normalized * convS;
                var yOffset = (boardCoordinates.y - boardMarks[refMarkerId].y) * refMarker.transform.up.normalized * convS;

                var temp = refPos - xOffset + yOffset;

                //Put result closer to avoid crossing fields across the board
                var delta = cam.transform.position - temp;
                var z = imgPos.z - convS;
                var closer = new Vector3(delta.x / delta.z, -delta.y / delta.z, 1f / scale) * (z - temp.z) * scale;
                result = result + temp + closer;
            }

            result /= count;

            //Debug.Log("Pozycja: " + result);

            return result;
        }

        public Vector3 GetClosestMarkerScale(Vector2 boardCoordinates)
        {
            if (!IsTrackingBoard())
                return Vector3.one;

#if !UNITY_EDITOR && UNITY_ANDROID
            float s = 100f;
#else
            float s = 100f * 1.14f;
#endif

            Vector3 result = Vector3.zero;
            float minDistance = float.MaxValue;
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
            {
                var dist = Vector2.Distance(boardMarks[refMarkerId], boardCoordinates);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);
                    result = refMarker.transform.localScale;
                }
            }

            return result * s;
        }


        public Quaternion ReferenceRotation()
        {
            if (!IsTrackingBoard())
                return Quaternion.identity;

            Dictionary<int, float> maxDifferences = new();

            foreach (var markerId in board.CurrentTrackedBoardMarks)
            {
                GameObject marker = arucoMarkHandler.FindModelById(markerId);
                float maxDiff = float.MinValue;
                foreach (var markerId2 in board.CurrentTrackedBoardMarks)
                {
                    GameObject marker2 = arucoMarkHandler.FindModelById(markerId2);
                    float diff = CalculateQuaternionDiff(marker.transform.localRotation, marker2.transform.localRotation);
                    if (markerId != markerId2 && diff > maxDiff)
                        maxDiff = diff;
                }
                maxDifferences[markerId] = maxDiff;
            }

            var bestMarkerId = maxDifferences.OrderBy(pair => pair.Value).First().Key;
            GameObject bestMarker = arucoMarkHandler.FindModelById(bestMarkerId);

            var rot = bestMarker.transform.localRotation.eulerAngles;
            rot.z = -rot.z;
            rot.y += 180;

            Debug.Log("Rotacja: " + rot);
            return Quaternion.Euler(rot);
        }

        public Vector2 WorldToBoard(Vector3 worldPos)
        {
            if (!IsTrackingBoard())
                return -Vector2.one;

            Dictionary<int, float> distanceFromMarkers = new Dictionary<int, float>();
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
            {
                GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);
                distanceFromMarkers[refMarkerId] = Vector3.Distance(refMarker.transform.position, worldPos);
            }

            int count = board.CurrentTrackedBoardMarks.Count > 4 ? 4 : board.CurrentTrackedBoardMarks.Count;
            var closestMarkerIds = distanceFromMarkers.OrderBy(pair => pair.Value).Take(count).Select(pair => pair.Key).ToList();

            Vector2 boardPos = Vector2.zero;
            foreach (var refMarkerId in closestMarkerIds)
            {
                GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);
                Vector3 referencePosition = refMarker.transform.position;

                // direction down the pawn
                Vector3 normalizedDirection = -1 * refMarker.transform.forward.normalized;

                // line from given position to reference marker in 3d space
                Vector3 lineToBoard = referencePosition - worldPos;

                // distance of the pawn from the board
                var convS = GetConversionScale(refMarker.transform.localScale);
                float projection = Vector3.Dot(lineToBoard, normalizedDirection);
                projection /= convS;

                // pawn projected onto the board
                Vector3 intersection = worldPos + projection * normalizedDirection;

                // offset from reference transform to projected pawn position
                Vector3 offset = intersection - referencePosition;
                float y_dist_board = Vector3.Dot(offset, refMarker.transform.up);
                float x_dist_board = Vector3.Dot(offset, refMarker.transform.right);
                Vector2 x_offset = Vector2.right * x_dist_board / convS;
                Vector2 y_offset = Vector2.up * y_dist_board / convS;
                boardPos = boardPos + boardMarks[refMarkerId] - x_offset + y_offset;
            }

            boardPos /= count;

            return boardPos;
        }

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            scale = 1f / 1.75f;
#endif
            boardMarks = new Dictionary<int, Vector2>();
            boardMarkIds = new List<int>();

            for (int i = 0; i < boardMarkPositions.Count; i++)
            {
                boardMarkIds.Add(boardMarkStartId + i);
                boardMarks[boardMarkIds[i]] = boardMarkPositions[i];
            }

            board = GetComponentInParent<BoardMono>();
        }

        private float GetConversionScale(Vector3 markerScale)
        {
            return markerScale.x * 100f * scale;
        }

        private float CalculateQuaternionDiff(Quaternion q1, Quaternion q2)
        {
            var dotProduct = Quaternion.Dot(q1, q2);
            var angleDifference = (float)Math.Acos(2 * Math.Pow(dotProduct, 2) - 1);
            return angleDifference;
        }

        private void Update()
        {
            if (imgPos == Vector3.zero)
            {
                imgPos = cam.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.transform.position;
                Debug.Log("Obraz first: " + imgPos);
            }
        }
    }
}