// 10:06

using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<(Packet, Packet)> Pairs = new List<(Packet, Packet)>();
List<Packet> Packets = new List<Packet>();
for (int i = 0; i < inputList.Count; i += 3)
{
    Packet nP1 = new Packet(inputList[i]);
    Packet nP2 = new Packet(inputList[i + 1]);
    Pairs.Add((nP1, nP2));
    Packets.Add(nP1);
    Packets.Add(nP2);
}

void P1()
{
    int result = 0;
    for (int i = 0; i < Pairs.Count; i++)
    {
        (Packet p1, Packet p2) = Pairs[i];
        int val = p1.Compare(p2);
        if (val < 0)
            result += i + 1;
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Packet dP1 = new Packet("[[2]]");
    Packet dP2 = new Packet("[[6]]");
    Packets.Add(dP1);
    Packets.Add(dP2);
    Packets.Sort((x, y) => x.Compare(y));
    Console.WriteLine((Packets.IndexOf(dP1) + 1) * (Packets.IndexOf(dP2) + 1));
    Console.ReadLine();
}

P1();
P2();

class Packet
{
    int? Value = null;
    List<Packet>? Packets = null;

    public Packet(string s)
    {
        if (s.Length >= 2 && s[0] == '[' && s[s.Length - 1] == ']')
        {
            Packets = new List<Packet>();
            if (s.Length > 2)
            {
                int depth = 0;
                int previousComma = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '[')
                        depth++;
                    else if (s[i] == ']')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            Packets.Add(new Packet(s.Substring(previousComma + 1, i - previousComma - 1)));
                        }
                    }
                    else if (s[i] == ',' && depth == 1)
                    {
                        Packets.Add(new Packet(s.Substring(previousComma + 1, i - previousComma - 1)));
                        previousComma = i;
                    }
                }
            }
        }
        else
        {
            Value = int.Parse(s);
        }
    }

    public override string ToString()
    {
        if (Value != null)
            return ((int)Value).ToString();
        else if (Packets != null)
            return $"[{string.Join(',', Packets)}]";
        else
            return "";
    }

    public int Compare(Packet otherPacket)
    {
        if (this.Value != null && otherPacket.Value != null)
            return (int)this.Value - (int)otherPacket.Value;
        else
        {
            Packet tempPacket1;
            if (this.Value != null)
                tempPacket1 = new Packet($"[{this.Value}]");
            else
                tempPacket1 = this;
            Packet tempPacket2;
            if (otherPacket.Value != null)
                tempPacket2 = new Packet($"[{otherPacket.Value}]");
            else
                tempPacket2 = otherPacket;

            if (tempPacket1.Packets is null || tempPacket2.Packets is null)
                throw new NotImplementedException();

            for (int i = 0; i < Math.Max(tempPacket1.Packets.Count, tempPacket2.Packets.Count); i++)
            {
                if (i >= tempPacket1.Packets.Count)
                    return -1;
                else if (i >= tempPacket2.Packets.Count)
                    return 1;
                else
                {
                    int val = tempPacket1.Packets[i].Compare(tempPacket2.Packets[i]);
                    if (val != 0)
                        return val;
                }
            }
            return 0;
        }
    }
}