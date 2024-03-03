using System.Reflection;
using BankOCR.Core;

namespace BankOCR.NTest;

public class TestAccountNumberReader
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
        var fileContents = GetResource("AccDigits.txt");
        File.WriteAllText(temp, fileContents);
        _tempFilePath = temp;
    }

    [Test(Description = "Should return array of account numbers")]    
    public void Read_Should_AccNumbers()
    {   
        var reader = new AccountNumberReader(_tempFilePath);
        var numbers = reader.Read();
        Assert.That(numbers, Is.Not.Null);
        Assert.That(numbers.Length, Is.GreaterThan(0));                
    }

    [Test(Description = "Should throw exception when path is null or empty or invalid")]
    public void Read_Should_Error()
    {
        var reader = new AccountNumberReader("");
        Assert.Throws<NotSupportedException>(() => reader.Read());

        var pathShouldNotExist = Path.Combine(Path.GetTempPath(), "Not_a_valid_f_i_l_e_000000000.txt");
        reader = new AccountNumberReader(pathShouldNotExist);
        Assert.Throws<Exception>(() => reader.Read());
    }

    [TearDown]
    public void Cleanup()
    {
        File.Delete(_tempFilePath);
    }
}