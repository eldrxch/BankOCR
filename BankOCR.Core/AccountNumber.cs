namespace BankOCR.Core;
using System;
using System.Text;

public class AccountNumber
{
    private const string IllegibleCode = "?";
    private IDigitReader _reader;
    private IDigitParser _parser;    
    private readonly int _accLen = 9;
    private string? _value;
    private Dictionary<string[], int> _errorDigits = new();

    public AccountNumber(IDigitReader reader, IDigitParser parser)
    {
        _reader = reader;
        _parser = parser;        
        _value = null;
    }

    public string Value()
    {
        try
        {
            if (_value != null) return _value;

            var value = new string[_accLen];
            int sPos = 0, count=0, ePos = _accLen;

            while (_reader.Read())
            {
                string[]digits = null;

                try
                {
                    if (count >= _accLen) break;
                    digits = _reader.NextFromStart();
                    _parser.Entry(digits);
                    value[sPos] = _parser.Parse().ToString();
                }
                catch(KeyNotFoundException)
                {
                    value[sPos] = IllegibleCode;
                    _errorDigits.Add(digits, sPos);
                }

                sPos++;
                count++;

                try
                {
                    if (count >= _accLen) break;
                    digits = _reader.NextFromEnd();
                    _parser.Entry(digits);
                    value[ePos-1] = _parser.Parse().ToString();
                }
                catch (KeyNotFoundException)
                {                    
                     value[ePos-1] = IllegibleCode;
                    _errorDigits.Add(digits, ePos-1);
                }

                ePos -= 1;
                count++;
            }
            _value = string.Join("", value);
            return _value;
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading account number", ex);
        }
    }

    public string[] ValueEstimates(IAccountNumberEstimator estimator)
    {
        if (_value == null) return [];
        return estimator.Estimate(_value, Validate, _errorDigits);
    }

    public bool IsValid()
    {
        if(_value == null || IsIllegible()) return false;
        return Validate(_value);
    }

    public bool Validate(string number) 
    {
        int cbase = 0, fact = 9, count = 9;         
        for (int i = 0; i < count; i++)
        {
            int num = int.Parse(number[i].ToString());
            cbase += num * fact;
            fact--;
        }
        return cbase % 11 == 0;

    }

    public bool IsIllegible()
    {
        return Value().Contains(IllegibleCode);
    }
}
