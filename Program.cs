if (args.Length != 3)
{
    Console.WriteLine("Usage: FolderSync <sourcePath> <replicaPath> <intervalSeconds>");
    return;
}

var sourceFolder = args[0];
var replicaFolder = args[1];
int.TryParse(args[2], out int intervalSeconds);

if (CheckInitialArgs())
{
    Console.WriteLine("Starting folder synchronization...");
} else
{
    Console.WriteLine("Invalid arguments. Exiting.");
}

bool CheckInitialArgs()
{

    if (intervalSeconds <= 0)
    {
        Console.WriteLine("Interval must be a positive number.");
        return false;
    }

    if (!Directory.Exists(sourceFolder))
    {
        Console.WriteLine($"Source folder '{sourceFolder}' does not exist.");
        return false;
    }

    if (!Directory.Exists(replicaFolder))
    {
        Directory.CreateDirectory(replicaFolder);
        return true;
    }
    return true;
}
