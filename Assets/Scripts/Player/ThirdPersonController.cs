using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

//Note: Animations are called via the controller for both the character and capsule using animator null checks

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float m_MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float m_SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float m_RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float m_SpeedChangeRate = 10.0f;

    [SerializeField] private AudioClip m_LandingAudioClip;
    [SerializeField] private AudioClip[] m_FootstepAudioClips;
    [Range(0, 1)] [SerializeField] private float m_FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float m_JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float m_Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float m_JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float m_FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool m_Grounded = true;

    [Tooltip("Useful for rough ground")]
    [SerializeField] private float m_GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float m_GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask m_GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject m_CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float m_TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float m_BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] private float m_CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    [SerializeField] private bool m_LockCameraPosition = false;

    // Cinemachine
    private float m_CinemachineTargetYaw;
    private float m_CinemachineTargetPitch;

    // Player
    private float m_Speed;
    private float m_AnimationBlend;
    private float m_TargetRotation = 0.0f;
    private float m_RotationVelocity;
    private float m_VerticalVelocity;
    private float m_TerminalVelocity = 53.0f;
    private bool m_IsSprinting = false;
    private bool m_IsJumping = false;
    private Vector2 m_MoveDirection, m_LookDirection;

    // Timeout deltaTime
    private float m_JumpTimeoutDelta;
    private float m_FallTimeoutDelta;

    // Animation IDs
    private int m_AnimIDSpeed;
    private int m_AnimIDGrounded;
    private int m_AnimIDJump;
    private int m_AnimIDFreeFall;
    private int m_AnimIDMotionSpeed;

    private PlayerInput m_PlayerInput;
    private Animator m_Animator;
    private CharacterController m_Controller;
    private PlayerInputsWrapper m_Input;
    private GameObject m_MainCamera;

    private const float m_Threshold = 0.01f;

    private bool m_HasAnimator;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            return m_PlayerInput.currentControlScheme == "KeyboardMouse";
        }
    }

    private void Awake()
    {
        m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera"); // Must match name of camera in editor
    }

    private void OnEnable()
    {
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.PlayerLook, SetLookDirection);
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.PlayerMove, SetMoveDirection);
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.SprintPerformed, SetSprint);
        InstanceManager.Get<GameEventSystem>().Subscribe(GameEventType.JumnpPerformed, SetJump);
    }

    private void OnDisable()
    {
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.PlayerLook, SetLookDirection);
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.PlayerMove, SetMoveDirection);
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.SprintPerformed, SetSprint);
        InstanceManager.Get<GameEventSystem>().Unsubscribe(GameEventType.JumnpPerformed, SetJump);
    }

    private void Start()
    {
        m_CinemachineTargetYaw = m_CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        m_HasAnimator = TryGetComponent(out m_Animator);
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInputsWrapper>();
        m_PlayerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();

        // Reset timeouts on start
        m_JumpTimeoutDelta = m_JumpTimeout;
        m_FallTimeoutDelta = m_FallTimeout;
    }

    private void Update()
    {
        m_HasAnimator = TryGetComponent(out m_Animator);

        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        m_AnimIDSpeed = Animator.StringToHash("Speed");
        m_AnimIDGrounded = Animator.StringToHash("Grounded");
        m_AnimIDJump = Animator.StringToHash("Jump");
        m_AnimIDFreeFall = Animator.StringToHash("FreeFall");
        m_AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // Set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - m_GroundedOffset,
            transform.position.z);
        m_Grounded = Physics.CheckSphere(spherePosition, m_GroundedRadius, m_GroundLayers,
            QueryTriggerInteraction.Ignore);

        // Update animator if using character
        if (m_HasAnimator)
        {
            m_Animator.SetBool(m_AnimIDGrounded, m_Grounded);
        }
    }

    private void CameraRotation()
    {
        // If there is an input and camera position is not fixed
        if (m_LookDirection.sqrMagnitude >= m_Threshold && !m_LockCameraPosition)
        {
            // Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            m_CinemachineTargetYaw += m_LookDirection.x * deltaTimeMultiplier;
            m_CinemachineTargetPitch += m_LookDirection.y * deltaTimeMultiplier;
        }

        // Clamp rotations so values are limited to 360 degrees
        m_CinemachineTargetYaw = ClampAngle(m_CinemachineTargetYaw, float.MinValue, float.MaxValue);
        m_CinemachineTargetPitch = ClampAngle(m_CinemachineTargetPitch, m_BottomClamp, m_TopClamp);

        // Cinemachine will follow this target
        m_CinemachineCameraTarget.transform.rotation = Quaternion.Euler(m_CinemachineTargetPitch + m_CameraAngleOverride,
            m_CinemachineTargetYaw, 0.0f);
    }

    private void SetSprint(GameEvent e)
    {
        m_IsSprinting = !m_IsSprinting;
    }

    private void SetJump(GameEvent e)
    {
        m_IsJumping = !m_IsJumping;
    }

    private void SetMoveDirection(GameEvent e)
    {
        m_MoveDirection = e.Data;
    }

    private void SetLookDirection(GameEvent e)
    {
        m_LookDirection = e.Data;
    }

    private void Move()
    {
        // Set target speed based on move speed, sprint speed, and if sprint is pressed
        float targetSpeed = m_IsSprinting ? m_SprintSpeed : m_MoveSpeed;

        // A simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // Note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude.
        // If there is no input, set the target speed to 0
        if (m_MoveDirection == Vector2.zero) targetSpeed = 0.0f;

        // A reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(m_Controller.velocity.x, 0.0f, m_Controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // Accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // Creates curved result rather than a linear one giving a more organic speed change
            // Note: T in Lerp is clamped, so don't need to clamp speed
            m_Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * m_MoveDirection.magnitude,
                Time.deltaTime * m_SpeedChangeRate);

            // Round speed to 3 decimal places
            m_Speed = Mathf.Round(m_Speed * 1000f) / 1000f;
        }
        else
        {
            m_Speed = targetSpeed;
        }

        m_AnimationBlend = Mathf.Lerp(m_AnimationBlend, targetSpeed, Time.deltaTime * m_SpeedChangeRate);
        if (m_AnimationBlend < 0.01f) m_AnimationBlend = 0f;

        // Normalise input direction
        Vector3 inputDirection = new Vector3(m_MoveDirection.x, 0.0f, m_MoveDirection.y).normalized;

        // Note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude.
        // If there is a move input rotate player when the player is moving
        if (m_MoveDirection != Vector2.zero)
        {
            m_TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              m_MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationVelocity,
                m_RotationSmoothTime);

            // Rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, m_TargetRotation, 0.0f) * Vector3.forward;

        // Move the player
        m_Controller.Move(targetDirection.normalized * (m_Speed * Time.deltaTime) +
                         new Vector3(0.0f, m_VerticalVelocity, 0.0f) * Time.deltaTime);

        // Update animator if using character
        if (m_HasAnimator)
        {
            m_Animator.SetFloat(m_AnimIDSpeed, m_AnimationBlend);
            m_Animator.SetFloat(m_AnimIDMotionSpeed, m_MoveDirection.magnitude);
        }
    }

    private void JumpAndGravity()
    {
        if (m_Grounded)
        {
            // Reset the fall timeout timer
            m_FallTimeoutDelta = m_FallTimeout;

            // Update animator if using character
            if (m_HasAnimator)
            {
                m_Animator.SetBool(m_AnimIDJump, false);
                m_Animator.SetBool(m_AnimIDFreeFall, false);
            }

            // Stop our velocity dropping infinitely when grounded
            if (m_VerticalVelocity < 0.0f)
            {
                m_VerticalVelocity = -2f;
            }

            // Jump
            if (m_IsJumping && m_JumpTimeoutDelta <= 0.0f)
            {
                // The square root of H * -2 * G = how much velocity needed to reach desired height
                m_VerticalVelocity = Mathf.Sqrt(m_JumpHeight * -2f * m_Gravity);

                // Update animator if using character
                if (m_HasAnimator)
                {
                    m_Animator.SetBool(m_AnimIDJump, true);
                }
            }

            // Jump timeout
            if (m_JumpTimeoutDelta >= 0.0f)
            {
                m_JumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // Reset the jump timeout timer
            m_JumpTimeoutDelta = m_JumpTimeout;

            // Fall timeout
            if (m_FallTimeoutDelta >= 0.0f)
            {
                m_FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // Update animator if using character
                if (m_HasAnimator)
                {
                    m_Animator.SetBool(m_AnimIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            //m_Input.GetIsJump() = false; // Might still need this
        }

        // Apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (m_VerticalVelocity < m_TerminalVelocity)
        {
            m_VerticalVelocity += m_Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (m_Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // When selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - m_GroundedOffset, transform.position.z),
            m_GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (m_FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, m_FootstepAudioClips.Length);
                // TODO: change to use audio manager
                AudioSource.PlayClipAtPoint(m_FootstepAudioClips[index], transform.TransformPoint(m_Controller.center), m_FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // TODO: change to use audio manager
            AudioSource.PlayClipAtPoint(m_LandingAudioClip, transform.TransformPoint(m_Controller.center), m_FootstepAudioVolume);
        }
    }
}