﻿using System.Collections;
using UnityEngine;

public class EntitySpellController : SpellController
{
    private Mage entity;
    private float maxMana;
    private float manaRestoreRate;

    private EntityMagicShield currentShield;
    private EntityHealSpell currentHealSpell;


    /*>>> Unity methods <<<*/
    protected override void Start()
    {
        entity = GetComponent<Mage>();
        maxMana = entity.maxMana;
        manaRestoreRate = entity.manaRestoreRate;

        uiPanelType = entity.GetUIPanelType();
        uiPanelController = Managers.UI.SetupUIPanelController(this.gameObject, uiPanelType);
        uiPanelController.SetupMana(maxMana, maxMana);

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    /*>>> Abstract methods <<<*/
    public override void ExecuteSpell()
    {
        if (canAttack)
        {
            StartCoroutine(AttackDesorientation(entity.minTimeAttackStartDelay, entity.maxTimeAttackStartDelay));
            canAttack = false;
            StartCoroutine(ResetAttack(entity.timeBetweenAttacks));
        }
        else
        {
            LogMessage("Nie możesz teraz atakować!");
            entity.AddRLReward(entity.GetMageRLParameters().tryShootWhenNoCooldown);
        }
    }

    protected override void SetElementUIBarValue(float value) { }

    protected override void LogMessage(string msg)
    {
        Debug.Log(msg);
    }


    /*>>> Spell casting <<<*/
    protected override void SetupCastballSpellInfo(EffectSettings effectSettings, SpellInfo spellInfo, CastSpellNode spellNode)
    {
        effectSettings.LayerMask =~ entity.OwnLayer;
        spellInfo.castSpellNode = spellNode;
        spellInfo.SetupSpellInfoOwner(entity);
    }

    protected override void RunCastSpellAnimation(float time, Transform castSpell)
    {
        entity.GetAnimationRiggingController().ThrowCastSpell(time, castSpell);
        if(entity.IsAttacking() || entity.IsChasing())
        {
            entity.AddRLReward(entity.GetMageRLParameters().useSpellWhenAttackOrChase);
        }
    }

    protected override void NoManaRLReward()
    {
        entity.AddRLReward(entity.GetMageRLParameters().tryShootWhenNoMana);
    }

    protected override void NoSpellRlReward()
    {
        entity.AddRLReward(entity.GetMageRLParameters().tryShootWhenNoSpell);
    }


    /*>>> Shield casting <<<*/
    protected override void SetupShieldObject(ShieldSpellNode shieldSpellNode)
    {
        if (currentShield == null)
        {
            currentShield = gameObject.AddComponent<EntityMagicShield>() as EntityMagicShield;
            currentShield.SetupShield(shieldSpellNode.time, entity.maxShieldTime, shieldSpellNode, shieldParent, entity, uiPanelController);
            entity.GetAnimationController().PlayBlockAnimation();
            if(entity.IsHidding())
            {
                entity.AddRLReward(entity.GetMageRLParameters().useShieldWhenHide);
            }
        }
        else
        {
            ShieldSpellNode currentShieldSpellNode = currentShield.GetShieldSpellNode();
            if (currentShieldSpellNode.spell == shieldSpellNode.spell)
            {
                ChargeShieldSpell(shieldSpellNode, currentShield);
                entity.GetAnimationController().PlayBlockAnimation(); 
            }
            else
            {
                LogMessage(gameObject.name + " nie może użyć " + shieldSpellNode.name + " podczas używania " + currentShieldSpellNode.name + "!");
            }
        }
    }


    /*>>> Heal casting <<<*/
    protected override void SetupHealObject(HealSpellNode healSpellNode)
    {
        if (currentHealSpell == null)
        {
            currentHealSpell = gameObject.AddComponent<EntityHealSpell>() as EntityHealSpell;
            currentHealSpell.SetupShield(healSpellNode.time, entity.maxHealTime, healSpellNode, shieldParent, entity, uiPanelController);
            entity.GetAnimationController().SetHealAnimation(true);

            if (entity.IsHealthLow())
            {
                entity.AddRLReward(entity.GetMageRLParameters().healWhenHealthUserLow);
            }
        }
        else
        {
            ChargeHealSpell(healSpellNode, currentHealSpell);
        }
    }


    /*>>> Area spell casting <<<*/
    protected override void LoadOwnerOfExplosion(AreaExplosionBullet owner)
    {
        owner.LoadEntity(entity);
    }

    protected override void AreaExplosionAdditionalConfiguration()
    {
        if (entity.IsHealthCriticalLow())
        {
            entity.AddRLReward(entity.GetMageRLParameters().areaSpellWhenHealthUnderCriticalLow);
        }
    }


    /*>>> Getters <<<*/
    protected override float GetMaxMana()
    {
        return entity.maxMana;
    }

    protected override float GetManaRestoreRate()
    {
        return entity.manaRestoreRate;
    }

    public EntityMagicShield GetCurrentShield()
    {
        return currentShield;
    }

    public EntityHealSpell GetCurrentHealSpell()
    {
        return currentHealSpell;
    }

    private IEnumerator AttackDesorientation(float minTimeDelay, float maxTimeDelay)
    {
        float DesorientationTime = UnityEngine.Random.Range(minTimeDelay, maxTimeDelay);
        yield return new WaitForSeconds(DesorientationTime);
        try
        {
            UseSpellType();
        }
        catch (UnsetSpellException err)
        {
            Debug.LogError(err);
        }
    }
}