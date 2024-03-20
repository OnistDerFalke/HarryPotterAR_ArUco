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
	using UnityEngine.Networking.Types;
	using TMPro;

	public class MarkerDetector : MonoBehaviour
	{
		//Debugging
		public bool debugMode;
		public TextMeshProUGUI log;
		public GameObject[] debugPoints;

		//On-screen camera
		public Camera cam;

		//3D models
		public GameObject[] models = new GameObject[9];
		private int[] ids;

		//Canvas objects
		public RawImage rawimage;
		public RectTransform rawTransform;
		public CanvasScaler canvasScaler;
		public Canvas canvas;

		//Physical camera
		private WebCamTexture webcamTexture;

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

		//Numeric variables
		private float basicMaxLength = 120f;
		private int frameBroker = 0;

		private void Start()
		{
			// Create default parameres for detection
			detectorParameters = DetectorParameters.Create();

			// Dictionary holds set of all available markers
			dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

			var res = WebCamTexture.devices[0].availableResolutions;
			int bestRatioIndex = -1;
			float bestDistance = 100;
			var wantedAspect = Screen.width / Screen.height;
			log.text += $"{Screen.width} {Screen.height}\n";
			if (res != null) 
			{
				for(var i=res.Length-1; i>=0; i--)
				{
					var ar = res[i].width / res[i].height;
					if(Mathf.Abs(wantedAspect-ar)<= bestDistance)
                    {
						bestDistance = Mathf.Abs(wantedAspect - ar);
						bestRatioIndex = i;
					}
				}
				log.text += $"{res[bestRatioIndex]} \n";
				webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, res[bestRatioIndex].width, res[bestRatioIndex].height);
			}
			else
            {
				webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
			}

			// New webcamtexture
			
			webcamTexture.Play();

			// Scaling whole image with webcamtexture size
			float scale = canvas.gameObject.GetComponent<RectTransform>().rect.height / webcamTexture.height;
			rawTransform.localScale = new Vector3(webcamTexture.width * scale / rawTransform.rect.width, webcamTexture.height * scale / rawTransform.rect.height, 1f);

			//Setting texture for raw image and new texture for processed image for CV
			rawimage.texture = webcamTexture;
			rawimage.material.mainTexture = webcamTexture;
			texture = new Texture2D(rawimage.texture.width, rawimage.texture.height, TextureFormat.RGBA32, false);

			//Fixes camera rotate by 90deg on mobile device 
			transform.rotation = transform.rotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.back);

			//log.text += $"Canvas: {canvas.gameObject.GetComponent<RectTransform>().rect.width}, {canvas.gameObject.GetComponent<RectTransform>().rect.height}\n";
			//log.text += $"RawTransform: {rawTransform.rect.width},  {rawTransform.rect.}\n";
		}

		void Update()
		{
			//Debug
			log.gameObject.SetActive(debugMode);

			//obraz z kamery
			if (webcamTexture.isPlaying)
			{
				colors = webcamTexture.GetPixels32();
				texture.SetPixels32(colors);
				texture.Apply();

                //TODO: Do we really need broker to optimize - it reduces the quality
                if (frameBroker % 10 != 0)
                {
                    frameBroker++;
                    return;
                }
                frameBroker = 1;

                //TODO: Would be nice to optimize by cropping unused image
                //TODO: May not be so important if we find another phone camera with better FOV
                //this.texture2 = CropImage(this.texture, canvas.gameObject.GetComponent<RectTransform>().rect.width, canvas.gameObject.GetComponent<RectTransform>().rect.height);
                mat = Unity.TextureToMat(this.texture);

				//Rect roi = new Rect(pixelsToCut, 0, image.Width - 2 * pixelsToCut, image.Height);
				//Mat croppedImage = new Mat(mat, roi);

				grayMat = new Mat();
				Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

				//var rawScale = rawTransform.transform.localScale;
				//Debug.Log($"Mat: {mat.Size()} \tRawimage: ({rawTransform.rect.width * rawScale.x}, {rawTransform.rect.height * rawScale.y}) " +
				//	$"\tCanvas: ({canvas.gameObject.GetComponent<RectTransform>().rect.width}, {canvas.gameObject.GetComponent<RectTransform>().rect.height})");

				CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

				//Clear
				foreach (var point in debugPoints)
					point.SetActive(false);

				for (int id = 0; id < ids.Length; id++)
					if (ids[id] < 9)
						DrawModel(id, ids[id]);

				for (int id = 0; id < 9; id++)
					if (!ids.Contains(id))
						models[id].SetActive(false);

				//To draw RGB lines on axes
				//CvAruco.DrawDetectedMarkers(mat, corners, ids);

				//if(outputTexture != null)
				//	Destroy(outputTexture);
				//outputTexture = Unity.MatToTexture(mat);
				//            rawimage.texture = outputTexture;

				grayMat.Dispose();
				mat.Dispose();
			}
			else
				Debug.Log("Kamera nie działa poprawnie.");
		}

		private Texture2D CropImage(Texture2D source, float width, float height)
		{
			var rawScale = rawTransform.transform.localScale;
			Debug.Log($"Canvas: {width}, {height}\trawImage: {source.width}, {source.height}\tScale: {rawScale}");
			width = width / rawScale.x;
			height = height / rawScale.y;
			int x = (int)(source.width - width) / 2;
			int y = 0;

			// Sprawdź czy wartości nie wychodzą poza granice obrazka
			if (x < 0 || y < 0 || width + x > source.width || height + y > source.height)
			{
				Debug.Log($"{x}, {y}, {width + x} > {source.width}, {height + y} > {source.height}");
				Debug.LogError("Próba obcięcia poza granicami obrazka!");
				return null;
			}

			// Tworzenie nowego obrazka z obciętą częścią
			Texture2D croppedImage = new Texture2D((int)width, (int)height);
			Color[] pixels = source.GetPixels(x, y, (int)width, (int)height);
			croppedImage.SetPixels(pixels);
			croppedImage.Apply();

			Debug.Log(croppedImage);

			return croppedImage;
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
			
			//Needs refreshing mat in update (need to uncomment it to have it working)
			//CvAruco.DrawAxis(mat, cameraMatrix, dist, rvec, tvec, marker_size / 2);

			//position, rotation and scale to model
			models[modelId].transform.localPosition = GetPosition(tvec, imagePoints, models[modelId].transform.localScale, true);
			models[modelId].transform.localRotation = GetRotation(rvec);
			models[modelId].transform.localScale = GetScale(objPts, imagePoints);
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
			var tvecUnity = new Vector3(
				 (rawTransform.rect.width / mat.Size().Width) * (float)tvec[0],
				 (rawTransform.rect.width / mat.Size().Width) * (float)-tvec[1],
				 (rawTransform.rect.width / mat.Size().Width) * (float)tvec[2]);

			Vector3 localpos = Vector3.zero;
			Vector3[] corn = new Vector3[4];

			for (var i = 0; i < 4; i++)
			{
				if (debug)
				{
					debugPoints[i].SetActive(true);
					debugPoints[i].transform.localPosition = Point2fToVec3Scaled(imagePoints[i]);
				}

				corn[i] = Point2fToVec3Scaled(imagePoints[i]);
				localpos += Point2fToVec3Scaled(imagePoints[i]);
			}

			localpos /= 4f;

			return localpos;
		}

		private Quaternion GetRotation(double[] rvec)
		{
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

			return worldrot;
		}

		Vector3 Point2fToVec3Scaled(Point2f point)
		{
			var xScaler = rawTransform.rect.width / mat.Size().Width;
			var yScaler = rawTransform.rect.height / mat.Size().Height;
			return new Vector3(point.X * xScaler - rawTransform.rect.width / 2, -(point.Y * yScaler - rawTransform.rect.height / 2), 0);
		}
	}
}