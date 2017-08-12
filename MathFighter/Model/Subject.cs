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
using SQLite.Net.Attributes;

namespace MathFighter.Model
{
    public class Subject
    {
        [PrimaryKey]
        public int SubjectID { get; set; }  
        public string SubjectName { get; set; }

        public Subject ()
        { 
            
        }

        public Subject(int subjectId, string subjectName)
        {
            this.SubjectID = subjectId;
            this.SubjectName = subjectName;
        }

    }
}   