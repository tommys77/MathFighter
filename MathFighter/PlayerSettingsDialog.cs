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
using Android.Provider;

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
            Bitmap bitmap = (Bitmap)data.Extras.Get("data");
        }



        private void IvPlayer_Click(object sender, EventArgs e)
        {
            Toast.MakeText(context, "TODO: Change photo!", ToastLength.Long).Show();
            Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(cameraIntent, 0);
        }
    }
}