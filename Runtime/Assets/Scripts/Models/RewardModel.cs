using System;
using Beamable.Common.Content;

namespace Senet.Scripts.Models
{
    [ContentType("reward_content")]
    public class RewardContent : ContentObject
    {
        public RewardModel rewardModel = new RewardModel();
    }

    [Serializable]
    public class RewardModel : System.Object
    {
        public long rewardAmount;
        public float rotationAngle;
    }

    [Serializable]
    public class PaymentModel : System.Object
    {
        public long paymentAmount;
		public string tournamentName;
		public long wonAmount;
		public long rank;
    }
}
