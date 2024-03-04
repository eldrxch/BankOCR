
namespace BankOCR.Core;

public interface IAccountNumberEstimator
{
    string[] Estimate(string accountNumber, Func<string, bool> accNumChecksum, Dictionary<string[], int> digitFaults = null);
}