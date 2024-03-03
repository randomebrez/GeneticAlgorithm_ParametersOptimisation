namespace GeneticAlgorithm.ClientAccessibleObjects
{
    public interface ISimulation
    {
        Task InitializeAsync(Dictionary<string, SearchableParameter> searchableParameters);

        Task RunAsync();

        Task<Dictionary<string, string>> GetResultAsync();

        Task<double> EvaluateAsync();
    }
}
