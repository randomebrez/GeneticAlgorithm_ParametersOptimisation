namespace GeneticAlgorithm.SimulationRun.Interfaces
{
    public interface ISimulation
    {
        Task RunAsync();

        Task<Dictionary<string, string>> GetResultAsync();

        Task<double> EvaluateAsync();
    }
}
