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

    //Code and digit matrix
    private readonly Dictionary<int, Dictionary<string, int>> NumberDigitCharCodeMap = new Dictionary<int, Dictionary<string, int>>
    {
        {0, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"| |", 3},
                {"|_|", 7},
            }
        },
        {1, new Dictionary<string, int>
            {
                {"   ", 1},
                {"  |", 4},
                {"  |", 4},
            }
        },
        {2, new Dictionary<string, int>
            {
                {" _ ", 2},
                {" _|", 5},
                {"|_ ", 6},
            }
        },
        {3, new Dictionary<string, int>
            {
                {" _ ", 2},
                {" _|", 5},
                {" _|", 5},
            }
        },
        {4, new Dictionary<string, int>
            {
                {"   ", 1},
                {"|_|", 7},
                {"  |", 4},
            }
        },
        {5, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"|_ ", 6},
                {" _|", 5},
            }
        },
        {6, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"|_ ", 6},
                {"|_|", 7},
            }
        },
        {7, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"  |", 4},
                {"  |", 4},
            }
        },
        {8, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"|_|", 7},
                {"|_|", 7},
            }
        },
        {9, new Dictionary<string, int>
            {
                {" _ ", 2},
                {"|_|", 7},
                {" _|", 5},
            }
        },
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

        if(digitFaults == null || digitFaults.Count == 0)
        {
            throw new NotSupportedException("digitFaults cannot be null");
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
        if (faultDigit.Length != 4)
        {
            return estimates.ToArray();
        }

        var segLook = FindFaultySegment(faultDigit);
        var faultSegment = segLook.Item1;
        var segIndex = segLook.Item2;

        if(segIndex < 0) return estimates.ToArray();
        
        var faultPart =  faultDigit[segIndex];
        HashSet<string> matches = [];

        for (int i = 0; i < 3; i++)
        {            
            StringBuilder faultPartBuilder = new(faultPart);
            var spaceKey = faultPartBuilder.Remove(i, 1).Insert(i, " ").ToString();
            var underscoreKey = faultPartBuilder.Remove(i, 1).Insert(i, "_").ToString();
            var pipeKey = faultPartBuilder.Remove(i, 1).Insert(i, "|").ToString();    

            if(faultSegment.ContainsKey(spaceKey))
            {
                matches.Add(spaceKey);
            }

            if(faultSegment.ContainsKey(underscoreKey))
            {
                matches.Add(underscoreKey);
            }

            if(faultSegment.ContainsKey(pipeKey))
            {
                matches.Add(pipeKey);
            }
        }

        StringBuilder accNumBuilder = new(accountNumber);

        foreach (var match in matches)
        {
            faultDigit[segIndex] = match;
            var digitKey = string.Join("", faultDigit);
            accNumBuilder.Remove(index, 1);
            accNumBuilder.Insert(index, DigitMap[digitKey].ToString());
            
            if(accNumChecksum(accNumBuilder.ToString()))
            {
                estimates.Add(accNumBuilder.ToString());
            }
        }

        return estimates.ToArray();
    }

    public Tuple<Dictionary<string, bool>, int> FindFaultySegment(string[] faultDigit)
    {
        var faultSegment = new Dictionary<string,bool>();        
        var faultCount = 0;
        var segIndex = -1;

        if(!SegmentOneMap.ContainsKey(faultDigit[0]))
        {
            faultSegment = SegmentOneMap;
            faultCount++;
            segIndex = 0;
        }
        else if(!SegmentTwoMap.ContainsKey(faultDigit[1]))
        {
            faultSegment = SegmentTwoMap;
            faultCount++;
            segIndex = 1;
        }
        else if(!SegmentThreeMap.ContainsKey(faultDigit[2]))
        {
            faultSegment = SegmentThreeMap;
            faultCount++;
            segIndex = 2;
        }

        if(faultCount > 1) return new Tuple<Dictionary<string, bool>, int>(new Dictionary<string, bool>(), -1);
        return new Tuple<Dictionary<string, bool>, int>(faultSegment, segIndex);
    }
}