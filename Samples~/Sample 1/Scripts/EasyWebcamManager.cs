using UnityEngine;

namespace GAG.EasyWebCam
{
    public class EasyWebcamManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            EasyWebcamEvents.OnSavePhotoPathCreated += OnReceivedPhotoPath;
        }
        void OnDisable()
        {
            EasyWebcamEvents.OnSavePhotoPathCreated -= OnReceivedPhotoPath;
        }

        public void InitializeCamera()
        {
            EasyWebcamEvents.RaiseOnCameraInitialized();
        }

        public void SwitchOnCamera(int cameraIndex)
        {
            EasyWebcamEvents.RaiseOnCameraStarted(cameraIndex);
        }

        public void TakePhoto()
        {
            EasyWebcamEvents.RaiseOnCameraCaptured();
        }

        public void CreatePreviewUI()
        {
            EasyWebcamEvents.RaiseOnPreviewUICreated();
        }

        public void CaptureAndSavePreview()
        {
            EasyWebcamEvents.RaiseOnPreviewCapturedAndSaved();
        }

        public void OnReceivedPhotoPath(string path)
        {
            print("Photo saved at: " + path);
        }
    }
}
