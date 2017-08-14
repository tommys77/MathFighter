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
        public List<HighscoreViewHelper> mItems;
        private Context mContext;

        public HighscoreListAdapter(Context context, List<HighscoreViewHelper> items)
        {
            mItems = items;
            mContext = context;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override int Count
        {
            get { return mItems.Count; }
            
        }

        public override HighscoreViewHelper this[int position]
        {
            get { return mItems[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.item_highscore, null, false);
            }

            var txtPos = row.FindViewById<TextView>(Resource.Id.tv_position);
            var txtPlayer = row.FindViewById<TextView>(Resource.Id.tv_player);
            var txtScore = row.FindViewById<TextView>(Resource.Id.tv_score);
            var txtPlaytime = row.FindViewById<TextView>(Resource.Id.tv_playtime);
            txtPos.SetText(mItems[position].HighscoreId.ToString(), null); 
            txtPlayer.SetText(mItems[position].Player, null);
            txtScore.SetText(mItems[position].Score.ToString(), null);
            txtPlaytime.SetText(mItems[position].Playtime, null);
            return row;
        }

    }
}