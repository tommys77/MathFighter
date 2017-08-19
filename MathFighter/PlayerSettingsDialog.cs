﻿using System;
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

namespace MathFighter
{
    public class PlayerSettingsDialog : DialogFragment
    {
        private string playerName;
        private string imgPath;

        private EditText editPlayerName;
        private ImageView ivPlayer;
        private Button btnSave;
        private Bitmap playerImg;

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

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }
            SetListeners(view);

            return view;
        }

        private void SetListeners(View view)
        {
            playerName = prefs.GetString("player", null);
            imgPath = prefs.GetString("imgPath", null);
            if (imgPath == null)
            {
                playerImg = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Adrian);
            }
            else
            {
                try
                {
                    playerImg = BitmapFactory.DecodeFile(imgPath);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                }
            }

            editPlayerName = view.FindViewById<EditText>(Resource.Id.et_player_settings);
            if (editPlayerName != null)
            {
                editPlayerName.Text = playerName;
            }
            ivPlayer = view.FindViewById<ImageView>(Resource.Id.iv_player_settings);
            ivPlayer.Click += IvPlayer_Click;
            btnSave = view.FindViewById<Button>(Resource.Id.btn_player_settings_save);
            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            editor = prefs.Edit();
            editor.PutString("player", editPlayerName.Text);
            editor.PutString("imgPath", null);
            editor.Apply();
            Dismiss();
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //Bitmap bitmap = (Bitmap)data.Extras.Get("data");

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile, FileProvider.GetUriForFile(context, AUTHORITY, App._file));
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            Activity.SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = ivPlayer.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                ivPlayer.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();

        }

        private void IvPlayer_Click(object sender, EventArgs e)
        {
            //Toast.MakeText(context, "TODO: Change photo!", ToastLength.Long).Show();
            App._file = new File(App._dir, String.Format("playerPhoto_{0}.jpg", Guid.NewGuid()));
            Intent intent = new Intent(MediaStore.ActionImageCapture, FileProvider.GetUriForFile(context, AUTHORITY, App._file));
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(context, AUTHORITY, App._file));
            try
            {
                Activity.StartActivityForResult(intent, 0);
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
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            
            return availableActivities != null && availableActivities.Count > 0;
        }

    }
}