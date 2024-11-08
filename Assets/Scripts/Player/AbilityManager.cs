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