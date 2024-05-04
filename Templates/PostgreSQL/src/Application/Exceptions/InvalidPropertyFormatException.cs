namespace Application.Exceptions;

[Serializable]
public class InvalidPropertyFormatException(string message) : Exception(message);