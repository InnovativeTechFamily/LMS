using LMS.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace LMS.Infrastructure.Persistence;

/// <summary>
/// Configures how the dependency-free domain entities map to MongoDB documents:
/// camelCase element names, ObjectId-backed string ids (including ids inherited from
/// <c>BaseEntity</c> and ids on embedded documents), and the few snake_case field names
/// carried over from the original Mongoose schema (<c>public_id</c>, <c>payment_info</c>).
/// Must run once before any collection is used.
/// </summary>
public static class MongoMappingConfig
{
    private static bool _registered;
    private static readonly object Gate = new();

    public static void Register()
    {
        if (_registered) return;
        lock (Gate)
        {
            if (_registered) return;

            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                // Order-independent: applies ObjectId representation to every string `Id` member,
                // whether it is a root id, an inherited id, or an embedded-document id.
                new StringObjectIdConvention(),
            };
            ConventionRegistry.Register("lms-conventions", pack, _ => true);

            // Only explicit renames remain: the snake_case fields kept for client/data compatibility.
            RegisterMediaLikeTypes();
            RegisterOrderPaymentInfo();

            _registered = true;
        }
    }

    private static void RegisterMediaLikeTypes()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Avatar)))
        {
            BsonClassMap.RegisterClassMap<Avatar>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(x => x.PublicId).SetElementName("public_id");
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(MediaFile)))
        {
            BsonClassMap.RegisterClassMap<MediaFile>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(x => x.PublicId).SetElementName("public_id");
            });
        }
    }

    private static void RegisterOrderPaymentInfo()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Order))) return;

        BsonClassMap.RegisterClassMap<Order>(cm =>
        {
            cm.AutoMap();
            cm.MapMember(x => x.PaymentInfo).SetElementName("payment_info");
        });
    }
}

/// <summary>
/// Represents any <c>string</c> member named <c>Id</c> as a BSON ObjectId and, when it is the
/// document's id member, assigns a driver-side ObjectId generator on insert.
/// </summary>
public sealed class StringObjectIdConvention : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        if (memberMap.MemberName != nameof(Domain.Common.BaseEntity.Id) ||
            memberMap.MemberType != typeof(string))
        {
            return;
        }

        memberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
        // Harmless on non-id members; only the designated id member's generator is invoked on insert.
        memberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
    }
}
