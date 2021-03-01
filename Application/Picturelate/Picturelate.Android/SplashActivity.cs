using Android.App;
using Android.Support.V7.App;
using Picturelate.Droid;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Picturelate
{
    [Activity(Label = "Picturelate", Icon = "@drawable/PicturelateIcon", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
    class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}
