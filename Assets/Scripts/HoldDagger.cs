using UnityEngine;

public class HoldDagger : MonoBehaviour
{
    [SerializeField] GameObject dagger, hand;

    // Update is called once per frame
    void Update()
    {
        dagger.transform.position = hand.transform.position;
    }
}
