using System;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Demo;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using static UnityEngine.GraphicsBuffer;

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

            Vector3 result = Vector3.zero;
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
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

            result = result / board.CurrentTrackedBoardMarks.Count;

            //Debug.Log("Pozycja: " + result);

            return result;
        }

        public Vector3 GetReferenceMarkerScale()
        {
            if (!IsTrackingBoard())
                return Vector3.one;

#if !UNITY_EDITOR && UNITY_ANDROID
            float s = 100f;
#else
            float s = 100f * 1.14f;
#endif

            Vector3 result = Vector3.zero;
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
            {
                GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);
                result += refMarker.transform.localScale;
            }
            result /= board.CurrentTrackedBoardMarks.Count;

            //Debug.Log("Field scale: " + referenceMarker.marker.transform.localScale * s);
            return result * s;
        }

        public Vector3 ConvertCoordinates(Vector2 boardCoordinates, int referenceMarkerId)
        {
            GameObject marker = arucoMarkHandler.FindModelById(referenceMarkerId);

            return marker.transform.position +
                    (boardCoordinates.x - boardMarks[referenceMarkerId].x) * marker.transform.right.normalized * scale +
                    (boardCoordinates.y - boardMarks[referenceMarkerId].y) * marker.transform.forward.normalized * scale;
        }

        public Quaternion ReferenceRotation()
        {
            if (!IsTrackingBoard())
                return Quaternion.identity;

            Quaternion avgRot = Quaternion.identity;
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
            {
                GameObject refMarker = arucoMarkHandler.FindModelById(refMarkerId);
                avgRot *= refMarker.transform.localRotation;
            }
            avgRot.Normalize();
            var rot = Quaternion.Slerp(Quaternion.identity, avgRot, 1.0f / board.CurrentTrackedBoardMarks.Count).eulerAngles;

            rot.z = -rot.z;

            //Debug.Log("Rotacja: " + rot);
            return Quaternion.Euler(rot);
        }

        public Vector2 WorldToBoard(Vector3 worldPos)
        {
            if (!IsTrackingBoard())
                return -Vector2.one;

            //TODO: poprawić, żeby działało
            Vector2 boardPos = Vector2.zero;
            foreach (var refMarkerId in board.CurrentTrackedBoardMarks)
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

            boardPos /= board.CurrentTrackedBoardMarks.Count;

            return boardPos;
        }

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            scale = 0.75f;
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
            if (IsTrackingBoard())
                return markerScale.x * 100f * scale;
            else
                return scale;
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