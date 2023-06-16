using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    bool isListening = true;

    bool isConfirm = false;
    bool isReject = false;
    bool isPlus = false;
    bool isMinus = false;

    bool isDice1Touched = false;
    bool isDice2Touched = false;
    bool isDice3Touched = false;

    public void ResetButtons()
    {
        isConfirm = false;
        isReject = false;
        isPlus = false;
        isMinus = false;

        isDice1Touched = false;
        isDice2Touched = false;
        isDice3Touched = false;
    }

    public void SetConfirm() { if(isListening) { isConfirm = true; } }
    public void SetReject() { if (isListening) { isReject = true; } }
    public void SetPlus() { if (isListening) { isPlus = true; } }
    public void SetMinus() { if (isListening) { isMinus = true; } }

    public void SetDice1Touched() { if (isListening) { isDice1Touched = true; } }
    public void SetDice2Touched() { if (isListening) { isDice2Touched = true; } }
    public void SetDice3Touched() { if (isListening) { isDice3Touched = true; } }

    public bool GetConfirm() { return isConfirm; }
    public bool GetReject() { return isReject; }
    public bool GetPlus() { return isPlus; }
    public bool GetMinus() { return isMinus; }

    public bool GetDice1Touched() { return isDice1Touched; }
    public bool GetDice2Touched() { return isDice2Touched; }
    public bool GetDice3Touched() { return isDice3Touched; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isListening = !(
            isConfirm && isReject && isPlus && isMinus && 
            isDice1Touched && isDice2Touched && isDice3Touched);
    }
}
