﻿using Assets.Scripts.Gore;
using Assets.Scripts.People;
using UnityEngine;
using UnityEventAggregator;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealthBehavior : MonoBehaviour, IDamageBehavior
    {
        private bool _isDead;
        public Sprite DeadBody;

        public void OnHit(HitContext hitContext)
        {
            if (_isDead) return;

            GetComponent<EnemySounds>().PlayHitSound();

            if (!hitContext.IsMelee) {
                var ejector = gameObject.GetComponent<BloodEjector>();
                if (ejector != null)
                {
                    ejector.Eject(hitContext);
                }
            }
        }

        public void OnDeath(HitContext hitContext)
        {
            if (_isDead) return;

            if (!hitContext.IsMelee) {
                var ejector = gameObject.GetComponent<BloodEjector>();
                if (ejector != null)
                {
                    ejector.Eject(hitContext);
                    ejector.Eject(hitContext);
                    ejector.Eject(hitContext);
                }
            }

            Destroy(GetComponent<EnemyMovement>());
            Destroy(GetComponent<NavMeshAgent>());

            _isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            gameObject.layer = LayerMask.NameToLayer("TurnStaticSoon");
            GetComponent<EnemySounds>().PlayDeathSound();

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddExplosionForce(hitContext.Force, transform.position - hitContext.Direction, 1f, 1f, ForceMode.Impulse);
            
            EventAggregator.SendMessage(new EnemyKilledMessage());
        }
    }
}