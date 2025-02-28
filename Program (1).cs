using System;
using System.Threading; // Import threading library

class Program
{
    // Synchronization lock object for shared counter
    private static readonly object lockObject = new object();
    private static int sharedCounter = 0; // Shared counter variable

    // Multi-threading counter method
    // Each thread calls this function to increment the shared counter safely.
    static void IncrementCounter(object threadIdObj)
    {
        int threadId = (int)threadIdObj; // Convert object to int (thread ID)

        for (int i = 0; i < 1000; i++)  // Each thread increments the counter 1000 times
        {
            lock (lockObject)  // Lock ensures only one thread modifies the counter at a time
            {
                sharedCounter++; // Increment shared counter
            }
        }
        Console.WriteLine($"Thread {threadId} finished execution."); // Print when thread completes
    }

    // Simulates a Deadlock Scenario
    // Two threads attempt to lock resources in a way that causes a deadlock.
    static void DeadlockScenario()
    {
        object resource1 = new object(); // First shared resource
        object resource2 = new object(); // Second shared resource

        // First thread locks resource1 first, then tries to lock resource2
        void DeadlockThread1()
        {
            lock (resource1)
            {
                Console.WriteLine("Thread 1: Locked resource 1");
                Thread.Sleep(100); // Simulate work delay

                if (Monitor.TryEnter(resource2, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        Console.WriteLine("Thread 1: Locked resource 2");
                    }
                    finally
                    {
                        Monitor.Exit(resource2);
                    }
                }
                else
                {
                    Console.WriteLine("Thread 1: Could not acquire resource 2, avoiding deadlock.");
                }
            }
        }

        // Second thread locks resource2 first, then tries to lock resource1
        void DeadlockThread2()
        {
            lock (resource2)
            {
                Console.WriteLine("Thread 2: Locked resource 2");
                Thread.Sleep(100); // Simulate work delay

                if (Monitor.TryEnter(resource1, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        Console.WriteLine("Thread 2: Locked resource 1");
                    }
                    finally
                    {
                        Monitor.Exit(resource1);
                    }
                }
                else
                {
                    Console.WriteLine("Thread 2: Could not acquire resource 1, avoiding deadlock.");
                }
            }
        }

        // Create and start both threads
        Thread t1 = new Thread(DeadlockThread1);
        Thread t2 = new Thread(DeadlockThread2);

        t1.Start();
        t2.Start();

        t1.Join(); // Wait for thread 1 to finish
        t2.Join(); // Wait for thread 2 to finish

        Console.WriteLine("Deadlock scenario executed.");
    }

    // Resolves Deadlocks using timeout-based locks
    // Instead of direct locks, it uses Monitor.TryEnter to prevent deadlock
    static void DeadlockResolution()
    {
        object resource1 = new object();
        object resource2 = new object();

        void SafeThread1()
        {
            bool lock1 = false, lock2 = false;
            try
            {
                lock1 = Monitor.TryEnter(resource1, TimeSpan.FromSeconds(1)); // Try to lock resource1
                if (!lock1) return;

                Console.WriteLine("Thread 1: Locked resource 1");
                Thread.Sleep(100);

                lock2 = Monitor.TryEnter(resource2, TimeSpan.FromSeconds(1)); // Try to lock resource2
                if (!lock2) return;

                Console.WriteLine("Thread 1: Locked resource 2");
            }
            finally
            {
                if (lock2) Monitor.Exit(resource2); // Release lock on resource2
                if (lock1) Monitor.Exit(resource1); // Release lock on resource1
            }
        }

        void SafeThread2()
        {
            bool lock1 = false, lock2 = false;
            try
            {
                lock1 = Monitor.TryEnter(resource2, TimeSpan.FromSeconds(1)); // Try to lock resource2
                if (!lock1) return;

                Console.WriteLine("Thread 2: Locked resource 2");
                Thread.Sleep(100);

                lock2 = Monitor.TryEnter(resource1, TimeSpan.FromSeconds(1)); // Try to lock resource1
                if (!lock2) return;

                Console.WriteLine("Thread 2: Locked resource 1");
            }
            finally
            {
                if (lock2) Monitor.Exit(resource1);
                if (lock1) Monitor.Exit(resource2);
            }
        }

        // Start both safe threads
        Thread t1 = new Thread(SafeThread1);
        Thread t2 = new Thread(SafeThread2);

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        Console.WriteLine("Deadlock resolved.");
    }

    // Banking System Implementation (Last)
    class BankAccount
    {
        public int Balance { get; private set; } // Account balance
        private readonly object lockObject = new object(); // Lock for thread safety

        public BankAccount(int initialBalance)
        {
            Balance = initialBalance;
        }

        // Withdraw money from the account (Thread-safe)
        public bool Withdraw(int amount)
        {
            lock (lockObject)
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    Console.WriteLine($"Withdrawn {amount}. New balance: {Balance}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Insufficient funds: {Balance}");
                    return false;
                }
            }
        }

        // Deposit money into the account (Thread-safe)
        public void Deposit(int amount)
        {
            lock (lockObject)
            {
                Balance += amount;
                Console.WriteLine($"Deposited {amount}. New balance: {Balance}");
            }
        }

        // Transfer money between accounts (Avoids deadlocks using ordered locks)
        public static void Transfer(BankAccount from, BankAccount to, int amount)
        {
            object firstLock = from.GetHashCode() < to.GetHashCode() ? from.lockObject : to.lockObject;
            object secondLock = from.GetHashCode() > to.GetHashCode() ? from.lockObject : to.lockObject;

            lock (firstLock)
            {
                Thread.Sleep(100);
                lock (secondLock)
                {
                    if (from.Withdraw(amount))
                    {
                        to.Deposit(amount);
                        Console.WriteLine($"Transferred {amount} from Account A to Account B");
                    }
                    else
                    {
                        Console.WriteLine("Transfer failed due to insufficient funds.");
                    }
                }
            }
        }
    }

    static void BankingSystem()
    {
        Console.WriteLine("\n🔹 Running Banking System...");
        BankAccount accountA = new BankAccount(1000);
        BankAccount accountB = new BankAccount(500);

        Thread t1 = new Thread(() => BankAccount.Transfer(accountA, accountB, 200));
        Thread t2 = new Thread(() => BankAccount.Transfer(accountB, accountA, 300));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        Console.WriteLine($"Final Balance - Account A: {accountA.Balance}, Account B: {accountB.Balance}");
    }

    // Main Menu
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n Select an option:");
            Console.WriteLine("1️⃣ Multi-Threading Counter");
            Console.WriteLine("2️⃣ Deadlock Scenario");
            Console.WriteLine("3️⃣ Deadlock Resolution");
            Console.WriteLine("4️⃣ Banking System");
            Console.WriteLine("5️⃣ Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n Running Multi-Threading Counter...");
                    Thread[] threads = new Thread[10];
                    for (int i = 0; i < 10; i++)
                    {
                        threads[i] = new Thread(new ParameterizedThreadStart(IncrementCounter));
                        threads[i].Start(i);
                    }
                    foreach (Thread t in threads) t.Join();
                    Console.WriteLine($" Final counter value: {sharedCounter}");
                    break;

                case "2":
                    DeadlockScenario();
                    break;

                case "3":
                    DeadlockResolution();
                    break;

                case "4":
                    BankingSystem();
                    break;

                case "5":
                    Console.WriteLine("\n Exiting program...");
                    return;

                default:
                    Console.WriteLine(" Invalid option. Please select again.");
                    break;
            }
        }
    }
}