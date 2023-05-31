using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    static System.Random randomGenerator = new System.Random();

    public int RandomSelectDice(int numDice)
    {
        return randomGenerator.Next(0, numDice);
    }

    public int RandomRaise()
    {
        return randomGenerator.Next(0, 2);
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
