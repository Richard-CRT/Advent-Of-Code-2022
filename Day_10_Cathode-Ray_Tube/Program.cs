// 7:38

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Instruction> Instructions = inputList.Select(x => new Instruction(x)).ToList();
void P1()
{
    int reg = 1;
    int cycle = 0;
    int result = 0;
    foreach (Instruction instruction in Instructions)
    {
        (int cyclesToIncrement, int newReg) = instruction.Execute(cycle, reg);
        for (int i = 0; i < cyclesToIncrement; i++)
        {
            cycle++;
            switch (cycle)
            {
                case 20: result += 20 * reg; break;
                case 60: result += 60 * reg; break;
                case 100: result += 100 * reg; break;
                case 140: result += 140 * reg; break;
                case 180: result += 180 * reg; break;
                case 220: result += 220 * reg; break;
            }
        }
        reg = newReg;
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int reg = 1;
    int cycle = 0;
    bool[,] screen = new bool[6, 40];
    foreach (Instruction instruction in Instructions)
    {
        (int cyclesToIncrement, int newReg) = instruction.Execute(cycle, reg);
        for (int i = 0; i < cyclesToIncrement; i++)
        {
            int temp = cycle % 240;
            int posY = temp / 40;
            int posX = temp % 40;

            if (Math.Abs(posX - reg) <= 1)
            {
                screen[posY, posX] = true;
            }

            cycle++;
        }
        reg = newReg;
    }
    for (int y = 0; y < 6; y++)
    {
        for (int x = 0; x < 40; x++)
        {
            Console.Write(screen[y,x] ? "██" : "  ");
        }
        Console.WriteLine();
    }
    Console.ReadLine();
}

P1();
P2();

public enum OpCode
{
    noop,
    addx
}
public class Instruction
{
    public OpCode OpCode;
    public int Operand;

    public Instruction(string s)
    {
        string[] split = s.Split(' ');
        switch (split[0])
        {
            case "noop": OpCode = OpCode.noop; break;
            case "addx": OpCode = OpCode.addx; Operand = int.Parse(split[1]); break;
        }
    }

    public (int, int) Execute(int cycle, int reg)
    {
        switch (OpCode)
        {
            case OpCode.noop: return (1, reg);
            case OpCode.addx: return (2, reg + Operand);
        }
        throw new NotImplementedException();
    }
}