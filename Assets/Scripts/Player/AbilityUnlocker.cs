using UnityEngine;

public class AbilityUnlocker : MonoBehaviour
{
    public enum AbilityType { DoubleJump, Glide, Dash, Break }
    public AbilityType abilityToUnlock;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (abilityToUnlock)
            {
                case AbilityType.DoubleJump:
                    AbilityManager.Instance.UnlockDoubleJump();
                    break;
                case AbilityType.Glide:
                    AbilityManager.Instance.UnlockGlide();
                    break;
                case AbilityType.Dash:
                    AbilityManager.Instance.UnlockDash();
                    break;
                case AbilityType.Break:
                    AbilityManager.Instance.UnlockBreak();
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}