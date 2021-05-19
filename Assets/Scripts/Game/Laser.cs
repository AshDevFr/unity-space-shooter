using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private float _topLimit = 8.0f;

    void Update()
    {
        Move();
        OutOfBounds();
    }

    private void Move()
    {
        float speedScale = _speed * Time.deltaTime;

        transform.Translate(Vector3.up * speedScale);
    }

    private void OutOfBounds()
    {
        if (transform.position.y > _topLimit)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
