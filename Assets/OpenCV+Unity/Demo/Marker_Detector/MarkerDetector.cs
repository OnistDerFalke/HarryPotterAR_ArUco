namespace OpenCvSharp.Demo 
{

	using UnityEngine;
	using System.Collections;
	using UnityEngine.UI;
	using Aruco;
	using OpenCvSharp.Util;

	public class MarkerDetector : MonoBehaviour 
	{
		public RawImage rawimage;

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
				
				//TODO: Delete
				CvAruco.DrawDetectedMarkers(mat, corners, ids);

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
		
	}
}