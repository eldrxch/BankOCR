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

    public static List<TestCaseData> ValidAccIllegibleTestCases()
    {        
        var tests = new List<TestCaseData>();
        var file = GetResource("AccIllegible.txt").Split("\n");
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

    public static List<TestCaseData> ValidAccInvalidTestCases()
    {        
        var tests = new List<TestCaseData>();
        var file = GetResource("AccInvalid.txt").Split("\n");
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
        var temp = Path.GetTempFileName();
        _tempFilePath = temp;
    }

    [Test(Description = "Should write array of account numbers")]
    public void Write_Should_Write()
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

    [Test(Description = "Should return array of estimated account numbers for illegible account number")]
    [TestCaseSource(nameof(ValidAccIllegibleTestCases))]
    public void Write_Illegible_Should_Write(string[] input, string expect)
    {
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var accNum = new AccountNumber(reader, parser);
        var numbers = new AccountNumber[] {accNum};
        var writer = new AccountNumberWriter(_tempFilePath);
        var count = writer.Write(numbers);
        Assert.That(count, Is.GreaterThan(0));

        var contents = File.ReadAllText(_tempFilePath);
        var estimates = accNum.ValueEstimates(new IllegibleNumberEstimator());
        
        switch (estimates.Length)
        {
            case 0:
                Assert.That(contents, Contains.Substring("ILL"));
                break;
            case 1:
                Assert.That(contents, Contains.Substring(estimates[0]));
                break;
            default:
                Assert.That(contents, Contains.Substring("AMB"));
                break;
        }
    }

    [Test(Description = "Should return array of estimated account numbers for invalid account number")]
    [TestCaseSource(nameof(ValidAccInvalidTestCases))]
    public void Write_Invalid_Should_Write(string[] input, string expect)
    {        
        var reader = new ScanDigitReader(input);
        var parser = new ScanDigitParser();
        var accNum = new AccountNumber(reader, parser);
        var numbers = new AccountNumber[] {accNum};
        var writer = new AccountNumberWriter(_tempFilePath);
        var count = writer.Write(numbers);
        Assert.That(count, Is.GreaterThan(0));

        var contents = File.ReadAllText(_tempFilePath);    
        var estimates = accNum.ValueEstimates(new InvalidNumberEstimator());

        switch (estimates.Length)
        {
            case 0:
                Assert.That(contents, Contains.Substring("ILL"));
                break;
            case 1:
                Assert.That(contents, Contains.Substring(estimates[0]));
                break;
            default:
                Assert.That(contents, Contains.Substring("AMB"));
                break;
        }
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