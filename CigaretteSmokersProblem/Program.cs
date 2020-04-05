namespace ConsoleApplication1
{
    using System;
    using System.Threading;

    class Program
    {
        private static readonly Mutex mut = new Mutex(false);

        private static readonly AutoResetEvent signalFromFirstAgent = new AutoResetEvent(false);
        private static readonly AutoResetEvent signalFromSecondAgent = new AutoResetEvent(false);
        private static readonly AutoResetEvent signalFromThirdAgent = new AutoResetEvent(false);

        private static int tobacco;
        private static int paper;
        private static int matches;

        public static void Main(string[] args)
        {
            Thread tobaccoSmoker = new Thread(TobaccoSmoker);
            Thread paperSmoker = new Thread(PaperSmoker);
            Thread matchesSmoker = new Thread(MatchesSmoker);
            Thread startAgents = new Thread(StartAgents);

            tobaccoSmoker.Start();
            paperSmoker.Start();
            matchesSmoker.Start();
            startAgents.Start();
        }

        private static void StartAgents()
        {
            while (true)
            {
                mut.WaitOne();
                int random = new Random().Next(0, 3);
                if (random == 0)
                {
                    Console.WriteLine("First Agent is putting Paper and Matches on the table.");
                    Console.WriteLine($"Paper is active. Current amount of Paper: {++paper}.");
                    Console.WriteLine($"Matches is active. Current amount of Matches: {++matches}.");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Tobacco Smoker.");
                    signalFromFirstAgent.Set();
                }
                else if (random == 1)
                {
                    Console.WriteLine("Second Agent is putting Tobacco and Matches on the table.");
                    Console.WriteLine($"Tobacco is active. Current amount of Tobacco: {++tobacco}.");
                    Console.WriteLine($"Matches is active. Current amount of Matches: {++matches}.");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Paper Smoker.");
                    signalFromSecondAgent.Set();
                }
                else
                {
                    Console.WriteLine("Third Agent is putting Paper and Tobacco on the table.");
                    Console.WriteLine($"Paper is active. Current amount of Paper: {++paper}.");
                    Console.WriteLine($"Tobacco is active. Current amount of Tobacco: {++tobacco}.");
                    Thread.Sleep(new Random().Next(100, 2000));
                    Console.WriteLine("Wakeup signal sent to Matches Smoker.");
                    signalFromThirdAgent.Set();
                }
                mut.ReleaseMutex();
            }
        }

        private static void TobaccoSmoker()
        {
            while (true)
            {
                signalFromFirstAgent.WaitOne();
                mut.WaitOne();
                Console.WriteLine($"Tobacco Smoker is making cigarette by Paper and Matches.");
                Console.WriteLine($"Current amount of Matches: {--matches}.");
                Console.WriteLine($"Current amount of Paper: {--paper}.");
                Thread.Sleep(new Random().Next(100, 2000));
                signalFromFirstAgent.Reset();
                mut.ReleaseMutex();
                Console.WriteLine("Tobacco Smoker is smoking.");
                Thread.Sleep(new Random().Next(100, 2000));
            }
        }

        private static void PaperSmoker()
        {
            while (true)
            {
                signalFromSecondAgent.WaitOne();
                mut.WaitOne();
                Console.WriteLine($"Paper Smoker is making cigarette by Tobacco and Matches.");
                Console.WriteLine($"Current amount of Tobacco: {--tobacco}.");
                Console.WriteLine($"Current amount of Matches: {--matches}.");
                Thread.Sleep(new Random().Next(100, 2000));
                signalFromSecondAgent.Reset();
                mut.ReleaseMutex();
                Console.WriteLine("Paper Smoker is smoking.");
                Thread.Sleep(new Random().Next(100, 2000));
            }
        }

        private static void MatchesSmoker()
        {
            while (true)
            {
                signalFromThirdAgent.WaitOne();
                mut.WaitOne();
                Console.WriteLine($"Matches Smoker is making cigarette by Paper and Tobacco.");
                Console.WriteLine($"Current amount of Tobacco: {--tobacco}.");
                Console.WriteLine($"Current amount of Paper: {--paper}.");
                Thread.Sleep(new Random().Next(100, 2000));
                mut.ReleaseMutex();
                signalFromThirdAgent.Reset();
                Console.WriteLine("Matches Smoker is smoking.");
                Thread.Sleep(new Random().Next(100, 2000));
            }
        }
    }
}