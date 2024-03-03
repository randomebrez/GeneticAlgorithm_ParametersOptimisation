using GeneticAlgorithm.ClientAccessibleObjects;
using GeneticAlgorithm.Genomic.Model;

namespace GeneticAlgorithm.SimulationRun.Model
{
    public class GlobalParameters
    {
        private Dictionary<string, SearchableParameter> _searchableDict = new Dictionary<string, SearchableParameter>();

        public int SimulationNumberByParameters { get; set; }
        public string StoragePath { get; set; }

        public GeneticParameters GeneticParameters { get; set; }

        public List<SearchableParameter> SimulationParametersList { get; set; } = new List<SearchableParameter>();

        public Dictionary<string, SearchableParameter> SimulationParametersDict()
        {
            if (_searchableDict.Count != SimulationParametersList.Count)
                _searchableDict = SimulationParametersList.ToDictionary(t => t.Name, t => t);

            return _searchableDict;
        }


        public GlobalParameters Copy()
        {
            var parametersCopy = SimulationParametersList.Select(t => t.Copy()).ToList();
            return new GlobalParameters
            {
                SimulationNumberByParameters = SimulationNumberByParameters,
                StoragePath = StoragePath,
                GeneticParameters = GeneticParameters,
                SimulationParametersList = parametersCopy
            };
        }
    }
}
