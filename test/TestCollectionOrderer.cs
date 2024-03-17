using Xunit.Abstractions;

namespace test;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]

public class TestCollectionPriority(int value) : Attribute
{
    public int Value = value;
}

public class CollectionOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(
        IEnumerable<ITestCollection> testCollections)
    {
        string assemblyName = typeof(TestCollectionPriority).AssemblyQualifiedName!;
        var sortedClasses = new SortedDictionary<int, ITestCollection>();
        foreach (ITestCollection testCollection in testCollections)
        {
            int priority = testCollection.TestAssembly.Assembly.GetCustomAttributes(assemblyName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>(nameof(TestCasePriority.Value)) ?? 0;

            sortedClasses[priority] = testCollection;
        }

        foreach (ITestCollection testCollection in
            sortedClasses.Keys.Select(
                priority => sortedClasses[priority]).Cast<ITestCollection>())
        {
            yield return testCollection;
        }
    }
}