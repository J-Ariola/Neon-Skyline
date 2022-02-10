using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedGenerator : MonoBehaviour
{
    public TextAsset adjectiveFile, nounFile;
    string[] adjectives, nouns;
    public string seed;
    System.Random gen = new System.Random();

    [Range(1, 5)]
    public int wordCount;

    //public Text currentSeed;

    void Start()
    {
        if (adjectiveFile != null)
        {
            adjectives = (adjectiveFile.text.Split(' '));
        }
        else
        {
            adjectives[0] = "MissingAdjectiveTextFile";
        }
        if (nounFile != null)
        {
            nouns = (nounFile.text.Split('\n'));
        }
        else
        {
            nouns[0] = "MissingNounTextFile";
        }

        GenerateSeed();
    }

    void Update()
    {
        //currentSeed.text = seed;
    }

    public void GenerateSeed()
    {
        seed = "null";
        for (int i = 0; i < wordCount; i++)
        {
            string word;
            if (seed == "null" && wordCount >= 2)
            {
                word = adjectives[gen.Next(0, adjectives.Length)];
                seed = word;
            }
            else if (seed == "null" && wordCount == 1)
            {
                word = nouns[gen.Next(0, nouns.Length)];
                seed = word;
            }
            else if (i < wordCount - 1)
            {
                word = adjectives[gen.Next(0, adjectives.Length)];
                seed = seed + " " + word;
            }
            else
            {
                word = nouns[gen.Next(0, nouns.Length)];
                seed = seed + " " + word;
            }
        }
    }

    //debugging version in case null errors occur
    public string GenSeedFromScratch()
    {
        if (adjectiveFile != null)
        {
            adjectives = (adjectiveFile.text.Split(' '));
        }
        else
        {
            adjectives[0] = "MissingAdjectiveTextFile";
        }
        if (nounFile != null)
        {
            nouns = (nounFile.text.Split('\n'));
        }
        else
        {
            nouns[0] = "MissingNounTextFile";
        }

        seed = "null";
        for (int i = 0; i < wordCount; i++)
        {
            string word;
            if (seed == "null" && wordCount >= 2)
            {
                word = adjectives[gen.Next(0, adjectives.Length)];
                seed = word;
            }
            else if (seed == "null" && wordCount == 1)
            {
                word = nouns[gen.Next(0, nouns.Length)];
                seed = word;
            }
            else if (i < wordCount - 1)
            {
                word = adjectives[gen.Next(0, adjectives.Length)];
                seed = seed + " " + word;
            }
            else
            {
                word = nouns[gen.Next(0, nouns.Length)];
                seed = seed + " " + word;
            }
        }
        return seed;
    }
}
