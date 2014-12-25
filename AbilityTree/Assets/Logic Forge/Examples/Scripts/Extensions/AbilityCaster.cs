using UnityEngine;
using System.Collections;

/// <summary>
/// Example base class for any abilities 
/// </summary>

[System.Serializable]
public class AbilityCaster : LogicObject 
{
    
    /// <summary>
    /// The time it takes to for the ability to be available again..
    /// </summary>
	public float cooldownLength = 1;

    /// <summary>
    /// The current cooldown time.
    /// </summary>
	[System.NonSerialized] public float cooldownCountdown = 0;
	
	/// <summary>
	/// The time it takes to cast the ability
	/// </summary>
	public float castLength = 0;

    /// <summary>
    /// The current cast time.
    /// </summary>
    public float castCountdown = 0;

   

	/// <summary>
	/// The amount that the ability is charged.
	/// </summary>
	public float chargeAmount = 0f;
	// [System.NonSerialized] 

	/// <summary>
	/// How switfly the ability is charged.
	/// </summary>
	public float chargeRate = 50f;
	
    /// <summary>
    /// How switfly the ability will be discharged once it reaches maximum charge.
    /// </summary>
	public float disChargeRate = 25f;
	
    /// <summary>
    /// The minimum charge amount needed to ast the ability.
    /// </summary>
	public float chargeMin = 15f;
    
    /// <summary>
    /// The max amount the ability will be able to charge.
    /// </summary>
	public float chargeMax = 100f;

    /// <summary>
    /// The behavior of the ability once it reaches the max amount of charge.
    /// </summary>
    [System.NonSerialized]
    public ChargeType chargeType;
    public enum ChargeType { ClampOnMax, DecreaseOnMax, CastOnMax }

    /// <summary>
    /// The current state of the ability.
    /// </summary>
    public State state;
    public enum State { OnCooldown, OffCooldown, Charging, Casting, Canceling }

    public AbilityManager abilityManager;
	

	/// <summary>
	/// Called when attempting to begin chargin the ability.
	/// </summary>
	public virtual void Charge ()
    {
		if (state != State.OffCooldown) return;
	
        state = State.Charging;
		abilityManager.ToggleCastingAbility(true);

        OnCharge();
	}

    /// <summary>
    /// Called when successfully began charging.
    /// </summary>
    public virtual void OnCharge()
    {

    }

    /// <summary>
    /// Run this every frame that you want the ability to charge.
    /// </summary>
    public virtual void Charging()
    {
        if (state != State.Charging) return;

        if (chargeAmount <= chargeMax)
            chargeAmount += chargeRate * Time.deltaTime;

        chargeAmount = Mathf.Clamp(chargeAmount, 0, chargeMax);

        if (chargeType == ChargeType.DecreaseOnMax)
        {
        }

        if (chargeRate == 0 || chargeType == ChargeType.CastOnMax) Cast();

    }

    /// <summary>
    /// Called to cast the ability
    /// </summary>
    public virtual void Cast()
    {
		if (state == State.Casting || state == State.OnCooldown || chargeAmount < chargeMin)
        {
            Cancel();
            return;
        }
        abilityManager.StartCoroutine(Casting());
    }
	
	/// <summary>
	/// Runs while the ability is casting.
	/// </summary>
	public virtual IEnumerator Casting ()
	{

		state = State.Casting;
		castCountdown = castLength;

		// Wait while the ability is casted/channeled
		while (castCountdown > 0.001f)
		{
		
			if (state == State.Canceling)
			{
				// Place custom cancel logic here:
				abilityManager.StartCoroutine(Cooldown()); 
				yield break;
			}

			castCountdown -= Time.deltaTime;
			
			yield return null;
		}

		castCountdown = 0;
        abilityManager.StartCoroutine(Cooldown()); 

        OnCast();
		
	}

    /// <summary>
    /// Called to cast the ability.
    /// </summary>
    public virtual void OnCast()
    {

       
    
    }

	public virtual IEnumerator Cooldown ()
	{

		state = State.OnCooldown;
		cooldownCountdown = cooldownLength;

		abilityManager.ToggleCastingAbility(false);

		// Wait while the ability is casted/channeled
		while (cooldownCountdown > 0)
		{
			cooldownCountdown -= Time.deltaTime;
			yield return null;
		}
		
		
		cooldownCountdown = 0;
		chargeAmount = 0;
		
		state = State.OffCooldown;

        OnOffCooldown();

	}

    /// <summary>
    /// Called once the ability is off cooldown
    /// </summary>
    public virtual void OnOffCooldown()
    {

    }

    /// <summary>
    /// Called to cancel the ability.
    /// </summary>
    public virtual void Cancel()
    {

        state = State.Canceling;

        chargeAmount = 0;
		castCountdown = castLength;

		abilityManager.ToggleCastingAbility(false);
   
        OnCanceled();

    }

    /// <summary>
    /// Called once the ability is successfully canceled.
    /// </summary>
    public virtual void OnCanceled()
    {

    }
	
	/// <summary>
	/// Initializing the ability
	/// </summary>
	/// <param name="manager"></param>
	public override void Initialize (MonoBehaviour abilityManager)
    {

		state = State.OffCooldown;
		this.abilityManager = (AbilityManager)abilityManager;
		
	}

}
