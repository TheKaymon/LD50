using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBubble : MonoBehaviour
{
    public SpriteRenderer sr;
    public GameObject text;
    public AnimationCurve sizeCurve;

    //private float speed = 1.25f;
    private float lifeTime = 1.5f;
    private float timer = 0;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ( timer > lifeTime )
            Destroy(gameObject);
        else
        {
            UpdateSize();
        }

    }

    public void Initialize( float duration, bool urgent, bool flipped, int debug ) //color
    {
        lifeTime = duration;
        UpdateSize();

        text.SetActive(true);
        text.GetComponent<TextMeshPro>().SetText(debug.ToString());

        if ( flipped )
            sr.flipX = true;

        if ( urgent )
        {
            sr.color = Color.red;
            text.SetActive(true);
            //Audio.instance.PlaySFX(pointsLost, transform.position);
        }
        else
            sr.color = Color.grey;
    }

    public void Remove()
    {
        timer = lifeTime;
    }

    private void UpdateSize()
    {
        float size = sizeCurve.Evaluate(timer / lifeTime);
        transform.localScale = new Vector3(size, size, 1);
    }
}
