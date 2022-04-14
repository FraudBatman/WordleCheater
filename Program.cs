//results from official set with all words (starting with CRATE)
//MostCommon (2.1):     3.78661 (18 fails)
//MC (1.0):             3.77667 (19)
//MostAverage(2.1):     3.95032 (35)
//MC (2.0):             3.89978 (40)
//MA (1.0):             3.99309 (48)
//LeastCommon (1.0):    4.04363 (70)
//LC (2.1):             4.11792 (73)
//MA (2.0):             4.07948 (74)
//LC (2.0):             4.33391 (149)

//results from full set with all words (starting with CRATE)
//MostCommon (2.1):     4.59644 (753 fails)
//MostAverage (2.1):    4.75442 (887)
//MC (1.0):             4.72768 (960)
//MA (1.0):             4.85892 (1087)
//MC (2.0):             4.85567 (1156)
//MA (2.0):             5.09969 (1509)
//LeastCommon (2.1):    5.14444 (1590)
//LC (2.0):             5.48009 (2313)     
//LC (1.0):             DNF. It was *just* that slow

//results from official set with all words (Engine chooses its starting word)
//MostCommon (2.2, Opening Word: ALERT):    3.87171 (38 fails) 
//MostAverage (2.2, THUMP):                 4.03672 (15)
//LeastCommon (2.2, JUMPY):                 4.55248 (85)

//results from full set with all words (Engine chooses its starting word)
//MostCommon (2.2, Opening Word: ASTER):    4.70340 (955 fails)
//MostAverage (2.2, MULCH):                 4.81293 (729)
//LeastCommon (2.2, JUMPY):                 5.34527 (1601)

namespace WordleCheater;

internal enum MenuOption
{
    Exit = 0,
    OfficialWordle = 1,
    UnofficialWordle = 2,
    Dordle = 3,
    Quordle = 4,
    Octordle = 5,
    Sedordle = 6,
    CheatTestOfficial = 7,
    CheatTestUnofficial = 8
}

public static class Program
{
    private const string AlphabetString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int DupesPenalty = 6;
    private const string FiveLettWordList = "5LetterWords.txt";
    private const string BestFirstGuess = "CRATE";
    private const string OfficialWordList = "OfficialList.txt";
    private const int WordSize = 5;

    private static List<string> _words = new List<string>();
    private static readonly Random Random = new Random();

    private static string? FirstGuess; //used for batch testing

    public static void Main()
    {
        // //Single word testing
        // Words = GetWordList(FiveLettWordList);
        // CheatTestLoop("SOLVE");

        // // Batch list testing
        // CheatTest(OfficialWordList);
        // Console.ReadLine();

        do
        {
            var option = MainMenu();
            if (option == MenuOption.Exit)
            {
                break;
            }
            else if (option == MenuOption.OfficialWordle)
            {
                _words = GetWordList(OfficialWordList);
            }
            else
            {
                _words = GetWordList(FiveLettWordList);
            }

            switch (option)
            {
                case MenuOption.OfficialWordle:
                case MenuOption.UnofficialWordle:
                    Cheater(1, 6);
                    break;
                case MenuOption.CheatTestOfficial:
                    CheatTest(OfficialWordList);
                    Console.ReadLine();
                    break;
                case MenuOption.CheatTestUnofficial:
                    CheatTest(FiveLettWordList);
                    Console.ReadLine();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Nope this isn't a thing yet");
                    Console.WriteLine("Press ENTER to continue...");
                    Console.ReadLine();
                    break;
            }
        } while (true);

        Console.Clear();
    }

    private static MenuOption MainMenu()
    {
        int mint;
        do
        {
            Console.Clear();
            Console.WriteLine("0)\tExit");
            Console.WriteLine("1)\tOfficial Wordle");
            Console.WriteLine("2)\tUnofficial Wordle\t(1 word - 6 guesses)");
            Console.WriteLine("3)\tDordle\t\t\t(2 - 7)");
            Console.WriteLine("4)\tQuordle\t\t\t(4 - 9)");
            Console.WriteLine("5)\tOctordle\t\t(8 - 13)");
            Console.WriteLine("6)\tSedordle\t\t(16 - 21)");
            Console.WriteLine("7)\tTest Cheater\t\tOfficial Set");
            Console.WriteLine("8)\tTest Cheater\t\tUnofficial Set");
            Console.Write("\nMake a selection: ");
            Int32.TryParse(Console.ReadLine(), out mint);

        } while (mint <= -1 || mint > (int)MenuOption.CheatTestUnofficial);
        return (MenuOption)mint;
    }

