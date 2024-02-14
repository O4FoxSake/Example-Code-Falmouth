using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphTriggerAndCollision : MonoBehaviour
{
    [SerializeField] private ShootableGameObject telegraphTarget;

    [SerializeField] private TelegraphMode telegraphMode;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (telegraphMode == TelegraphMode.Trigger) return;

        telegraphTarget.HandleImpact(collision.gameObject, collision.GetContact(0).point);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (telegraphMode == TelegraphMode.Collision) return;

        telegraphTarget.HandleImpact(collision.gameObject, collision.gameObject.transform.position);
    }

    public enum TelegraphMode
    {
        Both,
        Collision,
        Trigger,
    }
}
