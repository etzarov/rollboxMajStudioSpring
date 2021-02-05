using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateExplode : MonoBehaviour
{
    public Rigidbody2D[] allPieces;
    SpriteRenderer[] srs;
    public float explodeForce;
    public float torqueForce;
    public AnimationCurve opacityCurve;
    public float lifetime;
    float[] lifetimes;
    float t;
    // Start is called before the first frame update
    void Start()
    {
        lifetimes = new float[allPieces.Length];
        srs = new SpriteRenderer[allPieces.Length];
        for (int i = 0; i < lifetimes.Length; i++)
        {
            srs[i] = allPieces[i].GetComponent<SpriteRenderer>();
            lifetimes[i] = lifetime + Random.Range(0f, 1f);
        }
        foreach (Rigidbody2D rb in allPieces)
        {
            PolygonCollider2D coll = rb.GetComponent<PolygonCollider2D>();
            Vector2 centerPos = coll.bounds.center;
            rb.AddForceAtPosition((centerPos - (Vector2)transform.position).normalized * (explodeForce*Random.Range(.9f,1.1f)), transform.position, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torqueForce, torqueForce));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        for (int i = 0; i < allPieces.Length; i++)
        {
            t += Time.fixedDeltaTime;
            float pct = t / lifetimes[i];
            float scale = opacityCurve.Evaluate(pct);
            Color c = Color.white;
            c.a = scale;
            srs[i].color = c;
        }

        if (t > lifetime + 1)
        {
            Destroy(this.gameObject);
        }
    }
}
