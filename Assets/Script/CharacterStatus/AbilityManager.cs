using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

// PlayerStatus�ɓ����\�͊Ǘ��N���X
// �����i(ItemData)�����W���A����ɔ����^�O���W�v���A�Ή�����\��(TagEffect)��L�������A
// �ŏI�I�ȃv���C���[�\�͒l���v�Z����B
public class AbilityManager
{
    // ===== �t�B�[���h =====
    // �ێ����̑������X�g
    private List<ItemData> collectedItems = new List<ItemData>();

    // �^�O���Ƃ̏������J�E���g
    private Dictionary<string, int> tagCountMap = new Dictionary<string, int>();

    // ���ݗL���ɂȂ��Ă��鍇�v��TagEffect�i�W�v���ʗp�j
    private TagEffect totalTagEffect = new TagEffect();

    // ��{�������Z�i�A�C�e���̊�b�������v�j
    private TagEffect totalItemBaseEffect = new TagEffect();

    // ===== �Q�� =====
    private PlayerStatus playerStatus;

    // ===== �R���X�g���N�^ =====
    public AbilityManager(PlayerStatus playerStatus)
    {
        this.playerStatus = playerStatus;
    }

    // ===== ���\�b�h =====

    /// <summary>
    /// �A�C�e����ǉ�����Ƃ��Ăяo��
    /// �A�C�e���̊�{�������Z�E�^�O�����X�V����
    /// </summary>
    public void AddItem(ItemData item)
    {
        // �������X�g�ɒǉ�
        collectedItems.Add(item);

        // �A�C�e���̊�{���������Z
        AddItemBaseStats(item);

        // �^�O�J�E���g���X�V
        UpdateTagCountAdd(item);

        // �ŏI�I�Ȕ\�͂��Čv�Z
        RecalculateAllEffects();
    }

    /// <summary>
    /// �A�C�e�����폜����Ƃ��Ăяo��
    /// </summary>
    public void RemoveItem(ItemData item)
    {
        if (collectedItems.Contains(item))
        {
            collectedItems.Remove(item);

            // �A�C�e���̊�{���������Z
            RemoveItemBaseStats(item);

            // �^�O�J�E���g���X�V�i���Z�j
            UpdateTagCountRemove(item);

            // �Čv�Z
            RecalculateAllEffects();
        }
    }

    /// <summary>
    /// �S�Ă̌��ʂ��Čv�Z
    /// collectedItems�����tagCountMap�����Ƃ�
    /// totalTagEffect���v�Z���AplayerStatus�֔��f����
    /// </summary>
    private void RecalculateAllEffects()
    {
        // �^�O���ʂ̍Čv�Z
        RecalculateTagEffects();

        // �ŏI�I�ȍ��v���� = �A�C�e����b�������� + �^�O����
        TagEffect finalEffect = CombineEffects(totalItemBaseEffect, totalTagEffect);

        // �v�Z���ʂ�playerStatus�ɓK�p
        ApplyEffectsToPlayer(finalEffect);
    }

    /// <summary>
    /// �A�C�e����b���������Z
    /// </summary>
    private void AddItemBaseStats(ItemData item)
    {
        totalItemBaseEffect.hpMax += item.hpMax;
        totalItemBaseEffect.attackPower += item.attackPower;
        totalItemBaseEffect.criticalRate += item.criticalRate;
        totalItemBaseEffect.criticalDamage += item.criticalDamage;
        totalItemBaseEffect.moveSpeed += item.moveSpeed;
        totalItemBaseEffect.attackSpeed += item.attackSpeed;
        totalItemBaseEffect.attackRange += item.attackRange;
        totalItemBaseEffect.evasionRate += item.evasionRate;
        totalItemBaseEffect.ammoCapacity += item.ammoCapacity;
        // ��{�����Ɋւ��Ă͑���bool�\�͂̓f�t�H���gfalse/0�Ȃ̂ŉ��Z�s�v
    }

