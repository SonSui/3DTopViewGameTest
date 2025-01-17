using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

// PlayerStatusに内包する能力管理クラス
// 装備品(ItemData)を収集し、それに伴うタグを集計し、対応する能力(TagEffect)を有効化し、
// 最終的なプレイヤー能力値を計算する。
public class AbilityManager
{
    // ===== フィールド =====
    // 保持中の装備リスト
    private List<ItemData> collectedItems = new List<ItemData>();

    // タグごとの所持数カウント
    private Dictionary<string, int> tagCountMap = new Dictionary<string, int>();

    // 現在有効になっている合計のTagEffect（集計結果用）
    private TagEffect totalTagEffect = new TagEffect();

    // 基本属性加算（アイテムの基礎属性合計）
    private TagEffect totalItemBaseEffect = new TagEffect();

    // ===== 参照 =====
    private PlayerStatus playerStatus;

    // ===== コンストラクタ =====
    public AbilityManager(PlayerStatus playerStatus)
    {
        this.playerStatus = playerStatus;
    }

    // ===== メソッド =====

    /// <summary>
    /// アイテムを追加するとき呼び出す
    /// アイテムの基本属性加算・タグ数を更新する
    /// </summary>
    public void AddItem(ItemData item)
    {
        // 装備リストに追加
        collectedItems.Add(item);

        // アイテムの基本属性を加算
        AddItemBaseStats(item);

        // タグカウントを更新
        UpdateTagCountAdd(item);

        // 最終的な能力を再計算
        RecalculateAllEffects();
    }

    /// <summary>
    /// アイテムを削除するとき呼び出す
    /// </summary>
    public void RemoveItem(ItemData item)
    {
        if (collectedItems.Contains(item))
        {
            collectedItems.Remove(item);

            // アイテムの基本属性を減算
            RemoveItemBaseStats(item);

            // タグカウントを更新（減算）
            UpdateTagCountRemove(item);

            // 再計算
            RecalculateAllEffects();
        }
    }

    /// <summary>
    /// 全ての効果を再計算
    /// collectedItemsおよびtagCountMapをもとに
    /// totalTagEffectを計算し、playerStatusへ反映する
    /// </summary>
    private void RecalculateAllEffects()
    {
        // タグ効果の再計算
        RecalculateTagEffects();

        // 最終的な合計効果 = アイテム基礎属性効果 + タグ効果
        TagEffect finalEffect = CombineEffects(totalItemBaseEffect, totalTagEffect);

        // 計算結果をplayerStatusに適用
        ApplyEffectsToPlayer(finalEffect);
    }

    /// <summary>
    /// アイテム基礎属性を加算
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
        // 基本属性に関しては他のbool能力はデフォルトfalse/0なので加算不要
    }

    /// <summary>
    /// アイテム基礎属性を減算
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
    /// アイテム追加時、タグカウントを増やす
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
    /// アイテム削除時、タグカウントを減らす
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
    /// タグ効果を再計算する
    /// tagCountMapからthresholdsをチェックし、該当のTagEffectを合計する
    /// </summary>
    private void RecalculateTagEffects()
    {
        // 一度初期化
        totalTagEffect = new TagEffect();

        // 現在所持している全タグをループ
        // 所有数に応じてどのeffectが有効かを判定
        foreach (var pair in tagCountMap)
        {
            string tagName = pair.Key;
            int count = pair.Value;

            // 適合するタグ定義を全アイテムから拾う
            // （複数アイテムが同じTagDefinitionを持つ場合、thresholdsやeffectsは同じオブジェクトを指すことを想定）
            // 本例では簡略のため、itemsをループして最初に見つけたtagDefを採用
            AbilityTagDefinition tagDef = FindTagDefinitionFromName(tagName);

            if (tagDef == null) continue;

            // thresholdsをチェックして、countがどのレベルまで達したか確認
            // それぞれのthresholdを越えた分のeffectsを適用
            for (int i = 0; i < tagDef.thresholds.Count; i++)
            {
                if (count >= tagDef.thresholds[i])
                {
                    // そのthresholdに対応するTagEffectを加算
                    AddTagEffect(tagDef.effects[i]);
                }
            }
        }
    }

    /// <summary>
    /// tagNameに対応するAbilityTagDefinitionを全てのcollectedItemsから探す
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
    /// 指定したTagEffectを合計する（加算）
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
        totalTagEffect.ammoEcho = Mathf.Max(totalTagEffect.ammoEcho, effect.ammoEcho); // 例：値が大きい方を優先するなど、要件次第で処理変更可能
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
    /// 2つのTagEffectを合成（加算）する
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
        // 弾節約確率など合成のルールは任意、ここでは最大値を採用
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
    /// 計算結果をプレイヤーステータスに反映する
    /// PlayerStatusにSetterを利用して属性値を再設定する
    /// </summary>
    private void ApplyEffectsToPlayer(TagEffect effect)
    {
        // PlayerStatus側に加算結果を反映
        // プレイヤーは元々のベースステータス + effect分をSetterで上書きする想定
        // ここではplayerStatusのSet〇〇を使用し、最終値を設定する。

        // 元々のベース値に対して、effect分を加算する必要がある。
        // 例えば、playerStatus.GetAttackPower()はベース+アイテム前の値のため、
        // 元々のベース値を保持しておくか、最終値を直接上書きする設計が必要。
        // 本例ではPlayerStatus自体を"ベースステータス+装備効果込み"として扱い、
        // AbilityManagerからは最終値をplayerStatusに直接上書きする。

        playerStatus.SetHpMax(playerStatus.GetBaseHpMax() + effect.hpMax);
        playerStatus.SetHpNow(Mathf.Min(playerStatus.GetHpNow(), playerStatus.GetHpMax())); // HPNowは最大値を超えないように。

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

        // bool系はOR条件で有効化しているため、直接上書き
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

        // デバフ系フラグ
        playerStatus.SetIsDefenseReductionFlag(playerStatus.GetBaseIsDefenseReduction() || effect.isDefenseReduction);
        playerStatus.SetIsAttackReductionFlag(playerStatus.GetBaseIsAttackReduction() || effect.isAttackReduction);
        playerStatus.SetIsSlowEffectFlag(playerStatus.GetBaseIsSlowEffect() || effect.isSlowEffect);
        playerStatus.SetIsBleedingEffectFlag(playerStatus.GetBaseIsBleedingEffect() || effect.isBleedingEffect);
        playerStatus.SetIsStunFlag(playerStatus.GetBaseIsStun() || effect.isStun);
    }

    // 現在収集されているタグ定義とその数を取得する
    public Dictionary<AbilityTagDefinition, int> GetCollectedTagDefinitions()
    {
        // タグ定義とそのカウントを格納する辞書
        Dictionary<AbilityTagDefinition, int> tagDefinitionCountMap = new Dictionary<AbilityTagDefinition, int>();

        // tagCountMapのキー（タグ名）をもとに対応するタグ定義を探す
        foreach (var tagEntry in tagCountMap)
        {
            string tagName = tagEntry.Key;
            int count = tagEntry.Value;

            // タグ名に対応するタグ定義を取得
            AbilityTagDefinition tagDefinition = FindTagDefinitionFromName(tagName);

            // 定義が存在する場合のみ辞書に追加
            if (tagDefinition != null)
            {
                tagDefinitionCountMap[tagDefinition] = count;
            }
            else
            {
                Debug.LogWarning($"タグ名 '{tagName}' に対応するタグ定義が見つかりません。");
            }
        }

        return tagDefinitionCountMap;
    }
}
