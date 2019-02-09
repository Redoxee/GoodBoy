using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileGenerator
{
    private ContentDatabase database;
    private int nbNames = 0;
    private int nbTrivias = 0;
    private int nbPic = 0;

    private System.Random random = new System.Random();

    const int nbTriviaPerProfile = 3;

    public ProfileGenerator(ContentDatabase database)
    {
        this.database = database;
        this.nbNames = this.database.Names.Count;
        this.nbTrivias = this.database.DesciptionContent.Count;
        this.nbPic = this.database.RadomeProfilPics.Count;
    }

    public Profile GenerateProfile()
    {
        Profile profile = new Profile();

        int nameIndex = this.random.Next(this.nbNames);
        profile.Name = this.database.Names[nameIndex];
        int picIndex = this.random.Next(this.nbPic);
        profile.Pict = this.database.RadomeProfilPics[picIndex];

        List<int> trivias = new List<int>();
        for (int i = 0; i < nbTriviaPerProfile; ++i)
        {
            int triviaIndex;
            int safeCount = 0;
            do
            {
                safeCount++;
                triviaIndex = this.random.Next(this.nbTrivias);
            } while (safeCount < 1000 && trivias.Contains(triviaIndex));

            trivias.Add(triviaIndex);
        }

        profile.Tags = new string[nbTriviaPerProfile];
        profile.Trivias = new string[nbTriviaPerProfile];

        for (int index = 0; index < nbTriviaPerProfile; ++index)
        {
            TagTriviaCouple couple = this.database.DesciptionContent[trivias[index]];
            profile.Tags[index] = couple.Tag;
            profile.Trivias[index] = couple.Trivia;
        }
        
        return profile;
    }
}
