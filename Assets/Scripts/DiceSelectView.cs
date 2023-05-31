using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DiceSelectView : MonoBehaviour
{
    public Vector3 viewOffset = new Vector3(0f, 0f, 0f);
    public Vector3 viewRotation = new Vector3(0f, 0f, 0f);
    public Vector3 viewSpan = new Vector3(0f, 0f, 1.5f);
    public Vector3 viewScale = new Vector3(2f, 2f, 2f);

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = layer;
        }
    }

    public void SetSelectView()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        gameObject.transform.position = viewOffset;
        gameObject.transform.rotation = Quaternion.Euler(viewRotation);
        gameObject.transform.localScale = viewScale;
    }

    public void InitSelectPositionMove(int i)
    {
        viewOffset += viewSpan*i;
    }

    public void FlipDice()
    {
        gameObject.transform.Rotate(new Vector3(180, -90, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
