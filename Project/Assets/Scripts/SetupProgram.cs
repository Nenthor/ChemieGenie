using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class SetupProgram : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject gmTitle;
    [SerializeField] private GameObject gmInformation;
    [SerializeField] private GameObject gmDetails;
    [SerializeField] private GameObject gmClickCatcher;
    [SerializeField] private GameObject gmElectron;
    [SerializeField] private GameObject gmCloud;
    [SerializeField] private PanelManager panelManager;
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private string titleText;

    public void Awake()
    {
        //Set Default Settings
        Var.State = Var.DisplayState.displayElements;
        Var.Model = Var.ModelState.shellModel;
        Var.GmElements = new Element[Var.lenghtY * Var.lenghtX];

        //Set Framerate to 30fps
        Application.targetFrameRate = 30;
    }

    public void Start()
    {
        //Takes about 0,5s to execute this
        //Set BackButton to close app when pressed
        Input.backButtonLeavesApp = true;

        //Get Values (radius, mass, meltingpoint, ...)
        Var.Data = JsonConvert.DeserializeObject<Data>(jsonFile.text);

        SetElements();

        //Add title gameobject
        gmTitle.GetComponent<TextMeshProUGUI>().text = titleText;

        //Turn off panels/gameobjects
        gmInformation.SetActive(false);
        gmDetails.SetActive(false);
        gmClickCatcher.SetActive(false);

        //Remove "LoadElements"-instance
        Destroy(this);
    }

    private void SetElements()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject gm = parent.GetChild(i).gameObject;

            Var.Elements element = (Var.Elements)System.Enum.Parse(typeof(Var.Elements), gm.name); //Get element name (He, Cl, Ar)
            Var.GmElements[(int)element] = new Element(gm, panelManager, gmElectron, gmCloud, element); //Set new Element
        }
    }
}