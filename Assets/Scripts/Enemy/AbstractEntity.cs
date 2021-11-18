using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SpeedController))]
public abstract class AbstractEntity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected SpeedController speedController;
    [SerializeField] protected GameObject player;
    [SerializeField] protected Cover[] avaliableCovers;
    public LayerMask SolidGround;
    public LayerMask SolidWall;
    public LayerMask PlayerLayer;

    [Header("Health and armor")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float health;
    [SerializeField] protected float lowHealthThreshold;
    [SerializeField] protected float criticalLowHealthThreshold;
    [SerializeField] protected float healthRestoreRate;
    [SerializeField] protected float dieAwaitTime;
    [Range(0, 100)]
    [SerializeField] protected int armour;
    [SerializeField] protected int damage;
    [SerializeField] protected bool blocking;
    [SerializeField] protected bool immortal = false;

    [Header("Patroling")]
    public float restSpeed;
    public float walkSpeed;
    public float acceleration;
    public float walkPointRange;
    public float maxRestTime;
    public float minRestTime;
    [Range(10, 180)]
    public float turnMaxAngle;
    public float rotateAcceleration;

    [Header("Chasing")]
    public float runSpeed;
    public float obstacleRange;
    public float accelerationChaseBonus;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    public float minTimeAttackStartDelay;
    public float maxTimeAttackStartDelay;

    [Header("Senses")]
    public float sightRange;
    [Range(10, 180)]
    public float sightConeRange;
    public float hearRange;
    public float attackRange;

    [Space]
    [SerializeField] protected Node decisionTreeTopNode;
    [SerializeField] protected Vector3 currentDestination;

    protected virtual void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        speedController = GetComponent<SpeedController>();
        avaliableCovers = FindObjectsOfType<Cover>();
        SetPlayer();
    }

    /*>>> General methods <<<*/
    public bool IsDead()
    {
        if (health == 0.0f)
        {
            return true;
        }
        return false;
    }

    public void RunCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }


    /*>>> Getters <<<*/
    public NavMeshAgent GetNavMeshAgent()
    {
        return navAgent;
    }

    public Transform GetNavMeshAgentTransform()
    {
        return navAgent.transform;
    }

    public SpeedController GetSpeedController()
    {
        return speedController;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public Vector3 GetCurrentDestination()
    {
        return currentDestination;
    }


    /*>>> Setters <<<*/
    public void SetNavAgentDestination(Vector3 destination)
    {
        navAgent.SetDestination(destination);
    }

    public void SetPlayer()
    {
        player = FindObjectsOfType<PlayerMovement>()[0].transform.GetChild(0).gameObject;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void ChangeHealth(float value)
    {
        health += value;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0.0f)
        {
            health = 0.0f;
            Die();
        }
    }

    public void ChangeArmour(int change)
    {
        armour += change;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetBlock(bool blocking)
    {
        this.blocking = blocking;
    }

    public void SetCurrentDestination(Vector3 currentDestination)
    {
        this.currentDestination = currentDestination;
    }

    /*>>> ABSTRACT <<<*/
    protected abstract void ConstructBehaviourTree();
    public abstract void Die();
    public abstract void GetHit(float damage);
    public abstract void Attack();
}
