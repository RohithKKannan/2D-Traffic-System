using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button addNodeButton;
    [SerializeField] private Button removeNodeButton;
    [SerializeField] private Button clearNodesButton;
    [SerializeField] private Button drawModeButton;
    [SerializeField] private Button removeModeButton;
    [SerializeField] private Button getWeightButton;
    [SerializeField] private Button shortestPathButton;

    [Header("Text")]
    [SerializeField] private TMP_Text modeInfoLabel;
    [SerializeField] private TMP_Text helperInfoLabel;

    public Button AddNodeButton => addNodeButton;
    public Button RemoveNodeButton => removeNodeButton;
    public Button ClearNodesButton => clearNodesButton;
    public Button DrawModeButton => drawModeButton;
    public Button RemoveModeButton => removeModeButton;
    public Button GetWeightButton => getWeightButton;
    public Button ShortestPathButton => shortestPathButton;

    public TMP_Text ModeInfoLabel => modeInfoLabel;
    public TMP_Text HelperInfoLabel => helperInfoLabel;
}
