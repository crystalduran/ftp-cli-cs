using FluentFTP;

Console.WriteLine("=== FTP CLI Tool for Learning ===");

Console.Write("Enter FTP host (default 127.0.0.1): ");
string? host = Console.ReadLine();
if (string.IsNullOrWhiteSpace(host)) host = "127.0.0.1";

Console.Write("Enter username: ");
string? user = Console.ReadLine();

Console.Write("Enter password: ");
string pass = ReadPassword();


// === COMMAND FUNCTIONS ===

static string ReadPassword()
{
    string password = "";
    ConsoleKey key;

    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password[0..^1];
            Console.Write("\b \b");
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            password += keyInfo.KeyChar;
            Console.Write("*");
        }
    } while (key != ConsoleKey.Enter);


    Console.WriteLine();
    return password;
}


static void ListFiles(FtpClient client)
{
    Console.WriteLine("\nListing files and folders...");
    foreach (var item in client.GetListing("/"))
    {
        Console.WriteLine($"{item.Type}: {item.FullName}");
    }
}

static async Task UploadFile(AsyncFtpClient client)
{
    Console.Write("Enter local file path to uploead: ");
    string? localPath = Console.ReadLine();
    if (!File.Exists(localPath))
    {
        Console.Write($"{localPath} does not exist");
        return;
    }

    Console.Write("Enter remote file name (e.g /file.txt): ");
    string? remotePath = Console.ReadLine();

    FtpStatus status = await client.UploadFile(localPath, remotePath);

    switch(status)
    {
        case FtpStatus.Failed:
                    Console.WriteLine("File upload failed!");
            break;
        case FtpStatus.Success:
            Console.WriteLine("File uploaded successfully!");
            break;
        case FtpStatus.Skipped:
            Console.WriteLine("File upload skipped (file may already exist).");
            break;
    }
}

