namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using UnityEngine.UI;
    using Aruco;
    using OpenCvSharp;
    using Assets.Scripts;
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System;
    using System.Net.WebSockets;
    using System.Drawing;

    public class TestMarkerDetector : MonoBehaviour
    {
        public Camera cam;
        private int[] ids;
        public Canvas canvas;

        //OpenCV textures and mats
        private Texture2D texture;
        private Mat mat;
        private Mat temp;
        private Mat grayMat;

        //ArUco stuff
        private DetectorParameters detectorParameters;
        private Dictionary dictionary;
        private Point2f[][] corners;
        private Point2f[][] rejectedImgPoints;
        private MeshRenderer mr;
        private Texture2D outputTexture;

        [SerializeField] public RawImage RawImage;

        private Vector2 markerBoardPos = new Vector2(14, 10.5f);
        private Vector2 pointBoardPos = new Vector2(18.5f, 15.5f);
        private Vector2 markerRealPos = Vector2.zero;
        private Vector2 pointRealPos = Vector2.zero;
        private float markerSize = 2f;


        private IEnumerator FeedARCamera()
        {
            while (true)
            {
                
                yield return new WaitForEndOfFrame();

                try
                {
                    texture = new Texture2D(Screen.width, Screen.height);

                    mr = cam.gameObject.GetComponentInChildren<MeshRenderer>();

                    if (mr == null)
                    {
                        Debug.Log("Renderer not found");
                        continue;
                    }

                    Destroy(texture);
                    texture = (mr.material.mainTexture as Texture2D);

                    RawImage.rectTransform.sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
                }
                catch (System.Exception e)
                {
                    TestManager.Logs += e.StackTrace + "\n" + e.Message + "\n";
                }

                try
                {
                    mat = new Mat();
                    mat = Unity.TextureToMat(texture);
                    Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
                    Cv2.Flip(mat, mat, FlipMode.Y);

                    grayMat = new Mat();
                    Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

                    if (TestManager.DetectingStarted)
                    {
                        CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

                        for(int i = 0; i < ids.Length; i++)
                        {
                            TestManager.UpdatePositions(ids[i], ConvertToList(corners[i]));
                            SetPositions(corners[i]);
                        }
                        TestManager.UpdateDetected(ids);

                        var size = 5;
                        if (pointRealPos != Vector2.zero)
                            Cv2.Rectangle(grayMat, new OpenCvSharp.Rect((int)pointRealPos.x, (int)pointRealPos.y, size, size), new Scalar(255f, 0, 0), thickness: size);
                        CvAruco.DrawDetectedMarkers(grayMat, corners, ids);
                    }

                    Destroy(outputTexture);

                    //var canals = Cv2.Split(mat);
                    //temp = new Mat();
                    //Mat[] can = new Mat[4];
                    //can[0] = canals[2];
                    //can[1] = canals[1];
                    //can[2] = canals[0];
                    //can[3] = canals[0].SetTo(new Scalar(0, 0, 0));
                    //Cv2.Merge(can, temp);
                    //Cv2.CvtColor(temp, temp, ColorConversionCodes.RGB2RGBA);

                    outputTexture = Unity.MatToTexture(grayMat);
                    RawImage.texture = outputTexture;
                    RawImage.material.mainTexture = outputTexture;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.StackTrace + "\n" + e.Message);
                    TestManager.Logs += e.StackTrace + "\n" + e.Message + "\n";
                }

                if (grayMat != null) grayMat.Dispose();
                if (mat != null) mat.Dispose();
                if (temp != null) temp.Dispose();
            }
        }

        private List<Vector2> ConvertToList(Point2f[] pts)
        {
            List<Vector2> list = new();
            foreach (var pt in pts)
                list.Add(new Vector2(pt.X, pt.Y));
            return list;
        }

        private void SetPositions(Point2f[] corners)
        {
            TestManager.Logs = "";

            Vector2 point = Vector2.zero;
            int i = 0;
            foreach (var corner in corners)
            {
                point += new Vector2(corner.X, corner.Y);
                TestManager.Logs += $"{i} : {corner}\n";
                i++;
            }
            point /= corners.Length;
            markerRealPos = point;

            var pointRelevantPosBoard = pointBoardPos - (markerBoardPos - new Vector2(markerSize / 2f, markerSize / 2f));

            var leftBotToRightUpBoard = new Vector2(markerSize, markerSize);
            var leftBotToRightUpReal = new Vector2(corners[1].X - corners[3].X, corners[1].Y - corners[3].Y);
            var scale = leftBotToRightUpReal / leftBotToRightUpBoard;


            //var botBoard = new Vector2(markerSize, 0);
            //var botReal = new Vector2(corners[2].X - corners[3].X, corners[2].Y - corners[3].Y);
            //TestManager.Logs += "Bottom real: " + botReal + "\n";
            //var botScale = botReal.magnitude / botBoard.magnitude;
            //TestManager.Logs += "Bottom scale: " + botScale + "\n";
            //var botProjBoard = ProjectVector(botBoard, pointRelevantPosBoard);
            //TestManager.Logs += "BotProjBoard: " + botProjBoard + "\n";
            //var botProjReal = botProjBoard * botScale;
            //TestManager.Logs += "BotProjReal: " + botProjReal + "\n";

            //var leftBoard = new Vector2(0, markerSize);
            //var leftReal = new Vector2(corners[0].X - corners[3].X, corners[0].Y - corners[3].Y);
            //TestManager.Logs += "Left real: " + leftReal + "\n";
            //var leftScale = leftReal.magnitude / leftBoard.magnitude;
            //TestManager.Logs += "Left scale: " + leftScale + "\n";
            //var leftProjBoard = ProjectVector(leftBoard, pointRelevantPosBoard);
            //TestManager.Logs += "LeftProjBoard: " + leftProjBoard + "\n";
            //var leftProjReal = leftProjBoard * leftScale;
            //TestManager.Logs += "LeftProjReal: " + leftProjReal + "\n";

            //pointRealPos = new Vector2(corners[0].X, corners[0].Y) + botProjReal - leftProjReal;

            pointRealPos = new Vector2(corners[0].X, corners[0].Y) + pointRelevantPosBoard * scale;
            TestManager.Logs += "Point position: " + pointRealPos + "\n";
        }

        //private void SetPositions2(Point2f[] corners)
        //{
        //    TestManager.Logs = "";

        //    Vector2 point = Vector2.zero;
        //    int i = 0;
        //    foreach (var corner in corners)
        //    {
        //        point += new Vector2(corner.X, corner.Y);
        //        TestManager.Logs += $"{i} : {corner}\n";
        //        i++;
        //    }
        //    point /= corners.Length;
        //    markerRealPos = point;
        //    TestManager.Logs += "Pozycja œrodkowa markera: " + markerRealPos + "\n";

        //    var lineBottom = corners[2] - corners[3];
        //    var scaleBottom = new Vector2(lineBottom.X, lineBottom.Y).magnitude / markerSize;

        //    var lineUpper = corners[1] - corners[0];
        //    var scaleUpper = new Vector2(lineUpper.X, lineUpper.Y).magnitude / markerSize;

        //    var lineLeft = corners[0] - corners[3];
        //    var scaleLeft = new Vector2(lineLeft.X, lineLeft.Y).magnitude / markerSize;

        //    var lineRight = corners[1] - corners[2];
        //    var scaleRight = new Vector2(lineRight.X, lineRight.Y).magnitude / markerSize;

        //    var dist = pointBoardPos - markerBoardPos;
        //    var distLeftBot = pointBoardPos - (markerBoardPos - new Vector2(markerSize/2f, markerSize/2f));

        //    TestManager.Logs += "LineBottom: " + lineBottom + "   Scale: " + scaleBottom + "\n";
        //    TestManager.Logs += "LineUpper: " + lineUpper + "   Scale: " + scaleUpper + "\n";
        //    TestManager.Logs += "LineLeft: " + lineLeft + "   Scale: " + scaleLeft + "\n";
        //    TestManager.Logs += "LineRight: " + lineRight + "   Scale: " + scaleRight + "\n";
        //    TestManager.Logs += "Dist: " + distLeftBot + "\n";

        //    var botProj = ProjectVector(lineBottom, distLeftBot);
        //    var leftProj = ProjectVector(lineLeft, distLeftBot);
        //    TestManager.Logs += "BottomProjection: " + botProj + "   Len: " + botProj.magnitude + "\n";
        //    TestManager.Logs += "LeftProjection: " + leftProj + "   Len: " + leftProj.magnitude + "\n";

        //    //var scaleBot = scaleBottom - (scaleBottom - scaleUpper) * (dist.y + markerSize / 2f) / markerSize;
        //    var scaleBot = scaleBottom - (scaleBottom - scaleUpper) * leftProj.magnitude / markerSize;
        //    TestManager.Logs += "Scale Bot: " + scaleBot + "\n";
        //    //var scaleL = scaleLeft - (scaleLeft - scaleRight) * (dist.x + markerSize / 2f) / markerSize;
        //    var scaleL = scaleLeft - (scaleLeft - scaleRight) * botProj.magnitude / markerSize;
        //    TestManager.Logs += "Scale L: " + scaleL + "\n";

        //    pointRealPos = markerRealPos + ProjectVector(lineBottom, dist) * scaleL - ProjectVector(lineLeft, dist) * scaleBot;
        //    TestManager.Logs += "Point position: " + pointRealPos + "\n";
        //}

        /// <summary>
        /// Project vec2 to vec1
        /// </summary>
        private Vector2 ProjectVector(Vector2 vec1, Vector2 vec2)
        {
            var projection = Vector2.zero;

            float scalar = vec1.x * vec2.x + vec1.y * vec2.y;
            float squaredLen = (float)(Math.Pow(vec1.x, 2) + Math.Pow(vec1.y, 2));

            projection.x = scalar / squaredLen * vec1.x;
            projection.y = scalar / squaredLen * vec1.y;

            return projection;
        }

        //private void SetPositions2(Point2f[] corners)
        //{
        //    Vector2 point = Vector2.zero;
        //    int i = 0;
        //    foreach(var corner in corners)
        //    {
        //        point += new Vector2(corner.X, corner.Y);
        //        if (!save) TestManager.Logs += $"{i} : {corner}\n";
        //        i++;
        //    }
        //    point /= corners.Length;
        //    markerRealPos = point;
        //    if (!save) TestManager.Logs += "Pozycja œrodkowa markera: " + markerRealPos + "\n";

        //    var edge = new Point2f(corners[2].X - corners[3].X, corners[3].Y - corners[2].Y);
        //    if (!save) TestManager.Logs += "Edge: " + edge + "\n";
        //    float alpha = Mathf.Atan(edge.Y/edge.X);
        //    if (!save) TestManager.Logs += "Obrót znacznika: " + alpha + "\n";

        //    var dist = pointBoardPos - markerBoardPos;
        //    if (!save) TestManager.Logs += "Dist: " + dist + "\n";
        //    float beta = Mathf.Atan2(dist.y, dist.x);
        //    if (!save) TestManager.Logs += "k¹t miêdzy znacznikiem i markerem: " + beta + "\n";

        //    float len = (float)Math.Sqrt(dist.x * dist.x + dist.y * dist.y);
        //    if (!save) TestManager.Logs += "Len: " + len + "\n";
        //    if (!save) TestManager.Logs += "Cos(a+b): " + Mathf.Cos(alpha + beta) + "\n";
        //    pointRealPos = new Vector2(markerRealPos.x + Mathf.Cos(alpha + beta) * len, markerRealPos.y + Mathf.Sin(alpha + beta) * len);
        //    if (!save) TestManager.Logs += "Pozycja punktu: " + pointRealPos + "\n";

        //    save = true;
        //}


        void Start()
        {
            Debug.Log("TEST: " + cam.gameObject.GetComponentInChildren<MeshRenderer>());
            StartCoroutine(FeedARCamera());
        }

        void Update()
        {
            var mr = cam.gameObject.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                canvas.planeDistance = mr.gameObject.transform.position.z;
                gameObject.transform.position = mr.gameObject.transform.position;
            }

            // Create default parameres for detection
            detectorParameters = DetectorParameters.Create();

            // Dictionary holds set of all available markers
            dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);
        }
    }
}