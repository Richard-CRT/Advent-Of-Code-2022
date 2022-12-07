// 09:07

using AdventOfCodeUtilities;

List<string> inputList = AoCUtilities.GetInputLines();

Directory currentDirectory = Directory.RootDirectory;

foreach (string s in inputList)
{
    if (s[0] == '$')
    {
        string cmd = s.Substring(1).Trim();
        string[] cmdSplit = cmd.Split(' ');
        switch (cmdSplit[0])
        {
            case "cd":
                string target = cmdSplit[1];
                switch (target)
                {
                    case "/":
                        currentDirectory = Directory.RootDirectory;
                        break;
                    case "..":
                        if (currentDirectory.Parent != null)
                            currentDirectory = currentDirectory.Parent;
                        else
                            throw new NotImplementedException();
                        break;
                    default:
                        currentDirectory = currentDirectory.SubDirectories[target];
                        break;
                }
                break;
        }
    }
    else
    {
        if (s.Length > 4 && s.Substring(0,3) == "dir")
        {
            string dirName = s.Substring(4);
            if (!currentDirectory.SubDirectories.ContainsKey(dirName))
                currentDirectory.SubDirectories[dirName] = new Directory(dirName, currentDirectory);
        }
        else
        {
            File file = new File(s, currentDirectory);
            if (!currentDirectory.Files.ContainsKey(file.Name))
                currentDirectory.Files[file.Name] = file;
        }
    }
}

void P1()
{
    int total = 0;
    foreach (Directory d in Directory.AllDirectories)
    {
        int size = d.GetSize();
        if (size <= 100000)
        {
            total += size;
        }
    }
    Console.WriteLine(total);
    Console.ReadLine();
}

void P2()
{
    int freeSpace = 70000000 - Directory.RootDirectory.GetSize();
    int spaceToDelete = 30000000 - freeSpace;
    List<Directory> orderedDirectories = Directory.AllDirectories.OrderBy(d => d.GetSize()).ToList();
    Console.WriteLine(orderedDirectories.Find(d => d.GetSize() >= spaceToDelete).GetSize());
    Console.ReadLine();
}

//Console.WriteLine(Directory.RootDirectory.ToString());
P1();
P2();

class Directory
{
    static public List<Directory> AllDirectories = new List<Directory>();
    static public Directory RootDirectory = new Directory("");

    public Directory? Parent;
    public string Name;
    public Dictionary<string, Directory> SubDirectories = new Dictionary<string, Directory>();
    public Dictionary<string, File> Files = new Dictionary<string, File>();

    public int Depth = 0;

    public Directory(string name, Directory? parent = null)
    {
        this.Name = name;
        Parent = parent;
        if (parent != null)
            Depth = parent.Depth + 1;
        Directory.AllDirectories.Add(this);
    }

    private int? SizeCache = null;

    public int GetSize()
    {
        if (SizeCache == null)
        {
            SizeCache = Files.Values.Sum(f => f.Size) + SubDirectories.Values.Sum(d => d.GetSize());
            return (int)SizeCache;
        }
        else
            return (int)SizeCache;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < Depth; i++)
            s += "  ";
        s += $"- {Name} (dir)\n";
        foreach (Directory dir in SubDirectories.Values)
        {
            s += dir.ToString();
        }
        foreach (File f in Files.Values)
        {
            s += f.ToString();
        }
        return s;
    }
}

class File
{
    public Directory Parent;

    public string Name;
    public int Size;

    public File(string l, Directory parent)
    {
        string[] split = l.Split(' ');
        Name = split[1];
        Size = int.Parse(split[0]);
        Parent = parent;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < Parent.Depth + 1; i++)
            s += "  ";
        s += $"- {Name} (file, size={Size})\n";
        return s;
    }
}