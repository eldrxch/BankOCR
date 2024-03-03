namespace BankOCR.Core;

public class AccountNumberReader
{
    private readonly string _path;
    public AccountNumberReader(string path)
    {
        _path = path;
    }    
    public AccountNumber[] Read()
    {
        if (string.IsNullOrEmpty(_path))
        {
            throw new NotSupportedException("path cannot be null or empty");
        }
        
        try
        {
            var accountNumbers = new List<AccountNumber>();
            var contents = File.ReadAllLines(_path);
            for (int i = 0; i < contents.Length; i+=5)
            {
                string[] entry = 
                [
                    contents[i],
                    contents[i+1],
                    contents[i+2],
                    contents[i+3]
                ];
                var reader = new ScanDigitReader(entry);
                var parser = new ScanDigitParser();
                accountNumbers.Add(new AccountNumber(reader, parser));
            }
            return accountNumbers.ToArray();
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading account numbers", ex);
        }
    }
}
