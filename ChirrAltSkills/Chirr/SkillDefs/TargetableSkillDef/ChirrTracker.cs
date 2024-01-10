using RoR2;
using UnityEngine;

namespace ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(TeamComponent))]
    public class ChirrTracker : MonoBehaviour
    {
        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                ModifyRange();

                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                //HurtBox hurtBox = this.trackingTarget;
                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        private void ModifyRange()
        {
            var buffCount = characterBody.GetBuffCount(Buffs.snackiesBuff);
            maxTrackingDistanceCalculated = maxTrackingDistance + maxTrackingDistancePerStack * buffCount;
        }

        private bool CanTargetBoss(HurtBox hurtBox)
        {
            return !hurtBox.healthComponent.body.isBoss || targetCanBeBoss;
        }

        private bool CanTargetMaster(HurtBox hurtBox)
        {
            return !hurtBox.healthComponent.body.master || targetNeedsMaster;
        }

        private bool CanTargetHealthPercentage(HurtBox hurtBox)
        {
            return !targetHealthThreshold || hurtBox.healthComponent.combinedHealthFraction < targetHealthThresholdPercentage;
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = maxTrackingDistanceCalculated;
            this.search.maxAngleFilter = maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            var results = search.GetResults();
            bool hasGottenResult = false;
            foreach (var result in results)
            {
                if (!result.healthComponent
                    || !result.healthComponent.alive
                    || !result.healthComponent.body
                    || !CanTargetHealthPercentage(result)
                    || !CanTargetMaster(result)
                    || !CanTargetBoss(result)) continue;
                this.trackingTarget = result;
                hasGottenResult = true;
                break;
            }
            if (!hasGottenResult)
            {
                this.trackingTarget = null;
            }
        }

        public float maxTrackingDistanceCalculated = 5;
        public const float maxTrackingDistance = 5f;
        public const float maxTrackingDistancePerStack = 1f;

        public const float maxTrackingAngle = 20f;

        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private CharacterBody characterBody;

        private TeamComponent teamComponent;

        private InputBankTest inputBank;

        private float trackerUpdateStopwatch;

        private Indicator indicator;

        private readonly BullseyeSearch search = new BullseyeSearch();

        public bool targetHealthThreshold = false;
        public float targetHealthThresholdPercentage = 1;
        public bool targetCanBeBoss = false;
        public bool targetNeedsMaster = false;
    }
}