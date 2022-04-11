namespace WordleCheater;

public record Progress
{
    public string ConfirmedInWord { get; private set; }
    public string DeconfirmedLetters { get; private set; }
    public string SolvedPortion { get; private set; }
    public string[] SpotExceptions { get; set; }
    public bool ConfirmedDuplicates { get; private set; }

    public Progress(int wordSize = 5)
    {
        ConfirmedInWord = "";
        DeconfirmedLetters = "";
        SolvedPortion = "";
        for (var i = 0; i < wordSize; i++)
        {
            SolvedPortion += " ";
        }
        SpotExceptions = new string[wordSize];
        for (var i = 0; i < wordSize; i++)
        {
            SpotExceptions[i] = "";
        }

        ConfirmedDuplicates = false;
    }

    public void ParseProgress(string guess, string result)
    {
        if (guess == null) throw new ArgumentNullException(nameof(guess));
        for (var i = 0; i < result.Length; i++)
        {
            //either the letter is not in the word, or it is, but all instances are accounted for
            if (result[i] == 'X')
            {
                    //just add it to the SpotExceptions
                    SpotExceptions[i] += guess[i];
                //check if the letter has already been confirmed in the word
                if (!ConfirmedInWord.Contains(guess[i]))
                {
                    //wait until the last instance of the letter before making the call
                    if (i == result.Length - 1 || !guess.Substring(i + 1).Contains(guess[i]))
                    {
                        DeconfirmedLetters += guess[i]; 
                    }
                }
            }

            if (result[i] == 'Y')
            {
                SpotExceptions[i] += guess[i];
                if (!ConfirmedInWord.Contains(guess[i]))
                {
                    ConfirmedInWord += guess[i];
                }
            }

            if (result[i] == 'G')
            {
                var solvedCopy = (string)SolvedPortion.Clone();
                SolvedPortion = "";
                for (var j = 0; j < solvedCopy.Length; j++)
                {
                    if (j == i)
                    {
                        SolvedPortion += guess[j];
                    }
                    else
                    {
                        SolvedPortion += solvedCopy[j];
                    }
                }
                
                if (!ConfirmedInWord.Contains(guess[i]))
                {
                    ConfirmedInWord += guess[i];
                }
            }
            
            //rules for confirming duplicates
            if (!ConfirmedDuplicates)
            {
                for (var j = i + 1; j < guess.Length; j++)
                {
                    //check if the guess has any duplicates
                    if (guess[i] == guess[j])
                    {
                        //two instances of a letter are either Y or G
                        if ("YG".Contains(result[i]) && "YG".Contains(result[j]))
                        {
                            ConfirmedDuplicates = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}