using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeVisual : MonoBehaviour
{
    public Employee brains;

    public SpriteRenderer body;
    //public SpriteRenderer eyes;
    //public SpriteRenderer tie;

    private const float wobbleAngle = 15f;
    private bool wobbling = false;
    private float wobbleTimer = 0f;
    private const float wobbleInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        body.sprite = Game.instance.office.sprites[Random.Range(0, Game.instance.office.sprites.Count)];
        //eyes.color = Game.instance.office.eyeColors[Random.Range(0, Game.instance.office.eyeColors.Count)];
        //tie.color = Game.instance.office.tieColors[Random.Range(0, Game.instance.office.tieColors.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        if( wobbling && !Game.paused )
        {
            wobbleTimer += Time.deltaTime;
            if ( wobbleTimer > wobbleInterval )
                wobbleTimer -= wobbleInterval;
            float eval = Game.instance.wobbleCurve.Evaluate(wobbleTimer / wobbleInterval);
            transform.localRotation = Quaternion.Euler(0, 0, wobbleAngle * eval);
        }
    }

    public void SetWobble( bool wobble )
    {
        wobbling = wobble;
        if ( !wobbling )
            transform.localRotation = Quaternion.identity;
    }

}
