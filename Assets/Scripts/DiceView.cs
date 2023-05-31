using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceView : MonoBehaviour
{
    public Vector3 resultPanelOffset;
    public Vector3 resultPanelVSpan;
    public Vector3 resultPanelHSpan;
    public Vector3 resultPanelScale;
    public GameObject prefabResultPanel;

    GameObject[] diceSelection;

    public void SetSelectDices(GameObject[] diceSelection, int selectedDiceIndex)
    {
        this.diceSelection = diceSelection;
        SelectedDiceToHIghlightLayer(selectedDiceIndex);
    }

    public void SelectedDiceToHIghlightLayer(int selectedDiceIndex)
    {
        for (int i = 0; i < diceSelection.Length; i++)
        {
            if (diceSelection[i] != null && i == selectedDiceIndex) { diceSelection[i].GetComponent<DiceSelectView>().SetLayer(3); }
            else if (diceSelection[i] != null){ diceSelection[i].GetComponent<DiceSelectView>().SetLayer(5); }
        }
    }

    public void ResetDiceFaceFrame() 
    {
        foreach (RectTransform obj in gameObject.transform)
        {
            Destroy(obj.gameObject);
        }
    }

    public Texture2D GetFaceTexture(int faceIndex, string diceFaceMaterial)
    {
        return Resources.Load<Texture2D>($"DiceFaces/{diceFaceMaterial}/images/image_{faceIndex + 1}");
    }

    public void PlayerSetResultDiceFace(int player, int[,] resultFaceIndex, string diceFaceMaterial)
    {
        for (int j = 0; j < resultFaceIndex.GetLength(1); j++)
        {
            if (resultFaceIndex[player, j] >= 0)
            {
                Texture2D resultDiceFace = GetFaceTexture(resultFaceIndex[player, j], diceFaceMaterial);
                GameObject obj = Instantiate(prefabResultPanel);

                obj.GetComponent<Image>().sprite = Sprite.Create(
                    resultDiceFace, new Rect(0, 0, resultDiceFace.width, resultDiceFace.height), Vector2.zero
                    );
                obj.layer = 5;

                obj.transform.SetParent(gameObject.transform, false);

                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localPosition = resultPanelOffset + resultPanelVSpan * player + resultPanelHSpan * j;
                rt.localScale = resultPanelScale;
            }
        }
    }

    public void SetResultDiceFace(int[,] resultFaceIndex, string[] diceFaceMaterial)
    {
        ResetDiceFaceFrame();
        for (int i = 0; i < resultFaceIndex.GetLength(0); i++) 
        {
            PlayerSetResultDiceFace(i, resultFaceIndex, diceFaceMaterial[i]);
        }
    }

    public void DestroyDiceSelection()
    {
        if (!(diceSelection == null || diceSelection.Length == 0))
        {
            foreach (GameObject dice in diceSelection)
            {
                Destroy(dice);
            }
        }
    }

    public void FlipDice()
    {
        foreach (GameObject dice in diceSelection)
        {
            if (dice != null)
            {
                dice.GetComponent<DiceSelectView>().FlipDice();
            }
        }
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
