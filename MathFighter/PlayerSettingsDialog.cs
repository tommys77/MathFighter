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
using Android.Preferences;
using Android.Graphics;

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

        private ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public PlayerSettingsDialog()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_player_settings, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity.BaseContext);

            SetListeners(view);

            return view;
        }

        private void SetListeners(View view)
        {
            playerName = prefs.GetString("player", null);
            imgPath = prefs.GetString("imgPath", null);
            if (imgPath == null)
            {
                playerImg = BitmapFactory.DecodeResource(Activity.BaseContext.Resources, Resource.Drawable.Adrian);
            }
            else
            {
                try
                {
                    playerImg = BitmapFactory.DecodeFile(imgPath);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity.BaseContext, ex.Message, ToastLength.Long).Show();
                }
            }

            editPlayerName = view.FindViewById<EditText>(Resource.Id.et_player_settings);
            if (editPlayerName != null)
            {
                editPlayerName.Text = playerName;
            }
            ivPlayer = view.FindViewById<ImageView>(Resource.Id.iv_player_settings);
            ivPlayer.Click += IvPlayer_Click;

            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            editor = prefs.Edit();
            editor.PutString("player", null);
            editor.PutString("imgPath", null);
            editor.Apply();
            Dismiss();
        }

        private void IvPlayer_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Activity.BaseContext, "TODO: Change photo!", ToastLength.Long).Show();
        }
    }
}