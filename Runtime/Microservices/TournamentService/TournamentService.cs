using System;
using Beamable.Server;
using System.Threading.Tasks;
using MongoDB.Driver;
using UnityEngine;
using Senet.Scripts.Models;

namespace Beamable.Microservices
{
    [Microservice("TournamentService")]
    public class TournamentService : Microservice
    {
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
        public async Task<string> GetPaidTournamentById(long userId)
        {
            try
            {
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.Eq("UserId", userId);
                var payment = await collection.Find(filter).FirstOrDefaultAsync();

                if (payment == null)
                {
                    CreateTournamentPaymentRecord(userId);
                    return "";
                }

                if (payment.PaidTournamentId == null)
                {
                    return "";
                }

                return payment.PaidTournamentId;
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
            long[] rewardAmounts = { 1, 2, 3, 3, 1, 2, 2, 1 };

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
                    PaidTournamentId = ""
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
        public async void UpdatePaidTournamentRecord(long userId, string tournamentId)
        {
            try
            {
                var db = await Storage.GetDatabase<TournamentStorage>();
                var collection = db.GetCollection<Payment>("Payments");
                var filter = Builders<Payment>.Filter.Eq("UserId", userId);
                var update = Builders<Payment>.Update.Set("PaidTournamentId", tournamentId);
                await collection.UpdateOneAsync(filter, update);
                Debug.Log("Updated paid tournament record");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating payment record: {e}");
                throw;
            }
        }

        [ClientCallable]
        public async Task ClaimTournamentRewards(string eventId)
        {
            try
            {
                await Services.Events.Claim(eventId);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
    }
}
