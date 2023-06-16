using System;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    static System.Random randomGenerator = new System.Random();

    int tendency;

    public void SetTendency() 
    {
        tendency = randomGenerator.Next(1, 3);
    }

    public int RandomSelectDice(int numDice)
    {
        return randomGenerator.Next(0, numDice);
    }

    public int RandomRaise()
    {
        return (int)(randomGenerator.Next(0, tendency+1) / tendency);
    }

    public string RandomName() 
    {
        Guid g = System.Guid.NewGuid();
        return g.ToString("N").Substring(0, 8);
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
