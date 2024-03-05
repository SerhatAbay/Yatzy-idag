class Program
{
    static Random tilfældig = new Random();
    static void Main()
        
    {
        Console.WriteLine("Velkommen til Yatzy!");

        // Spillerscores
        int[] spillerscores = new int[2];

        // Scorekort
        int[,] scorekort = new int[2, 13]; // To spillere, 13 rækker

        // Hovedspill-løkke
        for (int runde = 0; runde < 13; runde++)
        {
            Console.WriteLine($"\nRunde {runde + 1}");

            // To spillere tager skiftevis
            for (int spiller = 0; spiller < 2; spiller++)
            {
                Console.WriteLine($"\nSpiller {spiller + 1}'s tur:");

                // Roll the dice
                int[] terninger = new int[5];
                bool[] beholdTerninger = new bool[5];
                RulTerninger(terninger, beholdTerninger);
                // Allow up to 2 more rolls
                for (int kast = 0; kast < 2; kast++)
                {
                    Console.WriteLine($"Du har {2 - kast} kast tilbage. Vælg terninger at beholde (1-5) eller 0 for at rulle alle igen:");
                    string input = Console.ReadLine();

                    if (input == "0")
                    {
                        RulTerninger(terninger, beholdTerninger);
                    }
                    else
                    {
                        int[] valgteTerninger = Array.ConvertAll(input.Split(' '), int.Parse);
                        foreach (int terning in valgteTerninger)
                        {
                            beholdTerninger[terning - 1] = !beholdTerninger[terning - 1];
                            Console.WriteLine($"Beholdt terning {terning}: {terninger[terning - 1]}");

                        }
                        for (int i = 0; i < terninger.Length; i++)
                        {
                            if (!beholdTerninger[i])
                            {
                                terninger[i] = tilfældig.Next(1, 7);
                            }
                        }
                        Console.WriteLine("Terninger for dette kast: " + string.Join(" ", terninger));

                    }

                }

                // Calculate score and update player's total score
                int score = BeregnScore(terninger);
                spillerscores[spiller] += score;

                // Update scorecard
                scorekort[spiller, runde] = score;

                Console.WriteLine($"Spiller {spiller + 1}'s score for denne runde: {score}");
                Console.WriteLine($"Total score for Spiller {spiller + 1}: {spillerscores[spiller]}");
            }
        }

        // Game over, determine the winner
        int vinder = (spillerscores[0] > spillerscores[1]) ? 1 : 2;
        Console.WriteLine($"\nSpillet er slut! Spiller {vinder} vinder med en samlet score på {spillerscores[vinder - 1]}.");

        // Display the scorecard
        Console.WriteLine("\nScorekort:");
        Console.WriteLine("Spiller\t1\t2\t3\t4\t5\t6\tSum\tBonus\t7\t8\t9\t10\t11\t12\tChance\tYahtzee\tBonus");

        for (int spiller = 0; spiller < 2; spiller++)
        {
            int sumOverSeks = 0;
            int sumUnderSeks = 0;
            bool harBonus = false;
            int chance = 0;
            int yahtzeeBonus = 0;

            for (int runde = 0; runde < 13; runde++)
            {
                sumOverSeks += scorekort[spiller, runde];

                if (runde < 6)
                {
                    sumUnderSeks += scorekort[spiller, runde];
                    harBonus = sumUnderSeks >= 63;
                }

                chance += scorekort[spiller, runde];

                // Check for Yahtzee and apply bonus
                if (runde == 12 && scorekort[spiller, runde] == 50)
                {
                    yahtzeeBonus += (yahtzeeBonus == 0) ? 100 : 0;
                }

                Console.Write($"{spiller + 1}\t");

                for (int kolonne = 0; kolonne < 13; kolonne++)
                {
                    Console.Write($"{scorekort[spiller, kolonne]}\t");
                }

                Console.WriteLine($"{sumOverSeks}\t{((harBonus) ? 35 : 0)}\t{chance}\t{yahtzeeBonus}");
            }
        }
    }

    static void RulTerninger(int[] terninger, bool[] beholdTerninger)
    {
        Random tilfældig = new Random();

        for (int i = 0; i < terninger.Length; i++)
        {
            if (!beholdTerninger[i])
            {
                terninger[i] = tilfældig.Next(1, 7);
            }
        }

        Console.WriteLine("Nuværende terninger: " + string.Join(" ", terninger));
    }

    static int BeregnScore(int[] terninger)
    {
        Array.Sort(terninger);

        // Check for Yatzy
        if (terninger[0] == terninger[4])
        {
            if (terninger[0] == 1)
            {
                Console.WriteLine("Yatzy");
                return 50; // Yatzy med etere
            }
            else
            {
                Console.WriteLine("Yatzy");
                return 100; // Yatzy med andre værdier
            }
        }

        // Check for fuldt hus
        if ((terninger[0] == terninger[2] && terninger[3] == terninger[4]) || (terninger[0] == terninger[1] && terninger[2] == terninger[4]))
        {
            Console.WriteLine("Fuldt Hus");
            return 25;
        }

        // Check for små straight
        if ((terninger[0] == 1 && terninger[4] == 5) && (terninger[1] == 2 || terninger[3] == 4))
        {
            Console.WriteLine("Lille straight");
            return 30;
        }

        // Check for stor straight
        if (terninger[0] == 1 && terninger[4] == 5)
        {
            Console.WriteLine("Stor straight");
            return 40;
        }

        // Check for tre ens
        for (int i = 0; i < 3; i++)
        {
            if (terninger[i] == terninger[i + 2])
            {
                Console.WriteLine("Tre ens");
                return terninger[i] * 3;
            }
        }

        // Check for fire ens
        for (int i = 0; i < 2; i++)
        {
            if (terninger[i] == terninger[i + 3])
            {
                Console.WriteLine("Fire ens");
                return terninger[i] * 4;
            }
        }

        // Check for par og to par
        for (int i = 0; i < 4; i++)
        {
            if (terninger[i] == terninger[i + 1])
            {
                if (i < 3 && terninger[i + 2] == terninger[i + 3])
                {
                    Console.WriteLine("To par");
                    return terninger[i] * 2 + terninger[i + 2] * 2; // To par
                }
                else
                {
                    Console.WriteLine("Et par");
                    return terninger[i] * 2; // Et par
                }
            }
        }

        // Hvis ingen af de ovenstående betingelser opfyldes, returneres summen af alle terninger
        return terninger[0] + terninger[1] + terninger[2] + terninger[3] + terninger[4];
    }
}
