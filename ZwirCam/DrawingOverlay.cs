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
using Android.Graphics;
using Android.Gms.Vision.Faces;

namespace ZwirCam
{
    class DrawingOverlay : GraphicOverlay.FrameGraphic
    {
        Paint frame;  // frame color that is drawn when the face is detected
        Face face;

        public DrawingOverlay(GraphicOverlay graphicOverlay)
            : base(graphicOverlay)
        {
            SetFrame();
        }

        private void SetFrame()
        {
            frame = new Paint();
            frame.Color = Color.Green;
            frame.SetStyle(Paint.Style.Stroke);
            frame.StrokeWidth = 5.0f;
        }

        /**
         * Updates the detected face and redraws the graph (frame)
         */
        public void UaktualnijTwarz(Face twarz)
        {
            this.face = twarz;
            PostInvalidate();
        }

        /**
         * Draw a frame based on the detected face
         */
        public override void Draw(Canvas canvas)
        {
            if (face == null)
                return;

            float x = TransformX(face.Position.X + face.Width / 2);
            float y = TransformY(face.Position.Y + face.Height / 2);

            // frame boundaries 
            float offsetX = ScalingX(face.Width / 2.0f);
            float offsetY = ScalingY(face.Height / 2.0f);
            float left = x - offsetX;
            float top = y - offsetY;
            float right = x + offsetX;
            float bottom = y + offsetY;

            canvas.DrawRect(left, top, right, bottom, frame);
        }
    }
}