    /// <summary>
    /// �A�C�e����b���������Z
    /// </summary>
    private void RemoveItemBaseStats(ItemData item)
    {
        totalItemBaseEffect.hpMax -= item.hpMax;
        totalItemBaseEffect.attackPower -= item.attackPower;
        totalItemBaseEffect.criticalRate -= item.criticalRate;
        totalItemBaseEffect.criticalDamage -= item.criticalDamage;
        totalItemBaseEffect.moveSpeed -= item.moveSpeed;
        totalItemBaseEffect.attackSpeed -= item.attackSpeed;
        totalItemBaseEffect.attackRange -= item.attackRange;
        totalItemBaseEffect.evasionRate -= item.evasionRate;
        totalItemBaseEffect.ammoCapacity -= item.ammoCapacity;
    }

    /// <summary>
    /// �A�C�e���ǉ����A�^�O�J�E���g�𑝂₷
    /// </summary>
    private void UpdateTagCountAdd(ItemData item)
    {
        foreach (var tagDef in item.tags)
        {
            string tagName = tagDef.tagName;
            if (!tagCountMap.ContainsKey(tagName))
                tagCountMap[tagName] = 0;
            tagCountMap[tagName]++;
        }
    }

    /// <summary>
    /// �A�C�e���폜���A�^�O�J�E���g�����炷
    /// </summary>
    private void UpdateTagCountRemove(ItemData item)
    {
        foreach (var tagDef in item.tags)
        {
            string tagName = tagDef.tagName;
            if (tagCountMap.ContainsKey(tagName))
            {
                tagCountMap[tagName] = Mathf.Max(tagCountMap[tagName] - 1, 0);
                if (tagCountMap[tagName] == 0)
                    tagCountMap.Remove(tagName);
            }
        }
    }

    /// <summary>
    /// �^�O���ʂ��Čv�Z����
    /// tagCountMap����thresholds���`�F�b�N���A�Y����TagEffect�����v����
    /// </summary>
    private void RecalculateTagEffects()
    {
        // ��x������
        totalTagEffect = new TagEffect();

        // ���ݏ������Ă���S�^�O�����[�v
        // ���L���ɉ����Ăǂ�effect���L�����𔻒�
        foreach (var pair in tagCountMap)
        {
            string tagName = pair.Key;
            int count = pair.Value;

            // �K������^�O��`��S�A�C�e������E��
            // �i�����A�C�e��������TagDefinition�����ꍇ�Athresholds��effects�͓����I�u�W�F�N�g���w�����Ƃ�z��j
            // �{��ł͊ȗ��̂��߁Aitems�����[�v���čŏ��Ɍ�����tagDef���̗p
            AbilityTagDefinition tagDef = FindTagDefinitionFromName(tagName);

            if (tagDef == null) continue;

            // thresholds���`�F�b�N���āAcount���ǂ̃��x���܂ŒB�������m�F
            // ���ꂼ���threshold���z��������effects��K�p
            for (int i = 0; i < tagDef.thresholds.Count; i++)
            {
                if (count >= tagDef.thresholds[i])
                {
                    // ����threshold�ɑΉ�����TagEffect�����Z
                    AddTagEffect(tagDef.effects[i]);
                }
            }
        }
    }

    /// <summary>
    /// tagName�ɑΉ�����AbilityTagDefinition��S�Ă�collectedItems����T��
    /// </summary>
    private AbilityTagDefinition FindTagDefinitionFromName(string tagName)
    {
        foreach (var item in collectedItems)
        {
            foreach (var td in item.tags)
            {
                if (td.tagName == tagName)
                    return td;
            }
        }
        return null;
    }

