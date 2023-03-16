namespace InnoGotchi.Application.Exceptions
{
    public class PetIsDeadException : Exception
    {
        public PetIsDeadException(string message) : base(message)
        {
        }
    }
}
