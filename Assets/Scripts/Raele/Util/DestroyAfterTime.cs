using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float TimeToDestroy;

    void Start()
        => Destroy(this.gameObject, this.TimeToDestroy);
}
