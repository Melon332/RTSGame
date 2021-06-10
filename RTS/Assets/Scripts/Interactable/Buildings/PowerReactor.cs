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

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.AmountOfPowerPlayerHas -= powerProduced;
            UIManager.Instance.PlayerPowerText();
        }
    }
}
