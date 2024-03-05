
using BenchmarkDotNet.Running;

class Program
{
    static void Main(string[] args)
    {
        // var config = new ManualConfig{
        //     SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(50),
        //     Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest),
        //     Options = ConfigOptions.Default
        // };
        
        //BenchmarkSummary = BenchmarkRunner.Run(TestAssemby, config);        
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
