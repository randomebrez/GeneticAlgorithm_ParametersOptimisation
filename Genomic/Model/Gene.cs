namespace GeneticAlgorithm.Genomic.Model
{
    public class Gene
    {
        private int[] _bitValues;

        public ParameterDetails EncodedParameter { get; }
        public string ParameterName => EncodedParameter.Name;

        public Gene(ParameterDetails encodedParameter, int bitNumber)
        {
            EncodedParameter = encodedParameter;
            _bitValues = new int[bitNumber];
        }

        public Gene(Gene gene)
        {
            EncodedParameter = gene.EncodedParameter;
            _bitValues = new int[gene._bitValues.Length];

            for (int i = 0; i < _bitValues.Length; i++)
                _bitValues[i] = gene._bitValues[i];
        }


        public double ValueGet()
        {
            var result = 0d;
            for (int i = 0; i < _bitValues.Length; i++)
                result += _bitValues[i] * Math.Pow(2, i);

            return result / (Math.Pow(2, _bitValues.Length) - 1);
        }

        public double LinearValueGet()
        {
            return (EncodedParameter.MaxValue - EncodedParameter.MinValue) * ValueGet() + EncodedParameter.MinValue;
        }

        public void RandomInitialization()
        {
            for (int i = 0; i < _bitValues.Length; i++)
            {
                var randomNumber = new Random().Next(10000) / 10000f;
                if (randomNumber < 0.5f)
                    _bitValues[i] = 1;
                else
                    _bitValues[i] = 0;
            }
        }

        public void Mutate()
        {
            var randomNumber = new Random().NextDouble();
            var mutationOccured = false;
            var bitToMutate = 0;

            while (mutationOccured == false)
            {
                var threshold = (float)(bitToMutate + 1) / _bitValues.Length;
                if (randomNumber < threshold)
                {
                    _bitValues[bitToMutate] = 1 - _bitValues[bitToMutate];
                    mutationOccured = true;
                }
                else
                    bitToMutate++;
            }
        }
    }
}
