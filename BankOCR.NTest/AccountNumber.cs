using System.Reflection;
using BankOCR.Core;

namespace BankOCR.NTest;

public class TestAccountNumber
{    
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
    public static List<TestCaseData> ValidAccDigitsTestCases()
    {        
        var tests = new List<TestCaseData>();
        var file = GetResource("AccDigits.txt").Split("\n");
        for (int i = 0; i < file.Length; i+=5)
        {
            var input = new string[]
            {
                file[i],
                file[i+1],
                file[i+2],
                file[i+3]
            };
            var expected = file[i+4];
            expected = expected.Split("=>")[1].Trim();
            var tcase = new TestCaseData(input, expected);                
            tests.Add(tcase);
        }

        return tests;
    }


    public static List<TestCaseData> ValidAccChecksumTestCases()
    {        
        var tests = new List<TestCaseData>();
        var file = GetResource("AccChecksum.txt").Split("\n");
        for (int i = 0; i < file.Length; i+=5)
        {
            var input = new string[]
            {
                file[i],
                file[i+1],
                file[i+2],
                file[i+3]
            };
            var expected = file[i+4];
            expected = expected.Split("=>")[1].Trim();
            var tcase = new TestCaseData(input, expected);                
            tests.Add(tcase);
        }

        return tests;
    }    

    [SetUp]
    public void Setup()
    {
    }

    [Test(Description = "Should return the value of the account number")]
    [TestCaseSource(nameof(ValidAccDigitsTestCases))]
    public void Value_Should_Number(string[] input, string expect)
    {           
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        var value = account.Value();        
        Assert.That(value, Is.EqualTo(expect));                
    }

    [Test]
    public void Value_ShouldError()
    {
        var reader = new ScanDigitReader(new string[] { " _ ", "| |", " _|", "   " });
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        Assert.Throws<Exception>(() => account.Value());
    }

    [Test]
    [TestCaseSource(nameof(ValidAccChecksumTestCases))]
    public void IsValid_Should_Pass(string[] input, string expect)
    {
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        var value = account.Value();
        Assert.That(value, Is.EqualTo(expect));
        Assert.That(account.IsValid(), Is.True);
    }
    [Test]    
    public void IsValid_Should_NotPass()
    {
        var input = new string[]
        {
            " _  _  _  _  _  _  _  _    ",
            "| || || || || || || || |  |",
            "|_||_||_||_||_||_||_||_|  |",
            "                           "
        };
        var expect = "000000001";
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        var value = account.Value();
        Assert.That(value, Is.EqualTo(expect));
        Assert.That(account.IsValid(), Is.False);
    }

    [Test]
    [TestCaseSource(nameof(ValidAccChecksumTestCases))]
    public void IsIllegible_Should_NotPass(string[] input, string expect)
    {
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        var value = account.Value();
        Assert.That(value, Is.EqualTo(expect));
        Assert.That(account.IsIllegible(), Is.False);
    }

    [Test]
    public void IsIllegible_Should_Pass()
    {
        var input = new string[]
        {
            " _  _  _  _  _  _  _  _    ",
            "| || || || || || || ||    |",
            "|_||_||_||_||_||_||_||_|  |",
            "                           "
        };
        var expect = "0000000?1";
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var account = new AccountNumber(reader, parser);
        var value = account.Value();
        Assert.That(value, Is.EqualTo(expect));
        Assert.That(account.IsIllegible(), Is.True);
    }        
}