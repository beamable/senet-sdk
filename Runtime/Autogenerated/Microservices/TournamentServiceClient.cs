//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Beamable.Server.Clients
{
    using System;
    using Beamable.Platform.SDK;
    using Beamable.Server;
    
    
    /// <summary> A generated client for <see cref="Beamable.Microservices.TournamentService"/> </summary
    public sealed class TournamentServiceClient : MicroserviceClient, Beamable.Common.IHaveServiceName
    {
        
        public TournamentServiceClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "TournamentService";
            }
        }
        
        /// <summary>
        /// Call the SetScore method on the TournamentService microservice
        /// <see cref="Beamable.Microservices.TournamentService.SetScore"/>
        /// </summary>
        public Beamable.Common.Promise<System.Threading.Tasks.Task> SetScore(string eventId, double score)
        {
            object raw_eventId = eventId;
            object raw_score = score;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("eventId", raw_eventId);
            serializedFields.Add("score", raw_score);
            return this.Request<System.Threading.Tasks.Task>("TournamentService", "SetScore", serializedFields);
        }
        
        /// <summary>
        /// Call the GetPaidTournamentById method on the TournamentService microservice
        /// <see cref="Beamable.Microservices.TournamentService.GetPaidTournamentById"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetPaidTournamentById(long userId)
        {
            object raw_userId = userId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("userId", raw_userId);
            return this.Request<string>("TournamentService", "GetPaidTournamentById", serializedFields);
        }
        
        /// <summary>
        /// Call the CalculateReward method on the TournamentService microservice
        /// <see cref="Beamable.Microservices.TournamentService.CalculateReward"/>
        /// </summary>
        public Beamable.Common.Promise<Senet.Scripts.Models.RewardModel> CalculateReward(float rotationAngle)
        {
            object raw_rotationAngle = rotationAngle;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("rotationAngle", raw_rotationAngle);
            return this.Request<Senet.Scripts.Models.RewardModel>("TournamentService", "CalculateReward", serializedFields);
        }
        
        /// <summary>
        /// Call the UpdatePaidTournamentRecord method on the TournamentService microservice
        /// <see cref="Beamable.Microservices.TournamentService.UpdatePaidTournamentRecord"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Unit> UpdatePaidTournamentRecord(long userId, string tournamentId)
        {
            object raw_userId = userId;
            object raw_tournamentId = tournamentId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("userId", raw_userId);
            serializedFields.Add("tournamentId", raw_tournamentId);
            return this.Request<Beamable.Common.Unit>("TournamentService", "UpdatePaidTournamentRecord", serializedFields);
        }
        
        /// <summary>
        /// Call the ClaimTournamentRewards method on the TournamentService microservice
        /// <see cref="Beamable.Microservices.TournamentService.ClaimTournamentRewards"/>
        /// </summary>
        public Beamable.Common.Promise<System.Threading.Tasks.Task> ClaimTournamentRewards(string eventId)
        {
            object raw_eventId = eventId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("eventId", raw_eventId);
            return this.Request<System.Threading.Tasks.Task>("TournamentService", "ClaimTournamentRewards", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersTournamentServiceClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_Double : MicroserviceClientDataWrapper<double>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_Int64 : MicroserviceClientDataWrapper<long>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_Single : MicroserviceClientDataWrapper<float>
        {
        }
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForTournamentServiceClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<TournamentServiceClient>();
        }
        
        public static TournamentServiceClient TournamentService(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<TournamentServiceClient>();
        }
    }
}
