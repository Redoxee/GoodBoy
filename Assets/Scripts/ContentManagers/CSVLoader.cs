using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLoader : Loader
{
    [SerializeField]
    public TextAsset CSVFile = null;
    public int NameColumn = 0;
    public int HashTagColumn = 2;
    public int TriviaColumn = 3;
    public int SpeechColumn = 6;

    public override bool Load()
    {
        ContentDatabase database = new ContentDatabase();
        System.IO.StringReader reader = new System.IO.StringReader(this.CSVFile.text);
        string row = reader.ReadLine();
        string[] array = row.Split('\t');

        row = reader.ReadLine();
        while (row != null) 
        {
            array = row.Split('\t');
            if (!string.IsNullOrEmpty(array[NameColumn]))
            {
                database.Names.Add(array[NameColumn]);
            }

            if (!string.IsNullOrEmpty(array[this.HashTagColumn]) && !string.IsNullOrEmpty(array[this.TriviaColumn]))
            {
                TagTriviaCouple couple = new TagTriviaCouple();
                couple.Tag = array[this.HashTagColumn];
                couple.Trivia = array[this.TriviaColumn];
                database.DesciptionContent.Add(couple);
            }

            if (!string.IsNullOrEmpty(array[this.SpeechColumn]))
            {
                database.DogSpeeches.Add(array[this.SpeechColumn]);
            }

            row = reader.ReadLine();
        }

        database.RadomeProfilPics = this.Pics;

        this.Database = database;

        return true;
    }
}