    /// <summary>
    /// �w�肵��TagEffect�����v����i���Z�j
    /// </summary>
    private void AddTagEffect(TagEffect effect)
    {
        totalTagEffect.hpMax += effect.hpMax;
        totalTagEffect.attackPower += effect.attackPower;
        totalTagEffect.criticalRate += effect.criticalRate;
        totalTagEffect.criticalDamage += effect.criticalDamage;
        totalTagEffect.moveSpeed += effect.moveSpeed;
        totalTagEffect.attackSpeed += effect.attackSpeed;
        totalTagEffect.attackRange += effect.attackRange;
        totalTagEffect.evasionRate += effect.evasionRate;

        totalTagEffect.isDefenseReduction = totalTagEffect.isDefenseReduction || effect.isDefenseReduction;
        totalTagEffect.isAttackReduction = totalTagEffect.isAttackReduction || effect.isAttackReduction;
        totalTagEffect.isSlowEffect = totalTagEffect.isSlowEffect || effect.isSlowEffect;
        totalTagEffect.isBleedingEffect = totalTagEffect.isBleedingEffect || effect.isBleedingEffect;
        totalTagEffect.isStun = totalTagEffect.isStun || effect.isStun;

        totalTagEffect.ammoCapacity += effect.ammoCapacity;
        totalTagEffect.ammoRecovery += effect.ammoRecovery;
        totalTagEffect.ammoEcho = Mathf.Max(totalTagEffect.ammoEcho, effect.ammoEcho); // ��F�l���傫������D�悷��ȂǁA�v������ŏ����ύX�\
        totalTagEffect.ammoPenetration += effect.ammoPenetration;
        totalTagEffect.resurrectionTime += effect.resurrectionTime;

        totalTagEffect.hpRecovery = totalTagEffect.hpRecovery || effect.hpRecovery;
        totalTagEffect.explosion = totalTagEffect.explosion || effect.explosion;
        totalTagEffect.timeStop = totalTagEffect.timeStop || effect.timeStop;
        totalTagEffect.teleport = totalTagEffect.teleport || effect.teleport;
        totalTagEffect.timedPowerUpMode = totalTagEffect.timedPowerUpMode || effect.timedPowerUpMode;
        totalTagEffect.swordBeam = totalTagEffect.swordBeam || effect.swordBeam;
        totalTagEffect.resurrection = totalTagEffect.resurrection || effect.resurrection;
        totalTagEffect.barrier = totalTagEffect.barrier || effect.barrier;
        totalTagEffect.oneHitKill = totalTagEffect.oneHitKill || effect.oneHitKill;
        totalTagEffect.multiAttack = totalTagEffect.multiAttack || effect.multiAttack;
        totalTagEffect.defensePenetration = totalTagEffect.defensePenetration || effect.defensePenetration;
    }

