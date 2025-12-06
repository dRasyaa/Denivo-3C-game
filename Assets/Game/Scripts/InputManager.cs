using System; 
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCancelClimb;
    public Action OnChangePOV;

    private void Update()
    {
        CheckMovementInput();
        CheckJumpInput();
        CheckOnsprintInput();
        CheckClimbInput();
        CheckCancelInput();
        CheckChangePOVInput();
    }

    // Update is called once per frame
    private void CheckMovementInput()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        Vector2 inputAxis = new Vector2(horizontalAxis, verticalAxis);

        if (OnMoveInput != null)
        {
            OnMoveInput(inputAxis);
        }

    }

    private void CheckJumpInput()
    {
        bool IsPressJumpInput = Input.GetKeyDown(KeyCode.Space);

        if (IsPressJumpInput)
        {
            if (OnJumpInput != null)
            {
                OnJumpInput();
            }
        }
    }

    private void CheckOnsprintInput()
    {
        bool isHoldSprintInput = Input.GetKey(KeyCode.LeftShift);
        if (isHoldSprintInput)
        {
            if (OnSprintInput != null)
            {
                OnSprintInput(true);
            }
        }
        else
        {
            if (OnSprintInput != null)
            {
                OnSprintInput(false);
            }
        }
    }

    private void CheckClimbInput()
    {
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);

        if (isPressClimbInput)
        {
            OnClimbInput();
        }
    }

    private void CheckCancelInput()
    {
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);

        if (isPressCancelInput)
        {
            if (OnCancelClimb != null)
            {
                OnCancelClimb();
            }
        }
    }

    private void CheckChangePOVInput()
    {
        bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);
 
        if (isPressChangePOVInput)
        {
            if (OnChangePOV != null)
            {
                OnChangePOV();
            }
        }
    }
}
