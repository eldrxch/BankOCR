using System.Text;

namespace BankOCR.Core;

public class IllegibleNumberEstimator : IAccountNumberEstimator
{
    private readonly Dictionary<string, int> DigitMap = new Dictionary<string, int>
    {
        {" _ | ||_|   ", 0},
        {"     |  |   ", 1},
        {" _  _||_    ", 2},
        {" _  _| _|   ", 3},
        {"   |_|  |   ", 4},
        {" _ |_  _|   ", 5},
        {" _ |_ |_|   ", 6},
        {" _   |  |   ", 7},
        {" _ |_||_|   ", 8},
        {" _ |_| _|   ", 9},
    };

    private readonly Dictionary<string,int[]> SegmentOneMap = new Dictionary<string,int[]>
    {
        {"   ", new int[] {1,4}},
        {" _ ", new int[] {0,2,3,5,6,8,9}},
    };

    private readonly Dictionary<string,int[]> SegmentTwoMap = new Dictionary<string,int[]>
    {
        {"| |", new int[] {0}},
        {"  |", new int[] {1,7}},
        {" _|", new int[] {2,3}},
        {"|_ ", new int[] {5,6}},
        {"|_|", new int[] {4,8,9}},
    };

    private readonly Dictionary<string,int[]> SegmentThreeMap = new Dictionary<string,int[]>
    {
        {"  |", new int[] {1,4,7}},
        {" _|", new int[] {3,5,9}},
        {"|_ ", new int[] {2}},
        {"|_|", new int[] {0,6,8}},
    };

    private readonly Dictionary<string,int[]> SegmentMap = new Dictionary<string,int[]>
    {
        {" _ ", new int[] {0,2,3,5,6,8,9}},
        {"| |", new int[] {0}},
        {"|_|", new int[] {0,4,6,8,9}},
        {"  |", new int[] {1,4,7}},
        {" _|", new int[] {2,3,5,9}},
        {"|_ ", new int[] {2,5,6}},
        {"   ", new int[] {1,4}},
    };

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