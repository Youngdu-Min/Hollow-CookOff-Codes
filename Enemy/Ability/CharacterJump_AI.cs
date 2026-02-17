using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class CharacterJump_AI : CharacterJump
    {
        GameObject target;

        protected override void Initialization()
        {
            base.Initialization();
            target = GameObject.FindGameObjectWithTag("Player");
        }

        public override void JumpStart()
        {
            if (!EvaluateJumpConditions())
            {
                return;
            }
            
            // we reset our walking speed
            if ((_movement.CurrentState == CharacterStates.MovementStates.Crawling)
                || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
                || (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing))
            {
                _characterHorizontalMovement.ResetHorizontalSpeed();
            }

            if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
            {
                _characterLadder.GetOffTheLadder();
            }

            _controller.ResetColliderSize();

            _lastJumpAt = Time.time;

            // if we're still here, the jump will happen
            // we set our current state to Jumping
            _movement.ChangeState(CharacterStates.MovementStates.Jumping);

            // we trigger a character event
            MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);


            // we start our feedbacks
            if ((_controller.State.IsGrounded) || _coyoteTime)
            {
                PlayAbilityStartFeedbacks();
            }
            else
            {
                AirJumpFeedbacks?.PlayFeedbacks();
            }

            if (ResetCameraOffsetOnJump && (_sceneCamera != null))
            {
                _sceneCamera.ResetLookUpDown();
            }

            if (NumberOfJumpsLeft != NumberOfJumps)
            {
                _doubleJumping = true;
            }

            // we decrease the number of jumps left
            NumberOfJumpsLeft--;

            // we reset our current condition and gravity
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            _controller.GravityActive(true);
            _controller.CollisionsOn();

            // we set our various jump flags and counters
            SetJumpFlags();
            CanJumpStop = true;

            // float distance = Vector3.Distance(this.transform.position, target.transform.position);
            float distance = Mathf.Abs(this.transform.position.y - target.transform.position.y);

            // we make the character jump
            _controller.SetVerticalForce(Mathf.Sqrt(2f * (JumpHeight + distance) * Mathf.Abs(_controller.Parameters.Gravity)));
            JumpHappenedThisFrame = true;

        }
    }
}