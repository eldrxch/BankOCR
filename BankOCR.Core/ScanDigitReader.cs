using System.Text;

namespace BankOCR.Core;

public class ScanDigitReader : IDigitReader
{
    private readonly string[] _digits;
    private readonly int _maxLineLen;
    private readonly int _xLen;
    private readonly int _yLine1;
    private readonly int _yLine2;
    private readonly int _yLine3;
    private readonly int _yLine4;
    private int _readStartCount;
    private int _readEndCount;

    private Exception DigitsException => new NotSupportedException(
                "digits must be an array of length 4 and each entry must be of length 27");            

    public ScanDigitReader(string[] digits)
    {
        this._digits = digits;
        _maxLineLen = 27;
        _xLen = 3;
        _yLine1 = 0;
        _yLine2 = 1;
        _yLine3 = 2;
        _yLine4 = 3;
        _readStartCount = 0;
        _readEndCount = 27;
    }

    private bool isDigitsValid()
    {
        if (_digits == null || _digits.Length != 4)
        {
            return false;
        }

        if (_digits[_yLine1].Length != _maxLineLen ||
            _digits[_yLine2].Length != _maxLineLen ||
            _digits[_yLine3].Length != _maxLineLen ||
            _digits[_yLine4].Length != _maxLineLen)
        {
            return false;
        }
        return true;
    }

    public string[] NextFromStart()
    {
        if (!isDigitsValid())
        {
            throw DigitsException;
        }
        
        string[] segments =
        [
            _digits[_yLine1].Substring(_readStartCount, _xLen),
            _digits[_yLine2].Substring(_readStartCount, _xLen),
            _digits[_yLine3].Substring(_readStartCount, _xLen),
            _digits[_yLine4].Substring(_readStartCount, _xLen),
        ];
        _readStartCount += _xLen;
        return segments;
    }

    public string[] NextFromEnd()
    {
        if (!isDigitsValid())
        {
            throw DigitsException;
        }

        string[] segments =
        [
            _digits[_yLine1].Substring(_readEndCount-_xLen, _xLen),
            _digits[_yLine2].Substring(_readEndCount-_xLen, _xLen),
            _digits[_yLine3].Substring(_readEndCount-_xLen, _xLen),
            _digits[_yLine4].Substring(_readEndCount-_xLen, _xLen),
        ];
        _readEndCount -= _xLen;
        return segments;
    }

    public bool Read()
    {
        if (!isDigitsValid())
        {
            throw DigitsException;
        }

        if (_readStartCount >= _readEndCount ||
            _readEndCount <= _readStartCount)
        {
            return false;
        }
        return true;
    }
}