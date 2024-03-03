using BankOCR.Core;

namespace BankOCR.NTest;

public class TestDigitReader
{
    static object[] ScanValidCases =
    {
        //expect start = 1 and expect end = 9
        new object[] { new string[] {
                "    _  _     _  _  _  _  _ ",
                "  | _| _||_||_ |_   ||_||_|",
                "  ||_  _|  | _||_|  ||_| _|",
                "                           "
        }, "     |  |   "," _ |_| _|   "},
        //expect start = 0 and expect end = 0
        new object[] { new string[] {
                " _  _  _  _  _  _  _  _  _ ",
                "| || || || || || || || || |",
                "|_||_||_||_||_||_||_||_||_|",
                "                           "
        }, " _ | ||_|   ", " _ | ||_|   "},
    };

    static object[] ErrorCases =
    {
        // line len not 27
        new object[] { new string[] {
                "    _  _     _  _  _  _  _  ",
                "  | _| _||_||_ |_   ||_||_| ",
                "  ||_  _|  | _||_|  ||_| _| ",
                "                            "
        }},
        // line len not 27
        new object[] { new string[] {
                " _  _     _  _  _  _  _  ",
                " _| _||_||_ |_   ||_||_| ",
                "|_  _|  | _||_|  ||_| _| ",
                "                            "
        }},        
        // entry len not 4
        new object[] { new string[] {
                " _  _  _  _  _  _  _  _  _ ",
                "| || || || || || || || || |",
                "|_||_||_||_||_||_||_||_||_|",                
        }},
    };

    [SetUp]
    public void Setup()
    {
    }

    [Test(Description = "Should return start digit")]
    [TestCaseSource(nameof(ScanValidCases))]
    public void NextFromStart_Should_Entry(string[] entries, string expectStart, string expectEnd)
    {
        var scan = new ScanDigitReader(entries);
        var start = scan.NextFromStart();
        Assert.That(string.Join("",start), Is.EqualTo(expectStart));
    }

    [Test(Description = "Should raise error")]
    [TestCaseSource(nameof(ErrorCases))]
    public void NextFromStart_Should_Error(string[] entries)
    {
        var scan = new ScanDigitReader(entries);
        Assert.Throws<NotSupportedException>(() => scan.NextFromStart());
    }

    [Test(Description = "Should return last digit")]
    [TestCaseSource(nameof(ScanValidCases))]
    public void NextFromEnd_Should_Entry(string[] entries, string _, string expectEnd)
    {
        var scan = new ScanDigitReader(entries);
        var start = scan.NextFromEnd();
        Assert.That(string.Join("",start), Is.EqualTo(expectEnd));
    }

    [Test(Description = "Should raise error")]
    [TestCaseSource(nameof(ErrorCases))]
    public void NextFromEnd_Should_Error(string[] entries)
    {
        var scan = new ScanDigitReader(entries);
        Assert.Throws<NotSupportedException>(() => scan.NextFromEnd());
    }

    [Test(Description = "Should return last digit")]
    [TestCaseSource(nameof(ScanValidCases))]
    public void Read_Should_Read(string[] entries, string expectStart, string expectEnd)
    {
        var scan = new ScanDigitReader(entries);
        var can = scan.Read();
        Assert.That(can, Is.EqualTo(true));
    }
    
    [Test(Description = "Should raise error")]
    [TestCaseSource(nameof(ErrorCases))]
    public void Read_Should_Error(string[] entries)
    {
        var scan = new ScanDigitReader(entries);
        Assert.Throws<NotSupportedException>(() => scan.Read());
    }

}