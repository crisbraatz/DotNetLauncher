namespace Application.Exceptions;

[Serializable]
public class InvalidPropertyValueException(string message) : Exception(message);