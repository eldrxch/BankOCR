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
    
    //Digit and code matrix
    private readonly Dictionary<string, int> DigitCharCodeMap = new Dictionary<string, int> 
    {
        {"   ",1},
        {" _ ",2},
        {"| |",3},
        {"  |",4},
        {" _|",5},
        {"|_ ",6},
        {"|_|",7},
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

        if(accountNumber.Split("?").Length != 2)
        {
            throw new NotSupportedException("accountNumber should contain exactly one illegible digit");
        }   

        if(digitFaults == null || digitFaults.Count != 1)
        {
            throw new NotSupportedException("digitFaults should be a non-empty dictionary with exactly one entry");
        }

        HashSet<KeyValuePair<string, int>> matches = new();
        List<string> estimates = [];        
        foreach (var item in digitFaults)
        {
            var estDigits = EstimateDigitFromFault(item.Key, item.Value);
            matches = matches.Concat(estDigits).ToHashSet();
        }

        foreach (var match in matches)
        {
            StringBuilder accNumBuilder = new(accountNumber);
            accNumBuilder.Remove(match.Value, 1);
            accNumBuilder.Insert(match.Value,match.Key);
            
            if(accNumChecksum(accNumBuilder.ToString()) &&
                !estimates.Contains(accNumBuilder.ToString()))
            {
                estimates.Add(accNumBuilder.ToString());
            }            
        }

        return estimates.ToArray();
    }

    private HashSet<KeyValuePair<string, int>> EstimateDigitFromFault(string[] faultDigit, int index)
    {
        HashSet<KeyValuePair<string, int>> matches = [];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                StringBuilder faultPartBuilder = new(faultDigit[i]);
                var spaceKey = faultPartBuilder.Remove(j, 1).Insert(j, " ").ToString();
                var underscoreKey = faultPartBuilder.Remove(j, 1).Insert(j, "_").ToString();
                var pipeKey = faultPartBuilder.Remove(j, 1).Insert(j, "|").ToString();    
                
                var segSpace = new string[4];
                var segUnderscore = new string[4];
                var segPipe = new string[4];

                faultDigit.CopyTo(segSpace, 0);
                faultDigit.CopyTo(segUnderscore, 0);
                faultDigit.CopyTo(segPipe, 0);

                segSpace[i] = spaceKey;
                segUnderscore[i] = underscoreKey;
                segPipe[i] = pipeKey;

                if(DigitMap.ContainsKey(string.Join("", segSpace)))
                {
                    var digit = DigitMap[string.Join("", segSpace)].ToString();
                    matches.Add(new KeyValuePair<string, int>(digit, index));
                }

                if(DigitMap.ContainsKey(string.Join("", segUnderscore)))
                {
                    var digit = DigitMap[string.Join("", segUnderscore)].ToString();
                    matches.Add(new KeyValuePair<string, int>(digit, index));
                }

                if(DigitMap.ContainsKey(string.Join("", segPipe)))
                {
                    var digit = DigitMap[string.Join("", segPipe)].ToString();
                    matches.Add(new KeyValuePair<string, int>(digit, index));
                }                
            }
        }

        return matches;
    }
}