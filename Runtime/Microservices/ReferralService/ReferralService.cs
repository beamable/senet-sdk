using Beamable.Common.Api.Inventory;
using Beamable.Server;
using MongoDB.Driver;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("ReferralService")]
    public class ReferralService : Microservice
    {
        private const long gweiPerSenet = 1000000000;

        [ClientCallable]
        public async Task<string> CreateReferral()
        {
            try
            {
                var referrerUserId = Context.UserId;

                var referralFilter = Builders<Referral>.Filter.Eq("UserId", referrerUserId);
                var db = await Storage.GetDatabase<ReferralStorage>();

                var collection = db.GetCollection<Referral>("Referrals");
                var referral = await collection.Find(referralFilter).FirstOrDefaultAsync();

                if (referral != null)
                {
                    System.Console.WriteLine($"Referral code exists for user: {referrerUserId}");
                    return "";
                }

                var code = GenerateReferralCode();

                await collection.InsertOneAsync(new Referral()
                {
                    Code = code,
                    UserId = referrerUserId
                });

                return code;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating referral: {e}");
                throw;
            }
        }

        [ClientCallable]
        public async Task ClaimReferralReward(string referralCode)
        {
            try
            {
                var userId = Context.UserId;
                var db = await Storage.GetDatabase<ReferralStorage>();
                var referralCollection = db.GetCollection<Referral>("Referrals");
                var redeemedReferralCollection = db.GetCollection<RedeemedReferral>("RedeemedReferrals");

                var referralFilter = Builders<Referral>.Filter.Eq("Code", referralCode);
                var referral = await referralCollection.Find(referralFilter).FirstOrDefaultAsync();

                if (referral == null)
                {
                    System.Console.WriteLine("Referral Code Doesn't Exist");
                    return;
                }

                if (referral.UserId == userId)
                {
                    System.Console.WriteLine("You Can't Redeem Your Own Referral");
                    return;
                }

                var filter = Builders<RedeemedReferral>.Filter.And(
                    Builders<RedeemedReferral>.Filter.Eq("UserId", userId),
                    Builders<RedeemedReferral>.Filter.Eq("ReferralId", referral.id)
                );

                var redeemedReferral = await redeemedReferralCollection.Find(filter).FirstOrDefaultAsync();

                if (redeemedReferral != null)
                {
                    Debug.Log("Referral Code Already Redeemed");
                    return;
                }

                await AddOrRemoveSenet(50, Services);

                var referrer = AssumeNewUser(referral.UserId, null, false);
                await AddOrRemoveSenet(120, referrer.Services);

                await redeemedReferralCollection.InsertOneAsync(new RedeemedReferral()
                {
                    UserId = userId,
                    ReferralId = referral.id
                });

                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error rewarding: {e}");
                throw;
            }
        }

        private string GenerateReferralCode()
        {
            var random = new System.Random();
            var code = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                code.Append(random.Next(0, 10));
            }

            for (int i = 0; i < 3; i++)
            {
                code.Append((char)random.Next('A', 'Z' + 1));
            }

            return code.ToString();
        }

        private async Task AddOrRemoveSenet(long amount, IBeamableServices Services)
        {
            try
            {
                var inventoryUpdateBuilder = new InventoryUpdateBuilder();
                inventoryUpdateBuilder.CurrencyChange("currency.senet_currency.senet_token", amount * gweiPerSenet);

                await Services.Inventory.Update(inventoryUpdateBuilder);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error Updating Tokens {e}");
            }
        }
    }
}
