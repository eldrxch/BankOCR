namespace BankOCR.Core;
public interface IDigitReader
{
    bool Read();
    string[] NextFromStart();
    string[] NextFromEnd();
}
