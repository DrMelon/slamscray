using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using Slamscray;

//@Author: J. Brown / DrMelon
//@Purpose:
//
//  This is an Otter2D component designed to manage the health of entities, allow them to damage other entities, and to die properly.
//

namespace Slamscray.Components
{
    public class HealthDamageComponent : Otter.Component
    {

        public struct AttackInfo
        {
            public float impulseAmt;
            public bool facingLeft;
        };




        public float Health = 100;
        public float MaximumHealth = 100;
        public bool Invulnerable = false;
        public float InvulnTime = 0;
        public bool Dead = false;

        // This Action is called when an object takes damage
        public Action<AttackInfo> OnDamageTaken;
        public Action StartDeath;

        public override void Update()
        {
            base.Update();

            // Check Invulnerability time 
            if (Invulnerable)
            {
                InvulnTime--;
            }

            if (InvulnTime < 0.0f)
            {
                Invulnerable = false;
            }

            // Check health, determine if dead
            if(Health <= 0 && Dead != true)
            {
                Dead = true;

                if (this.Entity != null && StartDeath == null) // no death action, die by removing
                {
                    this.Entity.RemoveSelf();
                }

                if(this.StartDeath != null)
                {
                    StartDeath();
                }
            }

            
            // Damage other entities if we're in damage mode. -- DEPRECATED, COMMENTED HERE FOR REFERENCE
            /*if (IsDamaging && DamageAmt > 0)
            {

                // Check what entities this entity is colliding with (slow?)
                if (this.Entity != null)
                {
                    List<Entity> collisionList = this.Entity.Collider.CollideEntities(this.Entity.X, this.Entity.Y, this.Entity.Collider.Tags);

                    foreach (Entity ent in collisionList)
                    {
                        // Try to get another health/damage component
                        HealthDamageComponent other = ent.GetComponent<HealthDamageComponent>();
                        if (other != null)
                        {
                            other.RemoveHealth(DamageAmt);
                        }
                    }


                }
            }
            */






        }

        public override void Render()
        {
            base.Render();
        }

        public void ChangeHealth(float newVar)
        {
            if(Health < newVar && Invulnerable)
            {
                // Don't take health if the character is invulnerable.
                return;
            }



            Health = newVar;
            // Clamp health
            MathHelper.Clamp(Health, 0, MaximumHealth);    
        }

        public void RemoveHealth(float removeAmt)
        {
            ChangeHealth(Health - removeAmt);
            
        }

        public void Attacked(float removeAmt, AttackInfo atk)
        {
            RemoveHealth(removeAmt);
            OnDamageTaken(atk);
        }

        public void GiveHealth(float giveAmt)
        {
            ChangeHealth(Health + giveAmt);
        }



    }
}
