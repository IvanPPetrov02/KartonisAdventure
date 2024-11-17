using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    public bool CanDoubleJump { get; private set; }
    public bool CanGlide { get; private set; }
    public bool CanDash { get; private set; }
    public bool CanBreak { get; private set; }

    public GameObject doubleJumpIcon;
    public GameObject glideIcon;
    public GameObject dashIcon;
    public GameObject breakIcon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAbilities();
    }

    private void InitializeAbilities()
    {
        ResetAbilities();
    }

    public void ResetAbilities()
    {
        // Lock all abilities
        CanDoubleJump = false;
        CanGlide = false;
        CanDash = false;
        CanBreak = false;

        // Explicitly set all ability icons to inactive (hidden)
        if (doubleJumpIcon != null)
        {
            doubleJumpIcon.SetActive(false);
            Debug.Log("Double Jump Icon is now hidden.");
        }
        if (glideIcon != null)
        {
            glideIcon.SetActive(false);
            Debug.Log("Glide Icon is now hidden.");
        }
        if (dashIcon != null)
        {
            dashIcon.SetActive(false);
            Debug.Log("Dash Icon is now hidden.");
        }
        if (breakIcon != null)
        {
            breakIcon.SetActive(false);
            Debug.Log("Break Icon is now hidden.");
        }

        Debug.Log("Abilities have been reset, and all icons are now hidden.");
    }


    public void UnlockDoubleJump()
    {
        CanDoubleJump = true;
        Debug.Log("Double Jump Unlocked!");
        if (doubleJumpIcon != null) doubleJumpIcon.SetActive(true);
    }

    public void UnlockGlide()
    {
        CanGlide = true;
        Debug.Log("Glide Unlocked!");
        if (glideIcon != null) glideIcon.SetActive(true);
    }

    public void UnlockDash()
    {
        CanDash = true;
        Debug.Log("Dash Unlocked!");
        if (dashIcon != null) dashIcon.SetActive(true);
    }

    public void UnlockBreak()
    {
        CanBreak = true;
        Debug.Log("Break Ability Unlocked!");
        if (breakIcon != null) breakIcon.SetActive(true);
    }
}
