using Interactable;
using UnityEngine;

public abstract class EnemyUnitBaseState
{
    public abstract void EnterState(Units entity);
    public abstract void Update(Units entity);
}
