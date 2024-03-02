namespace OpenCvSharp.Demo 
{

	using UnityEngine;
	using System.Collections;
	using UnityEngine.UI;
	using Aruco;
	using OpenCvSharp.Util;
	using System.Linq;
    using System.Collections.Generic;
	using OpenCvSharp.Aruco;
	using OpenCvSharp;

    public class MarkerDetector : MonoBehaviour 
	{
		public RawImage rawimage;
		public GameObject model;
		public RectTransform rawTransform;

		public Camera cam;

		public Texture2D texture;

		private DetectorParameters detectorParameters;
		private Dictionary dictionary;

		private Point2f[][] corners;
		private int[] ids;
		private Point2f[][] rejectedImgPoints;

		private WebCamTexture webcamTexture;
		private Color32[] colors;
		private Mat mat, grayMat;

		private Texture2D outputTexture;
		private void Start()
        {
			var aspectRatio = Screen.height / 600f;
			
			rawTransform.localScale = 
				new Vector3((float)Screen.height / 600f, (float)Screen.height / 600f, 1f);

			// Create default parameres for detection
			detectorParameters = DetectorParameters.Create();

			// Dictionary holds set of all available markers
			dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

			webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
			webcamTexture.Play();

			rawimage.texture = webcamTexture;
			rawimage.material.mainTexture = webcamTexture;

			texture = new Texture2D(rawimage.texture.width, rawimage.texture.height, TextureFormat.RGBA32, false);
		}

        void Update () 
		{
			//obraz z kamery
			if (webcamTexture.isPlaying)
			{
				colors = webcamTexture.GetPixels32();
                texture.SetPixels32(colors);
                texture.Apply();

                mat = Unity.TextureToMat(this.texture);
				grayMat = new Mat();
				Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

				CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

				for (int id = 0; id < ids.Length; id++)
					DrawModel(id);
				//CvAruco.DrawDetectedMarkers(mat, corners, ids);

				if(outputTexture != null)
					Destroy(outputTexture);
				outputTexture = Unity.MatToTexture(mat);
                rawimage.texture = outputTexture;

				grayMat.Dispose();
				mat.Dispose();
			}
			else 
				Debug.Log("Kamera nie działa poprawnie.");
		}

		void DrawModel(int id)
        {
			Point2f center = corners[id][0] + corners[id][1] + corners[id][2] + corners[id][3];
			Point3d center3D = new Point3d(center.X / 4f, center.Y / 4f, 0);
			
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

			var objPts = new Point3f[]
			{
				new Point3f(0,0,0),
				new Point3f(0,1,0),
				new Point3f(1,1,0),
				new Point3f(1,0,0)
			};

			Cv2.SolvePnP(objPts, corners[id], cameraMatrix, dist, out rvec, out tvec);
			CvAruco.DrawAxis(mat, cameraMatrix, dist, rvec, tvec, 0.5f);
			
			//position
			Vector3 localpos;
			localpos.x = (float)tvec[0];
			localpos.y = -(float)tvec[1];
			localpos.z = (float)tvec[2];
			
			Vector3 worldpos = cam.transform.TransformPoint(localpos);
            model.transform.position = worldpos;

			//rotation
			double[] flip = rvec;
			flip[1] = -flip[1];
			rvec = flip;
			
			Mat rotmatrix = new Mat();			
			MatOfDouble rvecMat = new MatOfDouble(1, 3);
			rvecMat.Set<double>(0, 0, rvec[0]);
			rvecMat.Set<double>(0, 1, rvec[1]);
			rvecMat.Set<double>(0, 2, rvec[2]);
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

			model.transform.rotation = worldrot;
		}
	}
}