namespace WordleCheater.Algorithms;

public class MostAverage : Algorithm
{
    private const int DupesPenalty = 6;

    public override string? Guess(Progress progress)
    {
        var possibleWords = AllPossibleWords(progress);

        if (possibleWords == null || possibleWords.Count == 0)
        {
            //there's no response. It's just an empty list.
            return null;
        }

        // create 2d dictionary
        // index: letter position
        // key: index of letter in alphabet
        // value: commonality of letter
        var letterMatrix = new Dictionary<int, double>[Program.WordSize];

        for (var i = 0; i < Program.WordSize; i++)
        {
            letterMatrix[i] = new Dictionary<int, double>();
        }

        //determine the commonality of letters in each spot in the word
        for (var letterIndex = 0; letterIndex < Program.WordSize; letterIndex++)
        {
            //get the letterIndex-th letter of every word
            var thLetter = "";
            foreach (var possibleWord in possibleWords)
            {
                thLetter += possibleWord[letterIndex];
            }

            //get the commonalities of each letter and write it to the array
            for (var i = 0; i < Program.AlphabetString.Length; i++)
            {
                var count = thLetter.Count(x => x == Program.AlphabetString[i]);
                var com = (double)count / thLetter.Length;
                letterMatrix[letterIndex].Add(i, com);
            }
        }

        //remove entries where letters never occur
        for (var i = 0; i < letterMatrix.Length; i++)
        {
            letterMatrix[i] = letterMatrix[i]
                .Where(kv => kv.Value != 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        //remove spot exceptions (where a letter has appeared yellow)
        for (var i = 0; i < progress.SpotExceptions.Length; i++)
        {
            foreach (var letter in progress.SpotExceptions[i])
            {
                var alphaIndex = Program.AlphabetString.IndexOf(letter);
                letterMatrix[i].Remove(alphaIndex);
            }
        }

        //overwrite any letters that have been solved
        for (var i = 0; i < progress.SolvedPortion.Length; i++)
        {
            if (Program.AlphabetString.Contains(progress.SolvedPortion[i]))
            {
                //this needs to add the alphabet index
                letterMatrix[i].Clear();
                letterMatrix[i].Add(Program.AlphabetString.IndexOf(progress.SolvedPortion[i]), 1);
            }
        }

        //sort letters according to each strategy
        var sortedLists = new List<KeyValuePair<int, double>>[Program.WordSize];

        //MOST AVERAGE LETTERS FIRST
        for (int i = 0; i < Program.WordSize; i++)
        {
            var sortedList = from entry in letterMatrix[i] orderby Math.Abs(entry.Value - (letterMatrix[i].Values.Average())) ascending select entry;
            sortedLists[i] = (sortedList.ToList());
        }

        string? word = null;
        var wordGolfScore = Int32.MaxValue;

        //new method: for every word in small list, find indices in sorted lists, pick word with lowest value (closest to criteria)
        //significantly faster for slower methods, slightly slower for most common (still too fast to really notice casually)
        foreach (var smallWord in possibleWords)
        {
            var currentWordScore = 0;
            for (var i = 0; i < sortedLists.Length; i++)
            {
                foreach (var list in sortedLists)
                {
                    foreach (var kv in list)
                    {
                        if (kv.Key == Program.AlphabetString.IndexOf(smallWord[i]))
                        {
                            currentWordScore += list.IndexOf(kv);
                        }
                    }
                }
            }

            //dupes clause (to avoid dupes first)
            if (HasDuplicates(smallWord) && !progress.ConfirmedDuplicates)
            {
                currentWordScore *= DupesPenalty;
            }

            if (currentWordScore < wordGolfScore)
            {
                word = smallWord;
                wordGolfScore = currentWordScore;
            }
        }

        return word;
    }
}