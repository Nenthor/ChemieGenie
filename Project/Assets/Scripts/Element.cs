using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Element
{
    private readonly GameObject gm;
    private readonly GameObject gmElectron;
    private readonly GameObject gmCloud;
    private readonly GameObject gmVisual;
    private readonly PanelManager panelManager;
    private readonly int id;

    public GameObject GM
    {
        get { return gm; }
    }

    public Element(GameObject gm, PanelManager panelManager, GameObject gmElectron, GameObject gmCloud, Var.Elements element)
    {
        //Default Version
        this.gm = gm;
        this.panelManager = panelManager;
        this.gmElectron = gmElectron;
        this.gmCloud = gmCloud;
        this.id = (int)element;

        gm.transform.Find("Foreground").Find("Symbol").Find("Text").GetComponent<TextMeshProUGUI>().text = element.ToString(); //Set element text
        gm.transform.Find("Foreground").Find("ClickListener").GetComponent<Button>().onClick.AddListener(OnClick); //Add click listener
        gmVisual = gm.transform.Find("Foreground").Find("Visual").gameObject;

        CreateModel();
        UpdateModel();
    }

    public Element(GameObject gm, GameObject gmElectron, GameObject gmCloud, Var.Elements element)
    {
        //Panel Version
        this.gm = gm;
        this.gmElectron = gmElectron;
        this.gmCloud = gmCloud;
        this.id = (int)element;

        gmVisual = gm;

        CreateModel();
        UpdateModel();
    }

    public void OnClick()
    {
        if (Var.State != Var.DisplayState.displayElements) return;
        panelManager.OpenDetails(id);
    }

    private void CreateModel()
    {
        CreateShellModel();
        CreateEnergyModel();
        CreateFormModel();
        CreateCloudModel();
    }

    private void CreateShellModel()
    {
        //Get gameobjects
        GameObject gmShellModel = gmVisual.transform.Find("ShellModel").gameObject;
        GameObject[] gmShells = {
            gmShellModel.transform.Find("FirstShell").gameObject,
            gmShellModel.transform.Find("SecondShell").gameObject,
            gmShellModel.transform.Find("ThirdShell").gameObject,
            gmShellModel.transform.Find("FourthShell").gameObject
        };

        //Turn off every Shell
        foreach (GameObject gm in gmShells)
        {
            gm.SetActive(false);
        }

        //Get stats
        int electrons = Var.Data.Stats[id].Electrons;
        int maxShellIndex = GetShellIndex(electrons);

        //Activate shells
        int currentShellIndex = 0;
        while (currentShellIndex <= maxShellIndex)
        {
            gmShells[currentShellIndex].SetActive(true);

            int maxElectrons = Mathf.Min(electrons, Var.maxShellSize[currentShellIndex]);
            float radius = gmShells[currentShellIndex].transform.Find("Rim").GetComponent<RectTransform>().rect.width * 0.49f;

            //Loop for the maximum capacity of the currentShell
            for (int i = 1; i <= maxElectrons; i++)
            {
                InstantiateShellElectron(gmShells[currentShellIndex], radius, maxElectrons, i);
                electrons--;
            }

            //To next Shell
            currentShellIndex++;
        }
    }

    private void InstantiateShellElectron(GameObject middlePoint, float radius, int maxElectrons, int index)
    {
        //Math: "Kugelgleichung" -> "Parameterform"
        float x = (radius * Mathf.Cos(2 * Mathf.PI / maxElectrons * index)) + middlePoint.transform.localPosition.x;
        float y = (radius * Mathf.Sin(2 * Mathf.PI / maxElectrons * index)) + middlePoint.transform.localPosition.y;

        GameObject gm = Object.Instantiate(gmElectron, middlePoint.transform);
        gm.transform.localPosition = new Vector3(x, y, 0);
        gm.name = index.ToString();
    }

    private void CreateEnergyModel()
    {
        //Get gameobjects
        GameObject gmEnergyModel = gmVisual.transform.Find("EnergyModel").gameObject;
        GameObject[] gmLines = {
            gmEnergyModel.transform.Find("FirstLine").gameObject,
            gmEnergyModel.transform.Find("SecondLine").gameObject,
            gmEnergyModel.transform.Find("ThirdLine").gameObject,
            gmEnergyModel.transform.Find("FourthLine").gameObject
        };

        //Turn off every line
        foreach (GameObject gm in gmLines)
        {
            gm.SetActive(true);
        }

        //Get stats
        int electrons = Var.Data.Stats[id].Electrons;
        int maxLineIndex = GetShellIndex(electrons);

        //Activate lines
        int currentLineIndex = 0;
        while (currentLineIndex <= maxLineIndex)
        {
            int maxElectrons = Mathf.Min(electrons, Var.maxShellSize[currentLineIndex]);
            float length = gmLines[currentLineIndex].GetComponent<RectTransform>().rect.width;

            //Loop for the maximum capacity of the currentShell
            for (int i = 1; i <= maxElectrons; i++)
            {
                InstantiateEnergyElectron(gmLines[currentLineIndex], length, maxElectrons, i);
                electrons--;
            }

            //To next Shell
            currentLineIndex++;
        }
    }

    private void InstantiateEnergyElectron(GameObject gmLine, float length, int maxElectrons, int index)
    {
        //Math:   [length per electron] * [middle index] - [start position] 
        float x = length / maxElectrons * (index - 0.5f) - length / 2;
        float y = 0;

        GameObject gm = Object.Instantiate(gmElectron, gmLine.transform);
        gm.transform.localPosition = new Vector3(x, y, 0);
        gm.name = index.ToString();
    }

    private void CreateFormModel()
    {
        //Get gameobjects
        GameObject gmFormModel = gmVisual.transform.Find("FormModel").gameObject;
        GameObject[] gmSigns = {
            gmFormModel.transform.Find("RightSign").gameObject,
            gmFormModel.transform.Find("LeftSign").gameObject,
            gmFormModel.transform.Find("LowerSign").gameObject,
            gmFormModel.transform.Find("UpperSign").gameObject
        };

        //Turn off every Sign Child
        foreach (GameObject gm in gmSigns)
        {
            for (int child = 0; child < gm.transform.childCount; child++)
            {
                gm.transform.GetChild(child).gameObject.SetActive(false);
            }
        }

        //Set Element Text
        gmFormModel.transform.Find("ElementText").GetComponent<TextMeshProUGUI>().text = ((Var.Elements)id).ToString();

        //Get stats
        int valenzElectrons = GetValenzElectronsCount(id);
        int oxidation = GetOxidation(id);

        //Activate Line or Dot
        foreach (GameObject gmSign in gmSigns)
        {
            //Stop because there are no more electrons 
            if (valenzElectrons <= 0) break;

            if (oxidation > 0)
            {
                //Tun on free Electron
                gmSign.transform.Find("Dot").gameObject.SetActive(true);
                oxidation--;
                valenzElectrons--;
            }
            else
            {
                //Turn on electron pair
                gmSign.transform.Find("Line").gameObject.SetActive(true);
                valenzElectrons -= 2;
            }
        }
    }

    private int GetOxidation(int id)
    {
        string oxidationText = Var.Data.Stats[id].Oxidation;

        if (oxidationText == "-")
            return 0;
        else
            return Mathf.Abs(int.Parse(oxidationText));
    }

    private void CreateCloudModel()
    {
        //Get gameobjects
        GameObject gmCloudModel = gmVisual.transform.Find("CloudModel").gameObject;
        GameObject[] gmClouds = {
            gmCloudModel.transform.Find("FirstShell").gameObject,
            gmCloudModel.transform.Find("SecondShell").gameObject,
            gmCloudModel.transform.Find("ThirdShell").gameObject,
            gmCloudModel.transform.Find("FourthShell").gameObject
        };

        //Turn off every Sign Child
        foreach (GameObject gm in gmClouds)
        {
            gm.SetActive(false);
        }

        //Get stats
        int valenzElectrons = GetValenzElectronsCount(id);
        int maxShellIndex = GetShellIndex(Var.Data.Stats[id].Electrons);
        Color singleColor = new Color(0.5f, 0.5f, 0.9f);
        Color doubleColor = new Color(0.9f, 0.5f, 0.5f);

        //Activate all full shells (stop before maxShellIndex is reached)
        for (int shellIndex = 0; shellIndex < maxShellIndex; shellIndex++)
        {
            Transform foreground = gmClouds[shellIndex].transform.Find("Foreground");

            if (shellIndex != 0)
            {
                int maxClouds = Mathf.CeilToInt(Var.maxShellSize[shellIndex] / 2);
                float radius = foreground.GetComponent<RectTransform>().rect.width * 0.45f;
                InstantiateClouds(foreground, maxClouds, radius, shellIndex, doubleColor); //Instantiate Clouds first (only in the fourth cloud)
            }
            else foreground.GetChild(0).GetComponent<Image>().color = doubleColor;

            gmClouds[shellIndex].SetActive(true);
        }

        //Activate last shell (with valenzelectrons)
        //Get last shells foreground
        Transform maxForeground = gmClouds[maxShellIndex].transform.Find("Foreground");

        //Make last cloud-border invisible
        gmClouds[maxShellIndex].GetComponent<Image>().color = new Color(0, 0, 0, 0);

        if (maxShellIndex != 0)
        {
            int maxClouds = Mathf.Min(Mathf.CeilToInt(Var.maxShellSize[maxShellIndex] / 2), valenzElectrons);
            float radius = maxForeground.GetComponent<RectTransform>().rect.width * 0.45f;
            valenzElectrons -= maxClouds;
            InstantiateClouds(maxForeground, maxClouds, radius, maxShellIndex, singleColor); //Instantiate Clouds first (only in the fourth cloud)

            //Set every Cloud in the last cloud to doubleColor (red)
            if (valenzElectrons > 0)
            {
                int[] childOrder = GetChildOrder(maxForeground.childCount, valenzElectrons);
                foreach (int child in childOrder)
                {
                    maxForeground.GetChild(child).GetComponent<Image>().color = doubleColor;
                }
            }
        }
        else
        {
            //For H + He (first shell)
            maxForeground.GetChild(0).GetComponent<Image>().color = valenzElectrons == 2 ? doubleColor : singleColor;
        }

        //Turn on Cloud
        gmClouds[maxShellIndex].SetActive(true);
    }

    private void InstantiateClouds(Transform foreground, int maxClouds, float radius, int shellIndex, Color color)
    {
        //Math: Range + start value -> [max value * percentage] + start value
        radius *= 0.4f * (shellIndex / (Var.maxShellSize.Length - 1.0f)) + 0.6f;
        float width = 5f * (shellIndex / (Var.maxShellSize.Length - 1.0f)) + 17.5f;
        float height = 5f * (shellIndex / (Var.maxShellSize.Length - 1.0f)) + 30f;

        for (int index = 0; index < maxClouds; index++)
        {
            //Math: "Kugelgleichung" -> "Parameterform"
            float x = (radius * Mathf.Cos(2 * Mathf.PI / maxClouds * index)) + foreground.transform.localPosition.x;
            float y = (radius * Mathf.Sin(2 * Mathf.PI / maxClouds * index)) + foreground.transform.localPosition.y;
            //Math: "Steigung" * x -> "Steigung" * (360° * percentage)
            float angle = 180 / Mathf.PI * (2 * Mathf.PI * index / maxClouds);

            GameObject gm = Object.Instantiate(gmCloud, foreground.transform);
            gm.transform.localPosition = new Vector3(x, y, 0);
            gm.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            gm.transform.Rotate(new Vector3(0, 0, angle));
            gm.GetComponent<Image>().color = color;
            gm.name = index.ToString();
        }
    }

    private int[] GetChildOrder(int childCount, int valenzElectrons)
    {
        /* Example-Children : 0 1 2 3 4 5 6 7 8 and valenzElectrons = 6 -> stepRange = 9 / 6 = 1.5 (always rounding down)
         * Result list      : 0 1   3 4   6 7
         */
        int length = Mathf.Min(childCount, valenzElectrons);
        int[] array = new int[length];
        float stepRange = 1.0f * childCount / length;

        float count = 0;
        for (int child = 0; child < length; child++)
        {
            array[child] = Mathf.FloorToInt(count);
            count += stepRange;
        }

        return array;
    }

    private int GetValenzElectronsCount(int id)
    {
        int electrons = Var.Data.Stats[id].Electrons;
        int maxShellIndex = GetShellIndex(electrons);

        for (int shellIndex = 0; shellIndex < maxShellIndex; shellIndex++)
        {
            electrons -= Var.maxShellSize[shellIndex];
        }

        return electrons;
    }

    private int GetShellIndex(int electrons)
    {
        int shellIndex = 0;
        int electronRest = electrons;

        foreach (int maxShellSize in Var.maxShellSize)
        {
            //If electrons do not fit in the first/Second/Third shell than go to the next one
            electronRest -= maxShellSize;
            if (electronRest <= 0) break;
            shellIndex++;
        }

        return shellIndex;
    }

    public void UpdateModel()
    {
        //Deactivate every model
        for (int child = 0; child < gmVisual.transform.childCount; child++)
        {
            gmVisual.transform.GetChild(child).gameObject.SetActive(false);
        }

        //Only for default version (not for panel version)
        if (gm != gmVisual)
        {
            gm.transform.Find("Foreground").Find("Symbol").Find("Text").gameObject.SetActive(true);
        }

        //Activate current model
        switch (Var.Model)
        {
            case Var.ModelState.shellModel:
                gmVisual.transform.Find("ShellModel").gameObject.SetActive(true);
                break;
            case Var.ModelState.energyModel:
                gmVisual.transform.Find("EnergyModel").gameObject.SetActive(true);
                break;
            case Var.ModelState.formModel:
                gmVisual.transform.Find("FormModel").gameObject.SetActive(true);

                //Only for default version (not for panel version)
                if (gm != gmVisual)
                {
                    gm.transform.Find("Foreground").Find("Symbol").Find("Text").gameObject.SetActive(false);
                }
                break;
            case Var.ModelState.cloudModel:
                gmVisual.transform.Find("CloudModel").gameObject.SetActive(true);
                break;
        }
    }
}
