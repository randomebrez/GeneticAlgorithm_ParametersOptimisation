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
        private List<ScoredGenome> _scoredGenomes = new List<ScoredGenome>();


        public async Task ExploreSimulationAsync(ISimulationBuilder simulationBuilder, string configFilePath)
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
                await ExecuteSimulationEnvironmentAsync(currentSimulationEnvironments.First(), _globalParameters.StoragePath);
                return;
            }

            // Otherwise loop until max generation
            for (int generationId = 0; generationId < _globalParameters.GeneticParameters.GenerationNumber; generationId++)
            {
                // Run all simulation for all different config files (genomes)
                await ExecuteALLSimulationEnvironmentsAsync(currentSimulationEnvironments, generationId);
                var average = 0d;
                foreach (var scoredGenome in _scoredGenomes)
                    average += scoredGenome.Score;

                average /= _scoredGenomes.Count;
                Console.WriteLine($"Iteration : {generationId} - Score {Math.Round(average, 4)}");

                // Get next generation
                currentSimulationEnvironments = _parametersManager.NextGenerationGet(_scoredGenomes);

                _scoredGenomes.Clear();
            }
        }

        private async Task ExecuteALLSimulationEnvironmentsAsync(List<SimulationEnvironment> simulationEnvironments, int generation)
        {
            await Task.Run(async () =>
            {
                // Create directory to store results of that generation
                var sub_directory = Path.Combine(simulationEnvironments[0].Parameters.StoragePath, $"GeneticIteration_{generation}");
                if (Directory.Exists(sub_directory) == false)
                    Directory.CreateDirectory(sub_directory);

                // For each different config file built from genomes
                List<Task> simulationResults = new List<Task>();
                for (int j = 0; j < simulationEnvironments.Count; j++)
                    simulationResults.Add(ExecuteSimulationEnvironmentAsync(simulationEnvironments[j], sub_directory));

                await Task.WhenAll(simulationResults);
            });
        }

        private async Task ExecuteSimulationEnvironmentAsync(SimulationEnvironment simulationEnvironment, string subDirectory = "")
        {
            Console.WriteLine($"Executin simuEnvironment {simulationEnvironment.Id}");
            // Class that handle multiple simulations with a single config file
            var simuManager = new Simulation_Runner(_simulationBuilder, simulationEnvironment.Parameters);

            // Run the simulations
            await simuManager.RunSimulationsAsync();

            // Store results if subDirectory parameter is filled
            if (string.IsNullOrEmpty(subDirectory) == false)
            {
                // Add subfolder to store results of simulation with that set of parameters
                simulationEnvironment.Parameters.StoragePath = Path.Combine(subDirectory, $"ParametersConfig_{simulationEnvironment.Id}");
                // Store results
                await simuManager.StoreSimulationResultsAsync();
            }

            // Evaluate parameters
            var score = await simuManager.EvaluateParametersAsync();

            _scoredGenomes.Add(new ScoredGenome
            {
                Genome = simulationEnvironment.Genome,
                Score = score
            });
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
