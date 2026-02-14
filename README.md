# Address Book + Label Maker (C#)

A simple C# console app that runs on Windows and lets you:
- store mailing contacts,
- edit/delete contacts,
- export mailing labels into a text file.

## Requirements

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Run on Windows

```powershell
dotnet run
```

## Build release executable

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

The executable is created under:

`bin\Release\net8.0\win-x64\publish\`

## Data files

- `contacts.json`: saved contact list (created automatically)
- `labels.txt`: default output for label export
