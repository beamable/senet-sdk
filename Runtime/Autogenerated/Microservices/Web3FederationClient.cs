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
    
    
    /// <summary> A generated client for <see cref="Beamable.Web3Federation.Web3Federation"/> </summary
    public sealed class Web3FederationClient : MicroserviceClient, Beamable.Common.IHaveServiceName, Beamable.Common.ISupportsFederatedLogin<Web3FederationCommon.SuiIdentity>, Beamable.Common.ISupportsFederatedInventory<Web3FederationCommon.SuiIdentity>
    {
        
        public Web3FederationClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "Web3Federation";
            }
        }
        
        /// <summary>
        /// Call the GetRealmAccount method on the Web3Federation microservice
        /// <see cref="Beamable.Web3Federation.Web3Federation.GetRealmAccount"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetRealmAccount()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<string>("Web3Federation", "GetRealmAccount", serializedFields);
        }
        
        /// <summary>
        /// Call the GenerateRealmAccount method on the Web3Federation microservice
        /// <see cref="Beamable.Web3Federation.Web3Federation.GenerateRealmAccount"/>
        /// </summary>
        public Beamable.Common.Promise<string> GenerateRealmAccount()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<string>("Web3Federation", "GenerateRealmAccount", serializedFields);
        }
        
        /// <summary>
        /// Call the ImportRealmAccount method on the Web3Federation microservice
        /// <see cref="Beamable.Web3Federation.Web3Federation.ImportRealmAccount"/>
        /// </summary>
        public Beamable.Common.Promise<string> ImportRealmAccount(string privateKey)
        {
            object raw_privateKey = privateKey;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("privateKey", raw_privateKey);
            return this.Request<string>("Web3Federation", "ImportRealmAccount", serializedFields);
        }
        
        /// <summary>
        /// Call the GetSuiEnvironment method on the Web3Federation microservice
        /// <see cref="Beamable.Web3Federation.Web3Federation.GetSuiEnvironment"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetSuiEnvironment()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<string>("Web3Federation", "GetSuiEnvironment", serializedFields);
        }
        
        /// <summary>
        /// Call the Withdrawal method on the Web3Federation microservice
        /// <see cref="Beamable.Web3Federation.Web3Federation.Withdrawal"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.FederatedInventoryProxyState> Withdrawal(string transaction, string toAddress, string contentId, long amount)
        {
            object raw_transaction = transaction;
            object raw_toAddress = toAddress;
            object raw_contentId = contentId;
            object raw_amount = amount;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("transaction", raw_transaction);
            serializedFields.Add("toAddress", raw_toAddress);
            serializedFields.Add("contentId", raw_contentId);
            serializedFields.Add("amount", raw_amount);
            return this.Request<Beamable.Common.FederatedInventoryProxyState>("Web3Federation", "Withdrawal", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersWeb3FederationClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_Int64 : MicroserviceClientDataWrapper<long>
        {
        }
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForWeb3FederationClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<Web3FederationClient>();
        }
        
        public static Web3FederationClient Web3Federation(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<Web3FederationClient>();
        }
    }
}
