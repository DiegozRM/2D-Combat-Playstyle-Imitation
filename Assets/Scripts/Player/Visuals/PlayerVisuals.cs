using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform facingRoot;

    private PlayerMotor playerMotor;
    private float originalScaleX;
    private bool facingLocked;
    private int lockedDirection;

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();

        if (playerMotor == null || facingRoot == null)
        {
            Debug.LogError(
                "PlayerVisuals requires a PlayerMotor and a FacingRoot.",
                this
            );

            enabled = false;
            return;
        }

        originalScaleX = Mathf.Abs(facingRoot.localScale.x);

        if (originalScaleX == 0f)
        {
            Debug.LogError(
                "FacingRoot scale X must be different from zero.",
                this
            );

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        ApplyFacing(
            facingLocked ? lockedDirection : playerMotor.FacingDirection
        );
    }

    public void LockFacing(int direction)
    {
        lockedDirection = direction >= 0 ? 1 : -1;
        facingLocked = true;
        ApplyFacing(lockedDirection);
    }

    public void UnlockFacing()
    {
        facingLocked = false;
        ApplyFacing(playerMotor.FacingDirection);
    }

    private void ApplyFacing(int direction)
    {
        Vector3 scale = facingRoot.localScale;
        scale.x = originalScaleX * direction;
        facingRoot.localScale = scale;
    }
}