using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _walkSpeed;

    [SerializeField]
    private InputManager _Input;

    [SerializeField]
    private float _rotationSmoothTime = 0.1f;
    private float _rotationSmoothVelocity;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _walkSprintTransistion;
    private float _speed;

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private Transform _groundDetector;

    [SerializeField]
    private float _detectorRadius;

    [SerializeField]
    private LayerMask _groundLayer;
    private bool _isGrounded;

    [SerializeField]
    private Vector3 _upperStepOffset;

    [SerializeField]
    private float _stepCheckerDistance;

    [SerializeField]
    private float _stepForce;

    [SerializeField]
    private Transform _climbDetector;

    [SerializeField]
    private float _climbCheckDistance;

    [SerializeField]
    private LayerMask _climbableLayer;

    [SerializeField]
    private Vector3 _climboffset;

    [SerializeField]
    private float _climbSpeed;

    private PlayerStance _playerStance;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _speed = _walkSpeed;
        _Input.OnJumpInput += Jump;
        _playerStance = PlayerStance.Stand;
    }


   private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = Vector3.zero;

        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;

        if (isPlayerClimbing)
        {
            Vector3 horizontal = axisDirection.x * transform.right;
            Vector3 vertical = axisDirection.y * transform.up;
            movementDirection = horizontal + vertical;
            _rigidbody.AddForce(movementDirection * _climbSpeed);
        }
        else if (axisDirection.magnitude >= 0.1f)
        {
            float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                rotationAngle,
                ref _rotationSmoothVelocity,
                _rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
            _rigidbody.AddForce(movementDirection * _speed);
        }
    }

    private void Start()
    {
        _Input.OnMoveInput += Move;
        _Input.OnSprintInput += Sprint;
        _Input.OnClimbInput += StartClimb;
        _Input.OnCancelClimb += CancelClimb;
    }

    private void Update()
    {
        CheckIsGrounded();
        CheckStep();
    }

    private void OnDestroy()
    {
        _Input.OnMoveInput -= Move;
        _Input.OnSprintInput -= Sprint;
        _Input.OnJumpInput -= Jump;
        _Input.OnClimbInput -= StartClimb;
        _Input.OnCancelClimb -= CancelClimb;

    }

    private void Sprint(bool isSprint)
    {
        if (isSprint)
        {
            if (_speed < _sprintSpeed)
            {
                _speed = _speed + _walkSprintTransistion * Time.deltaTime;
            }
        }

        else
        {
            if (_speed > _sprintSpeed)
            {
                _speed = _speed - _walkSprintTransistion * Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            Vector3 jumpDirection = Vector3.up;
            _rigidbody.AddForce(jumpDirection * _jumpForce);
        }
    }

    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
    }

    private void CheckStep()
    {
        bool isHitLowerStep = Physics.Raycast(_groundDetector.position, transform.forward, _stepCheckerDistance, _groundLayer);
        bool isHitUpperStep = Physics.Raycast(_groundDetector.position + _upperStepOffset, transform.forward, _stepCheckerDistance, _groundLayer);

        if (isHitLowerStep && !isHitUpperStep)
        {
            _rigidbody.AddForce(new Vector3(0, _stepForce, 0));
        }
    }

    private void StartClimb()
    {
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayer);
        bool isNotClimbing = _playerStance != PlayerStance.Climb;

        if (isInFrontOfClimbingWall && _isGrounded && isNotClimbing)
        {
            Vector3 offset = (transform.forward * _climboffset.z) + (Vector3.up * _climboffset.y);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;
        }
    }

    private void CancelClimb()
    {
        if (_playerStance == PlayerStance.Climb)
        {
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * 1f;
        }
    }
}
