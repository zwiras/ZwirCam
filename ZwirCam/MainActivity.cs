using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ZwirCam
{
    [Activity(Label = "ZwirCam", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button button2 = FindViewById<Button>(Resource.Id.buttonFaceDet);
            button2.Click += button2_Click;
        }

        void button2_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(FaceDetectionActivity)); 
        }
    }
}

