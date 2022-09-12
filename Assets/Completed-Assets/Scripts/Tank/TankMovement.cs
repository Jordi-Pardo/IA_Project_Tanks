using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Complete
{
    public enum Steerings
    {
        Wander,
        Patrol,
    }

    public class TankMovement : MonoBehaviour
    {
        public Steerings steering;
        public LayerMask wanderLayerMask;
        public Transform[] patrolPoints;
        private int targetPointIndex = 0;
        private NavMeshAgent agent;
        public Transform target;
       
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
		public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

        private float freq = 0f;
        private Vector3 movement;
        private Quaternion rotation;
        private float stopDistance = 5f;
        public NavMeshPath targetPath;
        public bool m_CanMove = true;
        private Vector3 worldTarget;
        public Shoot shootScript;
        TankHealth tankHealth;
        public bool needRecharge = false;
        bool recharged = false;
        private void Awake ()
        {
            tankHealth = GetComponent<TankHealth>();
            agent = GetComponent<NavMeshAgent>();

        }



        private void OnEnable ()
        {


            // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play();
            }
        }


        private void OnDisable ()
        {

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop();
            }
        }


        private void Start ()
        {
            targetPointIndex = Random.Range(0, patrolPoints.Length);
            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;
            if(steering == Steerings.Patrol)
            {
               // agent.SetDestination(patrolPoints[targetPointIndex].position);
            }
        }


        private void Update ()
        {
            EngineAudio();

            if (m_CanMove && !needRecharge)
            {
                switch (steering)
                {
                    case Steerings.Wander:
                        MovewithFrames();
                        break;
                    case Steerings.Patrol:
                        Patrol();
                        break;
                }
            }
        }

        public void SetRechargeDestination()
        {
            agent.SetDestination(tankHealth.factory.rechargePoint.position);

        }

        public void QuitTarget()
        {
            target = null;
            shootScript.target = null;
        }

        public void StopMove()
        {
            m_CanMove = false;
            agent.isStopped = true;
        }

        public void CanMove()
        {
            m_CanMove = true;
            agent.isStopped = false;
        }

        //private void Move()
        //{
        //    if (Vector3.Distance(target.transform.position, transform.position) < stopDistance)
        //        return;

        //    freq += Time.deltaTime;
        //    if (freq > 0.5f)
        //    {
        //        freq -= 0.5f;
        //        Seek();
        //    }

        //    m_TurnSpeed += m_TurnAcceleration * Time.deltaTime;
        //    m_TurnSpeed = Mathf.Min(m_TurnSpeed, m_MaxTurnSpeed);
        //    m_Speed += acceleration * Time.deltaTime;
        //    m_Speed = Mathf.Min(m_Speed, m_MaxSpeed);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_TurnSpeed);
        //    transform.position += transform.forward.normalized * m_Speed * Time.deltaTime;
        //}

        private void EngineAudio ()
        {
            // If there is no input (the tank is stationary)...
            if (agent.isStopped)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }




        //private void Seek()
        //{
        //    Vector3 direction = target.transform.position - transform.position;
        //    direction.y = 0;
        //    movement = direction.normalized * acceleration;
        //    float angle = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
        //    rotation = Quaternion.AngleAxis(angle, Vector3.up);

            
        //}

        void Wander()
        {
            float radius = 4f;
            float offset = 10f;

            Vector3 localTarget = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            localTarget.Normalize();
            localTarget *= radius;
            localTarget += new Vector3(0, 0, offset);

            worldTarget = transform.TransformPoint(localTarget);
            worldTarget.y = 1f;
            if (!Physics.CheckSphere(worldTarget, 0.2f,wanderLayerMask))
            {
                worldTarget.y = 0f;

                agent.SetDestination(worldTarget);
                
            }
            else
            {
                worldTarget.y = 0f;
                worldTarget *= -1f;
                agent.SetDestination(worldTarget);

            }
        }

        void MovewithFrames()
        {
            if (Vector3.Distance(agent.destination, transform.position) < stopDistance)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }


            freq += Time.deltaTime;
            if (freq > 1f)
            {
                freq -= 1f;
                Wander();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(worldTarget, 0.2f);
            if (agent != null)
                Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }

        void Patrol()
        {

            if(Vector3.Distance(agent.destination,transform.position) < 8f)
            {
                targetPointIndex++;
                if (targetPointIndex == patrolPoints.Length)
                    targetPointIndex = 0;
                agent.SetDestination(patrolPoints[targetPointIndex].position);
            }
        }
        
        
    }
    
}