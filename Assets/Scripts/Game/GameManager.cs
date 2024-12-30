using UnityEngine;

public class GameManager : MonoBehaviour
{
    MovementSubscription GetInput;

    private void Awake()
    {
        GetInput = GetComponent<MovementSubscription>();
    }
}
