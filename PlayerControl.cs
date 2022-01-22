using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float ACCELERATION = 10.0f;
    public static float SPEED_MIN = 4.0f;
    public static float SPEED_MAX = 8.0f;
    public static float JUMP_HEIGHT_MAX = 3.0f;
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f;

    public static float NARAKU_HEIGHT = -5.0f;

    public enum STEP
    {
        NONE = -1,
        RUN =0,
        JUMP,
        MISS,
        NUM,
    };

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;

    public float step_timer = 0.0f;
    private bool is_landed = false;
    private bool is_colided = false;
    private bool is_key_released = false;
    // Start is called before the first frame update
    void Start()
    {
        this.next_step = STEP.RUN;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
        this.check_landed();

        switch (this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                if(this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS;
                }
                break;
        }

        this.step_timer += Time.deltaTime;

        if(this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.RUN:
                    if(!this.is_landed)
                    {
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            this.next_step = STEP.JUMP;
                        }
                    }
                    break;
                case STEP.JUMP:
                    if (this.is_landed)
                    {
                        this.next_step = STEP.RUN;
                    }
                    break;
                case STEP.MISS:
                    velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                    if(velocity.x < 0.0f)
                    {
                        velocity.x = 0.0f;
                    }
                    break;
            }
        }

        while(this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.JUMP:
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    this.is_key_released = false;
                    break;
            }
            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case STEP.RUN:
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
                if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)
                {
                    velocity.x *= PlayerControl.SPEED_MAX / Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                }
                break;

            case STEP.JUMP:
                do
                {
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break;
                    }
                    if (this.is_key_released)
                    {
                        break;
                    }
                    if (velocity.y <= 0.0f)
                    {
                        break;
                    }
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;

                    this.is_key_released = true;
                } while (false);
                break;
        }
        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    private void check_landed()
    {
        this.is_landed = false;
        do
        {
            Vector3 s = this.transform.position;
            Vector3 e = s + Vector3.down * 1.0f;
            RaycastHit hit;
            if (!Physics.Linecast(s, e, out hit))
            {
                break;
            }

            if (this.step == STEP.JUMP)
            {
                if (this.step_timer < Time.deltaTime * 3.0f)
                {
                    break;
                }
            }
            this.is_landed = true;
        } while (false);
    }
}
