using System.Collections.Generic;
using UnityEngine;

public class ContentDatabase 
{
    public List<string> Names = new List<string>();
    public List<TagTriviaCouple> DesciptionContent = new List<TagTriviaCouple>();
    public List<string> DogSpeeches = new List<string>();
    public Dictionary<string, List<List<string>>> SpecialSpeeches = new Dictionary<string, List<List<string>>>();
    public List<Sprite> RadomeProfilPics = new List<Sprite>();

    public int ProceduralWeight = 10;

    public List<KeyValuePair<int, Profile>> LegendaryProfiles = new List<KeyValuePair<int, Profile>>();
}

public struct TagTriviaCouple
{
    public string Tag;
    public string Trivia;
}
