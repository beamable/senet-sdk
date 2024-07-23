using Beamable.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("ReferralStorage")]
	public class ReferralStorage : MongoStorageObject
	{
	}

    public class Referral
    {
        public ObjectId id;
        public string Code;
        public long UserId;
    }

    public class RedeemedReferral
    {
        public ObjectId id;
        public ObjectId ReferralId;
        public long UserId;
    }

    public static class ReferralStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for ReferralStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> ReferralStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<ReferralStorage>();

		/// <summary>
		/// Gets a MongoDB collection from ReferralStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="ReferralStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ReferralStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<ReferralStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from ReferralStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="ReferralStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ReferralStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<ReferralStorage, TCollection>();
	}
}
