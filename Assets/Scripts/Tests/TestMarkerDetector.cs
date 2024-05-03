namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using UnityEngine.UI;
    using Aruco;
    using OpenCvSharp;
    using Assets.Scripts;
    using System.Collections;
    using System.Collections.Generic;

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

        private IEnumerator FeedARCamera()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);

                mr = cam.gameObject.GetComponentInChildren<MeshRenderer>();

                if (mr == null)
                {
                    Debug.Log("Renderer not found");
                    continue;
                }

                Destroy(texture);
                texture = (mr.material.mainTexture as Texture2D);

                float scale = canvas.GetComponent<RectTransform>().rect.height / texture.height;
                RawImage.rectTransform.sizeDelta = new Vector2(texture.width * scale, texture.height * scale);

                try
                {
                    mat = new Mat();
                    Unity.TextureConversionParams par = new Unity.TextureConversionParams();
                    par.FlipVertically = true;
                    mat = Unity.TextureToMat(texture, par);

                    if (TestManager.DetectingStarted)
                    {
                        grayMat = new Mat();
                        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

                        CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

                        for(int i = 0; i < ids.Length; i++)
                            TestManager.UpdatePositions(ids[i], ConvertToList(corners[i]));
                        TestManager.UpdateDetected(ids);

                        CvAruco.DrawDetectedMarkers(mat, corners, ids);
                    }

                    outputTexture = Unity.MatToTexture(mat);
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