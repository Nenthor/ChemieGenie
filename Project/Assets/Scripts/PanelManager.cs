using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject gmClickCatcher;
    [SerializeField] private GameObject gmInformation;
    [SerializeField] private GameObject gmDetails;
    [SerializeField] private GameObject gmVisual;
    [SerializeField] private GameObject gmElectron;
    [SerializeField] private GameObject gmCloud;

    private Element currentElement;

    private IEnumerator OpenPanel(GameObject panel, Var.DisplayState newState)
    {
        //Ativate Panel + ClickCatcher
        gmClickCatcher.SetActive(true);
        panel.SetActive(true);
        panel.GetComponent<Animator>().SetBool("isOpen", true); //Start open-animation

        yield return new WaitForSeconds(1.5f); //Wait until the animatioon is finished

        Var.State = newState; //Change state
    }

    private IEnumerator ClosePanel(GameObject panel)
    {
        Var.State = Var.DisplayState.displayElements; //Change state
        panel.GetComponent<Animator>().SetBool("isOpen", false); //Start close-animation

        yield return new WaitForSeconds(1.5f); //Wait until the animatioon is finished

        //Deactivate ClickCatcher + Panel
        gmClickCatcher.SetActive(false);
        panel.SetActive(false);
    }

    //Information Section
    public void OpenInformation()
    {
        StartCoroutine(OpenPanel(gmInformation, Var.DisplayState.displaySettings));
        SetupInformation();
    }

    public void CloseInformation()
    {
        StartCoroutine(ClosePanel(gmInformation));
    }

    private void SetupInformation()
    {
        Transform informationBox = gmInformation.transform.Find("InformationBox");

        //Turn off every information text
        for (int child = 0; child < informationBox.childCount; child++)
        {
            informationBox.GetChild(child).gameObject.SetActive(false);
        }

        //Turn on Texts
        switch (Var.Model)
        {
            case Var.ModelState.shellModel:
                informationBox.Find("ShellmodelText").gameObject.SetActive(true);
                break;
            case Var.ModelState.energyModel:
                informationBox.Find("EnergymodelText").gameObject.SetActive(true);
                break;
            case Var.ModelState.formModel:
                informationBox.Find("FormmodelText").gameObject.SetActive(true);
                break;
            case Var.ModelState.cloudModel:
                informationBox.Find("CloudmodelText").gameObject.SetActive(true);
                break;
        }

        //Set Version
        Transform version = informationBox.Find("Version");
        version.GetComponent<TextMeshProUGUI>().text = "David Nentwich  –  v." + Application.version;
        version.gameObject.SetActive(true);
    }

    //Details Section
    public void OpenDetails(int elementID)
    {
        StartCoroutine(OpenPanel(gmDetails, Var.DisplayState.displayDetails));
        SetupDetails(elementID);
    }

    public void CloseDetails()
    {
        StartCoroutine(ClosePanel(gmDetails));
        StartCoroutine(ClearDetailsBox());
    }

    private void SetupDetails(int elementID)
    {
        //Get values
        Transform stats = gmDetails.transform.Find("DetailsBox").Find("StatsBox");
        Values values = Var.Data.Stats[elementID];

        //Set title
        gmDetails.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = values.Name + ":";

        //Set ElementView
        SetupDetailsBox(elementID);

        //Set values
        stats.Find("Electrons").GetComponent<TextMeshProUGUI>().text = "Elektronen: " + values.Electrons;
        stats.Find("Neutrons").GetComponent<TextMeshProUGUI>().text = "Neutronen: " + values.Neutrons;
        stats.Find("Protons").GetComponent<TextMeshProUGUI>().text = "Protonen: " + values.Protons;
        stats.Find("Radius").GetComponent<TextMeshProUGUI>().text = "Atomradius\t\t: " + values.Radius + "pm";
        stats.Find("Mass").GetComponent<TextMeshProUGUI>().text = "Atommasse\t\t: " + values.Mass + "u";
        stats.Find("Oxidation").GetComponent<TextMeshProUGUI>().text = "Oxidationszahl\t\t: " + values.Oxidation;
        stats.Find("Negativity").GetComponent<TextMeshProUGUI>().text = "Elektronegativität\t: " + values.Negativity;
        stats.Find("Meltingpoint").GetComponent<TextMeshProUGUI>().text = "Schmelztemperatur\t: " + values.Meltingpoint + "°C";
        stats.Find("Boilingpoint").GetComponent<TextMeshProUGUI>().text = "Siedetemperatur\t: " + values.Boilingpoint + "°C";
    }

    private void SetupDetailsBox(int elementID)
    {
        Transform parent = gmDetails.transform.Find("DetailsBox").Find("ElementView").Find("Foreground");

        //Create Element Instance
        GameObject gm = Instantiate(gmVisual, parent);
        currentElement = new Element(gm, gmElectron, gmCloud, (Var.Elements)elementID);
        gm.transform.localScale *= 2;
        gm.name = Var.Model.ToString();

        //Deactivate all Models
        for (int child = 0; child < gm.transform.childCount; child++)
        {
            gm.transform.GetChild(child).gameObject.SetActive(false);
        }

        //Activate Current Model
        switch (Var.Model)
        {
            case Var.ModelState.shellModel:
                gm.transform.Find("ShellModel").gameObject.SetActive(true);
                break;
            case Var.ModelState.energyModel:
                gm.transform.Find("EnergyModel").gameObject.SetActive(true);
                break;
            case Var.ModelState.formModel:
                gm.transform.Find("FormModel").gameObject.SetActive(true);
                break;
            case Var.ModelState.cloudModel:
                gm.transform.Find("CloudModel").gameObject.SetActive(true);
                break;
        }
    }

    private IEnumerator ClearDetailsBox()
    {
        //Wait until close-animatin is finished
        yield return new WaitForSeconds(0.5f);

        //Clear current Model
        Destroy(currentElement.GM);
        currentElement = null;
    }
}
