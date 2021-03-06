﻿using UnityEngine;
using System.Collections;

public class DestructibleBlocController : MonoBehaviour, IDefendable
{

	public int life = 2;
	public Transform explodePrefab;

	Animator animator;

	void Awake ()
	{
		animator = GetComponent<Animator> ();
	}

	public void Defend (GameObject attacker, int damage, Vector2 bumpVelocity, float bumpTime)
	//public void Hit (float damage)
	{
		animator.SetTrigger ("hit");
		life -= damage;
		if (life <= 0) {
			Transform explosion = (Transform)Instantiate (explodePrefab, this.gameObject.transform.position, Quaternion.identity);
			Destroy (this.gameObject);
			Destroy (explosion.gameObject, 0.5f);
		}
	}
}
