using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PowerReactor : Buildings
{
    [SerializeField] private int powerProduced;

    private void AddPowerToPlayer()
    {
        PlayerManager.Instance.AmountOfPowerPlayerHas += powerProduced;
        PlayerManager.Instance.CheckIfPowerIsSufficient(costOfPower,false);
        UIManager.Instance.PlayerPowerText();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayerManager.Instance != null && hasFinishedBuilding)
        {
            PlayerManager.Instance.AmountOfPowerPlayerHas -= powerProduced;
            PlayerManager.Instance.CheckIfPowerIsSufficient(0,false);
            UIManager.Instance.PlayerPowerText();
        }
    }

    public override void OnBuildingComplete()
    {
        base.OnBuildingComplete();
        AddPowerToPlayer();
    }
}
