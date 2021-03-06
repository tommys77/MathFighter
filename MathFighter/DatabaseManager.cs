﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathFighter.Model;
using SQLite;

namespace MathFighter
{
    public class DatabaseManager
    {
        private const string CreateTableHighscore = "CREATE TABLE IF NOT EXISTS Highscore (" +
                                                      "HighscoreId INTEGER NOT NULL," +
                                                      "Player TEXT NOT NULL," +
                                                      "ImagePath TEXT," +  
                                                      "Score INTEGER NOT NULL," +
                                                      "Playtime INTEGER NOT NULL," +
                                                      "SubjectId INTEGER NOT NULL," +
                                                      "DifficultyId INTEGER NOT NULL," +
                                                      "PRIMARY KEY (HighscoreId, SubjectId, DifficultyId)," +
                                                      "FOREIGN KEY (SubjectId) REFERENCES Subject(SubjectId)" +
                                                      ");";

        private const string CreateTableSubject = "CREATE TABLE IF NOT EXISTS Subject (" +
                                                    "SubjectId INTEGER NOT NULL," +
                                                    "SubjectName TEXT NOT NULL," +
                                                    "PRIMARY KEY (SubjectId)" +
                                                    ");";



        private string dbPath;
        private SQLiteConnection db;

        public DatabaseManager(string path)
        {
            this.dbPath = path;
            db = new SQLiteConnection(dbPath);
        }

        //Method to create table if not already exists.
        public void CreateTable()
        {
            db.Execute(CreateTableSubject);
            db.Execute(CreateTableHighscore);
            AddSubjects();
            AddDefaultHighscores();
        }

        //For better presentation, add some default highscores.
        private  void AddDefaultHighscores()
        {
            for (var i = 1; i <= 10; i++)
            {
                if (db.Find<Highscore>(i) == null)
                {
                    for (var j = 1; j <= 3; j++)
                    {
                        db.Insert(new Highscore(i, "Unregistered", "", 0, 0, 1, j));
                        db.Insert(new Highscore(i, "Unregistered", "", 0, 0, 2, j));
                        db.Insert(new Highscore(i, "Unregistered", "", 0, 0, 3, j));
                    }
                }
            }
        }

        //To easier add more subjects in the future
        private void AddSubjects()
        {
            var multi = new Subject(1, "Gangetabllen");
            var mixed = new Subject(2, "Lett blanding");
            var sqrt = new Subject(3, "Kvadratrot");
            var subjectList = new List<Subject> { multi, mixed, sqrt };
            foreach (var s in subjectList)
            {
                if (db.Find<Subject>(s.SubjectID) == null)
                {
                    db.Insert(s);
                }
            }
        }

        public List<HighscoreViewModel> GetHighscores(int subjectId, int difficultyId)
        {
            var highscoresTable = db.Table<Highscore>()
                .OrderByDescending(s => s.Score)
                .Where(h => h.SubjectId == subjectId && h.DifficultyId == difficultyId);

            var subjectName = db.Table<Subject>()
                .FirstOrDefault(s => s.SubjectID == subjectId)
                .SubjectName;

            var highscoresViewList = new List<HighscoreViewModel>();

            foreach (var item in highscoresTable)
            {
                if (item.SubjectId != subjectId) continue;
                var playtime = TimeSpan.FromMilliseconds(item.Playtime).Minutes + "m" +
                               TimeSpan.FromMilliseconds(item.Playtime).Seconds + "s";
                highscoresViewList.Add( new HighscoreViewModel( item.HighscoreId, item.Player, item.ImagePath, item.Score, playtime));
            }
            return highscoresViewList;
        }
       
        public void InsertHighscore(Highscore highscore)
        {
            db.InsertOrReplace(highscore);
        }

    }
}