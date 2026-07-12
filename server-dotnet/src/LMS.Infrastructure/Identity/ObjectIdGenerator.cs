using LMS.Application.Common.Interfaces.Services;
using MongoDB.Bson;

namespace LMS.Infrastructure.Identity;

/// <summary>Produces 24-character hex ObjectId strings for embedded documents.</summary>
public class ObjectIdGenerator : IIdGenerator
{
    public string NewId() => ObjectId.GenerateNewId().ToString();
}
