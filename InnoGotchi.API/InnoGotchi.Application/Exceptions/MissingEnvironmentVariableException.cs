namespace InnoGotchi.Application.Exceptions
{
    public class MissingEnvironmentVariableException : Exception
    {
        public MissingEnvironmentVariableException(string variable)
            : base($"An environmet variable \"{variable}\" is missing")
        {
        }
    }
}
