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
                try
                {
                    if (count >= _accLen) break;
                    var startDigits = _reader.NextFromStart();
                    _parser.Entry(startDigits);
                    value[sPos] = _parser.Parse().ToString();
                }
                catch(KeyNotFoundException ex)
                {
                    value[sPos] = IllegibleCode;
                }

                sPos++;
                count++;

                try
                {
                    if (count >= _accLen) break;
                    var endDigits = _reader.NextFromEnd();
                    _parser.Entry(endDigits);
                    value[ePos-1] = _parser.Parse().ToString();
                }
                catch (System.Exception)
                {                    
                     value[ePos-1] = IllegibleCode;
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

    public bool IsValid()
    {
        if(_value == null || IsIllegible()) return false;

        int cbase = 0, fact = 9, count = 9;         
        for (int i = 0; i < count; i++)
        {
            int num = int.Parse(_value[i].ToString());
            cbase += num * fact;
            fact--;
        }
        return cbase % 11 == 0;
    }

    public bool IsIllegible()
    {
        return Value().Contains(IllegibleCode);
    }

    public override string ToString()
    {
        return Value().ToString();
    }
}
