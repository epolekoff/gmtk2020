using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item
{
    public Collider SlashRange;
    public GameObject SwordGlow;

    private const float SlashArcOffsetFromTarget = 2f;
    private const float SlashHeight = 0.5f;
    private const float SlashCooldown = 1.5f;

    private const float SlashArcReachFirstPointSeconds = 0.15f;
    private const float SlashArcReachSecondPointSeconds = 0.15f;
    private const float SlashArcReturnSeconds = 0.15f;

    private bool m_isSlashing;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody.isKinematic = true;
    }

    /// <summary>
    /// When grabbed, enable the slash range.
    /// </summary>
    protected override void OnGrabbed()
    {
        SlashRange.enabled = true;
        Destroy(SwordGlow);
    }

    protected override void OnDropped()
    {
        SlashRange.enabled = false;
    }

    void OnTriggerStay(Collider col)
    {
        // Check the cooldown before slashing again.
        if(m_itemCooldownTimer > 0)
        {
            return;
        }

        // Cannot slash twice.
        if (m_isSlashing)
        {
            return;
        }

        // Get the target.
        ISlashable slashable = col.GetComponentInParent<ISlashable>();
        if (slashable != null)
        {
            if(slashable.CanBeSlashed())
            {
                StartSlash(slashable);
            }
        }
    }

    private void StartSlash(ISlashable target)
    {
        m_isSlashing = true;
        m_heldHand.CurrentState = HandState.UsingItem;
        StartCoroutine(SlashCoroutine(target));
    }

    private IEnumerator SlashCoroutine(ISlashable target)
    {
        // Get the slash arc.
        GameObject targetGameObject = target.GetGameObject();
        Vector3 playerToTargetVector = targetGameObject.transform.position - m_heldHand.Owner.transform.position;
        Quaternion workingRotation = Quaternion.FromToRotation(Vector3.forward, playerToTargetVector);
        Vector3 targetSlashPoint = targetGameObject.transform.position + (Vector3.up * SlashHeight);
        Vector3 firstPoint = targetSlashPoint + (workingRotation * (SlashArcOffsetFromTarget * Vector3.left));
        Vector3 secondPoint = targetSlashPoint + (workingRotation * (SlashArcOffsetFromTarget * Vector3.right));
        Quaternion slashingSwordRotation = Quaternion.LookRotation(Vector3.down, playerToTargetVector);

        // Turn off hand physics.
        m_heldHand.SetPhysicsEnabled(false);


        // Move the hand to the first point.
        float timer = 0;
        Vector3 handStartPosition = m_heldHand.transform.position;
        Quaternion startingRotation = transform.rotation;
        while (timer < SlashArcReachFirstPointSeconds)
        {
            timer += Time.deltaTime;
            float t = timer / SlashArcReachFirstPointSeconds;
            
            m_heldHand.transform.position = Vector3.Lerp(handStartPosition, firstPoint, t);
            transform.rotation = Quaternion.Lerp(startingRotation, slashingSwordRotation, t);

            yield return null;
        }

        // Slash the target
        target.OnSlashed(secondPoint - firstPoint);


        // Move the hand to the second point.
        timer = 0;
        while (timer < SlashArcReachSecondPointSeconds)
        {
            timer += Time.deltaTime;
            float t = timer / SlashArcReachSecondPointSeconds;
            
            m_heldHand.transform.position = Vector3.Lerp(firstPoint, secondPoint, t);

            yield return null;
        }


        // Return to the resting point.
        timer = 0;
        Vector3 handStartingLocalPosition = m_heldHand.transform.localPosition;
        while (timer < SlashArcReturnSeconds)
        {
            timer += Time.deltaTime;
            float t = timer / SlashArcReturnSeconds;

            m_heldHand.transform.localPosition = Vector3.Lerp(handStartingLocalPosition, Vector3.zero, t);
            transform.rotation = Quaternion.Lerp(slashingSwordRotation, startingRotation, t);

            yield return null;
        }

        //Reset vars.
        m_heldHand.CurrentState = HandState.Resting;
        m_isSlashing = false;
        m_itemCooldownTimer = SlashCooldown;
    }
}
