using UnityEngine;
using UnityEngine.Events;

namespace Movement
{
    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
        [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
        [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
        [SerializeField] private Collider2D m_UpperCollider;                        // A collider that will be disabled when crouching
        [SerializeField] private Collider2D m_LowerCollider;                    // My added collider.

        [SerializeField] private float groundedRaycastDistance;
        public bool m_Grounded;            // Whether or not the player is grounded. >>>> I made public when it was private <<<<<<
        const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private Vector3 m_Velocity = Vector3.zero;
        private bool hasPositiveJumpVelocity;
        [SerializeField] private int airJumps;
        private int jumpsRemaining;

        [Header("Events")]
        [Space]

        public UnityEvent OnLandEvent;

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent OnCrouchEvent;
        private bool m_wasCrouching = false;

        [Header("Debug")]
        [SerializeField] private bool enableDebugGizmos = true;


        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            if (OnLandEvent == null)
                OnLandEvent = new UnityEvent();

            if (OnCrouchEvent == null)
                OnCrouchEvent = new BoolEvent();

            jumpsRemaining = airJumps;
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            PlayerGroundedCheck();

            if (hasPositiveJumpVelocity && m_Rigidbody2D.velocity.y <= 0f)
                hasPositiveJumpVelocity = false;
        }


        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch)
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {

                // If crouching
                if (crouch)
                {
                    if (!m_wasCrouching)
                    {
                        m_wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }

                    // Reduce the speed by the crouchSpeed multiplier
                    move *= m_CrouchSpeed;

                    // Disable one of the colliders when crouching
                    if (m_UpperCollider != null)
                        m_UpperCollider.enabled = false;
                }
                else
                {
                    // Enable the collider when not crouching
                    if (m_UpperCollider != null)
                        m_UpperCollider.enabled = true;

                    if (m_wasCrouching)
                    {
                        m_wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if ((m_Grounded || jumpsRemaining > 0) && jump)
            {
                if (!m_Grounded)
                {
                    jumpsRemaining--;
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
                }
                //Air jump code resets vertical velocity before adding another force so you don't skyrocket due to the first jump's force.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                hasPositiveJumpVelocity = true;
            }
        }


        private void Flip()
        {
            m_FacingRight = !m_FacingRight;
            transform.Rotate(0f, 180, 0f);
            // Rotate the player 180 degrees on the Y axis when the player switches directions.
        }

        private void PlayerGroundedCheck()
        {
            bool wasGrounded = m_Grounded;
            m_Grounded = false;
            // The raycast position vectors can be set once in Awake/Start if we decide colliders will never change size during runtime.
            Vector2 raycastCornerRight = new Vector2(m_LowerCollider.bounds.max.x, m_LowerCollider.bounds.min.y);
            Vector2 raycastCornerLeft = m_LowerCollider.bounds.min;
            Vector2 raycastCenter = new Vector2(m_LowerCollider.bounds.center.x, m_LowerCollider.bounds.min.y);
            if
                ((
                Physics2D.Raycast(raycastCornerLeft, Vector2.down, groundedRaycastDistance, m_WhatIsGround)
                ||
                Physics2D.Raycast(raycastCornerRight, Vector2.down, groundedRaycastDistance, m_WhatIsGround)
                ||
                Physics2D.Raycast(raycastCenter, Vector2.down, groundedRaycastDistance, m_WhatIsGround)
                )
                &&
                !hasPositiveJumpVelocity
                )
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
            // Grounds player and invokes OnLandEvent if the raycasts detect ground and the player is not currently jumping upwards.
        }

        public void ResetAirJumps()
        {
            jumpsRemaining = airJumps;
            // Probably add this to some bigger and more whole landing sequence later.
        }



        // DEBUG RELATED METHODS BELOW THIS POINT V V V V V

        private void OnDrawGizmosSelected()
        {
            if (enableDebugGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_LowerCollider.bounds.min, .01f);

                Gizmos.DrawWireSphere(new Vector2(m_LowerCollider.bounds.center.x, m_LowerCollider.bounds.min.y), .01f); //New
                Debug.DrawRay(new Vector2(m_LowerCollider.bounds.center.x, m_LowerCollider.bounds.min.y), Vector2.down * groundedRaycastDistance, Color.red);

                Gizmos.DrawWireSphere(new Vector2(m_LowerCollider.bounds.max.x, m_LowerCollider.bounds.min.y), .01f);
                Debug.DrawRay(m_LowerCollider.bounds.min, Vector2.down * groundedRaycastDistance, Color.red);
                Debug.DrawRay(new Vector2(m_LowerCollider.bounds.max.x, m_LowerCollider.bounds.min.y), Vector2.down * groundedRaycastDistance, Color.red);
                // Ground check raycasts from the lower corners of the bottom collider. WireSphere rendered at origins.
            }
        }
    }
}