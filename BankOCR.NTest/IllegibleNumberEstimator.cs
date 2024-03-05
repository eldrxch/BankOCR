using BankOCR.Core;
using System.Text;

namespace BankOCR.NTest;

public class TestIllegibleNumberEstimator
{
    private bool AccNumChecksum(string accNum)
    {
        return accNum.Length == 9;
    }

    static object[] FaultyDigits =
    {
        new object[] { new string[] {
                " * ",
                "  |",
                "  |",
                "   "
        }, 0, 1 },
        new object[] { new string[] {
                " _ ",
                " _*",
                "|_ ",
                "   "
        }, 1, 2 },
        new object[] { new string[] {
                " _ ",
                "  |",
                " _|",
                "   "
        }, 2, 3 },
        new object[] { new string[] {
                "   ",
                "|_|",
                "  _",
                "   "
        }, 3, 4 },
        new object[] { new string[] {
                " _ ",
                "|_ ",
                " _ ",
                "   "
        }, 4, 5 },
        new object[] { new string[] {
                " _ ",
                "|_ ",
                "| |",
                "   "
        }, 5, 6 },
        new object[] { new string[] {
                " _ ",
                "  |",
                "   ",
                "   "
        }, 6, 7 },
        new object[] { new string[] {
                "   ",
                "|_|",
                "|_|",
                "   "
        }, 7, 8 },
        new object[] { new string[] {
                "   ",
                "|_|",
                " _|",
                "   "
        }, 8, 9 }
    };

    [Test(Description = "Should return array of estimated account numbers")]
    [TestCaseSource(nameof(FaultyDigits))]
    public void Estimate_Should_Values(string[] faultyDigit, int index,  int expect)
    {
        var accNum = new StringBuilder("123456789");
        accNum.Remove(index, 1);
        accNum.Insert(index, "?");

        var fd = new Dictionary<string[], int> { { faultyDigit, index } };
        var estimator = new IllegibleNumberEstimator();
        var result = estimator.Estimate(accNum.ToString(), AccNumChecksum, fd);
        Assert.That(result.Length, Is.GreaterThan(0));
        Assert.That(result, Has.Member(accNum.ToString().Replace("?", expect.ToString())));
    }

    [Test(Description = "Should return empty array")]
    public void Estimate_Should_NoValues()
    {
        var faultyDigit = new string[] {
                " _ ",
                "| |",
                "  |",
                "   "
        };
        var fd = new Dictionary<string[], int> { { faultyDigit, 0}};
        var estimator = new IllegibleNumberEstimator();
        var result = estimator.Estimate("?000", AccNumChecksum, fd);
        Assert.That(result.Length, Is.EqualTo(0));
    }

    [Test(Description = "Should throw exception")]
    public void Estimate_Should_Error()
    {
        var estimator = new IllegibleNumberEstimator();
        var digitFaults = new Dictionary<string[], int>
        {
                { new string[] {
                        " _ ",
                        "| |",
                        "  |",
                        "   "
                }, 0 },
                { new string[] {
                        " _ ",
                        "  |",
                        "|_|",
                        "   "
                }, 1 }
        };
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("", AccNumChecksum, null));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate(null, AccNumChecksum, null));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("123456789", null, null));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("??3456789", null, digitFaults));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("123456789", null, digitFaults));
    }
}