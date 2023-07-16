using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform paddleMin;
    [SerializeField] private Transform paddleMax;
    [SerializeField] private Transform ballTransform;
    
    private float baseSpeed = 5f;
    private float speedVariation = 2f;

    private float speed;
    private Difficulty difficulty = Difficulty.normal;

    public enum Difficulty
    {
        easy,
        normal,
        hard,
        impossible
    }

    public void SetDifficulty(int level)
    {
        difficulty = (Difficulty) level;

        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.easy:
                baseSpeed = 2f;
                speedVariation = 1f;
                break;
            case Difficulty.normal:
                baseSpeed = 4f;
                speedVariation = 1.5f;
                break;
            case Difficulty.hard:
                baseSpeed = 5f;
                speedVariation = 2f;
                break;
            case Difficulty.impossible:
                baseSpeed = 50f;
                speedVariation = 0f;
                break;
        }

        speed = baseSpeed;
    }

    private void Start()
    {
        UpdateDifficulty();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position,
            new(Mathf.Clamp(ballTransform.position.x, paddleMin.position.x, paddleMax.position.x), Mathf.Clamp(ballTransform.position.y, paddleMin.position.y, paddleMax.position.y), transform.position.z), Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
