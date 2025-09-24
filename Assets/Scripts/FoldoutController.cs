using onnaMUD;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoldoutController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public GameObject collapseButton;
    public GameObject expandButton;
    public RectTransform content;
    public bool isExpanded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExpandContent(bool isExpanded)
    {
        this.isExpanded = isExpanded;
        //enable/disable arrow buttons as needed
        if (isExpanded)
        {
            //if we are expanded out, turn off expand button and turn on collapse button
            expandButton.gameObject.SetActive(false);
            collapseButton.gameObject.SetActive(true);
            content.gameObject.SetActive(true);
        }
        else
        {
            //we are collapsed down, need to turn on expand button, turn off collapse button
            expandButton.gameObject.SetActive(true);
            collapseButton.gameObject.SetActive(false);
            content.gameObject.SetActive(false);
        }

//        accountsListObject.SetActive(isExpanded);
//        addAccountButton.SetActive(isExpanded);
        //MainController.instance.ListServers(true);
//        MainController.instance.AdjustServerWindow();
        //expandAccountListCO = ExpandAccountListCO(isExpanded);
        //StartCoroutine(expandAccountListCO);
        //Debug.Log(isExpanded);
    }

    public IEnumerator ExpandContentCO(bool isExpanded)//?
    {
//        accountsListObject.SetActive(isExpanded);
//        addAccountButton.SetActive(isExpanded);
        /*       if (isExpanded)
               {
                   //Debug.Log("accounts list should be expanded");
                   accountsListObject.SetActive(true);
                   //accountListLayout.minHeight = 0;
               } else
               {
                   //Debug.Log("accounts list should be collapsed down");
                   accountsListObject.SetActive(false);
               }*/
        yield return new WaitForEndOfFrame();
        MainController.instance.AdjustServerWindow();
        //expandAccountListCO = null;
        yield return null;

    }

}
