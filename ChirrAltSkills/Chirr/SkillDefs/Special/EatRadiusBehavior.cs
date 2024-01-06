using RoR2;
using UnityEngine;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    internal class EatRadiusBehavior : MonoBehaviour
    {
        private CharacterBody characterBody;

        //SkillLocator skillLocator;
        private readonly float updateFrequency = 0.5f;

        private float stopwatch = 0;

        private void Awake()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
            //skillLocator = characterBody.skillLocator;
        }

        private void OnEnable()
        {
            indicatorEnabled = true;
        }

        private void OnDisable()
        {
            indicatorEnabled = false;
        }

        private void FixedUpdate()
        {
        }

        private bool indicatorEnabled
        {
            get
            {
                return indicator;
            }
            set
            {
                if (indicatorEnabled == value)
                {
                    return;
                }
                if (value)
                {
                    GameObject original = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
                    indicator = Instantiate(original, characterBody.corePosition, Quaternion.identity);
                    indicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(gameObject, null);
                    return;
                }
                Destroy(indicator);
                indicator = null;
            }
        }

        private GameObject indicator;
    }
}