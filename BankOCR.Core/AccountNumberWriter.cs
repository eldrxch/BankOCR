using System.Text;
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
            var illEstimator = new IllegibleNumberEstimator();
            var invEstimator = new InvalidNumberEstimator();
            var file = File.OpenWrite(_path);
            
            foreach (var number in numbers)
            {
                var accNum = number.Value();
                var note = string.Empty;

                if(number.IsValid())
                {
                    file.Write(Encoding.UTF8.GetBytes($"{accNum}\n"));
                    continue;
                }

                note = "ERR";
                if(number.IsIllegible()) {
                    note = "ILL";
                }

                var estimates = number.IsIllegible() ? 
                    number.ValueEstimates(illEstimator) : 
                    number.ValueEstimates(invEstimator);
                switch (estimates.Length)
                {
                    case 0:
                        file.Write(Encoding.UTF8.GetBytes($"{accNum} ILL\n"));
                        break;
                    case 1:
                        file.Write(Encoding.UTF8.GetBytes($"{estimates[0]}\n"));
                        break;
                    default:
                        file.Write(Encoding.UTF8.GetBytes($"{accNum} AMB [{string.Join(", ", estimates)}]\n"));
                        break;
                }
            }
            
            long count = file.Length;
            file.Close();
            return count;
        }
        catch (Exception ex)
        {
            throw new Exception("Error writing account numbers", ex);
        }
    }
}
