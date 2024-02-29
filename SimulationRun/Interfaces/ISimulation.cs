namespace GeneticAlgorithm.SimulationRun.Interfaces
{
    public interface ISimulation
    {
        void Run();

        Dictionary<string, string> GetResult();

        double Evaluate();
    }
}
