namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using UnityEngine.UI;
    using Aruco;
    using System.Linq;
    using OpenCvSharp;
    using TMPro;
    using Assets.Scripts;
    using Vuforia;
    using Image = Vuforia.Image;
    using System.Collections;

    public class MarkerDetector : MonoBehaviour
    {
        //Debugging
        public bool debugMode;
        public TextMeshProUGUI log;
        //public GameObject[] debugPoints;

        //On-screen camera
        public Camera cam;

        //3D models
        //TODO: add other models to board markers
        [SerializeField] public GameObject[] models = new GameObject[90];       //9 characters, 81 to board
        public float basicMaxLength = 20f;
        private int[] ids;

        //Canvas objects
        public RawImage rawimage;
        public RectTransform rawTransform;
        public CanvasScaler canvasScaler;
        public Canvas canvas;

        //OpenCV textures and mats
        private Texture2D texture;
        private Texture2D texture2;
        private Texture2D outputTexture;
        private Mat mat;
        private Mat grayMat;

        //ArUco stuff
        private DetectorParameters detectorParameters;
        private Dictionary dictionary;
        private Point2f[][] corners;
        private Point2f[][] rejectedImgPoints;
        private Color32[] colors;
        private MeshRenderer mr;

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
                //rawimage.texture = texture;
                //rawimage.material.mainTexture = texture;
                //rawimage.SetNativeSize();


                    try
                    {
                        mat = new Mat();
                        Unity.TextureConversionParams par = new Unity.TextureConversionParams();
                        par.FlipVertically = true;
                        //par.RotationAngle = 90;
                        mat = Unity.TextureToMat(texture, par);
                        //Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
                    
                        grayMat = new Mat();
                        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
                    //Cv2.ImShow("window", grayMat);

                    CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
                    

                    for (int id = 0; id < ids.Length; id++)
                        DrawModel(id, ids[id]);  

                    for (int id = 0; id < models.Length; id++)
                        if (!ids.Contains(id))
                            UntrackModel(id);

                    }
                    catch (System.Exception e)
                    {
                        log.text += e.StackTrace + '\n';
                        log.text += e.Message + '\n';
                    }
                grayMat.Dispose();
                    mat.Dispose();
               
            }
        }


        void Start()
        {

            foreach (var model in models)
                model.SetActive(false);

           
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

            //Debug
            log.gameObject.SetActive(debugMode);

            // Create default parameres for detection
            detectorParameters = DetectorParameters.Create();

            // Dictionary holds set of all available markers
            dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

        }

        public GameObject FindModelById(int id)
        {
            if (id >= models.Count() || id < 0)
                return null;
            return models[id];
        }

        private void UntrackModel(int id)
        {
            if (GameManager.CurrentTrackedObjects.ContainsKey(id))
            {
                models[id].SetActive(false);
                GameManager.CurrentTrackedObjects.Remove(id);
                EventBroadcaster.InvokeOnMarkerLost(id);
            }
        }

        private void TrackModel(int id, Vector3 markerPos)
        {
            var isCharacter = id < 9 && id >= 0;

            if (!GameManager.CurrentTrackedObjects.ContainsKey(id))
            {
                models[id].SetActive(isCharacter);
                GameManager.CurrentTrackedObjects.Add(id, markerPos);
                EventBroadcaster.InvokeOnMarkerDetected(id);
            }
            else
            {
                //TODO: zrobić to też w Vuforii - bo tak powinno być dokładniejsze wykrywanie pól
                GameManager.CurrentTrackedObjects[id] = markerPos;
            }
        }

        void DrawModel(int foundId, int modelId)
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

            Cv2.SolvePnP(objPts, corners[foundId], cameraMatrix, dist, out rvec, out tvec);
            Point2f[] imagePoints;
            double[,] jacobian;

            //rvec[0] = -rvec[0];
            //rvec[1] = rvec[1];
            //rvec[2] = -rvec[2];

            Cv2.ProjectPoints(objPts, rvec, tvec, cameraMatrix, dist, out imagePoints, out jacobian);

            //Needs refreshing mat in update (need to uncomment it to have it working)
            //CvAruco.DrawAxis(mat, cameraMatrix, dist, rvec, tvec, marker_size / 2);

            //position, rotation and scale to model
            models[modelId].transform.localPosition = GetPosition(tvec, imagePoints, models[modelId].transform.localScale, true);
            models[modelId].transform.localRotation = GetRotation(rvec);

            var factor = (int)GameManager.GetMyPlayer().Character - 1 == modelId ? 2.1f : 1.3f;
            factor = (int)Character.Luna - 1 == modelId ? 1.45f : factor;
            models[modelId].transform.localScale = GetScale(objPts, imagePoints) * factor;

            TrackModel(modelId, models[modelId].transform.position);
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

        private Vector3 GetPosition(double[] tvec, Point2f[] imagePoints, Vector3 modelScale, bool debug = false)
        {
            Vector3 localpos = Vector3.zero;
            Vector3[] corn = new Vector3[4];

            for (var i = 0; i < 4; i++)
            {
                //if (debug)
                //{
                //    debugPoints[i].SetActive(true);
                //    debugPoints[i].transform.localPosition = Point2fToVec3Scaled(imagePoints[i]);
                //}

                //corn[i] = Point2fToVec3Scaled(imagePoints[i]);
                localpos += Point2fToVec3Scaled(imagePoints[i]);
            }

            localpos /= 4f;
#if !UNITY_EDITOR && UNITY_ANDROID
            localpos = new Vector3(localpos.y, -localpos.x, localpos.z);
#endif
            //Debug.Log($"X point: {localpos}");
            return localpos;
        }

        private Quaternion GetRotation(double[] rvec)
        {
            double[] flip = rvec;
#if !UNITY_EDITOR && UNITY_ANDROID
            flip[0] = flip[0];
            flip[1] = -flip[1];
            flip[2] = flip[2];
#else
            flip[0] = flip[0];
            flip[1] = -flip[1];
            flip[2] = flip[2];
#endif
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

#if !UNITY_EDITOR && UNITY_ANDROID
            
            Vector3 rotVec = worldrot.eulerAngles;
            rotVec.x = worldrot.eulerAngles.y;
            rotVec.y = worldrot.eulerAngles.x;
            rotVec.z = worldrot.eulerAngles.z - 90;
            worldrot = Quaternion.Euler(rotVec);
#endif
            return worldrot;
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

            //log.text += "MrSize: " + mrsize.x + " " + mrsize.y + "\n";
            //log.text += "Canvas: " + canvas.GetComponent<RectTransform>().rect.width + " " + canvas.GetComponent<RectTransform>().rect.height + "\n";
            //log.text += "Scale: " + canvas.GetComponent<RectTransform>().localScale;
            //var scalerCanvas = canvas.GetComponent<RectTransform>().rect.height * canvas.GetComponent<RectTransform>().localScale.x / mrsize.y;
            return new Vector3((point.X * xScaler - mrsize.x / 2), -(point.Y * yScaler - mrsize.y / 2), 0);
        }

    }
}