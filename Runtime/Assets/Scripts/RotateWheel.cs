using System;
using Beamable;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;

public class RotateWheel : MonoBehaviour
{
    public float StopPower;
    public GameObject ConfirmationPanel;
    private long _amount;
    public TextMeshProUGUI TMP_Amount;
    private Rigidbody2D rbody;
    int inRotate;

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    float t;
    private void Update()
    {
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += 1 * Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();

                inRotate = 0;
                t = 0;
            }
        }
    }

    public void Rotate()
    {
        if (inRotate == 0)
        {
            PlayerPrefs.SetString("LastDateSpun", DateTime.Now.Ticks.ToString());
            rbody.AddTorque(UnityEngine.Random.Range(10000f, 20000f));
            inRotate = 1;
        }
    }

    private async void GetReward()
    {
        var _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;

        float rot = transform.eulerAngles.z;
        var rewardData = await _beamContext.Microservices().TournamentService().CalculateReward(rot);
        
        long rewardAmount = rewardData.rewardAmount;
        float newRotationAngle = rewardData.rotationAngle;
        
        RewardHelper(new Vector3(0, 0, newRotationAngle), rewardAmount);
    }

    public async void ClaimReward()
    {
        await CurrencyManager.Instance.AddOrRemoveSenet(+_amount);
        ConfirmationPanel.SetActive(false);
    }

    private void RewardHelper(Vector3 vector3, long amount)
    {
        GetComponent<RectTransform>().eulerAngles = vector3;
        TMP_Amount.text = amount.ToString();
        ConfirmationPanel.SetActive(true);
        _amount = amount;
    }
}
