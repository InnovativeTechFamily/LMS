namespace LMS.Application.Common.Interfaces.Services;

/// <summary>
/// Generates ids for embedded documents (course lectures, questions, replies, reviews) that
/// MongoDB does not auto-generate. Matches the ObjectId ids Mongoose assigns to sub-documents.
/// </summary>
public interface IIdGenerator
{
    string NewId();
}
