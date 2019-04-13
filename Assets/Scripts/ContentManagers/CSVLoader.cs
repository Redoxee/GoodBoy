using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

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
        Nothing___,
        SpecialSpeech,
    }

    public override bool Load()
    {
        ContentDatabase database = new ContentDatabase();
        System.IO.StringReader reader = new System.IO.StringReader(this.CSVFile.text);
        string row = reader.ReadLine();
        string[] splitted = row.Split('\t');

        Regex regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
        row = reader.ReadLine();
        while (row != null) 
        {
            MatchCollection columns = regex.Matches(row);
            
            splitted = columns.Cast<Match>().Select(m => m.Value.Replace("\"", "")).ToArray();
            if (!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.Names]))
            {
                database.Names.Add(splitted[(int)CSVLoader.Columns.Names]);
            }

            if (!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.HashTags]) && !string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.Trivias]))
            {
                TagTriviaCouple couple = new TagTriviaCouple();
                couple.Tag = splitted[(int)CSVLoader.Columns.HashTags];
                couple.Trivia = splitted[(int)CSVLoader.Columns.Trivias];
                database.DesciptionContent.Add(couple);
            }

            if (!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.DogSpeech]))
            {
                database.DogSpeeches.Add(splitted[(int)CSVLoader.Columns.DogSpeech]);
            }

            if (!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.LegendNames]))
            {
                Profile profile = new Profile();
                profile.Name = splitted[(int)CSVLoader.Columns.LegendNames];
                profile.isMute = !string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.LegendMute]);
                string hashtag = splitted[(int)CSVLoader.Columns.LegendHashTags];
                string stories = splitted[(int)CSVLoader.Columns.LegendStories];
                string textureName = splitted[(int)CSVLoader.Columns.LegendTextures];

                stories = stories.Replace("\\n", "\n");

                profile.Tags = new string[] { hashtag };
                profile.Trivias = new string[] { stories };

                string stringWeight = splitted[(int)CSVLoader.Columns.LegendWeight];
                int weight = 0;
                int.TryParse(stringWeight, out weight);
                Sprite texture = this.Pics.Find((Sprite sprite) => sprite.name == textureName);
                if(texture == null)
                {
                    Debug.LogError("LegendProfil texture not found");
                }
                this.Pics.Remove(texture);
                profile.Pict = texture;

                KeyValuePair<int, Profile> pair = new KeyValuePair<int, Profile>(weight, profile);

                database.LegendaryProfiles.Add(pair);
            }

            if (!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.ProceduralWeight]))
            {
                string stringWeight = splitted[(int)CSVLoader.Columns.ProceduralWeight];
                int weight = int.Parse(stringWeight);

                database.ProceduralWeight = weight;
            }

            if(!string.IsNullOrEmpty(splitted[(int)CSVLoader.Columns.SpecialSpeech]))
            {
                string speechTrigger = splitted[(int)CSVLoader.Columns.SpecialSpeech].ToLower();
                List<List<string>> speeches;
                if (database.SpecialSpeeches.ContainsKey(speechTrigger))
                {
                    speeches = database.SpecialSpeeches[speechTrigger];
                }
                else
                {
                    speeches = new List<List<string>>();
                    database.SpecialSpeeches[speechTrigger] = speeches;
                }

                List<string> speech = new List<string>();

                int replyIndex = (int)CSVLoader.Columns.SpecialSpeech + 1;
                while (replyIndex < splitted.Length && !string.IsNullOrEmpty(splitted[replyIndex]))
                {
                    speech.Add(splitted[replyIndex]);
                    replyIndex++;
                }

                speeches.Add(speech);
            }

            row = reader.ReadLine();
        }
        

        database.LegendaryProfiles.Sort((KeyValuePair<int, Profile> a, KeyValuePair<int, Profile> b) => a.Key - b.Key);

        database.RadomeProfilPics = this.Pics;

        this.Database = database;

        return true;
    }
}
