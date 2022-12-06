using AdventOfCodeUtilities;

List<string> inputList = AoCUtilities.GetInputLines();

void P1()
{
    Queue<char> buffer = new Queue<char>();
    int count = 0;

    foreach (char c in inputList[0])
    {
        if (buffer.Count < 4)
            buffer.Enqueue(c);
        else if (buffer.Distinct().Count() == 4)
            break;
        else
        {
            buffer.Dequeue();
            buffer.Enqueue(c);
        }
        count++;
    }    

    Console.WriteLine(count);
    Console.ReadLine();
}

void P2()
{
    Queue<char> buffer = new Queue<char>();
    int count = 0;

    foreach (char c in inputList[0])
    {
        if (buffer.Count < 14)
            buffer.Enqueue(c);
        else if (buffer.Distinct().Count() == 14)
            break;
        else
        {
            buffer.Dequeue();
            buffer.Enqueue(c);
        }
        count++;
    }

    Console.WriteLine(count);
    Console.ReadLine();
}

P1();
P2();