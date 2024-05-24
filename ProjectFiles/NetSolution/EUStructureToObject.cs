#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.RAEtherNetIP;
using FTOptix.NativeUI;
using FTOptix.OPCUAServer;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using System.Linq;
using System.Collections.Generic;
#endregion

public class EUStructureToObject : BaseNetLogic
{
    public override void Start()
    {
        // populate ComboBox for EU
        EUDictionaryToObjectModel();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    private void EUDictionaryToObjectModel(){
        Panel euComboBoxOwner = Owner as Panel;
        NodeId euDictionaryId = euComboBoxOwner.GetVariable("InpEUDictionary").Value;       

        if (euDictionaryId == null)
        {
            Log.Warning("EUComboBoxx", "InpEUDictionary is Null!");
            return;
        }

        UAVariable euDictionary = InformationModel.Get<UANode>(euDictionaryId) as UAVariable;
        UAValue dicValue = euDictionary.Value;
        var dicObjects = dicValue.Value as Struct[];

        // start making the data model for the ComboBox
        var euListObject = InformationModel.MakeObject("EUListModel");
        // Add default item 
        var euListItem = InformationModel.MakeVariable("(None)", OpcUa.DataTypes.Int32);
        euListObject.Add(euListItem); // Item 0 for "None" selection 

        if (dicObjects.Length == 0)
        {
            Log.Warning("EUComboBoxx", "InpEUDictionary has no Engineering Units!");
            return;
        }
        
        List<KeyValuePair<string,Int32>> keyValuePairs = new List<KeyValuePair<string, int>>();

        // convert EngineeringUnitDictionaryItem to Object variable
        foreach (EngineeringUnitDictionaryItem item in dicObjects)
        {
            keyValuePairs.Add(new KeyValuePair<string, int>(item.DisplayName.Text,item.UnitId));
            // Log.Info(item.DisplayName.Text.ToString() + " | " + item.UnitId.ToString());
        }
        
        // Sorting
         var sortedKeyValuePairs = keyValuePairs.OrderBy(kvp => kvp.Key).ToList();
        foreach (var kvp in sortedKeyValuePairs)
        {
            euListItem = InformationModel.MakeVariable(kvp.Key.ToString(), OpcUa.DataTypes.Int32);
            euListItem.Value = kvp.Value;
            euListObject.Add(euListItem);
        }
        // get handle to the target combo box
        ComboBox euComboBox = euComboBoxOwner.Children.Get<ComboBox>("ComboBox");
        // add data model
        euComboBox.Add(euListObject);        
        // Attach model to ComboBox
        euComboBox.GetVariable("Model").Value = euListObject.NodeId;
    }
}
