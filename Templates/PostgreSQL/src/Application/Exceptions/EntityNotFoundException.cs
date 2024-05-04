namespace Application.Exceptions;

[Serializable]
public class EntityNotFoundException(string message) : Exception(message);