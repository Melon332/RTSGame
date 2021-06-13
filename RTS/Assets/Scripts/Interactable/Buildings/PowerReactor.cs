using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PowerReactor : Buildings
{
    [SerializeField] private int powerProduced;

    public void AddPowerToPlayer()
    {
        PlayerManager.Instance.AmountOfPowerPlayerHas += powerProduced;
        UIManager.Instance.PlayerPowerText();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayerManager.Instance != null && hasFinishedBuilding)
        {
            PlayerManager.Instance.AmountOfPowerPlayerHas -= powerProduced;
            UIManager.Instance.PlayerPowerText();
        }
    }

    public override void OnBuildingComplete()
    {
        base.OnBuildingComplete();
        AddPowerToPlayer();
    }
}
