int frames = 64; //количество делений физической памяти
long[] physMemory = new long[frames]; //инициализируем физическую память
List<List<TableEntry>> virtMemoryTable = new List<List<TableEntry>>(); //вирутальная память представленная множеством виртуальных адресных пространств и соответствующих таблиц

while (true) //цикл ввода
{
    string? input = Console.ReadLine(); //ожидаем ввод от пользователя
    if (input != null)
    {
        string[] command = input.Split(" "); //разбиваем команду поступившую от пользователя на саму команду и числовые параметры
        switch (command[0])
        {
            case "add":
                AddProcess(int.Parse(command[1])); //добавляем в память процесс с определённым кол-вом страниц
                ShowPhysicalMemory();
                break;
            case "remove":
                RemoveProcess(int.Parse(command[1])); //удаляем процесс вместе с его таблицей страниц
                ShowPhysicalMemory();
                break;
            case "getpa": 
                GetPhysicalAddress(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3])); //получаем физический адрес по номеру процесса, номеру виртуальной страницы и добавляем смещение
                break;
            case "showpm":
                ShowPhysicalMemory(); //наглядно показываем пользователю карту физической памяти
                break;
        }
    }
}

void AddProcess(int pages)
{
    List<TableEntry> process = new List<TableEntry>();
    List<int> spareFramesIndexes = GetSpareFrames(); //получаем свободные страницы физической памяти
    if (spareFramesIndexes.Count >= pages)
    {
        for (int i = 0; i < pages; i++) //для указанного числа страниц создаём записи в таблице
        {
            TableEntry tableEntry = new TableEntry(spareFramesIndexes[i]); 
            process.Add(tableEntry); //добавляем их в процесс
        }
    }
    else Console.WriteLine("Нет места для процесса с таким кол-вом страниц");
    virtMemoryTable.Add(process); //добавляем процесс в виртуальную память
}

void RemoveProcess(int index)
{
    virtMemoryTable.RemoveAt(index);
}

List<int> GetSpareFrames()
{
    List<int> spareFramesIndexes = new List<int>();
    bool[] physMemoryController = Enumerable.Repeat<bool>(false, frames).ToArray(); //создаём массив проекции виртуальных страниц на физические
    for (int i = 0; i < virtMemoryTable.Count; i++)
    {
        for (int j = 0; j < virtMemoryTable[i].Count; j++)
        {
            physMemoryController[virtMemoryTable[i][j].physAddrs] = true; //если страница записана в виртуальную память, то меняем значение на true
        }
    }
    for (int i = 0; i < physMemoryController.Length; i++) //все страницы физической памяти, которые не ассоциированы с виртуальными (false) добавляем в лист свободных страниц
    {
        if (physMemoryController[i] == false) spareFramesIndexes.Add(i); 
    }
    return spareFramesIndexes;
}

void GetPhysicalAddress(int process, int pageAddress, int offset)
{
    Console.WriteLine(virtMemoryTable[process][pageAddress].physAddrs + " " + offset);
}

void ShowPhysicalMemory()
{
    Array colors = Enum.GetValues(typeof(ConsoleColor));
    Console.ForegroundColor = ConsoleColor.Black;
    Random random = new Random();

    ConsoleColor[] physMemoryColors = Enumerable.Repeat<ConsoleColor>(ConsoleColor.White, frames).ToArray(); //создаём массив цветов физической памяти, изначально все элементы окрашены в белый цвет, который означает свободную страницу
    for (int i = 0; i < virtMemoryTable.Count; i++)
    {
        ConsoleColor processColor = (ConsoleColor)(colors.GetValue(random.Next(1, 14)) ?? 1); //получаем случайный цвет процесса
        for (int j = 0; j < virtMemoryTable[i].Count; j++) //для каждой страницы устанавливаем цвет процесса
        {
            physMemoryColors[virtMemoryTable[i][j].physAddrs] = processColor; 
        }
    }

    for (int i = 0; i < physMemoryColors.Length; i++) //выводим на экран страницы физической памяти окрашенные в соответствующие цвета
    {
        Console.BackgroundColor = physMemoryColors[i];
        Console.Write(i + "|");
    }
    Console.WriteLine();
    Console.ResetColor();
}

class TableEntry
{
    public int physAddrs;
    public List<Attr> attrs;

    public TableEntry (int physAddrs, List<Attr> attrs)
    {
        this.physAddrs = physAddrs;
        this.attrs = attrs;
    }

    public TableEntry (int physAddrs)
    {
        this.physAddrs = physAddrs;
        attrs = new List<Attr>();
        attrs.Add(Attr.rw);
    }
}

enum Attr
{
    r,
    rw
}