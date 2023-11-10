using EntityStates;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Skills;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Events;
using EntityStates.GummyClone;

namespace ChirrAltSkills.Chirr.States.Special
{
    internal class TransformEnemy : BaseState
    {
        public virtual bool AllowBoss => false;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("SS2UChirrSpecial", gameObject);
            if (NetworkServer.active)
            {
                foreach (var enemy in CharacterMaster.readOnlyInstancesList)
                {
                    if (enemy.teamIndex == teamComponent.teamIndex)
                        continue;
                    var enemyCB = enemy.GetBody();
                    if (!enemyCB || Vector3.Distance(enemyCB.corePosition, characterBody.corePosition) > 5)
                        continue;
                    if (enemyCB.isBoss && !AllowBoss)
                        continue;
                    var enemyHC = enemyCB.healthComponent;
                    if (!enemyHC || !enemyHC.alive || enemyHC.health <= 0 || enemyHC.combinedHealthFraction > 0.5f)
                        continue;
                    var enemyFootPos = enemyCB.footPosition;
                    //var enemyRot = enemyCB.inputBank._aimDirection;

                    MasterCopySpawnCard masterCopySpawnCard = MasterCopySpawnCard.FromMaster(characterBody.master, false, false, null);
                    if (!masterCopySpawnCard)
                    {
                        continue;
                    }
                    //masterCopySpawnCard.GiveItem(DLC1Content.Items.GummyCloneIdentifier, 1);
                    //masterCopySpawnCard.GiveItem(RoR2Content.Items.BoostDamage, this.damageBoostCount);
                    //masterCopySpawnCard.GiveItem(RoR2Content.Items.BoostHp, this.hpBoostCount);
                    DirectorCore.MonsterSpawnDistance input = DirectorCore.MonsterSpawnDistance.Close;
                    DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Direct,
                        position = enemyFootPos
                    };
                    DirectorCore.GetMonsterSpawnDistance(input, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(masterCopySpawnCard, directorPlacementRule, new Xoroshiro128Plus(Run.instance.seed + (ulong)Run.instance.fixedTime));
                    directorSpawnRequest.teamIndexOverride = new TeamIndex?(characterBody.master.teamIndex);
                    directorSpawnRequest.ignoreTeamMemberLimit = true;
                    directorSpawnRequest.summonerBodyObject = characterBody.gameObject;
                    DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
                    directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                    {
                        CharacterMaster component3 = result.spawnedInstance.GetComponent<CharacterMaster>();
                        Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
                        //characterBody.master.AddDeployable(deployable, DeployableSlot.GummyClone);
                        deployable.onUndeploy = deployable.onUndeploy ?? new UnityEvent();
                        deployable.onUndeploy.AddListener(new UnityAction(component3.TrueKill));

                        component3.inventory.CopyItemsFrom(characterBody.inventory);
                        component3.inventory.CopyEquipmentFrom(characterBody.inventory);

                        foreach (string str in ChirrFriendController.itemCopyBlacklist)
                        {
                            ItemIndex blacklistItem = ItemCatalog.FindItemIndex(str);
                            if (blacklistItem != ItemIndex.None)
                            {
                                component3.inventory.RemoveItem(blacklistItem, component3.inventory.GetItemCount(blacklistItem));
                            }
                        }

                        component3.AddComponent<MasterSuicideOnTimer>().lifeTimer = 30f;

                        GameObject bodyObject = component3.GetBodyObject();
                        if (bodyObject)
                        {
                            foreach (EntityStateMachine entityStateMachine in bodyObject.GetComponents<EntityStateMachine>())
                            {
                                if (entityStateMachine.customName == "Body")
                                {
                                    entityStateMachine.SetState(new GummyCloneSpawnState());
                                    return;
                                }
                            }
                        }

                    }));
                    var outcome = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    UnityEngine.Object.Destroy(masterCopySpawnCard);
                    enemyHC.Suicide(characterBody.gameObject);
                    break;
                }
            }
            outer.SetNextStateToMain();
        }
    }
}
