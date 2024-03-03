namespace BankOCR.Core;
public interface IDigitParser
{
    void Entry(string[] entry);
    int Parse();
}