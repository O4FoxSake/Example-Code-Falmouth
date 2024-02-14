using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootableGameObject : MonoBehaviour
{
    public CardAttackInfoSO CardData { get; private set; }

    protected Vector2 SpawnPosition;
    protected LayerMask damageMask;
    protected LayerMask environmentMask;

    public delegate void CollisionEvent(ShootableGameObject shotObject, GameObject hitObject, Vector2 _shootPosition, Vector2 hitPosition);

    public event CollisionEvent OnCollision;
    public event CollisionEvent OnTermination;

    protected void InvokeOnCollision(ShootableGameObject shotObject, GameObject hitObject, Vector2 _shootPosition, Vector2 hitPosition) 
        => OnCollision?.Invoke(shotObject, hitObject, _shootPosition, hitPosition);

    protected void InvokeOnTermination(ShootableGameObject shotObject, GameObject hitObject, Vector2 _shootPosition, Vector2 hitPosition) 
        => OnTermination?.Invoke(shotObject, hitObject, _shootPosition, hitPosition);

    public virtual void Setup(CardAttackInfoSO _newCardData)
    {
        CardData = _newCardData;

        damageMask = CardData.CastConfig.EnemyCollisionLayer;
        environmentMask = CardData.CastConfig.EnvironmentalCollisionLayer;
        SpawnPosition = transform.position;
    }

    public virtual void HandleImpact(GameObject impactedGO, Vector3 impactPosition) { }
}
