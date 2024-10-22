using UnityEngine;

public class FlappyFish : MonoBehaviour
{
    public float jumpForce = 200f;
    public GameObject gameOverPanel;

    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isDead && Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pipe") || col.gameObject.CompareTag("Ground"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isDead = true;
        // Show game over panel or handle game over logic here
        gameOverPanel.SetActive(true);
    }
}
