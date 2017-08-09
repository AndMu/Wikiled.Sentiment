namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class SvmResult
    {
        public const int Threshold = 2;

        public double? SvmDistance { get; set; }

        public bool? IsPositive
        {
            get
            {
                if (!SvmDistance.HasValue)
                {
                    return null;
                }

                return SvmDistance.Value > 0;
            }
        }

        public double? Probability
        {
            get
            {
                if (!SvmDistance.HasValue)
                {
                    return null;
                }

                if (SvmDistance.Value > Threshold)
                {
                    return 1;
                }

                if (SvmDistance.Value < -Threshold)
                {
                    return 0;
                }

                return (Threshold + SvmDistance.Value) / (2 * Threshold);
            }
        }

        public double? Positivity
        {
            get
            {
                if (!SvmDistance.HasValue)
                {
                    return null;
                }

                var value = 2 * (Probability.Value - 0.5);
                return value;
            }
        }
    }
}
