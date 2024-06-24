using Beamable.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("TournamentStorage")]
	public class TournamentStorage : MongoStorageObject
	{
	}
	
	public class Payment
	{
		public ObjectId id;
		public long UserId;
		public string PaidTournamentId;
	}

	public static class TournamentStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for TournamentStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> TournamentStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<TournamentStorage>();

		/// <summary>
		/// Gets a MongoDB collection from TournamentStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="TournamentStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> TournamentStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<TournamentStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from TournamentStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="TournamentStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> TournamentStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<TournamentStorage, TCollection>();
	}
}
