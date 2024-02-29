namespace GeneticAlgorithm.SimulationRun.Parameters_DTO
{
    public class SearchableParameter
    {
        public SearchableParameter Copy()
        {
            return new SearchableParameter
            {
                Name = Name,
                Value = Value,
                Search = Search,
                BitNumber = BitNumber,
                MaxValue = MaxValue,
                MinValue = MinValue
            };
        }
        public string Name { get; set; }
        public double Value { get; set; }

        public bool Search { get; set; }

        public int BitNumber { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
