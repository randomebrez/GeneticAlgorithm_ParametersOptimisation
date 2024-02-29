namespace GeneticAlgorithm.Genomic.Model
{
    public enum DataTypeEnum
    {
        Integer,
        Float
    }

    public class ParameterDetails
    {
        public string Name { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public DataTypeEnum ValueType { get; set; }
    }
}
