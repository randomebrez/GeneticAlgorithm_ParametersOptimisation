using GeneticAlgorithm.ClientAccessibleObjects;
using GeneticAlgorithm.Examples.TSP;

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
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\no_genome_config.json";
            var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\test_config.json";
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\ConfigFiles\functionnal_config.json";

            var exploManager = new GeneticExplorationManager<TSP_Simulation>();
            exploManager.ExploreSimulationAsync(configFilePath).GetAwaiter().GetResult();
        }
    }
}