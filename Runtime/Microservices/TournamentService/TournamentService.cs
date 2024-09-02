using Beamable.Common.Api.Auth;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using MongoDB.Driver;
using Senet.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("TournamentService")]
    public class TournamentService : Microservice
    {
        private const long gweiPerSenet = 1000000000;

        [ClientCallable]
        public async Task SetScore(string eventId, double score)
        {
            try
            {
                await Services.Events.SetScore(eventId, score);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        [ClientCallable]
        public async Task CheckOrCreatePayment(string tournamentId)
        {
            try
            {
                var userId = Context.UserId;
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.And(
                    Builders<Payment>.Filter.Eq("UserId", userId),
                    Builders<Payment>.Filter.Eq("TournamentId", tournamentId)
                );

                var payment = await collection.Find(filter).FirstOrDefaultAsync();

                if (payment == null)
                {
                    var paymentAmount = 20;
                    await AddOrRemoveSenet(-paymentAmount);
                    await collection.InsertOneAsync(new Payment()
                    {
                        UserId = userId,
                        TournamentId = tournamentId,
                        TournamentName = "",
                        PaidAmount = paymentAmount,
                        WonAmount = 0,
                    });
                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        [ClientCallable]
        public async Task<bool> HasUserParticipated()
        {
            try
            {
                var userId = Context.UserId;
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.Eq("UserId", userId);
                var payment = await collection.Find(filter).FirstOrDefaultAsync();

                if (payment == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error retrieving payment record: {e}");
                throw;
            }
        }

        [ClientCallable]
        public async Task<List<PaymentModel>> GetUserActivities()
        {
            try
            {
                var userId = Context.UserId;
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.Eq("UserId", userId);

                var payments = await collection.Find(filter).ToListAsync();

                var paymentMapping = payments.Select(s => new PaymentModel
                {
                    paymentAmount = s.PaidAmount,
                    tournamentName = s.TournamentName,
                    wonAmount = s.WonAmount,
                    rank = s.Rank,
                }).ToList();

                return paymentMapping;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error retrieving payment record: {e}");
                throw;
            }
        }


        [ClientCallable]
        public RewardModel CalculateReward(float rotationAngle)
        {
            float[] angleRanges = { 22f, 67f, 112f, 157f, 202f, 247f, 292f, 337f };
            long[] rewardAmounts = { 5, 1, 0, 1, 8, 1, 0, 3 };

            int index = FindAngleRangeIndex(rotationAngle, angleRanges);

            var rewardModel = new RewardModel
            {
                rewardAmount = 0,
                rotationAngle = 0
            };

            if (index != -1)
            {
                rewardModel.rewardAmount = rewardAmounts[index];
                rewardModel.rotationAngle = angleRanges[index + 1] - 22;
            }

            var rewardContent = new RewardContent
            {
                rewardModel = rewardModel
            };

            return rewardContent.rewardModel;
        }

        // Helper method to find the index of the angle range that contains the rotation angle
        private int FindAngleRangeIndex(float rotationAngle, float[] angleRanges)
        {
            for (int i = 0; i < angleRanges.Length - 1; i++)
            {
                if (rotationAngle >= angleRanges[i] && rotationAngle <= angleRanges[i + 1])
                {
                    return i;
                }
            }
            return -1; // If no matching range is found
        }


        private async void CreateTournamentPaymentRecord(long userId)
        {
            try
            {
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                await collection.InsertOneAsync(new Payment()
                {
                    UserId = userId,
                    TournamentId = "",
                    TournamentName = "",
                    PaidAmount = 0,
                    WonAmount = 0,
                });
                Debug.Log("Payment Record");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating payment record: {e}");
                throw;
            }
        }

        [ClientCallable]
        public async Task ClaimTournamentRewards(string eventId, string tournamentName, long amountWon, long rank)
        {
            try
            {
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.And(
                   Builders<Payment>.Filter.Eq("UserId", Context.UserId),
                   Builders<Payment>.Filter.Eq("TournamentId", eventId)
               );

                var update = Builders<Payment>.Update.Set("WonAmount", amountWon)
                                                     .Set("TournamentName", tournamentName)
                                                     .Set("Rank", rank);

                await collection.UpdateOneAsync(filter, update);

                await Services.Events.Claim(eventId);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        private async Task AddOrRemoveSenet(long amount)
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
