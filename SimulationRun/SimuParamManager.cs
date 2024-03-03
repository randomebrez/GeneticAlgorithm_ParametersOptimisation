using GeneticAlgorithm.ClientAccessibleObjects;
using GeneticAlgorithm.Genomic;
using GeneticAlgorithm.Genomic.Model;
using GeneticAlgorithm.SimulationRun.Model;

namespace GeneticAlgorithm.SimulationRun
{
    public class SimuParamManager
    {
        private GenomeManager _genomeManager = new();
        private GlobalParameters _globalParameters;

        public SimuParamManager(GlobalParameters parameters)
        {
            _globalParameters = parameters;
        }

        public async Task<(bool GeneticIteration, List<SimulationEnvironment> SimulationEnvironments)> SimulationEnvironmentsFromParametersAsync()
        {
            var simulationEnvironments = new List<SimulationEnvironment>();
            var needGenomes = IsGenomeNeeded();

            if (needGenomes.Required == false)
            {
                simulationEnvironments.Add(new SimulationEnvironment
                {
                    Id = 0,
                    Parameters = _globalParameters
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    var parametersToExplore = needGenomes.ParametersToExplore.Select(t => ToGeneticParameter(t));
                    var genomes = _genomeManager.RandomGenomesGet(_globalParameters.GeneticParameters.GenomeNumber, parametersToExplore.ToList());
                    simulationEnvironments = SimulationEnvironmentsFromGenomes(genomes);
                });
            }

            return (needGenomes.Required, simulationEnvironments);
        }

        public List<SimulationEnvironment> NextGenerationGet(List<SimulationEnvironment> simulationEnvironments)
        {
            // Generate next genomes generation
            simulationEnvironments = simulationEnvironments.OrderByDescending(t => t.Score)
                .Take(_globalParameters.GeneticParameters.ParentGenomeNumber).ToList();
            var nextGenomes = _genomeManager.ReproduceFromGenome(simulationEnvironments.Select(t => t.Genome).ToList(), _globalParameters.GeneticParameters.MutationRate);

            // Translate them to 
            return SimulationEnvironmentsFromGenomes(nextGenomes).ToList();
        }


        private List<SimulationEnvironment> SimulationEnvironmentsFromGenomes(List<Genome> genomes)
        {
            var simulations = new List<SimulationEnvironment>();
            for (int i = 0; i < genomes.Count; i++)
            {
                var environment = GenomeToSimulationEnvironment(genomes[i], i);
                simulations.Add(environment);
            }

            return simulations;
        }

        private (bool Required, List<SearchableParameter> ParametersToExplore) IsGenomeNeeded()
        {
            var parametersToExplore = _globalParameters.SimulationParametersList.Where(t => t.Search);
            return (parametersToExplore.Any(), parametersToExplore.ToList());
        }

        private SimulationEnvironment GenomeToSimulationEnvironment(Genome genome, int environmentId)
        {
            var translatedGenome = _genomeManager.TranslateGenome(genome);

            var distinctParameters = _globalParameters.Copy();
            foreach (var parameter in distinctParameters.SimulationParametersList)
            {
                if (parameter.Search == false)
                    continue;

                parameter.Value = translatedGenome[parameter.Name];
            }

            return new SimulationEnvironment
            {
                Id = environmentId,
                Genome = genome,
                Parameters = distinctParameters
            };
        }

        private ParameterDetails ToGeneticParameter(SearchableParameter parameterToExplore)
        {
            return new ParameterDetails
            {
                Name = parameterToExplore.Name,
                MaxValue = parameterToExplore.MaxValue,
                MinValue = parameterToExplore.MinValue,
            };
        }
    }
}
