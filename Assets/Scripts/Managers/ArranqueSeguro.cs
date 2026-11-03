using UnityEngine;

public class ArranqueSeguro : MonoBehaviour
{
    public GameManager gameManagerPrefab;

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            var gm = Instantiate(gameManagerPrefab);
            gm.NuevaPartida();
        }
    }
}