    /// <summary>
    /// 2��TagEffect�������i���Z�j����
    /// </summary>
    private TagEffect CombineEffects(TagEffect baseEffect, TagEffect addEffect)
    {
        TagEffect combined = new TagEffect();

        combined.hpMax = baseEffect.hpMax + addEffect.hpMax;
        combined.attackPower = baseEffect.attackPower + addEffect.attackPower;
        combined.criticalRate = baseEffect.criticalRate + addEffect.criticalRate;
        combined.criticalDamage = baseEffect.criticalDamage + addEffect.criticalDamage;
        combined.moveSpeed = baseEffect.moveSpeed + addEffect.moveSpeed;
        combined.attackSpeed = baseEffect.attackSpeed + addEffect.attackSpeed;
        combined.attackRange = baseEffect.attackRange + addEffect.attackRange;
        combined.evasionRate = baseEffect.evasionRate + addEffect.evasionRate;

        combined.isDefenseReduction = baseEffect.isDefenseReduction || addEffect.isDefenseReduction;
        combined.isAttackReduction = baseEffect.isAttackReduction || addEffect.isAttackReduction;
        combined.isSlowEffect = baseEffect.isSlowEffect || addEffect.isSlowEffect;
        combined.isBleedingEffect = baseEffect.isBleedingEffect || addEffect.isBleedingEffect;
        combined.isStun = baseEffect.isStun || addEffect.isStun;

        combined.ammoCapacity = baseEffect.ammoCapacity + addEffect.ammoCapacity;
        combined.ammoRecovery = baseEffect.ammoRecovery + addEffect.ammoRecovery;
        // �e�ߖ�m���ȂǍ����̃��[���͔C�ӁA�����ł͍ő�l���̗p
        combined.ammoEcho = Mathf.Max(baseEffect.ammoEcho, addEffect.ammoEcho);
        combined.ammoPenetration = baseEffect.ammoPenetration + addEffect.ammoPenetration;
        combined.resurrectionTime = baseEffect.resurrectionTime + addEffect.resurrectionTime;

        combined.hpRecovery = baseEffect.hpRecovery || addEffect.hpRecovery;
        combined.explosion = baseEffect.explosion || addEffect.explosion;
        combined.timeStop = baseEffect.timeStop || addEffect.timeStop;
        combined.teleport = baseEffect.teleport || addEffect.teleport;
        combined.timedPowerUpMode = baseEffect.timedPowerUpMode || addEffect.timedPowerUpMode;
        combined.swordBeam = baseEffect.swordBeam || addEffect.swordBeam;
        combined.resurrection = baseEffect.resurrection || addEffect.resurrection;
        combined.barrier = baseEffect.barrier || addEffect.barrier;
        combined.oneHitKill = baseEffect.oneHitKill || addEffect.oneHitKill;
        combined.multiAttack = baseEffect.multiAttack || addEffect.multiAttack;
        combined.defensePenetration = baseEffect.defensePenetration || addEffect.defensePenetration;

        return combined;
    }

