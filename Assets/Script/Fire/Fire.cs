using System.Collections;
using UnityEngine;

public enum DamageType
{
    None,
    Effective,
    Ineffective,
    VeryIneffective
}

public enum FireType
{
    Organic,
    Electric,
    Chemical,
    Gas,
    Liquid
}

public class Fire : MonoBehaviour
{
    public float hp;
    public FireType fireType;
    public bool canTakeDamage;
    public bool isHit;

    protected virtual void Start()
    {
        // Initialize HP based on fire type
        switch (fireType)
        {
            case FireType.Organic:
            case FireType.Electric:
            case FireType.Chemical:
            case FireType.Gas:
            case FireType.Liquid:
                hp = 100f;
                break;
        }
    }

    protected virtual void Update()
    {
        // Fire logic can be updated here
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        if (canTakeDamage)
        {
            float finalDamage = ModifyDamageForType(damage, damageType);
            hp -= finalDamage;
            if (hp <= 0 && !IsInvoking(nameof(DestroyFire)))
            {
                StartCoroutine(DestroyFire());
            }
        }
        isHit = true; // Mark fire as hit when interacting with particle
    }

    private float ModifyDamageForType(float damage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Effective:
                return damage * 1f;
            case DamageType.Ineffective:
                return damage * 0.5f;
            case DamageType.VeryIneffective:
                return damage * 0.25f;
            default:
                return damage;
        }
    }

    private IEnumerator DestroyFire()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds
        Destroy(gameObject);
    }

    public bool IsExtinguished()
    {
        return (canTakeDamage && hp <= 0) || (!canTakeDamage && isHit);
    }

    public void CheckDamageability(DamageType damageType)
    {
        canTakeDamage = damageType != DamageType.None;
    }
}
