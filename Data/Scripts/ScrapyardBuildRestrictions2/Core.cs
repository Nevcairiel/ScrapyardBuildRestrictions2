using System;
using System.IO;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace ZebraMonkeys.Scrapyard
{
    public class BlockMapping
    {
        public BlockMapping() { }

        public Type TypeId { get; set; }
        public string TypeString { get; set; }
        public string Subtype { get; set; }
        public string ScrapPart { get; set; }
        public int NumComponents { get; set; }
        public int NumComponentsLargeGrid { get; set; }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public partial class BuildRestrictionsSession : MySessionComponentBase
    {
        public static string SettingsFileName = "BuildRestrictions.xml"; // the file that gets saved to world storage under your mod's folder
        const string VariableId = "Scrapyard.BuildRestrictions.Config"; // IMPORTANT: must be unique as it gets written in a shared space (sandbox.sbc)

        public class BuildRestrictionSettings
        {
            public string[] Exemptions;

            public BuildRestrictionSettings()
            {
                Exemptions = new string[] { "CubeBlock/BlockSubtype" };
            }
        }

        private BuildRestrictionSettings Settings { get; set; }

        public BuildRestrictionsSession()
        {
            InitBlockMappings();
        }

        public override void LoadData()
        {
            if (MyAPIGateway.Session.IsServer)
            {
                LoadConfigServer();
                WriteWorldSettings();
            }
            else
            {
                LoadConfigClient();

                if (Settings == null)
                    Settings = new BuildRestrictionSettings();
            }

            // process config
            foreach (var def in Settings.Exemptions)
            {
                MyDefinitionId outDef;
                if (MyDefinitionId.TryParse(def, out outDef) == true)
                {
                    MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Parsed config type: {outDef.TypeId} / {outDef.SubtypeName}");

                    // insert rule at top so it overrides any built-in rules
                    BlockRestrictions.Insert(0, new BlockMapping { TypeId = outDef.TypeId, Subtype = outDef.SubtypeName });
                }
            }

            int nCountBlocks = 0;
            int nCountBlocksRestricted = 0, nCountBlocksAllowed = 0;

            MyComponentDefinition componentDefaultScrap = null;
            if (MyDefinitionManager.Static.TryGetComponentDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Component), DefaultScrapComponent), out componentDefaultScrap) == false)
            {
                MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Unable to load default scrap component: {DefaultScrapComponent}");
                return;
            }

            MyPhysicalItemDefinition scrapDeconstruct = null;
            if (MyDefinitionManager.Static.TryGetPhysicalItemDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Scrap"), out scrapDeconstruct) == false)
            {
                MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Scrap could not be loaded?!?");
                return;
            }

            var defs = MyDefinitionManager.Static.GetAllDefinitions();
            foreach (var def in defs)
            {
                var cubeDef = def as MyCubeBlockDefinition;
                if (cubeDef == null)
                    continue;

                var typeId = def.Id.TypeId;
                var subtypeId = def.Id.SubtypeName;
                var largeGrid = cubeDef.CubeSize == MyCubeSize.Large;

                var mapping = BlockRestrictions.Find(r => (typeId.Equals(r.TypeId) || typeId.ToString().Equals(r.TypeString)) && (String.IsNullOrEmpty(r.Subtype) || (subtypeId.IndexOf(r.Subtype, StringComparison.OrdinalIgnoreCase) >= 0)));
                if (mapping != null)
                {
                    // use the specified scrap component
                    if (String.IsNullOrEmpty(mapping.ScrapPart) == false)
                    {
                        MyComponentDefinition componentScrap = null;
                        if (MyDefinitionManager.Static.TryGetComponentDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Component), mapping.ScrapPart), out componentScrap))
                        {
                            var numComponents = mapping.NumComponents > 0 ? mapping.NumComponents : 1;
                            if (largeGrid)
                            {
                                if (mapping.NumComponentsLargeGrid > 0)
                                    numComponents = mapping.NumComponentsLargeGrid;
                                else
                                    numComponents *= LargeGridComponentMultiplier;
                            }

                            BlockToolkit.Block_InsertInitialModComponent(cubeDef, componentScrap, numComponents, scrapDeconstruct);
                        }
                        else
                        {
                            MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Unable to load scrap component: {mapping.ScrapPart}");
                        }

                        nCountBlocksRestricted++;
                    }
                    else
                    {
                        // else free to build
                        nCountBlocksAllowed++;
                    }
                }
                else
                {
                    MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Unspecified block: {typeId.ToString()} / {subtypeId}");

                    // block is not specified, use default scrap component
                    var numComponents = 1 * (largeGrid ? LargeGridComponentMultiplier : 1);
                    BlockToolkit.Block_InsertInitialModComponent(cubeDef, componentDefaultScrap, numComponents, scrapDeconstruct);
                }

                nCountBlocks++;
            }

            MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Found {nCountBlocks} blocks, {nCountBlocksAllowed} are free to build, {nCountBlocksRestricted} have specific restrictions");
        }

        protected override void UnloadData()
        {
            // TODO: restore blocks
        }

        private void LoadConfigServer()
        {
            // load file if exists then save it regardless so that it can be sanitized and updated
            if (MyAPIGateway.Utilities.FileExistsInWorldStorage(SettingsFileName, typeof(BuildRestrictionSettings)))
            {
                try
                {
                    using (TextReader file = MyAPIGateway.Utilities.ReadFileInWorldStorage(SettingsFileName, typeof(BuildRestrictionSettings)))
                    {
                        string text = file.ReadToEnd();

                        Settings = MyAPIGateway.Utilities.SerializeFromXML<BuildRestrictionSettings>(text);
                        if (Settings != null)
                            return;
                    }
                }
                catch (Exception exc)
                {
                    MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Could not load settings file");
                    MyLog.Default.WriteLine(exc);
                }
            }

            // if no config was loaded, save a new one
            Settings = new BuildRestrictionSettings();

            try
            {
                using (TextWriter writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(SettingsFileName, typeof(BuildRestrictionSettings)))
                {

                    writer.Write(MyAPIGateway.Utilities.SerializeToXML<BuildRestrictionSettings>(Settings));

                }
            }
            catch (Exception exc)
            {
                MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Could not write settings file");
                MyLog.Default.WriteLine(exc);
            }
        }

        private void WriteWorldSettings()
        {
            string saveText = MyAPIGateway.Utilities.SerializeToXML<BuildRestrictionSettings>(Settings);
            MyAPIGateway.Utilities.SetVariable<string>(VariableId, saveText);
        }

        private void LoadConfigClient()
        {
            try
            {
                string text;
                if (!MyAPIGateway.Utilities.GetVariable<string>(VariableId, out text))
                {
                    MyLog.Default.WriteLineAndConsole($"BuildRestrictions: no session config");
                    return;
                }

                Settings = MyAPIGateway.Utilities.SerializeFromXML<BuildRestrictionSettings>(text);
                if (Settings == null)
                {
                    MyLog.Default.WriteLineAndConsole($"BuildRestrictions: unable to parse xml");
                    return;
                }
            }
            catch (Exception exc)
            {
                MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Could not write settings file");
                MyLog.Default.WriteLine(exc);
            }
        }
    }
}
