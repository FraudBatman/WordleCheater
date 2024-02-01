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

using WordleCheater.Algorithms;

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
    public const string AlphabetString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string FiveLettWordList = "5LetterWords.txt";
    private const string BestFirstGuess = "CRATE";
    private const string OfficialWordList = "OfficialList.txt";
    public const int WordSize = 5;

    public static List<string> _words = new List<string>();
    private static readonly Random Random = new Random();

    private static string? FirstGuess; //used for batch testing

    private static Algorithm algorithm = new LeastCommon();

    public static void Main()
    {
        // //Single word testing
        // var Words = GetWordList(FiveLettWordList);
        // CheatTestLoop("SOLVE");

        // // Batch list testing
        // CheatTest(FiveLettWordList);
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
                    Console.WriteLine("Nope this isn't a thing");
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
            // Console.WriteLine("3)\tDordle\t\t\t(2 - 7)");
            // Console.WriteLine("4)\tQuordle\t\t\t(4 - 9)");
            // Console.WriteLine("5)\tOctordle\t\t(8 - 13)");
            // Console.WriteLine("6)\tSedordle\t\t(16 - 21)");
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
        var progress = new Progress();
        var results = new List<string>();
        var guesses = new List<string?>();

        for (var guessNum = 1; guessNum <= maxGuesses; guessNum++)
        {
            var guess = guessNum == 1 ? BestFirstGuess : GuessWord(progress);

            // var guess = GuessWord(progresses);

            guesses.Add(guess);

            if (guess == null)
            {
                Console.WriteLine("The cheater has given up. Good luck.");
                Console.ReadLine();
                return;
            }
            if (wordleCount == 1)
            {
                Console.WriteLine($"Words remaining: {ShrinkWordsLists(progress).Count}");
            }
            Console.WriteLine($"Guess #{guessNum}:\t{guess}");

            string? result;
            var cleanResult = true;
            do
            {
                Console.Write("Result:\t\t");
                result = Console.ReadLine()?.ToUpperInvariant().Trim().Substring(0, WordSize);

                if (result == "") continue;

                foreach (var character in result)
                {
                    if (!"XYG".Contains(character))
                    {
                        cleanResult = false;
                        break;
                    }
                }
            } while (result == "" || !cleanResult || result.Length != WordSize);

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
            progress.ParseProgress(guess, result);
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

        // FirstGuess = GuessWord(dumProg);
        FirstGuess = BestFirstGuess;

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
                            var proggers = new Progress();
                            proggers.ParseProgress(FirstGuess, testResult);
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
        var progress = new Progress();
        Console.WriteLine($"Target Word: {targetWord}");

        var guess = FirstGuess;

        Console.WriteLine($"Guess: {guess}");

        var guesses = 1;
        while (guess != targetWord)
        {
            var result = GetGuessResult(guess, targetWord);
            progress.ParseProgress(guess, result);

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

                returnString += itsY ? "Y" : "X";
            }
            else
            {
                returnString += "X";
            }
        }
        Console.WriteLine($"Result: {returnString}");
        return returnString;
    }

    private static string? GuessWord(Progress progress)
    {
        return algorithm.Guess(progress);
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

    private static List<string> ShrinkWordsLists(Progress prog)
    {
        throw new Exception("wrong ShrinkWordsLists(), dumbass.");
    }

}