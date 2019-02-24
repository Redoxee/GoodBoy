using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLoader : Loader
{
    [SerializeField]
    public TextAsset CSVFile = null;

    public enum Columns
    {
        Names,
        Nothing,
        HashTags,
        Trivias,
        ProceduralWeight,
        Nothing_,
        LegendNames,
        LegendHashTags,
        LegendStories,
        LegendTextures,
        LegendWeight,
        LegendMute,
        Nothing__,
        DogSpeech,
    }

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
            if (!string.IsNullOrEmpty(array[(int)CSVLoader.Columns.Names]))
            {
                database.Names.Add(array[(int)CSVLoader.Columns.Names]);
            }

            if (!string.IsNullOrEmpty(array[(int)CSVLoader.Columns.HashTags]) && !string.IsNullOrEmpty(array[(int)CSVLoader.Columns.Trivias]))
            {
                TagTriviaCouple couple = new TagTriviaCouple();
                couple.Tag = array[(int)CSVLoader.Columns.HashTags];
                couple.Trivia = array[(int)CSVLoader.Columns.Trivias];
                database.DesciptionContent.Add(couple);
            }

            if (!string.IsNullOrEmpty(array[(int)CSVLoader.Columns.DogSpeech]))
            {
                database.DogSpeeches.Add(array[(int)CSVLoader.Columns.DogSpeech]);
            }

            row = reader.ReadLine();
        }

        database.RadomeProfilPics = this.Pics;

        this.Database = database;

        return true;
    }
}
