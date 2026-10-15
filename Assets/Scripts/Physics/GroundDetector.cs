using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize = new Vector2(.7f, .08f);
    [SerializeField, Min(0.01f)] private float castDistance = 0.08f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool logGroundChanges = true;

    private bool? previousGroundState;

    public bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,
            boxSize,
            0f,
            Vector2.down,
            castDistance,
            groundLayer);

        return hit.collider != null;
    }

    private void FixedUpdate()
    {
        if (!logGroundChanges)
            return;

        bool grounded = IsGrounded();

        if (previousGroundState != grounded)
        {
            Debug.Log(
                grounded ? "Player: grounded" : "Player: airbone",
                this);

            previousGroundState = grounded;

        }
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, boxSize);

        Gizmos.color = Color.green;
        Vector3 endPosition =
            groundCheck.position + Vector3.down * castDistance;

        Gizmos.DrawWireCube(endPosition, boxSize);
    }
}
