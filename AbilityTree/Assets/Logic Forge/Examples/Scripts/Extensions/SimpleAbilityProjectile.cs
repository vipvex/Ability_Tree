using UnityEngine;
using System.Collections;

/// <summary>
/// Example class that simply spawns a projectile object
/// </summary>
public class SimpleAbilityProjectile : LogicObject 
{

	public GameObject projectile;


	public string spawnPosition;
	public string spawnDirection;


	public float cooldownLength = 1;
	private float cooldownCountdown = 0;


	public AbilityManager AbilityManager;


	/// <summary>
	/// Call this function to cast the LogicObject
	/// </summary>
	public void Cast ()
	{
		if (cooldownCountdown == 0)
			OnCast();
	}


	/// <summary>
	/// On a succesfull cast
	/// </summary>
	public void OnCast ()
	{
		Debug.Log(AbilityManager);
		GameObject newProjectile = (GameObject)Instantiate(projectile, AbilityManager.GetSpawn(spawnPosition).position, AbilityManager.GetSpawn(spawnDirection).rotation);
		AbilityManager.StartCoroutine(Cooldown()); 
	}


	/// <summary>
	/// Start the cooldown coorutine to start the cooldown
	/// </summary>
	public virtual IEnumerator Cooldown ()
	{

		cooldownCountdown = cooldownLength;

		// Wait while the LogicObject is casted/channeled
		while (cooldownCountdown > 0)
		{
			cooldownCountdown -= Time.deltaTime;
			yield return null;
		}

		cooldownCountdown = 0;
		
	}

	/// <summary>
	/// Initializing the LogicObject
	/// </summary>
	/// <param name="manager"></param>
	public override void Initialize (MonoBehaviour AbilityManager)
	{
		this.AbilityManager = (AbilityManager)AbilityManager;
	}

}
