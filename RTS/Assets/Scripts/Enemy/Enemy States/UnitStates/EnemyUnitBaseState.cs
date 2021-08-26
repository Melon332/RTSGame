using Interactable;
using UnityEngine;

public abstract class EnemyUnitBaseState
{
    public abstract void EnterState(Units unit);
    public abstract void Update(Units unit);
}
