using UnityEngine;

public class DiceBase : MonoBehaviour
{
    public int diceID;

    public int numDiceFaces = 6;
    public string diceFaceMaterial;

    int[] faces;

    public void SetDiceFaceMaterial(string material)
    {
        diceFaceMaterial = material;
    }

    public Material GetFaceMaterial(int faceIndex)
    {
        return Resources.Load<Material>($"DiceFaces/{diceFaceMaterial}/Materials/material_{faces[faceIndex]+1}");
    }

    public void Initialize(int[] faces)
    {
        this.faces = faces;

        for (int i = 0; i < numDiceFaces; i++)
        {
            GameObject face = transform.Find($"DiceFace_{i + 1}").gameObject;
            face.GetComponent<MeshRenderer>().materials = new Material[] { GetFaceMaterial(i) };
        };
    }

    public int GetFace(int index)
    {
        return faces[index];
    }

    private GameObject GetInstance(bool isThrowDice, bool isDiceSelectView, Player owner)
    {
        GameObject instance = Instantiate(gameObject);
        instance.GetComponent<ThrowDice>().enabled = isThrowDice;
        instance.GetComponent<DiceSelectView>().enabled = isDiceSelectView;

        if ( isDiceSelectView )
        {
            instance.GetComponent<DiceSelectButton>().owner = owner;
        }
        else
        {
            instance.GetComponent<BaseButton>().enabled = false;
            instance.GetComponent<DiceSelectButton>().enabled = false;
        }
        instance.SetActive(true);
        return instance;
    }

    public GameObject GetThrowDice()
    {
        return GetInstance(true, false, null);
    }

    public GameObject GetDiceSelectView(Player owner)
    {
        return GetInstance(false, true, owner);
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
