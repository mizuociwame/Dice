using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBase : MonoBehaviour
{
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

    private GameObject GetInstance(bool isThrowDice, bool isDiceSelectView)
    {
        GameObject instance = Instantiate(gameObject);
        instance.GetComponent<ThrowDice>().enabled = isThrowDice;
        instance.GetComponent<DiceSelectView>().enabled = isDiceSelectView;
        instance.SetActive(true);
        return instance;
    }

    public GameObject GetThrowDice()
    {
        return GetInstance(true, false);
    }

    public GameObject GetDiceSelectView()
    {
        return GetInstance(false, true);
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
