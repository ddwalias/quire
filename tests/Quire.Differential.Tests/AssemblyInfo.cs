using Xunit;

// Scenarios mutate runtime defaults and caches on both sides. Cross-class concurrency
// would allow one scenario to change another scenario's inputs.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
