namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using Aruco;
    using OpenCvSharp;
    using Assets.Scripts;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TestMarkerDetector : MonoBehaviour
    {
        [SerializeField] public GameObject[] models = new GameObject[90];//9 characters, 81 to board
        public float basicMaxLength = 35f;
        public float posThreshold = 20f;

        public Camera cam;
        private int[] ids;
        public Canvas canvas;

        //OpenCV textures and mats
        private Texture2D texture;
        private Mat mat;
        //private Mat temp;
        private Mat grayMat;

        //ArUco stuff
        private DetectorParameters detectorParameters;
        private Dictionary dictionary;
        private Point2f[][] corners;
        private Point2f[][] rejectedImgPoints;
        private MeshRenderer mr;
        //private Texture2D outputTexture;

        //[SerializeField] public RawImage RawImage;


        private IEnumerator FeedARCamera()
        {
            while (true)
            {
                
                yield return new WaitForEndOfFrame();

                try
                {
                    //texture = new Texture2D(Screen.width, Screen.height);
                    texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);

                    mr = cam.gameObject.GetComponentInChildren<MeshRenderer>();

                    if (mr == null)
                    {
                        Debug.Log("Renderer not found");
                        continue;
                    }

                    Destroy(texture);
                    texture = (mr.material.mainTexture as Texture2D);

                    //RawImage.rectTransform.sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
                }
                catch (System.Exception e)
                {
                    TestManager.Logs += e.StackTrace + "\n" + e.Message + "\n";
                }

                try
                {
                    mat = new Mat();
                    mat = Unity.TextureToMat(texture);
                    //Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
                    Cv2.Flip(mat, mat, FlipMode.Y);

                    grayMat = new Mat();
                    Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

                    if (TestManager.DetectingStarted)
                    {
                        CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

                        Dictionary<int, List<Vector2>> dict = new();
                        for (int i = 0; i < ids.Length; i++)
                        {
                            dict[ids[i]] = ConvertToList(corners[i]);
                            DrawModel(ids[i], corners[i]);
                        }
                        TestManager.UpdateDetected(dict);

                        for (int i = 0; i < models.Count(); i++)
                            if (!ids.Contains(i))
                                models[i].SetActive(false);

                        //CvAruco.DrawDetectedMarkers(grayMat, corners, ids);
                    }
                    else
                    {
                        for (int i = 0; i < models.Count(); i++)
                            models[i].SetActive(false);
                    }

                    //Destroy(outputTexture);

                    //outputTexture = Unity.MatToTexture(grayMat);
                    //RawImage.texture = outputTexture;
                    //RawImage.material.mainTexture = outputTexture;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.StackTrace + "\n" + e.Message);
                    //TestManager.Logs += e.StackTrace + "\n" + e.Message + "\n";
                }

                if (grayMat != null) grayMat.Dispose();
                if (mat != null) mat.Dispose();
                //if (temp != null) temp.Dispose();
            }
        }

        private List<Vector2> ConvertToList(Point2f[] pts)
        {
            List<Vector2> list = new();
            foreach (var pt in pts)
                list.Add(new Vector2(pt.X, pt.Y));
            return list;
        }

        void DrawModel(int modelId, Point2f[] corners)
        {
            var rvec = new double[] { 0, 0, 0 };
            var tvec = new double[] { 0, 0, 0 };

            var cameraMatrix = new double[3, 3]
            {
                { 1.6794270741269229e+03, 0.0f, 9.5502638499315810e+02 },
                { 0.0f, 1.6771700842601067e+03, 5.5015085362115587e+02 },
                { 0.0f, 0.0f, 1.0f }
            };

            var dist = new double[] { 1.9683061388899192e-01, -7.3624850611674697e-01,
                1.4290920850579334e-04, 7.8329305994069088e-04, 5.4742631511243833e-01 };

            float marker_size = 0.03f;

            var objPts = new Point3f[]
            {
                new Point3f(-marker_size / 2, marker_size / 2, 0),
                new Point3f(marker_size / 2, marker_size / 2, 0),
                new Point3f(marker_size / 2, -marker_size / 2, 0),
                new Point3f(-marker_size / 2, -marker_size / 2, 0)
            };

            Cv2.SolvePnP(objPts, corners, cameraMatrix, dist, out rvec, out tvec);
            Point2f[] imagePoints;
            double[,] jacobian;

            Cv2.ProjectPoints(objPts, rvec, tvec, cameraMatrix, dist, out imagePoints, out jacobian);

            models[modelId].transform.localPosition = GetPosition(imagePoints, models[modelId].transform.localPosition);
            models[modelId].transform.localRotation = GetRotation(rvec, models[modelId].transform.localRotation);
            models[modelId].transform.localScale = GetScale(objPts, imagePoints);
            models[modelId].SetActive(true);
        }

        private Vector3 GetScale(Point3f[] objPts, Point2f[] imgPts)
        {
            var distance01 = GetDistance(imgPts[0], imgPts[1]);
            var distance12 = GetDistance(imgPts[1], imgPts[2]);
            var distance23 = GetDistance(imgPts[2], imgPts[3]);
            var distance30 = GetDistance(imgPts[3], imgPts[0]);
            var maxDistance = Mathf.Max(distance01, distance12, distance23, distance30);

            var s = maxDistance / basicMaxLength;
            Vector3 scale = new Vector3(s, s, s);

            return scale;
        }

        private float GetDistance(Point2f a, Point2f b)
        {
            return Vector2.Distance(new Vector2(a.X, a.Y), new Vector2(b.X, b.Y));
        }

        private Vector3 GetPosition(Point2f[] imagePoints, Vector3 currentPos)
        {
            Vector3 localpos = Vector3.zero;
            Vector3[] corn = new Vector3[4];

            for (var i = 0; i < 4; i++)
            {
                localpos += Point2fToVec3Scaled(imagePoints[i]);
            }

            localpos /= 4f;

            localpos.x *= -1f;
            localpos.y *= -1f;

#if !UNITY_EDITOR && UNITY_ANDROID
            localpos = new Vector3(localpos.y, -localpos.x, localpos.z);
#endif

            if (Vector3.Distance(currentPos, localpos) <= posThreshold)
                return currentPos;

            return localpos;
        }

        private Quaternion GetRotation(double[] rvec, Quaternion oldRot)
        {
            double[] flip = rvec;
            flip[0] = flip[0];
            flip[1] = -flip[1];
            flip[2] = flip[2];
            double[] rvec_m = flip;

            Mat rotmatrix = new Mat();
            MatOfDouble rvecMat = new MatOfDouble(1, 3);
            rvecMat.Set<double>(0, 0, rvec_m[0]);
            rvecMat.Set<double>(0, 1, rvec_m[1]);
            rvecMat.Set<double>(0, 2, rvec_m[2]);
            Cv2.Rodrigues(rvecMat, rotmatrix);

            Vector3 forward;
            forward.x = (float)rotmatrix.At<double>(2, 0);
            forward.y = (float)rotmatrix.At<double>(2, 1);
            forward.z = (float)rotmatrix.At<double>(2, 2);

            Vector3 up;
            up.x = (float)rotmatrix.At<double>(1, 0);
            up.y = (float)rotmatrix.At<double>(1, 1);
            up.z = (float)rotmatrix.At<double>(1, 2);

            Quaternion rot = Quaternion.LookRotation(forward, up);
            rot *= Quaternion.Euler(0, 0, 180);
            Quaternion worldrot = cam.transform.rotation * rot;

            Vector3 rotVec = worldrot.eulerAngles;
            rotVec.x = -worldrot.eulerAngles.x;
            rotVec.y = -worldrot.eulerAngles.y;
            rotVec.z = worldrot.eulerAngles.z - 180f;
            worldrot = Quaternion.Euler(rotVec);

#if !UNITY_EDITOR && UNITY_ANDROID            
            Vector3 rotVec2 = worldrot.eulerAngles;
            rotVec2.x = worldrot.eulerAngles.y;
            rotVec2.y = worldrot.eulerAngles.x;
            rotVec2.z = worldrot.eulerAngles.z - 90;
            worldrot = Quaternion.Euler(rotVec2);
#endif

            Vector3 tmpRot = worldrot.eulerAngles;
            var limitRot = 15f;
            var diff = tmpRot - oldRot.eulerAngles;
            var t = Vector3.Lerp(oldRot.eulerAngles, tmpRot, limitRot / (tmpRot - oldRot.eulerAngles).magnitude);
            if (Mathf.Abs(diff.y) > limitRot)
                tmpRot.y = t.y;

            return Quaternion.Euler(tmpRot);
        }

        Vector3 Point2fToVec3Scaled(Point2f point)
        {
            if (mr == null)
            {
                Debug.Log("Renderer not found");
                return new Vector3(point.X, -point.Y, 0);
            }
            Vector2 mrsize = new Vector2(2 * mr.gameObject.transform.localScale.x, 2 * mr.gameObject.transform.localScale.z);
            var xScaler = mrsize.x / mat.Size().Width;
            var yScaler = mrsize.y / mat.Size().Height;

            return new Vector3((point.X * xScaler - mrsize.x / 2), -(point.Y * yScaler - mrsize.y / 2), 0);
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