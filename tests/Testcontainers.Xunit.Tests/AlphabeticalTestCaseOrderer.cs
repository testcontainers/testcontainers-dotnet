namespace Testcontainers.Xunit.Tests;

public class AlphabeticalTestCaseOrderer : ITestCaseOrderer
{
#if XUNIT_V3
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : notnull, ITestCase
    {
        return testCases.OrderBy(testCase => testCase.TestMethod?.MethodName).ToImmutableList();
    }
#else
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
    }
#endif
}