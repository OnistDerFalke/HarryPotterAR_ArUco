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
			Debug.Log(corners[id][0] + " " + corners[id][1] + " " + corners[id][2] + " " + corners[id][3]);
			Point2f center = corners[id][0] + corners[id][1] + corners[id][2] + corners[id][3];
			Point3d center3D = new Point3d(center.X / 4, center.Y / 4, 0);

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

			//var objPts = new Point3f[]
			//{
			//	new Point3f(0,1,0),
			//	new Point3f(1,1,0),
			//	new Point3f(1,0,0),
			//	new Point3f(0,0,0)
			//};

			var objPts = new Point3f[]
			{
				new Point3f(0,0,0),
				new Point3f(0,1,0),
				new Point3f(1,1,0),
				new Point3f(1,0,0)
			};

			//Cv2.CalibrateCamera

			//double[,] jacobian;
			//Point2f[] imgPts;
			//Cv2.ProjectPoints(objPts, rvec, tvec, cameraMatrix, dist, out imgPts, out jacobian);
			Cv2.SolvePnP(objPts, corners[id], cameraMatrix, dist, out rvec, out tvec);

			//Debug.Log($"{rvec[0]}, {rvec[1]}, {rvec[2]} {tvec[0]}, {tvec[1]}, {tvec[2]}");

			CvAruco.DrawAxis(mat, cameraMatrix, dist, rvec, tvec, 0.5f);
			
			model.transform.localPosition = Point3dToVector3(GetWorldPoint(center3D, rvec, tvec));
		}

		private Point3d GetWorldPoint(Point3d input, double[] rvec, double[] tvec)
		{
			var rot_mat = new Mat();
			Cv2.Rodrigues(MatOfDouble.FromArray(rvec[0], rvec[1], rvec[2]), rot_mat);

			var pointProject = (rot_mat.Transpose() * MatOfDouble.FromArray(input.X, input.Y, input.Z)).ToMat();
			
			return new Point3d(tvec[0], tvec[1], tvec[2]) + 
				new Point3d(pointProject.At<double>(0, 0), pointProject.At<double>(0, 1), pointProject.At<double>(0, 2));
		}

		private Vector3 Point3dToVector3(Point3d p)
        {
			//Point3d p3d = new Point3d(-100, 250, -340);
			//return new Vector3((float)p3d.X, (float)p3d.Y, (float)p3d.Z);
			return new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }
	}
}