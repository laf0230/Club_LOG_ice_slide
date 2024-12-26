using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Move : MonoBehaviour
{
    public bool moveable = false;
    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rigid.MovePosition((Vector2)transform.position + Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            rigid.MovePosition((Vector2)transform.position + Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            rigid.MovePosition((Vector2)transform.position + Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rigid.MovePosition((Vector2)transform.position + Vector2.right);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TileBG>())
        {
            moveable = true;
        }
    }
}
