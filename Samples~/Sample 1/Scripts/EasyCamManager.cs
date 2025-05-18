using UnityEngine;

namespace GAG.EasyWebCam
{
    public class EasyCamManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            EasyCamEvents.OnSavePhotoPathCreated += OnReceivedPhotoPath;
        }
        void OnDisable()
        {
            EasyCamEvents.OnSavePhotoPathCreated -= OnReceivedPhotoPath;
        }

        public void InitializeCamera()
        {
            EasyCamEvents.RaiseOnCameraInitialized();
        }

        public void SwitchOnCamera(int cameraIndex)
        {
            EasyCamEvents.RaiseOnCameraStarted(cameraIndex);
        }

        public void TakePhoto()
        {
            EasyCamEvents.RaiseOnCameraCaptured();
        }

        public void CreatePreviewUI()
        {
            EasyCamEvents.RaiseOnPreviewUICreated();
        }

        public void CaptureAndSavePreview()
        {
            EasyCamEvents.RaiseOnPreviewCapturedAndSaved();
        }

        public void OnReceivedPhotoPath(string path)
        {
            print("Photo saved at: " + path);
        }
    }
}
