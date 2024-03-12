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
		public GameObject[] testPoints;
		public RawImage rawimage;
		public GameObject[] models = new GameObject[9];
		public RectTransform rawTransform;
		public CanvasScaler canvasScaler;
		public Canvas canvas;

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
					if(ids[id] < 9)
						DrawModel(id, ids[id]);

				for (int id = 0; id < 9; id++)
					if (!ids.Contains(id))
						models[id].SetActive(false);

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

		void DrawModel(int foundId, int modelId)
		{
			models[modelId].SetActive(true);

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
			Cv2.ProjectPoints(objPts, rvec, tvec, cameraMatrix, dist, out imagePoints, out jacobian);
			CvAruco.DrawAxis(mat, cameraMatrix, dist, rvec, tvec, marker_size/2);

			//rotation
			double[] flip = rvec;
			flip[1] = -flip[1];
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
			//Quaternion localrot = Quaternion.Inverse(models[modelId].transform.rotation) * worldrot;


			//position
			var tvecUnity = new Vector3(
				 (rawTransform.rect.width / mat.Size().Width) * (float)tvec[0],
				 (rawTransform.rect.width / mat.Size().Width) * (float)-tvec[1],
				 (rawTransform.rect.width / mat.Size().Width) * (float)tvec[2]);

			Vector3 localpos = Vector3.zero;
			Vector3[] corn = new Vector3[4];
			for (var i = 0; i < 4; i++)
			{
				testPoints[i].SetActive(true);
				testPoints[i].transform.localPosition = Point2fToVec3Scaled(imagePoints[i]);
				testPoints[i].transform.rotation = worldrot;
				corn[i] = Point2fToVec3Scaled(imagePoints[i]);
				localpos += Point2fToVec3Scaled(imagePoints[i]);
			}

			localpos /= 4f;
			//var globalpos = models[modelId].transform.TransformPoint(localpos);
			//localpos = models[modelId].transform.InverseTransformPoint(globalpos);

			models[modelId].transform.localPosition = localpos;
			models[modelId].SetActive(true);
   //         var temp = models[modelId].transform.position;
			//temp.z = tvecUnity.z;//canvas.planeDistance - models[modelId].GetComponent<Collider>().bounds.size.y;
   //         models[modelId].transform.position = temp;

            models[modelId].transform.localRotation = Quaternion.Inverse(models[modelId].transform.parent.rotation) * worldrot;

		}


        Vector3 Point2fToVec3Scaled(Point2f point)
        {
			var xScaler = rawTransform.rect.width / mat.Size().Width;
			var yScaler = rawTransform.rect.height / mat.Size().Height;
			return new Vector3(point.X * xScaler - rawTransform.rect.width/2, -(point.Y * yScaler - rawTransform.rect.height/2), 0);
        }
	}
}