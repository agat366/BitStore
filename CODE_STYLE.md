## C# Private Field Naming Convention (Including Primary Constructors)

### Overview

To ensure consistency and clarity in our codebase, **all private fields must use a leading underscore (`_`) and be in `camelCase`,** even when using C# primary constructors.

### Rationale

- The underscore prefix clearly distinguishes private fields from parameters and properties.
- This keeps the style consistent between traditional constructor-based classes and those using primary constructors.
- It aids in code readability and refactoring.

### Pattern

#### **Standard Class**
```csharp
public class DataService : IDataService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;

    public DataService(
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
    }
}
```

#### **Primary Constructor Class**
```csharp
public class DataService(
    IAuditLogRepository auditLogRepository,
    IUserRepository userRepository)
    : IDataService
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IUserRepository _userRepository = userRepository;

    // Use _auditLogRepository and _userRepository throughout the class.
}
```

### **Summary**

- **Always** assign primary constructor parameters to private fields with leading underscores.
- **Do not** use primary constructor parameters directly throughout the class if the field naming convention differs.
- Document this pattern in your code reviews and enforce with static analysis (if possible).

### **References**

- [C# Coding Conventions - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Primary Constructors - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/primary-constructors)
