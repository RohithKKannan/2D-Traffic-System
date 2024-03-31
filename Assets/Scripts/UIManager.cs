using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button addNodeButton;
    [SerializeField] private Button removeNodeButton;
    [SerializeField] private Button clearNodesButton;
    [SerializeField] private Button drawModeButton;
    [SerializeField] private Button removeModeButton;
    [SerializeField] private Button getWeightButton;
    [SerializeField] private Button shortestPathButton;

    public Button AddNodeButton => addNodeButton;
    public Button RemoveNodeButton => removeNodeButton;
    public Button ClearNodesButton => clearNodesButton;
    public Button DrawModeButton => drawModeButton;
    public Button RemoveModeButton => removeModeButton;
    public Button GetWeightButton => getWeightButton;
    public Button ShortestPathButton => shortestPathButton;
}