    private static void Cheater(int wordleCount, int maxGuesses)
    {
        Console.Clear();
        var progresses = new Progress[wordleCount];
        var results = new List<string>();
        var guesses = new List<string?>();

        for (var i = 0; i < wordleCount; i++)
        {
            progresses[i] = new Progress();
        }

        for (var guessNum = 1; guessNum <= maxGuesses; guessNum++)
        {
            var guess = guessNum == 1 ? BestFirstGuess : GuessWord(progresses);

            // var guess = GuessWord(progresses);

            guesses.Add(guess);

            if (guess == null)
            {
                Console.WriteLine("The cheater has given up. Good luck.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Guess #{guessNum}:\t{guess}");
            Console.Write("Result:\t\t");

            string? result;
            var cleanResult = true;
            do
            {
                result = Console.ReadLine()?.ToUpperInvariant().Trim().Substring(0, WordSize);

                if (result == null) continue;

                foreach (var character in result)
                {
                    if (!"XYG".Contains(character))
                    {
                        cleanResult = false;
                        break;
                    }
                }
            } while (result == null || !cleanResult || result.Length != WordSize);

            results.Add(result);
            if (!result.Contains("X") && !result.Contains("Y"))
            {
                //WINNER IS YOU
                Console.Clear();
                Console.WriteLine("epiku\n");

                for (var i = 0; i < guesses.Count; i++)
                {
                    Console.WriteLine($"Guess #{i + 1}:\t{guesses[i]}");
                    Console.WriteLine($"Result:\t\t{results[i]}\n");
                }
                Console.Write("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
            progresses[0].ParseProgress(guess, result);
            Console.WriteLine();
        }
        Console.WriteLine("Oof!");
        Console.ReadLine();
    }

    private static void CheatTest(string wordSet, int runs = -1)
    {
        _words = GetWordList(wordSet);
        Console.WriteLine("Generating opening set...");

        var dumProg = new Progress[1];
        dumProg[0] = new Progress();

        FirstGuess = GuessWord(dumProg);

        if (FirstGuess == null)
        {
            throw new NullReferenceException("Failed first guess for CheatTest");
        }

        var openingSet = new Dictionary<string, string?>();

        //this is built for 5 letter words i'm doing it in a terrible manner
        var resultOptions = "XYG";

        foreach (var t in resultOptions)
        {
            foreach (var t1 in resultOptions)
            {
                //oh no
                foreach (var t2 in resultOptions)
                {
                    //oh god
                    foreach (var t3 in resultOptions)
                    {
                        //im gonna be sick
                        foreach (var t4 in resultOptions)
                        {
                            var testResult = t.ToString()
                                             + t1.ToString()
                                             + t2.ToString()
                                             + t3.ToString()
                                             + t4.ToString();
                            Console.WriteLine(testResult);
                            var proggers = new Progress[1];
                            proggers[0] = new Progress();
                            proggers[0].ParseProgress(FirstGuess, testResult);
                            openingSet.Add(testResult, GuessWord(proggers));
                        }
                    }
                }
            }
        }

        //well that was bad.
        //lets move on and pretend that didn't happen
        string[] testWords;
        if (runs == -1)
        {
            //we're doing every word
            testWords = new string[_words.Count];
            for (var i = 0; i < testWords.Length; i++)
            {
                testWords[i] = _words[i];
            }
        }
        else
        {
            //we're doing a random sample of words
            testWords = new string[runs];
            for (var i = 0; i < testWords.Length; i++)
            {
                testWords[i] = GetRandomWord();
            }
        }
        var guesses = new int[testWords.Length];


        var fails = 0;

        for (var i = 0; i < testWords.Length; i++)
        {
            Console.Clear();
            Console.WriteLine(i);
            guesses[i] = CheatTestLoop(testWords[i], openingSet);
            if (guesses[i] > 6)
            {
                fails++;
            }
        }
        Console.WriteLine($"Average guesses over {testWords.Length} games: {guesses.Average()}");
        Console.WriteLine($"Cheater lost {fails} time(s)");
        Console.WriteLine($"First word: {FirstGuess}");
    }

    private static int CheatTestLoop(string targetWord, Dictionary<string, string?> openingSet)
    {
        var progress = new Progress[1];
        progress[0] = new Progress();
        Console.WriteLine($"Target Word: {targetWord}");

        var guess = FirstGuess;

        Console.WriteLine($"Guess: {guess}");

        var guesses = 1;
        while (guess != targetWord)
        {
            var result = GetGuessResult(guess, targetWord);
            progress[0].ParseProgress(guess, result);

            //dictionary skip
            guess = guesses == 1 ? openingSet[result] : GuessWord(progress);


            if (guess != null)
            {
                Console.WriteLine($"Guess: {guess}");
            }
            else
            {
                throw new Exception($"Word {targetWord} caused the guesser to fail.");
            }

            guesses++;
        }
        Console.WriteLine($"Guessed in {guesses} guesses");
        if (guesses > 6)
        {
            Console.WriteLine("FAILURE");
        }

        Console.WriteLine();
        return guesses;
    }

    private static string GetGuessResult(string guess, string targetWord)
    {
        var returnString = "";
        for (var i = 0; i < guess.Length; i++)
        {
            //correct letter in right spot
            if (guess[i] == targetWord[i])
            {
                returnString += "G";
            }
            else if (targetWord.Contains(guess[i]))
            {
                //this is where it gets complicated
                //find all indexes where the letter is
                //if all instances of the letter are G, or there are enough Ys for the letter then this particular guess is an X
                //if there is at least one instance of the letter that's not G, then this guess is a Y
                var itsY = false;
                var targetYCount = 0;
                var guessYCount = 0;
                var yIndex = 0;
                for (var j = 0; j < targetWord.Length; j++)
                {
                    //find the total times the letter appears in the target word
                    //G's are ignored
                    if (targetWord[j] == guess[i] && targetWord[j] != guess[j])
                    {
                        targetYCount++;
                    }

                    //find the total times the letter appears in the same guess
                    //G's are ignored
                    if (guess[j] != guess[i] || targetWord[j] == guess[j]) continue;
                    guessYCount++;
                    //if this is the letter we're at, record which number of the letter it appears for later calculation
                    if (i == j)
                    {
                        yIndex = guessYCount;
                    }
                }

                if (yIndex <= targetYCount)
                {
                    itsY = true;
                }

                // if (targetWord[j] == guess[i] && targetWord[j] != guess[j])
                // {
                //     itsY = true;
                //     break;
                // }

                returnString += (itsY ? "Y" : "X");
            }
            else
            {
                returnString += "X";
            }
        }
        Console.WriteLine($"Result: {returnString}");
        return returnString;
    }

    private static string? GuessWord(Progress[] progress)
    {
        Console.Write("Guessing word.");

        var unsolvedWords = new List<int>();
        var smallWordLists = new List<List<string>>();
        for (int i = 0; i < progress.Length; i++)
        {
            if (!progress[i].Solved)
            {
                unsolvedWords.Add(i);
                smallWordLists.Add(ShrinkWordsLists(progress[i]));
            }
        }

        if (smallWordLists.Count == 0)
        {
            //the puzzle is solved?
            //what are you doing here?
            return null;
        }

        // create 2d dictionary
        // index: letter position
        // key: index of letter in alphabet
        // value: commonality of letter
        var letterMatrix = new Dictionary<int, double>[WordSize];

        for (var i = 0; i < WordSize; i++)
        {
            letterMatrix[i] = new Dictionary<int, double>();
        }

        //determine the commonality of letters in each spot in the word
        Console.Write(".");
        for (var letterIndex = 0; letterIndex < WordSize; letterIndex++)
        {
            //get the letterIndex-th letter of every word
            var thLetter = "";
            foreach (var smallWord in smallWordLists[0])
            {
                thLetter += smallWord[letterIndex];
            }

            //get the commonalities of each letter and write it to the array
            for (var i = 0; i < AlphabetString.Length; i++)
            {
                var count = thLetter.Count(x => (x == AlphabetString[i]));
                var com = (double)count / thLetter.Length;
                letterMatrix[letterIndex].Add(i, com);
            }
        }

        //remove entries where letters never occur
        Console.Write(".");
        for (var i = 0; i < letterMatrix.Length; i++)
        {
            letterMatrix[i] = letterMatrix[i]
                .Where(kv => kv.Value != 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        //remove spot exceptions (where a letter has appeared yellow)
        Console.Write(".");
        for (var i = 0; i < progress[0].SpotExceptions.Length; i++)
        {
            foreach (var letter in progress[0].SpotExceptions[i])
            {
                var alphaIndex = AlphabetString.IndexOf(letter);
                letterMatrix[i].Remove(alphaIndex);
            }
        }

        //overwrite any letters that have been solved
        Console.Write(".");
        for (var i = 0; i < progress[0].SolvedPortion.Length; i++)
        {
            if (AlphabetString.Contains(progress[0].SolvedPortion[i]))
            {
                //this needs to add the alphabet index
                letterMatrix[i].Clear();
                letterMatrix[i].Add(AlphabetString.IndexOf(progress[0].SolvedPortion[i]), 1);
            }
        }

        Console.Write(".");

        //sort letters according to each strategy
        var sortedLists = new List<KeyValuePair<int, double>>[WordSize];

        // //MOST AVERAGE LETTERS FIRST
        // for (int i = 0; i < WordSize; i++)
        // {
        //     var sortedList = from entry in letterMatrix[i] orderby Math.Abs(entry.Value - (letterMatrix[i].Values.Average())) ascending select entry;
        //     sortedLists[i] = (sortedList.ToList());
        // }

        //MOST COMMON LETTERS FIRST
        for (var i = 0; i < WordSize; i++)
        {
            var sortedList = from entry in letterMatrix[i] orderby entry.Value descending select entry;
            sortedLists[i] = (sortedList.ToList());
        }

        // //LEAST COMMON LETTERS FIRST
        // for (int i = 0; i < WordSize; i++)
        // {
        //     var sortedList = from entry in letterMatrix[i] orderby entry.Value ascending select entry;
        //     sortedLists[i] = (sortedList.ToList());
        // }

        string? word = null;
        var wordGolfScore = Int32.MaxValue;

        //new method: for every word in small list, find indices in sorted lists, pick word with lowest value (closest to criteria)
        //significantly faster for slower methods, slightly slower for most common (still too fast to really notice casually)
        foreach (var smallWord in smallWordLists[0])
        {
            var currentWordScore = 0;
            for (var i = 0; i < sortedLists.Length; i++)
            {
                foreach (var list in sortedLists)
                {
                    foreach (var kv in list)
                    {
                        if (kv.Key == AlphabetString.IndexOf(smallWord[i]))
                        {
                            currentWordScore += list.IndexOf(kv);
                        }
                    }
                }
            }

            //dupes clause (to avoid dupes first)
            if (HasDuplicates(smallWord) && !progress[0].ConfirmedDuplicates)
            {
                currentWordScore *= DupesPenalty;
            }

            if (currentWordScore < wordGolfScore)
            {
                word = smallWord;
                wordGolfScore = currentWordScore;
            }

        }
        Console.WriteLine("!");
        return word;
    }

    private static string GetRandomWord()
    {
        return _words[Random.Next(_words.Count())];
    }

    private static List<string> GetWordList(string fileName)
    {
        var inFs = new FileStream(fileName, FileMode.Open);
        var sr = new StreamReader(inFs);

        var returnList = new List<string>();

        Console.WriteLine("Reading word list...");

        //get the list of words to find data from
        while (!sr.EndOfStream)
        {
            var fileWord = sr.ReadLine();
            if (fileWord is { Length: WordSize })
            {
                returnList.Add(fileWord);
            }
        }
        sr.Close();

        return returnList;
    }

    private static bool HasDuplicates(string word)
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

    private static List<string> ShrinkWordsLists(Progress prog)
    {
        var returnList = new List<string>(_words);
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

}