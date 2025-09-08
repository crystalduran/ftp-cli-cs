# Console FTP Client (C#)

A simple FTP command-line client built with **C#** and **.NET**, using the [FluentFTP](https://github.com/robinrodricks/FluentFTP) library.  
This tool allows you to **connect to an FTP server** (in my case, a **personal FileZilla server**) and perform basic file operations:

- Upload files
- Download files
- Delete files
- List directories and files

## Motivation

I built this project for **learning purposes**.  
My main goal was to explore how **FTP** works and get hands-on experience with **FluentFTP’s** `AsyncFtpClient`.  
It also served as practice for working with asynchronous methods and handling file transfers.

## Features

**Supported operations**:
- `upload` → Upload files to the server
- `download` → Download files from the server
 - `delete` → Delete files 
- `list` → List files and directories

## Future Improvements

- Add SFTP support.
- Enforce Explicit FTP over TLS only by removing support for insecure plain FTP connections
- Improve error handling and logging to provide clearer feedback when connections fail or operations are denied
- Unit tests
