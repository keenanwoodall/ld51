using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Player : Character
{
    public static Player Instance;

    public Transform swordContainer;
    public PlayerInput playerInput;

    public override CharacterInput CharacterInput => playerInput;
    
    private void Awake()
    {
        Instance = this;
        playerInput.PickupSword += OnPickupSwordInput;
        playerInput.DropSword += OnDropSwordInput;
    }
    
    private void OnDropSwordInput()
    {
        Sword.Instance.Drop();
    }

    private void OnPickupSwordInput()
    {
        Sword.Instance.Pickup(swordContainer);
    }
}