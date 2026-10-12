namespace DeliveryProject.Exceptions
{
    public class SamePasswordException : Exception
    {
        public SamePasswordException(string message) : base(message) { }
    }
}
