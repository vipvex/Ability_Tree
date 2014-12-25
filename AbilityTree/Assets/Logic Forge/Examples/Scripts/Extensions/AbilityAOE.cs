using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogicObjectAOE : AbilityCaster 
{


	public float maxCastDistance = 100;

	public int manaCost = 0;


    /// <summary>
    /// The projectile that will be spawned.
    /// </summary>
    public GameObject aoeProjectile;

    /// <summary>
    /// The spawn position of the projectile.
    /// </summary>
    public string aoeSpawnPos;

    /// <summary>
    /// The the position that the projectile will face.
    /// </summary>
    public string aoeSpawnDir;

    /// <summary>
    /// The number of projectiles that will be spawned.
    /// </summary>
    public int aoeCount;

    /// <summary>
    /// The delay between spawning multiple projectiles.
    /// </summary>
    public float projectileDelay = 0.0f;

    /// <summary>
    /// The offset to the position that the projectile will fly towards. Evaluated based on charge amount.
    /// </summary>
    public AnimationCurve aoeSpread = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));


    /// <summary>
    /// The effects that will be spawned once this LogicObject is selected.
    /// </summary>
    public List<EffectObj> selectEffects;

    /// <summary>
    /// An effect that spawns when casting or charging.
    /// </summary>
    public List<EffectObj> chargeEffects;

    /// <summary>
    /// The effect that will spawn when the LogicObject is casted successfuly.
    /// </summary>
    public List<EffectObj> castEffects;


    /// <summary>
    /// Used to enable and disable the effect we use.
    /// </summary>
    protected GameObject[] selectEffectObjs;

    protected GameObject[] chargeEffectObjs;



    private Vector3 spawnPos;


    public EffectFrequency castEffectFrequency;
    public enum EffectFrequency { Once, EveryProjecile }


    public override void OnSelect(bool value)
    {
        ToggleGameObjects(selectEffectObjs, value);
    }


    public override void OnCharge()
    {
        ToggleGameObjects(chargeEffectObjs, true);
    }

    public override void OnCanceled()
    {
        ToggleGameObjects(chargeEffectObjs, false);
    }

    /// <summary>
    /// Lets ovveride the cast function so that it will check if the selected AOE area is 
    /// </summary>
    public override void Cast()
    {
        if (state == State.Casting || state == State.OnCooldown || chargeAmount < chargeMin)
        {
            return;
        }
        spawnPos = abilityManager.GetSpawn(aoeSpawnPos).position;
        abilityManager.StartCoroutine(Casting());
        ToggleGameObjects(selectEffectObjs, false);
    }

    public override void OnCast()
    {
        abilityManager.StartCoroutine(SpawnAOEs());
        abilityManager.characterScript.SendMessage("UpdateMana", manaCost);   
    }

    // Spawn the projectiles when the LogicObject has been casted
    public IEnumerator SpawnAOEs()
    {
        for (int p = 0; p < aoeCount; p++)
        {

            if (castEffects.Count > 0 && castEffectFrequency == EffectFrequency.Once && p == 0)
                SpawnEffects(castEffects.ToArray(), true);

            if (castEffects.Count > 0 && castEffectFrequency == EffectFrequency.EveryProjecile)
                SpawnEffects(castEffects.ToArray(), true);


            // Calculate bullet spread based on charge amount
            Vector3 lookPosition = abilityManager.GetSpawn(aoeSpawnDir).position - abilityManager.GetSpawn(aoeSpawnPos).position;
            //lookPosition += Random.insideUnitSphere * aoeSpread.Evaluate(1 - chargeAmount / (chargeMax + 0.001f));

            // Get the rotation that would be looking at lookPosition
            Quaternion lookRotation = Quaternion.LookRotation(lookPosition);


            GameObject projectile = (GameObject)Instantiate(aoeProjectile,
                                                            spawnPos,
                                                            lookRotation);

            // You may want to change or modefy components the projectile once it spawns
            // Do so here:

            yield return new WaitForSeconds(projectileDelay);

        }

        ToggleGameObjects(chargeEffectObjs, false);

    }

    void ToggleGameObjects(GameObject[] gameObjects, bool value)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj)
                obj.SetActive(value);
        }
    }

    public override void Initialize(MonoBehaviour abilityManager)
    {

        this.abilityManager = (AbilityManager)abilityManager;
        chargeEffectObjs = SpawnEffects(chargeEffects.ToArray(), true);
        selectEffectObjs = SpawnEffects(selectEffects.ToArray(), true);


        ToggleGameObjects(chargeEffectObjs, false);
        ToggleGameObjects(selectEffectObjs, false);

    }

    public override void Terminate()
    {
        foreach (GameObject oldObj in chargeEffectObjs)
            if (oldObj) Destroy(oldObj);

        foreach (GameObject oldObj in selectEffectObjs)
            if (oldObj) Destroy(oldObj);
    }


    GameObject[] SpawnEffects(EffectObj[] effects, bool parentToSpawn)
    {
        List<GameObject> effectObjects = new List<GameObject>();
        foreach (EffectObj effect in effects)
        {
            Transform spawnTransform = abilityManager.GetSpawn(effect.spawnName);
            if (spawnTransform == null || effect.prefab == null)
                return null;

            GameObject newEffect = (GameObject)Instantiate(effect.prefab, spawnTransform.position, spawnTransform.rotation);
            effectObjects.Add(newEffect);
            if (parentToSpawn)
                newEffect.transform.parent = spawnTransform;
        }
        return effectObjects.ToArray();
    }
}
