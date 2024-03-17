using Xunit.Abstractions;
using Xunit.Sdk;

namespace test;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestCasePriority(int value) : Attribute
{
    public int Value { get; private set; } = value;
}
public class TestCaseOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        string assemblyName = typeof(TestCasePriority).AssemblyQualifiedName!;

        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

        foreach (TTestCase testCase in testCases)
        {
            int priority = testCase.TestMethod.Method
                .GetCustomAttributes(assemblyName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>(nameof(TestCasePriority.Value)) ?? 0;

            sortedMethods.TryGetValue(priority, out var cases);

            if (cases != null)
            {
                sortedMethods[priority].Add(testCase);
            }
            else
            {
                sortedMethods[priority] = [testCase];
            };
        }

        foreach (TTestCase testCase in
            sortedMethods.Keys.SelectMany(
                priority => sortedMethods[priority]))
        {
            yield return testCase;
        }
    }
}