using System.Reflection;
using BankOCR.Core;

namespace BankOCR.NTest;

public class TestAccountNumberWriter
{
    private static string _tempFilePath = "";
    private static Assembly? _assembly;
    public static Assembly TestAssemby
    {
        get
        {
            if (_assembly == null)
            {
                _assembly = Assembly.GetExecutingAssembly();
            }
            return _assembly;
        }
    }
    public static string GetResource(string name)
    {
        var resname = TestAssemby.GetManifestResourceNames().First(x => x.Contains(name));
        using (var stream = TestAssemby.GetManifestResourceStream(resname))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    [SetUp]
    public void Setup()
    {
        var temp = Path.GetTempFileName();
        _tempFilePath = temp;
    }

    [Test(Description = "Should write array of account numbers")]
    public void Write_Should_Save()
    {
        var entry = new string[]
        {
            "    _  _     _  _  _  _  _ ",
            "  | _| _||_||_ |_   ||_||_|",
            "  ||_  _|  | _||_|  ||_| _|",
            "                           "
        };
        var reader = new ScanDigitReader(entry);
        var parser = new ScanDigitParser();        
        var numbers = new AccountNumber[] { new AccountNumber(reader, parser) };
        var writer = new AccountNumberWriter(_tempFilePath);
        var count = writer.Write(numbers);
        Assert.That(count, Is.GreaterThan(0));

        var contents = File.ReadAllLines(_tempFilePath);
        Assert.That(contents.Length, Is.GreaterThan(0));
    }

    [Test(Description = "Should throw exception when path is null or empty or invalid")]
    public void Write_Should_Error()
    {
        var numbers = new AccountNumber[] { };
        var writer = new AccountNumberWriter("");
        Assert.Throws<NotSupportedException>(() => writer.Write(numbers));
    }

    [TearDown]
    public void Cleanup()
    {
        File.Delete(_tempFilePath);
    }
}