using GeneticAlgorithm.Examples.MinimumFunction;
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
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\no_genome.config";
            var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\test.config";
            //var configFilePath = @"D:\Codes\VisualStudio\GeneticAlgorithm\MinFunc.config";

            var exploManager = new GeneticExplorationManager();
            exploManager.ExploreSimulation(new TSP_Simulation(), configFilePath);
            //exploManager.ExploreSimulation(new MinFunc_Simulation(), configFilePath);
        }
    }
}