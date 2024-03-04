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

    private readonly Dictionary<string,bool> SegmentOneMap = new Dictionary<string,bool>
    {
        {"   ",true},
        {" _ ",true},
    };

    private readonly Dictionary<string,bool> SegmentTwoMap = new Dictionary<string,bool>
    {
        {"| |",true},
        {"  |",true},
        {" _|",true},
        {"|_ ",true},
        {"|_|",true},
    };

    private readonly Dictionary<string,bool> SegmentThreeMap = new Dictionary<string,bool>
    {
        {"  |", true},
        {" _|", true},
        {"|_ ", true},
        {"|_|", true},
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

        if(digitFaults == null || digitFaults.Count != 4)
        {
            throw new NotSupportedException("digitFaults should be an array of 4 elements");
        }

        string[] estimates = [];
        string accNum = accountNumber;
        foreach (var item in digitFaults)
        {
            var ests = estimate(item.Key, item.Value, accNum, accNumChecksum);
            estimates = estimates.Concat(ests).ToArray();
        }
        return estimates;        
    }

    private string[] estimate(string[] faultDigit, int index, string accountNumber, Func<string, bool> accNumChecksum)
    {
        List<string> estimates = [];
        StringBuilder accNumBuilder = new(accountNumber);
        var matches = EstimateDigitFromFault(faultDigit);
        
        foreach (var match in matches)
        {
            accNumBuilder.Remove(index, 1);
            accNumBuilder.Insert(index,match);
            
            if(accNumChecksum(accNumBuilder.ToString()))
            {
                estimates.Add(accNumBuilder.ToString());
            }
        }

        return estimates.ToArray();
    }

    private HashSet<string> EstimateDigitFromFault(string[] faultDigit)
    {
        HashSet<string> matches = [];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                StringBuilder faultPartBuilder = new(faultDigit[i]);
                var spaceKey = faultPartBuilder.Remove(j, 1).Insert(j, " ").ToString();
                var underscoreKey = faultPartBuilder.Remove(j, 1).Insert(j, "_").ToString();
                var pipeKey = faultPartBuilder.Remove(j, 1).Insert(j, "|").ToString();    
                
                var segSpace = faultDigit;
                var segUnderscore = faultDigit;
                var segPipe = faultDigit;

                segSpace[i] = spaceKey;
                segUnderscore[i] = underscoreKey;
                segPipe[i] = pipeKey;

                if(DigitMap.ContainsKey(string.Join("", segSpace)))
                {
                    matches.Add(string.Join("", segSpace));
                }

                if(DigitMap.ContainsKey(string.Join("", segUnderscore)))
                {
                    matches.Add(string.Join("", segUnderscore));
                }

                if(DigitMap.ContainsKey(string.Join("", segPipe)))
                {
                    matches.Add(string.Join("", segPipe));
                }                
            }
        }

        return matches;
    }
}