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

namespace MathFighter.Adapters
{
    public class HighscoreListAdapter : BaseAdapter<HighscoreViewHelper>
    {
        public List<HighscoreViewHelper> highscores;
        private Context mContext;

        public HighscoreListAdapter(Context context, List<HighscoreViewHelper> items)
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

        public override HighscoreViewHelper this[int position]
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
            var pos = highscores.IndexOf(highscores[position]) + 1;
            txtPos.SetText(pos.ToString(), null); 
            txtPlayer.SetText(highscores[position].Player, null);
            txtScore.SetText(highscores[position].Score.ToString(), null);
            txtPlaytime.SetText(highscores[position].Playtime, null);

            return highscoreView;
        }

    }
}