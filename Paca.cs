using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Paca : ShootableGameObject
{
    [SerializeField] private Collider2D HurtboxCollider;
    [SerializeField] private SpriteRenderer pacaSymbolObject;

    [SerializeField] private UnityEvent<ClassIdentifier> OnInitialisationUE;
    [SerializeField] private UnityEvent OnHurtboxTriggerUE;
    [SerializeField] private UnityEvent TriggerTerminationUE;

    private float lifetime;
    private float hurtboxTriggerRate;
    private bool onlyDealDamageForFirstFrame;

    public override void Setup(CardAttackInfoSO cardData)
    {
        base.Setup(cardData);


        CastConfigurationScriptableObject config = cardData.CastConfig;

        transform.localScale = Vector3.one * config.PacaSize;

        lifetime = config.PacaLifetime;
        hurtboxTriggerRate = config.PacaHurtboxTriggerRate;
        onlyDealDamageForFirstFrame = config.PacaOnlyDealDamageForFirstFrame;
        HurtboxCollider.enabled = true;


        if (config.PacaSymbol != null) pacaSymbolObject.sprite = config.PacaSymbol;
        else pacaSymbolObject.gameObject.SetActive(false);

        // The triggering of the hurtbox and paca lifetime are decoupled
        if (onlyDealDamageForFirstFrame) StartCoroutine(HandleSingleHurtboxTrigger());
        else StartCoroutine(HandleRepeatedHurtboxTriggers());


        OnInitialisationUE?.Invoke(cardData.MyPrimaryClass);

        // The triggering of the hurtbox and paca lifetime are decoupled
        StartCoroutine(HandleTermination());
    }

    private IEnumerator HandleSingleHurtboxTrigger()
    {
        HurtboxCollider.enabled = true;
        OnHurtboxTriggerUE?.Invoke();

        // The little wait ensures that collisions have enough time to be detected
        yield return new WaitForSeconds(0.1f);

        HurtboxCollider.enabled = false;
    }

    private IEnumerator HandleRepeatedHurtboxTriggers()
    {
        HurtboxCollider.enabled = true;
        OnHurtboxTriggerUE?.Invoke();

        // The little wait ensures that collisions have enough time to be detected
        yield return new WaitForSeconds(0.1f);

        HurtboxCollider.enabled = false;

        yield return new WaitForSeconds(hurtboxTriggerRate - 0.1f);

        StartCoroutine(HandleRepeatedHurtboxTriggers());
    }

    private IEnumerator HandleTermination()
    {
        yield return new WaitForSeconds(lifetime - 0.1f);

        if (!onlyDealDamageForFirstFrame)
        {
            HurtboxCollider.enabled = true;
            OnHurtboxTriggerUE?.Invoke();
        }

        yield return new WaitForSeconds(0.1f);

        StopAllCoroutines();

        InvokeOnTermination(this, null, SpawnPosition, Vector3.negativeInfinity);
        TriggerTerminationUE?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageMask.ContainsLayer(collision.gameObject.layer))
        {
            InvokeOnCollision(this, collision.gameObject, SpawnPosition, collision.gameObject.transform.position);
        }
    }
}
