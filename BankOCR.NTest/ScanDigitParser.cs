using BankOCR.Core;

namespace BankOCR.NTest;

public class TestScanDigitParser
{
    static object[] ParseValidCases =
    {
        new object[] { new string[] {
                " _ ",
                "| |",
                "|_|",
                "   "
        }, 0 },
        new object[] { new string[] {
                "   ",
                "  |",
                "  |",
                "   "
        }, 1 },
        new object[] { new string[] {
                " _ ",
                " _|",
                "|_ ",
                "   "
        }, 2 },
        new object[] { new string[] {
                " _ ",
                " _|",
                " _|",
                "   "
        }, 3 },
        new object[] { new string[] {
                "   ",
                "|_|",
                "  |",
                "   "
        }, 4 },
        new object[] { new string[] {
                " _ ",
                "|_ ",
                " _|",
                "   "
        }, 5 },
        new object[] { new string[] {
                " _ ",
                "|_ ",
                "|_|",
                "   "
        }, 6 },
        new object[] { new string[] {
                " _ ",
                "  |",
                "  |",
                "   "
        }, 7 },
        new object[] { new string[] {
                " _ ",
                "|_|",
                "|_|",
                "   "
        }, 8 },
        new object[] { new string[] {
                " _ ",
                "|_|",
                " _|",
                "   "
        }, 9 }
    };

    static object[] ErrorCases =
    {
        new object[] { new string[] {
                " _ ",
                "|  ",
                "|_ ",
                "   "                
        } },
        new object[] { new string[] {
                " _ ",
                "| |",
                " _|",
                "   "
         } }
    };

    [SetUp]
    public void Setup()
    {
    }

    [Test(Description = "Should return a digit from a string")]
    [TestCaseSource(nameof(ParseValidCases))]
    public void Parse_Should_Digit(string[] entry, int expect)
    {
        var parser = new ScanDigitParser();
        parser.Entry(entry);
        var value = parser.Parse();
        TestContext.WriteLine($"Expected: {expect} Actual: {value}");
        Assert.That(value, Is.EqualTo(expect));
    }

    [Test(Description = "Should throw an error when the entry is invalid")]
    [TestCaseSource(nameof(ErrorCases))]
    public void Parse_Should_Error(string[] entry)
    {
        var parser = new ScanDigitParser();
        parser.Entry(entry);
        Assert.Throws<KeyNotFoundException>(() => parser.Parse());
    }
}