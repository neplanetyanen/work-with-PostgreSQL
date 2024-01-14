namespace Core;

public class Exceptions : Exception
{
    public override string Message { get; }

    protected Exceptions(string message)
    {
        Message = message;
    }
}
public class IncorrectIdException : Exceptions
{
    public IncorrectIdException(string sender) : base($"ERROR: Incorrect {sender} ID") { }
}

public class NonExistentEntityException : Exceptions
{
    public NonExistentEntityException(string sender) : base($"ERROR: {sender} not found!") { }
}

public class FailedUpdateDbException : Exceptions
{
    public FailedUpdateDbException() : base("ERROR: Failed to update the database") {}
}