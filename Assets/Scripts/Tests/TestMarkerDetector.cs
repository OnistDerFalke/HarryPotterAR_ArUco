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

    public class TestMarkerDetector : MonoBehaviour
    {
        public Camera cam;
        private int[] ids;
        public Canvas canvas;

        //OpenCV textures and mats
        private Texture2D texture;
        private Mat mat;
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
        private Vector2 markerRealPos;
        private Vector2 pointRealPos;


        private IEnumerator FeedARCamera()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

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
                            //SetPositions(corners[i]);
                        }
                        TestManager.UpdateDetected(ids);

                        //CvAruco.DrawDetectedMarkers(mat, corners, ids);
                        CvAruco.DrawDetectedMarkers(grayMat, corners, ids);
                    }

                    //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGBA);
                    outputTexture = Unity.MatToTexture(grayMat);
                    RawImage.texture = outputTexture;
                    RawImage.material.mainTexture = outputTexture;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.StackTrace + "\n" + e.Message);
                }

                if (grayMat != null)
                    grayMat.Dispose();
                if (mat != null)
                    mat.Dispose();
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
            Vector2 point = Vector2.zero;
            foreach(var corner in corners)
                point += new Vector2(corner.X, corner.Y);
            point /= corners.Length;
            markerRealPos = point;

            // TODO: odpowiednie rogi znaleŸæ i odj¹æ wspó³rzêdne (jeœli to jest od lewego górnego w prawo)
            var edge = corners[2] - corners[1];
            float alpha = Mathf.Atan2(edge.Y, edge.X);

            var dist = pointBoardPos - markerBoardPos;
            float beta = Mathf.Atan2(dist.y, dist.x);

            float len = (float)Math.Sqrt(dist.x * dist.x + dist.y * dist.y);
            pointRealPos = new Vector2(Mathf.Cos(alpha + beta) * len, Mathf.Sin(alpha + beta) * len);
        }


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