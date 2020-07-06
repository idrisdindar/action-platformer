using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IDamageable
{
    
    private Rigidbody2D _rigid;
    [SerializeField]
    private float _jumpForce = 5.0f;
    private bool _resetJump = false;
    private bool _grounded = false;
    private bool jumpAttack = false;
    [SerializeField]
    private float _speed = 1.0f;
    private PlayerAnimation _playerAnim;
    private Animator anim;
    private SpriteRenderer _playerSprite;
    private SpriteRenderer _swordArcSprite;

    public int Health { get; set; }
    public int diamonds;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<PlayerAnimation>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _swordArcSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        Health = 4;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (CrossPlatformInputManager.GetButtonDown("A_Button") && isGrounded() == true)
        {
            _playerAnim.Attack();
        }
        



    }

    void Movement()
    {
        if (Health <= 0)
            return;

        float move = CrossPlatformInputManager.GetAxis("Horizontal");
        _grounded = isGrounded();

        if (move > 0)
        {
            Flip(true);
        }
        else if (move < 0)
        {
            Flip(false);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("B_Button"))  && isGrounded() == true)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpForce);
            StartCoroutine(ResetJumpRoutine());
            _playerAnim.Jump(true);
        }
        _rigid.velocity = new Vector2(move * _speed, _rigid.velocity.y);

        _playerAnim.Move(move);
    }

    bool isGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, 1 << 8);

        if (hitInfo.collider != null)
        {
            if (_resetJump == false)
            {
                _playerAnim.Jump(false);
                return true;
            }
        }
        return false;
    }

    void Flip(bool faceRight)
    {
        if (faceRight == true)
        {
            _playerSprite.flipX = false;
            _swordArcSprite.flipX = false;
            _swordArcSprite.flipY = false;

            Vector3 newPos = _swordArcSprite.transform.localPosition;
            newPos.x = 1.01f;
            _swordArcSprite.transform.localPosition = newPos;
        }
            
        else if (faceRight == false)
        {
            _playerSprite.flipX = true;
            _swordArcSprite.flipX = true;
            _swordArcSprite.flipY = true;

            Vector3 newPos = _swordArcSprite.transform.localPosition;
            newPos.x = -1.01f;
            _swordArcSprite.transform.localPosition = newPos;
        }
            
    }

    IEnumerator ResetJumpRoutine()
    {
        _resetJump = true;
        yield return new WaitForSeconds(0.1f);
        _resetJump = false;
    }
    public void Damage()
    {
        if (Health < 1)
            return;

        UnityEngine.Debug.Log("Player::Damage()");
        Health--;
        UIManager.Instance.UpdateLives(Health);

        if(Health < 1)
        {
            _playerAnim.Death();
        }
        else
            _playerAnim.Hit();
    }
    public void AddGems(int amount)
    {
        diamonds += amount;
        UIManager.Instance.UpdateGemCount(diamonds);
    }

}
