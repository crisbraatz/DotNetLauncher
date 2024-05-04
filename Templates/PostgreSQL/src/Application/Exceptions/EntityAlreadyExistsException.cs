namespace Application.Exceptions;

[Serializable]
public class EntityAlreadyExistsException(string message) : Exception(message);