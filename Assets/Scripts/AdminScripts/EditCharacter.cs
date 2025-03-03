using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using onnaMUD.BaseClasses;
using TMPro;
using System.Security.Policy;
using System;

public class EditCharacter : MonoBehaviour
{
    public TMP_InputField characterID;
    public TMP_InputField accountID;
    public TMP_InputField charName;
    public TMP_Dropdown raceDD;

    // Start is called before the first frame update
    void Start()
    {
        raceDD.ClearOptions();
        
        List<string> raceDDOptions = new List<string>();
        for (int i = 0; i < Enum.GetNames(typeof(Races)).Length; i++)
        {
            raceDDOptions.Add(((Races)i).ToString());

        }
        raceDD.AddOptions(raceDDOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillForm(Character charToEdit)
    {
        characterID.text = charToEdit.Id.ToString();
        accountID.text = charToEdit.AccountID.ToString();
        charName.text = charToEdit.Name;
        raceDD.value = (int)charToEdit.Race;
    }

    public Character SaveCharacter()
    {
        Character charEdit = new Character();
        charEdit.Id = new Guid(characterID.text);
        charEdit.AccountID = new Guid(accountID.text);
        charEdit.Name = charName.text;
        charEdit.Race = (Races)raceDD.value;

        return charEdit;
    }
}
