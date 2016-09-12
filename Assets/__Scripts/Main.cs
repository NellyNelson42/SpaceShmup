﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;
	public float enemySpawnPadding = 1.5f;
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[]{
		WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
	};

	public bool ______________;

	WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;

	void Awake(){
		S = this;
		Utils.SetCameraBounds (this.GetComponent<Camera>());
		enemySpawnRate = 1f / enemySpawnPerSecond;
		Invoke ("SpawnEnemy", enemySpawnRate);

		W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
		foreach( WeaponDefinition def in weaponDefinitions ) {
			W_DEFS[def.type] = def;
		}
	}
	static public WeaponDefinition GetWeaponDefinition( WeaponType wt ) {
		if (W_DEFS.ContainsKey(wt)) {
			return( W_DEFS[wt]);
		}
		return( new WeaponDefinition() );
	}


	void Start(){
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i = 0; i < weaponDefinitions.Length; i++) {
			activeWeaponTypes [i] = weaponDefinitions [i].type;
		}
	}

	public void SpawnEnemy(){
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		Invoke ("SpawnEnemy", enemySpawnRate);
	}
	public void DelayedRestart(float delay){
		Invoke ("Restart", delay);
	}
	public void Restart(){
		Application.LoadLevel ("_Scene_0");
	}
	public void ShipDestroyed( Enemy e ) {
		// Potentially generate a PowerUp
		if (Random.value <= e.powerUpDropChance) {
			// Random.value generates a value between 0 & 1 (though never == 1)
			// If the e.powerUpDropChance is 0.50f, a PowerUp will be generated
			//   50% of the time. For testing, it's now set to 1f.

			// Choose which PowerUp to pick
			// Pick one from the possibilities in powerUpFrequency
			int ndx = Random.Range(0,powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency[ndx];

			// Spawn a PowerUp
			GameObject go = Instantiate( prefabPowerUp ) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp>();
			// Set it to the proper WeaponType
			pu.SetType( puType );

			// Set it to the position of the destroyed ship
			pu.transform.position = e.transform.position;
		}
	}
}