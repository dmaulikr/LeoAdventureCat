﻿using UnityEngine;

public interface ISentinel
{
	GameObject CheckLoS ();
}

public interface IAttackable
{
	void Attack ();
}

public interface IDefendable
{
	void Defend (GameObject attacker, int damage, Vector2 bumpVelocity, float bumpTime);
}

public interface IPatroller
{
	void Patrol ();
}