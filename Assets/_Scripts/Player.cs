using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour le nouveau Input System

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;

    [Header("Movement Settings")]
    public float speed;
    public Vector2 moveInput;
    public int facingDirection = 1; // 1 pour la droite, -1 pour la gauche

    private void Update()
    {
        // On vérifie à chaque frame s'il faut retourner le personnage
        Flip();
    }

    private void FixedUpdate()
    {
        // Calcul de la vitesse cible sur l'axe X
        float targetSpeed = moveInput.x * speed;

        // Application de la vitesse au Rigidbody
        // On conserve la vélocité verticale (y) actuelle pour ne pas interférer avec la gravité
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    // Méthode appelée par le composant Player Input (Action "Move")
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Flip()
    {
        // Si on appuie vers la droite et qu'on ne regarde pas déjà vers la droite
        if (moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        // Si on appuie vers la gauche et qu'on ne regarde pas déjà vers la gauche
        else if (moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }

        // Appliquer la direction au scale local du transform pour retourner le sprite et ses enfants
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }
}