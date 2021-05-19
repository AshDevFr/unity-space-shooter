using Sirenix.OdinInspector;
using UnityEngine;

public enum PowerupType
{
    TRIPLE_SHOT,
    SPEED,
    SHIELD
}

public class Powerup : MonoBehaviour
{


    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _bottomLimit = -6.0f;
    [Required] [SerializeField] private PowerupType _powerupType;
    [Required] [SerializeField] private AudioClip _powerupAudioClip;

    void Update()
    {
        Move();
        OutOfBounds();
    }

    private void Move()
    {
        float speedScale = _speed * Time.deltaTime;

        transform.Translate(Vector3.down * speedScale);
    }

    private void OutOfBounds()
    {
        if (transform.position.y < _bottomLimit)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                AudioSource.PlayClipAtPoint(_powerupAudioClip, Vector3.zero);
                player.EnablePowerup(_powerupType);
            }
            
            Destroy(gameObject);
        }
    }

}