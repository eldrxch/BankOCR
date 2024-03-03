namespace BankOCR.Core;

public class AccountNumberWriter
{
    private readonly string _path;
    public AccountNumberWriter(string path)
    {
        _path = path;
    }    
    public long Write(AccountNumber[] numbers)
    {
        if (string.IsNullOrEmpty(_path))
        {
            throw new NotSupportedException("path cannot be null or empty");
        }
        
        try
        {
            var accountNumbers = new List<AccountNumber>();
            var contents = File.OpenWrite(_path);
            
            foreach (var number in numbers)
            {
                var accNum = number.Value();
                var note = string.Empty;
                
                if(number.IsIllegible()) {
                    note = "ILL";
                }
                else if(!number.IsValid()) {
                    note = "ERR";                 
                }

                contents.Write(System.Text.Encoding.UTF8.GetBytes($"{accNum} {note}\n"));    
            }
            
            long count = contents.Length;
            contents.Close();
            return count;
        }
        catch (Exception ex)
        {
            throw new Exception("Error writing account numbers", ex);
        }
    }
}
