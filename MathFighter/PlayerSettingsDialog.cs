using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Graphics;
using Android.Provider;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Support.V4.Content;
using Android.Util;
using Android.Media;

namespace MathFighter
{
    public class PlayerSettingsDialog : DialogFragment
    {
        private string playerName;
        private string imgPath;

        private EditText editPlayerName;
        private ImageView ivPlayer;
        private Button btnSave;

        public EventHandler DialogClosed;
        private Context context;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private static string AUTHORITY = "mathfighter.fileprovider";

        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }

        public PlayerSettingsDialog(Context context)
        {
            this.context = context;
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            DialogClosed?.Invoke(this, null);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_player_settings, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            editor = prefs.Edit();

            SetListeners(view);
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }

            return view;
        }

        private void SetListeners(View view)
        {
            ivPlayer = view.FindViewById<ImageView>(Resource.Id.iv_player_settings);
            btnSave = view.FindViewById<Button>(Resource.Id.btn_player_settings_save);

            ivPlayer.Click += IvPlayer_Click;
            btnSave.Click += BtnSave_Click;

            playerName = prefs.GetString("player", null);
            imgPath = prefs.GetString("imgPath", null);

            File file = null;
            var playerImg = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Adrian);
            if (imgPath != null)
            {
                file = new File(imgPath);
            }

            if (file != null)
            {
                playerImg = BitmapFactory.DecodeFile(file.Path);
            }

            //int height = context.Resources.DisplayMetrics.HeightPixels;
            //int width = ivPlayer.Width;
            //if (file != null)
            //{
            //    playerImg = playerImg.PreparePlayerImage(width, height, file.Path);
            //}
            //else playerImg.PreparePlayerImage(width, height);

            ivPlayer.SetImageBitmap(playerImg);

            editPlayerName = view.FindViewById<EditText>(Resource.Id.et_player_settings);
            if (editPlayerName != null)
            {
                editPlayerName.Text = playerName;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            editor.PutString("player", editPlayerName.Text);
            editor.PutString("imgPath", imgPath);
            editor.Apply();
            Dismiss();
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = FileProvider.GetUriForFile(context, AUTHORITY, App._file);
            mediaScanIntent.SetData(contentUri);
            context.SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int width = ivPlayer.Height;
            int height = Resources.DisplayMetrics.HeightPixels;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                var bitmap = App._file.Path.ExifRotateBitmap(App.bitmap);
                var newPath = CreateLesserVersionOfImage(App._file.Path, bitmap);
                if (newPath != null)
                {
                    imgPath = newPath;
                    ivPlayer.SetImageBitmap(BitmapFactory.DecodeFile(newPath));
                }
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();

        }


        // Trying to create a smaller version of the image and save to MathFighterPhotos.
        private string CreateLesserVersionOfImage(string path, Bitmap bitmap)
        {
            //var filePath = path.Substring(0, path.Length - 4) + "small.jpg";
            //var file = new File(filePath);
            

            try
            {
                System.IO.FileStream fos = new System.IO.FileStream(path, System.IO.FileMode.Open);
                Bitmap b = Bitmap.CreateScaledBitmap(bitmap, (int) (bitmap.Width * 0.25), (int) (bitmap.Height * 0.25), false);
                b.Compress(Bitmap.CompressFormat.Jpeg, 50, fos);
                fos.Close();
                return path;
                //return filePath;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity.BaseContext, ex.Message + " \nCould not create smaller version, no image saved", ToastLength.Long).Show();
            }
            return null;
        }

        private void IvPlayer_Click(object sender, EventArgs e)
        {
            //Toast.MakeText(context, "TODO: Change photo!", ToastLength.Long).Show();
            App._file = new File(App._dir, String.Format("playerPhoto_{0}.jpg", Guid.NewGuid()));
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(context, AUTHORITY, App._file));
            try
            {
                StartActivityForResult(intent, 0);
            }
            catch (ActivityNotFoundException ex)
            {
                Log.Debug("MyTAG", ex.Message);
            }
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "MathFighterPhotos");
            App._dir.Mkdirs();
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);

            return availableActivities != null && availableActivities.Count > 0;
        }

    }
}