    /// <summary>
    /// �v�Z���ʂ��v���C���[�X�e�[�^�X�ɔ��f����
    /// PlayerStatus��Setter�𗘗p���đ����l���Đݒ肷��
    /// </summary>
    private void ApplyEffectsToPlayer(TagEffect effect)
    {
        // PlayerStatus���ɉ��Z���ʂ𔽉f
        // �v���C���[�͌��X�̃x�[�X�X�e�[�^�X + effect����Setter�ŏ㏑������z��
        // �����ł�playerStatus��Set�Z�Z���g�p���A�ŏI�l��ݒ肷��B

        // ���X�̃x�[�X�l�ɑ΂��āAeffect�������Z����K�v������B
        // �Ⴆ�΁AplayerStatus.GetAttackPower()�̓x�[�X+�A�C�e���O�̒l�̂��߁A
        // ���X�̃x�[�X�l��ێ����Ă������A�ŏI�l�𒼐ڏ㏑������݌v���K�v�B
        // �{��ł�PlayerStatus���̂�"�x�[�X�X�e�[�^�X+�������ʍ���"�Ƃ��Ĉ����A
        // AbilityManager����͍ŏI�l��playerStatus�ɒ��ڏ㏑������B

        playerStatus.SetHpMax(playerStatus.GetBaseHpMax() + effect.hpMax);
        playerStatus.SetHpNow(Mathf.Min(playerStatus.GetHpNow(), playerStatus.GetHpMax())); // HPNow�͍ő�l�𒴂��Ȃ��悤�ɁB

        playerStatus.SetAttackPower(playerStatus.GetBaseAttackPower() + effect.attackPower);
        playerStatus.SetMoveSpeed(playerStatus.GetBaseMoveSpeed() + effect.moveSpeed);
        playerStatus.SetAttackSpeed(playerStatus.GetBaseAttackSpeed() + effect.attackSpeed);
        playerStatus.SetEvasionRate(playerStatus.GetBaseEvasionRate() + effect.evasionRate);
        playerStatus.SetCriticalRate(playerStatus.GetBaseCriticalRate() + effect.criticalRate);
        playerStatus.SetCriticalDamage(playerStatus.GetBaseCriticalDamage() + effect.criticalDamage);
        playerStatus.SetAttackRange(playerStatus.GetBaseAttackRange() + effect.attackRange);

        playerStatus.SetAmmoCapacity(playerStatus.GetBaseAmmoCapacity() + effect.ammoCapacity);
        playerStatus.SetAmmoRecovery(playerStatus.GetBaseAmmoRecovery() + effect.ammoRecovery);
        playerStatus.SetAmmoEcho(Mathf.Max(playerStatus.GetBaseAmmoEcho(), effect.ammoEcho));
        playerStatus.SetAmmoPenetration(playerStatus.GetBaseAmmoPenetration() + effect.ammoPenetration);
        playerStatus.SetResurrectionTime(playerStatus.GetBaseResurrectionTime() + effect.resurrectionTime);

        // bool�n��OR�����ŗL�������Ă��邽�߁A���ڏ㏑��
        playerStatus.SetHpAutoRecovery(playerStatus.GetBaseHpAutoRecovery() || effect.hpRecovery);
        playerStatus.SetExplosion(playerStatus.GetBaseExplosion() || effect.explosion);
        playerStatus.SetTimeStop(playerStatus.GetBaseTimeStop() || effect.timeStop);
        playerStatus.SetTeleport(playerStatus.GetBaseTeleport() || effect.teleport);
        playerStatus.SetTimedPowerUpMode(playerStatus.GetBaseTimedPowerUpMode() || effect.timedPowerUpMode);
        playerStatus.SetSwordBeam(playerStatus.GetBaseSwordBeam() || effect.swordBeam);
        playerStatus.SetResurrection(playerStatus.GetBaseResurrection() || effect.resurrection);
        playerStatus.SetBarrier(playerStatus.GetBaseBarrier() || effect.barrier);
        playerStatus.SetOneHitKill(playerStatus.GetBaseOneHitKill() || effect.oneHitKill);
        playerStatus.SetMultiAttack(playerStatus.GetBaseMultiAttack() || effect.multiAttack);
        playerStatus.SetDefensePenetration(playerStatus.GetBaseDefensePenetration() || effect.defensePenetration);

        // �f�o�t�n�t���O
        playerStatus.SetIsDefenseReductionFlag(playerStatus.GetBaseIsDefenseReduction() || effect.isDefenseReduction);
        playerStatus.SetIsAttackReductionFlag(playerStatus.GetBaseIsAttackReduction() || effect.isAttackReduction);
        playerStatus.SetIsSlowEffectFlag(playerStatus.GetBaseIsSlowEffect() || effect.isSlowEffect);
        playerStatus.SetIsBleedingEffectFlag(playerStatus.GetBaseIsBleedingEffect() || effect.isBleedingEffect);
        playerStatus.SetIsStunFlag(playerStatus.GetBaseIsStun() || effect.isStun);
    }

    // ���ݎ��W����Ă���^�O��`�Ƃ��̐����擾����
    public Dictionary<AbilityTagDefinition, int> GetCollectedTagDefinitions()
    {
        // �^�O��`�Ƃ��̃J�E���g���i�[���鎫��
        Dictionary<AbilityTagDefinition, int> tagDefinitionCountMap = new Dictionary<AbilityTagDefinition, int>();

        // tagCountMap�̃L�[�i�^�O���j�����ƂɑΉ�����^�O��`��T��
        foreach (var tagEntry in tagCountMap)
        {
            string tagName = tagEntry.Key;
            int count = tagEntry.Value;

            // �^�O���ɑΉ�����^�O��`���擾
            AbilityTagDefinition tagDefinition = FindTagDefinitionFromName(tagName);

            // ��`�����݂���ꍇ�̂ݎ����ɒǉ�
            if (tagDefinition != null)
            {
                tagDefinitionCountMap[tagDefinition] = count;
            }
            else
            {
                Debug.LogWarning($"�^�O�� '{tagName}' �ɑΉ�����^�O��`��������܂���B");
            }
        }

        return tagDefinitionCountMap;
    }
}
