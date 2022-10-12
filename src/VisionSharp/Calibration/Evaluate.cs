namespace VisionSharp.Calibration
{
    public struct Evaluate
    {
        public double AverageError;

        public override string ToString()
        {
            return $"Average Error:\t{AverageError:F4}";
        }
    }
}