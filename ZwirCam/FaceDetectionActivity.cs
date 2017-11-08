using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision.Faces;
using Android.Gms.Vision;

[assembly: MetaData("com.google.android.gms.vision.DEPENDENCIES", Value = "barcode,face")]

namespace ZwirCam
{
    [Activity(Label = "ZwirCam")]
    public class FaceDetectionActivity : Activity
    {
        const string TAG = "ZwirCam";

        CameraSource cameraSource;
        CameraPreview preview;
        GraphicOverlay graphicOverlay;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.FaceDetector);

            preview = FindViewById<CameraPreview>(Resource.Id.preview);
            graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            FaceDetector detector = new FaceDetector.Builder(Application.Context).Build();
            detector.SetProcessor(new MultiProcessor.Builder(new TrackingFacesGraphics(graphicOverlay)).Build());

            if (!detector.IsOperational)
            {
                //TODO    Face detection is not available
            }

            cameraSource = new CameraSource.Builder(Application.Context, detector)
                .SetRequestedPreviewSize(640, 480)
                .SetFacing(CameraFacing.Front)
                .SetRequestedFps(30.0f)
                .Build();
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                preview.Start(cameraSource, graphicOverlay);
            }
            catch (Exception)
            {
                cameraSource.Release();
                cameraSource = null;
            }
        }


        protected override void OnPause()
        {
            base.OnPause();
            preview.Stop();
        }
        
        protected override void OnDestroy()
        {
            cameraSource.Release();
            base.OnDestroy();
        }

        /**
         * Class for tracking new faces. For each face tracking is separate
        */
        class TrackingFacesGraphics : Java.Lang.Object, MultiProcessor.IFactory
        {
            public GraphicOverlay graphicOverlay { get; private set; }

            public TrackingFacesGraphics(GraphicOverlay graphicOverlay)
                : base()
            {
                this.graphicOverlay = graphicOverlay;
            }

            public Android.Gms.Vision.Tracker Create(Java.Lang.Object item)
            {
                return new TrakerFace(graphicOverlay);
            }
        }

        /**
         * Squeeze graphics (frame) for each detected face.
         */
        class TrakerFace : Tracker
        {
            GraphicOverlay graphicOverlay;
            DrawingOverlay faceGraphic;

            public TrakerFace(GraphicOverlay nakladkaGraficzna)
            {
                this.graphicOverlay = nakladkaGraficzna;
                faceGraphic = new DrawingOverlay(nakladkaGraficzna);
            }

            /**
            * Facial position update
            */
            public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
            {
                graphicOverlay.Remove(faceGraphic);
                graphicOverlay.Add(faceGraphic);
                faceGraphic.UaktualnijTwarz(item.JavaCast<Face>());
            }

            /**
            * Hiding graphics if the face disappears from the camera.
            */
            public override void OnMissing(Detector.Detections detections)
            {
                graphicOverlay.Remove(faceGraphic);
            }

            /**
            *  It triggers if faces disappear from view.
            */
            public override void OnDone()
            {
                graphicOverlay.Remove(faceGraphic);
            }
        }
    }
}

