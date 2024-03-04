using System.Text;

namespace BankOCR.Core;

public class InvalidNumberEstimator : IAccountNumberEstimator
{
    private readonly Dictionary<string, string[]> DigitSimilMap = new Dictionary<string, string[]>
    {
        {"0", new string[]{"8"}},
        {"1", new string[] {"7"}},
        {"3", new string[] {"9"}},
        {"5", new string[] {"6", "9"}},
        {"6", new string[] {"5", "8"}},
        {"7", new string[]{"1"}},
        {"8", new string[] {"0","6","9"}},
        {"9", new string[] {"3", "5", "8"}},
    };

    public string[] Estimate(string accountNumber, Func<string, bool> accNumChecksum, Dictionary<string[], int> digitFaults)
    {
        if(string.IsNullOrEmpty(accountNumber))
        {
            throw new NotSupportedException("accountNumber cannot be null");
        }

        if(accNumChecksum == null)
        {
            throw new NotSupportedException("accNumChecksum cannot be null");
        }

        string[] estimates = [];
        string accNum = accountNumber;

        for (int i = 0; i < accNum.Length; i++)
        {
            estimates = estimate(accNum[i], i, accountNumber, accNumChecksum);
        }

        return estimates;        
    }

    private string[] estimate(char digit, int index, string accountNumber, Func<string, bool> accNumChecksum)
    {
        if (!DigitSimilMap.ContainsKey(digit.ToString()))
        {
            return [];
        }

        StringBuilder accNum = new StringBuilder(accountNumber);
        string [] replacements = DigitSimilMap[digit.ToString()];
        List<string> estimates = [];

        for (int i = 0; i < replacements.Length; i++)
        {
            accNum.Remove(index, 1);
            accNum.Insert(index, replacements[i]);
            
            if(accNumChecksum(accNum.ToString()))
            {
                estimates.Add(accNum.ToString());
            }
        }

        return estimates.ToArray();
    }
}