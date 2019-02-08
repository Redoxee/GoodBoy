using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public TextAsset[] FileToLoad = null;
    
    public ContentDatabase Database;
    [System.NonSerialized]
    public bool isLoaded = false;

    public System.Collections.Generic.List<Sprite> Pics = null;

    public bool Load()
    {
        this.Database = new ContentDatabase();

        foreach (TextAsset asset in this.FileToLoad)
        {
            if (!this.LoadAsset(asset))
            {
                return false;
            }
        }

        this.Database.RadomeProfilPics = this.Pics;

        this.isLoaded = true;
        return true;
    }

    bool LoadAsset(TextAsset asset)
    {
        System.IO.StringReader reader = new System.IO.StringReader(asset.ToString());
        string currentLine = reader.ReadLine();
        if (currentLine == null)
        {
            return true;
        }
        if (currentLine.Trim() == "Names")
        {
            return this.LoadNames(reader);
        }
        else if (currentLine.Trim() == "Trivias")
        {
            return this.LoadTrivias(reader);
        }

        return true;
    }

    bool LoadNames(System.IO.StringReader reader)
    {
        string currentLine = reader.ReadLine();
        while (currentLine != null)
        {
            string name = currentLine.Trim();
            if (!this.Database.Names.Contains(name))
            {
                this.Database.Names.Add(name);
            }

            currentLine = reader.ReadLine();
        }

        return true;
    }

    bool LoadTrivias(System.IO.StringReader reader)
    {
        string currentLine = reader.ReadLine();
        while (currentLine != null)
        {
            while (currentLine.Length == 0)
            {
                currentLine = reader.ReadLine();
                if (currentLine == null)
                {
                    return true;
                }
                if (currentLine[0] != '#')
                {
                    return true;
                }
            }
            TagTriviaCouple couple = new TagTriviaCouple();
            couple.Tag = currentLine.Trim();
            couple.Trivia = string.Empty;
            do
            {
                currentLine = reader.ReadLine();
                if (currentLine == null)
                {
                    return true;
                }
            } while (currentLine.Length == 0);
            
            do
            {
                couple.Trivia += currentLine.Trim();
                currentLine = reader.ReadLine();
            } while (currentLine != null && !(currentLine.Length > 0 && currentLine[0] == '#'));

            this.Database.DesciptionContent.Add(couple);
        }
        return true;
    }
}
