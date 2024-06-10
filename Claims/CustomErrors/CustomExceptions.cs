namespace Claims.CustomErrors;

public class BadRequestError(string message) : Exception(message);
public class NotFoundError(string message) : Exception(message);

public class InvalidDateError(string message) : Exception(message);