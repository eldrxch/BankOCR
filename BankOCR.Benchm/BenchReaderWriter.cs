using BenchmarkDotNet;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using System.Text;
using BankOCR.Core;
using BenchmarkDotNet.Diagnosers;

namespace BankOCR.NTest;

[MemoryDiagnoser]
[EventPipeProfiler(EventPipeProfile.GcVerbose)]
public class BenchReaderWriter
{
    private static string _tempReadFilePath = "";
    private static string _tempWriteFilePath = "";
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

    public BenchReaderWriter()
    {
    }    

    [BenchmarkDotNet.Attributes.GlobalSetup]
    public void BenchSetup()
    {
        var temp = Path.GetTempFileName();
        var content = GetResource("AccLarge.txt");
        File.WriteAllText(temp, content);
       _tempReadFilePath = temp;
    }

    [Benchmark]
    public void ReadFile()
    {
        var reader = new AccountNumberReader(_tempReadFilePath);
        _ = reader.Read();
    }

    [BenchmarkDotNet.Attributes.GlobalCleanup]
    public void BenchCleanup()
    {
        File.Delete(_tempReadFilePath);
    }
}
