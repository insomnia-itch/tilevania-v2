using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int points = 100;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            FindObjectOfType<GameSession>().IncreaseScore(points);
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }

    // IEnumerator playPickupSound() {

    // }
}
