if (args.Length != 4)
{
    Console.WriteLine("Usage: FolderSync <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>");
    return;
}

var sourceFolder = args[0];
var replicaFolder = args[1];
int.TryParse(args[2], out int intervalSeconds);
var logFilePath = args[3];

if (CheckInitialArgs())
{
    Console.WriteLine("Starting folder synchronization...");

    while (true)
    {
        Sync();
        DeleteExtraFilesAndFolders();
        Thread.Sleep(intervalSeconds * 1000);
    }

}
else
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

void Sync()
{
    Directory.EnumerateFileSystemEntries(sourceFolder, "*", SearchOption.AllDirectories).ToList().ForEach(sourcePath =>
    {
        var relativePath = Path.GetRelativePath(sourceFolder, sourcePath);
        var replicaPath = Path.Combine(replicaFolder, relativePath);

        if (Directory.Exists(sourcePath))
        {
            if (!Directory.Exists(replicaPath))
            {
                Directory.CreateDirectory(replicaPath);
                Log($"Created directory: {replicaPath}");
            }
        }
        else
        {
            if (!File.Exists(replicaPath) || File.GetLastWriteTimeUtc(sourcePath) > File.GetLastWriteTimeUtc(replicaPath))
            {
                File.Copy(sourcePath, replicaPath, true);
                Log($"Copied file: {replicaPath}");
            }
        }
    });
}

void DeleteExtraFilesAndFolders()
{
    Directory.EnumerateFileSystemEntries(replicaFolder, "*", SearchOption.AllDirectories).ToList().ForEach(replicaPath =>
    {
        var relativePath = Path.GetRelativePath(replicaFolder, replicaPath);
        var sourcePath = Path.Combine(sourceFolder, relativePath);

        if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
        {
            if (Directory.Exists(replicaPath))
            {
                Directory.Delete(replicaPath, true);
                Log($"Deleted directory: {replicaPath}");
            }
            else
            {
                File.Delete(replicaPath);
                Log($"Deleted file: {replicaPath}");
            }
        }
    });
}

void Log(string Message)
{
    File.AppendAllText(logFilePath, $"{DateTime.UtcNow}: {Message}{Environment.NewLine}");
}
