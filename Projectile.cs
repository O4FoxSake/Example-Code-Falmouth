using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : ShootableGameObject
{
    [SerializeField] private UnityEvent<ClassIdentifier> OnInitialisationUE;
    [SerializeField] private UnityEvent OnTerminationUE;

    private Rigidbody2D rb;

    private int bounceCounter;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public override void Setup(CardAttackInfoSO cardData)
    {
        base.Setup(cardData);

        CastConfigurationScriptableObject config = cardData.CastConfig;

        rb.AddForce(config.AttackMoveSpeed * config.ShootSpeedMultiplicationRandomness * transform.up, ForceMode2D.Impulse);

        OnInitialisationUE?.Invoke(cardData.MyPrimaryClass);

        StartCoroutine(DelayedDestroy(config.ProjectileLifetime));
    }

    private IEnumerator DelayedDestroy(float _delayedDisableTime)
    {
        yield return new WaitForSeconds(_delayedDisableTime);
        InvokeOnTermination(this, gameObject, SpawnPosition, transform.position);
        Destroy(gameObject);
    }

    // This is called via "TelegraphTriggerAndCollision"
    public override void HandleImpact(GameObject impactedGO, Vector3 impactPosition)
    {
        if (damageMask.ContainsLayer(impactedGO.layer) || environmentMask.ContainsLayer(impactedGO.layer))
            InvokeOnCollision(this, impactedGO, SpawnPosition, impactPosition);

        if (bounceCounter == CardData.CastConfig.ProjectileNumberOfTimesToBounce || damageMask.ContainsLayer(impactedGO.layer))
        {
            InvokeOnTermination(this, impactedGO, SpawnPosition, impactPosition);

            OnTerminationUE?.Invoke();

            Destroy(gameObject);
        }

        bounceCounter++;
    }
}
