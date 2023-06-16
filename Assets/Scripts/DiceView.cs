using UnityEngine;
using UnityEngine.UI;

public class DiceView : MonoBehaviour
{
    public Vector3 resultPanelOffset;
    public Vector3 resultPanelVSpan;
    public Vector3 resultPanelHSpan;
    public Vector3 resultPanelScale;
    public GameObject prefabResultPanel;

    DiceSelectView[] diceSelection;

    public void SetSelectDices(DiceSelectView[] diceSelection, int selectedDiceIndex)
    {
        this.diceSelection = diceSelection;
        SelectedDiceToHIghlightLayer(selectedDiceIndex);
    }

    public void SelectedDiceToHIghlightLayer(int selectedDiceIndex)
    {
        for (int i = 0; i < diceSelection.Length; i++)
        {
            if (diceSelection[i] != null && i == selectedDiceIndex) { diceSelection[i].SetLayer(3); }
            else if (diceSelection[i] != null){ diceSelection[i].SetLayer(5); }
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

    public void SetResultDiceFace(int position, int[] resultFaceIndex, string[] diceFaceMaterial)
    {
        for (int j = 0; j < resultFaceIndex.Length; j++)
        {
            if (resultFaceIndex[j] >= 0)
            {
                Texture2D resultDiceFace = GetFaceTexture(resultFaceIndex[j], diceFaceMaterial[j]);
                GameObject obj = Instantiate(prefabResultPanel);

                obj.GetComponent<Image>().sprite = Sprite.Create(
                    resultDiceFace, new Rect(0, 0, resultDiceFace.width, resultDiceFace.height), Vector2.zero
                    );
                obj.layer = 5;

                obj.transform.SetParent(gameObject.transform, false);

                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localPosition = resultPanelOffset + resultPanelVSpan * position + resultPanelHSpan * j;
                rt.localScale = resultPanelScale;
            }
        }
    }

    public void DestroyDiceSelection()
    {
        if (!(diceSelection == null || diceSelection.Length == 0))
        {
            foreach (DiceSelectView dice in diceSelection)
            {
                if (dice != null) { Destroy(dice.gameObject); }
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
