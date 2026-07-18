using LMS.Domain.Entities;

namespace LMS.Application.Common.Models;

/// <summary>Outcome of a successful authentication: the user plus freshly issued tokens.</summary>
public record AuthResult(User User, string AccessToken, string RefreshToken);

/// <summary>Outcome of a registration: an activation token the client echoes back with the emailed code.</summary>
public record RegistrationResult(string ActivationToken, string Email);
