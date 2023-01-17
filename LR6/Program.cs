class OS
{
    private static AutoResetEvent signal = new AutoResetEvent(false);
    private volatile static bool interruptFlag;

    private static uint counter = 0;

    static void Main(string[] args)
    {
        Thread process = new Thread(Process);
        process.Start();

        Thread interrupt = new Thread(Interrupt);
        interrupt.Start();
    }

    private static void Process()
    {
        while (true)
        {
            if (interruptFlag)
            {
                Console.WriteLine("Работа процесса приостановлена, нажмите F для возобновления");
                if (signal.WaitOne())
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine(string.Concat("Программа работает. Шаг: ", (counter++).ToString()));
            Task.Delay(200).Wait();
        }
    }

    private static void Interrupt()
    {
        while (true)
        {
            ConsoleKeyInfo c = Console.ReadKey();
            if (c.Key == ConsoleKey.Enter)
            {
                interruptFlag = true;
                
                c = Console.ReadKey();

                if(c.Key == ConsoleKey.F)
                {
                    interruptFlag = false;
                    signal.Set();
                }
            }
        }
    }
}
