using FluentFTP;
using FluentFTP.Exceptions;

Console.WriteLine("=== FTP CLI Tool for Learning ===");

Console.Write("Enter FTP host (default 127.0.0.1): ");
string? host = Console.ReadLine();
if (string.IsNullOrWhiteSpace(host)) host = "127.0.0.1";

Console.Write("Enter username: ");
string? user = Console.ReadLine();

Console.Write("Enter password: ");
string pass = ReadPassword();

// create ftp client

Console.WriteLine("\nConnecting to FTP server...");
var client = await ConnectAsync(host, user, pass);

Console.WriteLine("\nAvailable commands: list, mkdir, upload, delete, download, exit");

while (true)
{
    Console.Write("\nftp> ");
    string? command = Console.ReadLine()?.Trim().ToLower();

    if (string.IsNullOrWhiteSpace(command)) continue;

    switch (command)
    {
        case "list":
            await ListFiles(client);
            break;
        case "mkdir":
            await CreateFolder(client);
            break;
        case "upload":
            await UploadFile(client);
            break;
        case "delete":
            await DeleteFile(client);
            break;
        case "download":
            await DownloadFile(client);
            break;
        case "exit":
            await client.Disconnect();
            Console.WriteLine("Bye!");
            return;
        default:
            Console.WriteLine("Unknown command. You can only use list, mkdir, upload, delete, download, exit. Try again.");
            break;
    }
}


static async Task<AsyncFtpClient> ConnectAsync(string? host, string? user, string? pass)
{
    var token = new CancellationToken();

    var conn = new AsyncFtpClient()
    {
        Port = 21,
        Host = host,
        Credentials = new System.Net.NetworkCredential(user, pass)
    };

    try
    {
        await conn.Connect(token);
        Console.WriteLine("Connected successfully!");
    }
    catch (FtpCommandException ex)
    {
        Console.WriteLine($"FTP command error: {ex.Message}");
    }
    catch (FtpException ex)
    {
        Console.WriteLine($"FTP error: {ex.Message}");
        throw;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        throw;
    }

    return conn;
}


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


static async Task ListFiles(AsyncFtpClient client)
{
    Console.WriteLine("\nListing files and folders...");
    string path = "."; // root virtual del usuario

    try
    {
        var listing = await client.GetListing(path);
        foreach (var item in listing)
        {
            Console.WriteLine($"{item.Type}: {item.FullName}");
        }
    }
    catch (FtpCommandException ex) when (ex.CompletionCode == "501")
    {
        Console.WriteLine($"Directory '{path}' does not exist.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
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

    switch (status)
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

static async Task DownloadFile(AsyncFtpClient client)
{
    Console.Write("Enter the remote file path: ");
    string? remotePath = Console.ReadLine();

    Console.Write("Enter local destination path: ");
    string? localPath = Console.ReadLine();

    try
    {
        FtpStatus status = await client.DownloadFile(localPath, remotePath);

        if (status == FtpStatus.Success)
        {
            Console.WriteLine("File downloaded successfully!");
        }
        else
        {
            Console.WriteLine("File download failed!");
        }
    }
    catch (FtpCommandException ex)
    {
        Console.WriteLine($"Failed to download file: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error occurred: {ex.Message}");
    }
}

static async Task CreateFolder(AsyncFtpClient client)
{
    Console.Write("Enter new folder name: ");
    string? folderName = Console.ReadLine();
    if (await client.CreateDirectory(folderName))
    {
        Console.WriteLine("Folder created successfully!");
    }
    else
    {
        Console.WriteLine("Failed to create folder!");
    }
}

static async Task DeleteFile(AsyncFtpClient client)
{
    Console.Write("Enter the remote file path to delete: ");
    string? remotePath = Console.ReadLine();

    try
    {
        await client.DeleteFile(remotePath);
        Console.WriteLine("File deleted successfully!");
    }
    catch (FtpCommandException ex)
    {
        Console.WriteLine($"Failed to delete file: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error occurred: {ex.Message}");
    }
}



