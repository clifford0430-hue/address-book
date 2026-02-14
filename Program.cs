using System.Text;
using System.Text.Json;

namespace AddressBookLabelMaker;

internal static class Program
{
    private const string DataFile = "contacts.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private static List<Contact> _contacts = new();

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        LoadContacts();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Address Book + Label Maker (C# / Windows)");
            Console.WriteLine("=========================================");
            Console.WriteLine("1) View contacts");
            Console.WriteLine("2) Add contact");
            Console.WriteLine("3) Edit contact");
            Console.WriteLine("4) Delete contact");
            Console.WriteLine("5) Export mailing labels");
            Console.WriteLine("0) Exit");
            Console.Write("Select: ");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ViewContacts();
                    break;
                case "2":
                    AddContact();
                    break;
                case "3":
                    EditContact();
                    break;
                case "4":
                    DeleteContact();
                    break;
                case "5":
                    ExportLabels();
                    break;
                case "0":
                    return;
                default:
                    Pause("Unknown option.");
                    break;
            }
        }
    }

    private static void LoadContacts()
    {
        if (!File.Exists(DataFile))
        {
            _contacts = new List<Contact>();
            return;
        }

        var json = File.ReadAllText(DataFile);
        _contacts = JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
    }

    private static void SaveContacts()
    {
        var json = JsonSerializer.Serialize(_contacts, JsonOptions);
        File.WriteAllText(DataFile, json);
    }

    private static void ViewContacts()
    {
        Console.Clear();
        Console.WriteLine("Contacts");
        Console.WriteLine("========");

        if (_contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            Pause();
            return;
        }

        for (var i = 0; i < _contacts.Count; i++)
        {
            var c = _contacts[i];
            Console.WriteLine($"[{i + 1}] {c.FullName}");
            Console.WriteLine($"    {c.AddressLine1}");
            if (!string.IsNullOrWhiteSpace(c.AddressLine2))
            {
                Console.WriteLine($"    {c.AddressLine2}");
            }
            Console.WriteLine($"    {c.City}, {c.StateOrProvince} {c.PostalCode}");
            Console.WriteLine($"    {c.Country}");
            Console.WriteLine();
        }

        Pause();
    }

    private static void AddContact()
    {
        Console.Clear();
        Console.WriteLine("Add contact");
        Console.WriteLine("===========");

        var contact = ReadContactFromInput();
        _contacts.Add(contact);
        SaveContacts();

        Pause("Contact added.");
    }

    private static void EditContact()
    {
        if (!TrySelectContact(out var index))
        {
            return;
        }

        var current = _contacts[index];

        Console.Clear();
        Console.WriteLine("Edit contact (leave empty to keep current value)");
        Console.WriteLine("==============================================");

        _contacts[index] = new Contact
        {
            FullName = Prompt("Full name", current.FullName, allowEmpty: false),
            AddressLine1 = Prompt("Address line 1", current.AddressLine1, allowEmpty: false),
            AddressLine2 = Prompt("Address line 2", current.AddressLine2, allowEmpty: true),
            City = Prompt("City", current.City, allowEmpty: false),
            StateOrProvince = Prompt("State/Province", current.StateOrProvince, allowEmpty: false),
            PostalCode = Prompt("Postal code", current.PostalCode, allowEmpty: false),
            Country = Prompt("Country", current.Country, allowEmpty: false)
        };

        SaveContacts();
        Pause("Contact updated.");
    }

    private static void DeleteContact()
    {
        if (!TrySelectContact(out var index))
        {
            return;
        }

        var contact = _contacts[index];
        Console.Write($"Delete '{contact.FullName}'? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Pause("Canceled.");
            return;
        }

        _contacts.RemoveAt(index);
        SaveContacts();
        Pause("Contact deleted.");
    }

    private static void ExportLabels()
    {
        Console.Clear();
        Console.WriteLine("Export mailing labels");
        Console.WriteLine("=====================");

        if (_contacts.Count == 0)
        {
            Pause("No contacts to export.");
            return;
        }

        Console.Write("Output file (default labels.txt): ");
        var outputFile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(outputFile))
        {
            outputFile = "labels.txt";
        }

        var builder = new StringBuilder();
        foreach (var c in _contacts)
        {
            builder.AppendLine(c.FullName);
            builder.AppendLine(c.AddressLine1);
            if (!string.IsNullOrWhiteSpace(c.AddressLine2))
            {
                builder.AppendLine(c.AddressLine2);
            }
            builder.AppendLine($"{c.City}, {c.StateOrProvince} {c.PostalCode}");
            builder.AppendLine(c.Country);
            builder.AppendLine(new string('-', 40));
        }

        File.WriteAllText(outputFile, builder.ToString());
        Pause($"Labels exported to '{outputFile}'.");
    }

    private static bool TrySelectContact(out int index)
    {
        index = -1;

        if (_contacts.Count == 0)
        {
            Pause("No contacts available.");
            return false;
        }

        Console.Clear();
        Console.WriteLine("Select contact");
        Console.WriteLine("==============");

        for (var i = 0; i < _contacts.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {_contacts[i].FullName}");
        }

        Console.Write("Enter number: ");
        if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > _contacts.Count)
        {
            Pause("Invalid selection.");
            return false;
        }

        index = choice - 1;
        return true;
    }

    private static Contact ReadContactFromInput()
    {
        return new Contact
        {
            FullName = Prompt("Full name", allowEmpty: false),
            AddressLine1 = Prompt("Address line 1", allowEmpty: false),
            AddressLine2 = Prompt("Address line 2", allowEmpty: true),
            City = Prompt("City", allowEmpty: false),
            StateOrProvince = Prompt("State/Province", allowEmpty: false),
            PostalCode = Prompt("Postal code", allowEmpty: false),
            Country = Prompt("Country", allowEmpty: false)
        };
    }

    private static string Prompt(string label, string current = "", bool allowEmpty = false)
    {
        while (true)
        {
            var suffix = string.IsNullOrEmpty(current) ? string.Empty : $" [{current}]";
            Console.Write($"{label}{suffix}: ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                if (!string.IsNullOrEmpty(current))
                {
                    return current;
                }

                if (allowEmpty)
                {
                    return string.Empty;
                }

                Console.WriteLine("This field is required.");
                continue;
            }

            return input;
        }
    }

    private static void Pause(string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine();
            Console.WriteLine(message);
        }

        Console.WriteLine();
        Console.Write("Press ENTER to continue...");
        Console.ReadLine();
    }
}

internal sealed class Contact
{
    public string FullName { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string StateOrProvince { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
