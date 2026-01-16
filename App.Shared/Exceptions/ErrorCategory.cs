namespace App.Shared.Exceptions;

public enum ErrorCategory
{
    Validation,
    Business,
    System
}

/**
Validation = “Is the request well-formed?”
Business = “Is the request allowed by the domain rules?”
System = “Did the system fail to process a valid request?”


Can I detect this before business logic?
        |
       YES
        |
   VALIDATION
        |
       NO
        |
Does it violate a domain rule?
        |
       YES
        |
    BUSINESS

Does it violate a domain rule?
        |
       NO
        |
Is it unexpected or technical?
        |
       YES
        |
     SYSTEM

**/


