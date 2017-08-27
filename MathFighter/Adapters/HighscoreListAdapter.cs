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
using MathFighter.Model;
using Android.Graphics;
using Android.Content.Res;
using Java.IO;

namespace MathFighter.Adapters
{
    public class HighscoreListAdapter : BaseAdapter<HighscoreViewModel>
    {
        public List<HighscoreViewModel> highscores;
        private Context mContext;

        public HighscoreListAdapter(Context context, List<HighscoreViewModel> items)
        {
            highscores = items;
            mContext = context;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override int Count
        {
            get { return highscores.Count; }

        }

        public override HighscoreViewModel this[int position]
        {
            get { return highscores[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View highscoreView = convertView;

            if (highscoreView == null)
            {
                highscoreView = LayoutInflater.From(mContext).Inflate(Resource.Layout.item_highscore, null, false);
            }

            var txtPos = highscoreView.FindViewById<TextView>(Resource.Id.tv_position);
            var txtPlayer = highscoreView.FindViewById<TextView>(Resource.Id.tv_player);
            var txtScore = highscoreView.FindViewById<TextView>(Resource.Id.tv_score);
            var txtPlaytime = highscoreView.FindViewById<TextView>(Resource.Id.tv_playtime);
            var ivPlayer = highscoreView.FindViewById<ImageView>(Resource.Id.iv_mugshot);
            var pos = highscores.IndexOf(highscores[position]) + 1;

            int width = ivPlayer.Height;
            int height = mContext.Resources.DisplayMetrics.HeightPixels;

            var image = SetPlayerImage(width, height, highscores[position].ImagePath);

            txtPos.SetText(pos.ToString(), null);
            ivPlayer.SetImageBitmap(image);
            txtPlayer.SetText("Navn: " + highscores[position].Player, null);
            txtScore.SetText(highscores[position].Score.ToString(), null);
            txtPlaytime.Text = "Tid: " + highscores[position].Playtime;

            return highscoreView;
        }

        private Bitmap SetPlayerImage(int width, int height, string path)
        {
            Bitmap playerImg = BitmapFactory.DecodeResource(mContext.Resources, id: Resource.Drawable.Adrian);
            var file = new File(path);
            if (file.Exists())
            {
                playerImg = BitmapFactory.DecodeFile(file.Path);
                
            }
            else
            {
                playerImg = playerImg.PreparePlayerImage(width, height);

            }
            return playerImg;
        }
    }
}