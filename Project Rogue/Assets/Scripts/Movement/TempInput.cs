using Movement;
using UnityEngine;

public class TempInput : MonoBehaviour
{
    CharacterController2D charControl;

    float InputX;
    bool jumped;
    // Start is called before the first frame update
    void Start()
    {
        charControl = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumped = true;
        }
    }

    private void FixedUpdate()
    {
        charControl.Move(InputX, false, jumped);
        jumped = false;
    }
}
