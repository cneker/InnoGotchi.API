namespace InnoGotchi.Application.Exceptions
{
    public class IncorrectRequestException : Exception
    {
        public IncorrectRequestException(string message) : base(message)
        {
        }
    }
}
