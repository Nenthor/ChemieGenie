using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private PanelManager panelManager;
    [SerializeField] private GameObject gmInformation;
    [SerializeField] private GameObject gmDetails;
    [SerializeField] private GameObject[] gmModelButtons;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;

    public void Start()
    {
        //Set Default Button
        SetButtonColor(gmModelButtons[(int)Var.Model], selectedColor);
    }

    private void SetButtonColor(GameObject button, Color color)
    {
        button.transform.Find("InnerCircle").GetComponent<Image>().color = color;
    }

    public void OnClick(int button)
    {
        if (Var.State != Var.DisplayState.displayElements) return;

        if (button == 0)
            panelManager.OpenInformation();
        else
            ChangeModel(button - 1);
    }

    private void ChangeModel(int modelState)
    {
        Var.Model = (Var.ModelState)modelState;

        foreach (GameObject gm in gmModelButtons)
        {
            SetButtonColor(gm, defaultColor);
        }
        SetButtonColor(gmModelButtons[modelState], selectedColor);

        foreach (Element element in Var.GmElements)
        {
            if (element == null) continue;
            element.UpdateModel();
        }
    }

    public void OnClickCatch()
    {
        switch (Var.State)
        {
            case Var.DisplayState.displayElements:
                break; //Do nothing
            case Var.DisplayState.displaySettings:
                panelManager.CloseInformation();
                break;
            case Var.DisplayState.displayDetails:
                panelManager.CloseDetails();
                break;
        }
    }
}
