﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhitePlayer : MonoBehaviour
{
    public Collider2D bulletCollider;
    Quaternion bulletRotation;
    public GameObject[] bullets;

    public Camera cam;
    public GameObject bullet;

    public Vector3 respawn = new Vector3(3.0f, -4.0f, -1.0f);
    int lives;

    BoxCollider2D boxCol;

    public LayerMask groundLayer;
    public float speed;
    Vector2 position;
    Vector2 direction;
    Rigidbody2D rb;
    bool doubleBounced = false;
    float jumpHeight = 1.0f;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start(){
        cam = Camera.main;
        position = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        lives = 10;
    }

    // Update is called once per frame
    void Update(){
        DoMovement();
        GotShot();

        // '/' to shoot
        if (Input.GetKeyDown(KeyCode.Slash)) {
            DoShoot();
        }

        //turn grey if you lose yourself
        if (Input.GetKey(KeyCode.DownArrow))
        {
            spriteRenderer.color = Color.grey;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

        if (lives <= 0)
        {
            DoRespawn();
        }

        if(transform.position.y <= -6f) {
            DoRespawn();
        }


    }

    //I think it's pretty obvious, but this is where we control the shooting
    private void DoShoot() {
        //Quaternion  rotation = new Quaternion(0,0,180,0);
        Instantiate(bullet, transform.position, bulletRotation);
    }

    //check if we got shot 
    private void GotShot() {
        bullets = GameObject.FindGameObjectsWithTag("bullet");

        //go through all the bullets that currently exist
        foreach (GameObject b in bullets)
        {

            bulletCollider = b.GetComponent<Collider2D>();
            if (bulletCollider.IsTouching(boxCol)){
                //u got shot
                Debug.Log("white got shot");
                lives--;
            }
        }
    }

    void DoRespawn() {
        transform.position = respawn;
        lives = 10;
    }




    private void DoMovement()
    {
        if (IsGrounded() && rb.velocity.x < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x + (0 + Math.Abs(rb.velocity.x)) * 0.1f, rb.velocity.y);
        }
        else if (IsGrounded() && rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x - (rb.velocity.x * 0.1f), rb.velocity.y);
        }

        if (IsGrounded() && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, speed * jumpHeight);
        }

        //double bounce
        else if (Input.GetKeyDown(KeyCode.UpArrow) && doubleBounced == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed * 1.3f);
            doubleBounced = true;
        }
        //getKey rather than getKey down so you can hold it down
        if (Input.GetKey(KeyCode.RightArrow))
        {
            bulletRotation = new Quaternion(0, 0, 0, 0);
            if (IsGrounded())
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(speed * 0.8f, rb.velocity.y);
            }

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            bulletRotation = new Quaternion(0, 0, 180, 0);
            if (IsGrounded())
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-speed * 0.8f, rb.velocity.y);
            }
        }

    }

    bool IsGrounded(){
        position = transform.position;
        direction = Vector2.down;
        float distance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null){
            doubleBounced = false;
            return true;
        }
        return false;
    }


    //when they go off the screen, make them come back the other side
    void OnBecameInvisible() {
        //Debug.Log("invisible");
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        Vector3 newPos = transform.position; 
        if (viewportPos.x > 0.99f) {
            //Debug.Log("working");
            newPos = cam.ViewportToWorldPoint(new Vector3(0.001f, viewportPos.y, viewportPos.z));
            transform.position = newPos;
        }
        else if (viewportPos.x < 0.01f){
            //Debug.Log("working");
            newPos = cam.ViewportToWorldPoint(new Vector3(0.99f, viewportPos.y, viewportPos.z));
            transform.position = newPos;
        }
    }



}

