namespace BankOCR.Core;

public class ScanDigitParser : IDigitParser
{
    private const int MaxStringLen = 3;
    private const int MaxEntryLen = 4;
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

    private string[]? _entry;

    private bool isEntryValid(string[] entry){
        if(entry == null || entry.Length != MaxEntryLen)
        {
            return false;
        }
        
        foreach (var e in entry)
        {
            if(e.Length != MaxStringLen)
            {
                return false;
            }
        }
        return true;
    }

    public int Parse(string[] digitEntry)
    {
        if(!isEntryValid(digitEntry))
        {
            throw new NotSupportedException(
                 "entry must be an array of length 4 and each entry must be of length 3"
            );
        }

        var key = string.Join("", digitEntry);        
        if(!DigitMap.TryGetValue(key, out int value))
        {
            throw new KeyNotFoundException($"The entry cannot be parsed");
        }
        return value;
    }
}