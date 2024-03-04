using BankOCR.Core;

namespace BankOCR.NTest;

public class TestIllegibleNumberEstimator
{
    private bool AccNumChecksum(string accNum)
    {
        return accNum.Length == 9;
    }

    [Test(Description = "Should return array of estimated account numbers")]
    public void Estimate_Should_Values()
    {
        var estimates = new string[]
        {
            "723456789",
            "123956783",
            "123956785",
            "123956788",
        };
        var estimator = new InvalidNumberEstimator();
        var result = estimator.Estimate("123456789", AccNumChecksum, null);
        Assert.That(result.Length, Is.GreaterThan(0));
        Assert.That(result.Any(x => estimates.Contains(x)), Is.True);
    }

    [Test(Description = "Should return empty array")]
    public void Estimate_Should_NoValues()
    {
        var estimator = new InvalidNumberEstimator();
        var result = estimator.Estimate("000", AccNumChecksum, null);
        Assert.That(result.Length, Is.EqualTo(0));
    }

    [Test(Description = "Should throw exception")]
    public void Estimate_Should_Error()
    {
        var estimator = new InvalidNumberEstimator();
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("", AccNumChecksum, null));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate(null, AccNumChecksum, null));
        Assert.Throws<NotSupportedException>(() => estimator.Estimate("123456789", null, null));
    }
}