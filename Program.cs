using GeneticAlgorithm.Examples.TSP;
using GeneticAlgorithm.SimulationRun;

namespace GeneticAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Test Genetic Algorithm
            //var test_GA = new GeneticAlgorithmTest();
            //test_GA.TestGeneticAlgorithm();

            // Test TSP
            var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\no_genome_config.json";
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\test_config.json";
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\functionnal_config.json";

            var exploManager = new GeneticExplorationManager();
            exploManager.ExploreSimulationAsync(new TSP_Simulation(), configFilePath).GetAwaiter().GetResult();
        }
    }
}