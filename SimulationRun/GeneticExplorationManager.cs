using GeneticAlgorithm.SimulationRun.Interfaces;
using GeneticAlgorithm.SimulationRun.Parameters_DTO;
using Newtonsoft.Json;
using static GeneticAlgorithm.Genomic.GeneticAlgorithmTest;

namespace GeneticAlgorithm.SimulationRun
{
    public class GeneticExplorationManager
    {
        private GlobalParameters _globalParameters;
        private SimulationParameters_Manager _parametersManager;
        private ISimulationBuilder _simulationBuilder;


        public void ExploreSimulation(ISimulationBuilder simulationBuilder, string configFilePath)
        {
            _simulationBuilder = simulationBuilder;

            // Map config file
            _globalParameters = ReadConfigFile(configFilePath);

            // Initialize ParameterManager
            _parametersManager = new SimulationParameters_Manager(_globalParameters);

            // Build configuration file with exploratory parameters' value set. If no genome is required return a list of a single element with null genome
            var (geneticIteration, currentSimulationEnvironments) = _parametersManager.SimulationEnvironmentsFromParameters();

            // If no genetic iteration required, execute a single environment then return
            if (geneticIteration == false)
            {
                ExecuteSimulationEnvironment(currentSimulationEnvironments.First(), _globalParameters.StoragePath);
                return;
            }

            // Otherwise loop until max generation
            for (int generationId = 0; generationId < _globalParameters.GeneticParameters.GenerationNumber; generationId++)
            {
                // Run all simulation for all different config files (genomes)
                var scoredGenomes = ExecuteALLSimulationEnvironments(currentSimulationEnvironments, generationId);
                var average = 0d;
                foreach (var scoredGenome in scoredGenomes)
                    average += scoredGenome.Score;

                average /= scoredGenomes.Count;
                Console.WriteLine($"Iteration : {generationId} - Score {Math.Round(average, 4)}");

                // Get next generation
                currentSimulationEnvironments = _parametersManager.NextGenerationGet(scoredGenomes);
            }
        }

        private List<ScoredGenome> ExecuteALLSimulationEnvironments(List<SimulationEnvironment> simulationEnvironments, int generation)
        {
            var scoredGenomes = new List<ScoredGenome>();

            // Create directory to store results of that generation
            var sub_directory = Path.Combine(simulationEnvironments[0].Parameters.StoragePath, $"GeneticIteration_{generation}");
            if (Directory.Exists(sub_directory) == false)
                Directory.CreateDirectory(sub_directory);

            // For each different config file built from genomes
            for (int j = 0; j < simulationEnvironments.Count; j++)
            {
                var scoredGenome = ExecuteSimulationEnvironment(simulationEnvironments[j], sub_directory);

                scoredGenomes.Add(scoredGenome);
            }

            return scoredGenomes;
        }

        private ScoredGenome ExecuteSimulationEnvironment(SimulationEnvironment simulationEnvironment, string subDirectory = "")
        {
            // Class that handle multiple simulations with a single config file
            var simuManager = new Simulation_Runner(_simulationBuilder, simulationEnvironment.Parameters);

            // Run the simulations
            simuManager.RunSimulations();

            // Store results if subDirectory parameter is filled
            if (string.IsNullOrEmpty(subDirectory) == false)
            {
                // Add subfolder to store results of simulation with that set of parameters
                simulationEnvironment.Parameters.StoragePath = Path.Combine(subDirectory, $"ParametersConfig_{simulationEnvironment.Id}");
                // Store results
                simuManager.StoreSimulationResults();
            }

            // Evaluate parameters
            var score = simuManager.EvaluateParameters();

            return new ScoredGenome
            {
                Genome = simulationEnvironment.Genome,
                Score = score
            };
        }


        private GlobalParameters ReadConfigFile(string configFilePath)
        {
            var jsonFile = File.ReadAllText(configFilePath);

            return JsonConvert.DeserializeObject<GlobalParameters>(jsonFile);
        }

        private void PrintStuff()
        {

        }
    }
}
