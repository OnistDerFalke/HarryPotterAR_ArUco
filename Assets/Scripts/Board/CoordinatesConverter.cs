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
        //TODO2: change to ArUco
        [SerializeField] private MarkerDetector arucoMarkHandler;

        [SerializeField] private float scale = 1.2f;

        public List<int> BoardMarkIds => boardMarkIds;
        public float Scale => scale;

        private Dictionary<int, Vector2> boardMarks;
        private (int id, GameObject marker) referenceMarker;
        private BoardMono board;
        private float conversionScale;
        [SerializeField] private Camera camera;
        private Vector3 imgPos = Vector3.zero;

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
                ////TODO3: change to ArUco
                SetConversionScale();
                var refPos = referenceMarker.marker.transform.position;
                var xOffset = (boardCoordinates.x - boardMarks[referenceMarker.id].x) * referenceMarker.marker.transform.right.normalized * conversionScale;
                var yOffset = (boardCoordinates.y - boardMarks[referenceMarker.id].y) * referenceMarker.marker.transform.up.normalized * conversionScale;

                var temp = refPos - xOffset + yOffset;

                //Put result closer to avoid crossing fields across the board
                var delta = camera.transform.position - temp;
                var z = imgPos.z - conversionScale;
                //Debug.Log("Obraz: " + imgPos.z + " Skala konwersji: " + conversionScale);
                var closer = new Vector3(delta.x / delta.z, - delta.y / delta.z, 1f/scale) * (z - temp.z) * scale;
                var result = temp + closer;

                //Debug.Log("Value: " + result + "\tCloser: " + closer + "\tFinal value: " + result2);
                //Debug.Log("Board coordinates: " + boardCoordinates + "\tRef marker id: " + referenceMarker.id +
                //    "\nRef pos: " + refPos + "\tOffset: " + (closer - xOffset + yOffset) + "\tFinal value: " + result);

                return result;
            }
            else
            {
                return -Vector3.one;
            }
        }

        public Vector3 GetReferenceMarkerScale()
        {
            if (IsTrackingBoard())
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                float s = 100f;
#else
                float s = 100f * 1.14f;
#endif
                //Debug.Log("Field scale: " + referenceMarker.marker.transform.localScale * s);
                return referenceMarker.marker.transform.localScale * s;
            }
            else
                return Vector3.one;
        }

        public Vector3 ConvertCoordinates(Vector2 boardCoordinates, int referenceMarkerId)
        {
            //TODO2: change to ArUco
            //Debug.Log("ConvertCoordinates - FindModelById: " + referenceMarkerId);
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
                var rot = referenceMarker.marker.transform.rotation.eulerAngles;
                rot.y += 180;
                rot.z = -rot.z;

                //Debug.Log("Rotacja: " + rot);
                return Quaternion.Euler(rot);
            }
            else
                return Quaternion.identity;
        }

        public Vector2 WorldToBoard(Vector3 worldPos)
        {
            if(referenceMarker.marker == null)
                return -Vector2.one;

            //TODO2: change to ArUco
            Vector3 referencePosition = referenceMarker.marker.transform.position;

            // direction down the pawn
            Vector3 normalizedDirection = -1 * referenceMarker.marker.transform.forward.normalized;

            // line from given position to reference marker in 3d space
            Vector3 lineToBoard = referencePosition - worldPos;

            // distance of the pawn from the board
            SetConversionScale();
            float projection = Vector3.Dot(lineToBoard, normalizedDirection);
            projection /= conversionScale;

            // pawn projected onto the board
            Vector3 intersection = worldPos + projection * normalizedDirection;

            // offset from reference transform to projected pawn position
            Vector3 offset = intersection - referencePosition;
            float y_dist_board = Vector3.Dot(offset, referenceMarker.marker.transform.up);
            float x_dist_board = Vector3.Dot(offset, referenceMarker.marker.transform.right);
            Vector2 x_offset = Vector2.right * x_dist_board / conversionScale;
            Vector2 y_offset = Vector2.up * y_dist_board / conversionScale;
            Vector2 boardPos = boardMarks[referenceMarker.id] - x_offset + y_offset;

            //Debug.Log("Pozycja markera referencyjnego na planszy: " + boardMarks[referenceMarker.id] + 
            //    "\nOffset x: " + -x_offset + "\tOffset y: " + y_offset + 
            //    "\nFinal value: " + boardPos);

            return boardPos;
        }

        private double CalculateErrorRate(int markerId)
        {
            double err = 0f;
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

        private (int, double) ChooseReferenceMarker()
        {
            int bestId = -1;
            double minErrorRate = double.MaxValue;

            foreach (var markerId in board.CurrentTrackedBoardMarks)
            {
                double err = CalculateErrorRate(markerId);
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

        private void SetConversionScale()
        {
            if (IsTrackingBoard())
                conversionScale = referenceMarker.marker.transform.localScale.x * 100f * scale;
            else
                conversionScale = scale;
        }

        private void SetImgPos()
        {
            imgPos = camera.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.transform.position;
            Debug.Log("Obraz first: " + imgPos);
        }

        private void Update()
        {
            SetConversionScale();
            if (imgPos == Vector3.zero)
                SetImgPos();

            var bestMarker = ChooseReferenceMarker();
            //Debug.Log("BestMarker: " + bestMarker.Item1);
            if (bestMarker.Item1 != referenceMarker.id)
            {
                //TODO2: change to ArUco
                GameObject marker = arucoMarkHandler.FindModelById(bestMarker.Item1);
                if (marker != null)
                {
                    if (referenceMarker.marker != null && board.CurrentTrackedBoardMarks.Contains(referenceMarker.id))
                    {
                        double referenceError = CalculateErrorRate(referenceMarker.id);
                        double bestMarkerError = CalculateErrorRate(bestMarker.Item1);
                        if (Math.Abs(bestMarkerError - referenceError) > 1e6)
                        {
                            referenceMarker = (bestMarker.Item1, marker);
                            //Debug.Log("ReferenceMarker (better one found): " + referenceMarker.id);
                        }
                    }
                    else
                    {
                        referenceMarker = (bestMarker.Item1, marker);
                        //Debug.Log("ReferenceMarker (last one lost): " + referenceMarker.id);
                    }
                }
            }
        }
    }
}