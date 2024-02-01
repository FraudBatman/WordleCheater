namespace WordleCheater.Algorithms;

public abstract class Algorithm
{
    /// <summary>
    /// All words used by the current ruleset
    /// </summary>
    protected static string[] AllWords = Program._words.ToArray();

    // /// <summary>
    // /// Sets the word list used by the solver
    // /// </summary>
    // /// <param name="words">All words usable by the solver</param>
    // public static void SetDictionary(string[] words)
    // {
    //     AllWords = words;
    // }

    /// <summary>
    /// Gets all words that can possibly be the solution, based on the current progress
    /// </summary>
    /// <param name="prog">The current progress of the Wordle</param>
    /// <returns>A list of all words that could possibly be the solution</returns>
    protected static List<string> AllPossibleWords(Progress prog)
    {
        var returnList = new List<string>(AllWords);

        //remove any words that don't have all confirmed letters
        foreach (var letter in prog.ConfirmedInWord)
        {
            returnList.RemoveAll(s => !s.Contains(letter));
        }
        //remove any words that have any letters that are confirmed not to be there
        foreach (var letter in prog.DeconfirmedLetters)
        {
            returnList.RemoveAll(s => s.Contains(letter));
        }
        //remove any words that have letters where they aren't supposed to be
        for (var i = 0; i < prog.SpotExceptions.Length; i++)
        {
            returnList.RemoveAll(s => prog.SpotExceptions[i].Contains(s[i]));
        }
        //remove any words that don't have letters where they are supposed to be
        for (var i = 0; i < prog.SolvedPortion.Length; i++)
        {
            if (prog.SolvedPortion[i] != ' ')
            {
                returnList.RemoveAll(s => s[i] != prog.SolvedPortion[i]);
            }
        }
        //if a word definitely has duplicates, get rid of words that don't have duplicates
        if (prog.ConfirmedDuplicates)
        {
            returnList.RemoveAll(s => !HasDuplicates(s));
        }

        return returnList;
    }

    /// <summary>
    /// Checks if a word has duplicate letters
    /// </summary>
    protected static bool HasDuplicates(string word)
    {
        for (var i = 0; i < word.Length - 1; i++)
        {
            for (var j = i + 1; j < word.Length; j++)
            {
                if (word[i] == word[j])
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Makes a guess based on the current Wordle progression
    /// </summary>
    /// <param name="progress">The current progress of the Wordle</param>
    /// <returns>A word to be used by the guesser</returns>
    public abstract string? Guess(Progress progress);
}