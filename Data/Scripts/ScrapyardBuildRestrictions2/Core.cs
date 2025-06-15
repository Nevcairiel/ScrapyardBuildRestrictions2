using System;
using Sandbox.Definitions;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace ZebraMonkeys.Scrapyard
{
    public class BlockMapping
    {
        public BlockMapping() { }

        public Type TypeId { get; set; }
        public string Subtype { get; set; }
        public string ScrapPart { get; set; }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public partial class BuildRestrictionsSession : MySessionComponentBase
    {
        public BuildRestrictionsSession()
        {
            InitBlockMappings();
        }

        public override void LoadData()
        {
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
                if (!(def is MyCubeBlockDefinition))
                    continue;

                var typeId = def.Id.TypeId;
                var subtypeId = def.Id.SubtypeName;

                var mapping = BlockRestrictions.Find(r => typeId.Equals(r.TypeId) && (String.IsNullOrEmpty(r.Subtype) || subtypeId.Contains(r.Subtype)));
                if (mapping != null)
                {
                    // use the specified scrap component
                    if (String.IsNullOrEmpty(mapping.ScrapPart) == false)
                    {
                        MyComponentDefinition componentScrap = null;
                        if (MyDefinitionManager.Static.TryGetComponentDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Component), mapping.ScrapPart), out componentScrap))
                        {
                            BlockToolkit.Block_InsertInitialModComponent(def as MyCubeBlockDefinition, componentScrap, 1, scrapDeconstruct);
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
                    BlockToolkit.Block_InsertInitialModComponent(def as MyCubeBlockDefinition, componentDefaultScrap, 1, scrapDeconstruct);
                }

                nCountBlocks++;
            }

            MyLog.Default.WriteLineAndConsole($"BuildRestrictions: Found {nCountBlocks} blocks, {nCountBlocksAllowed} are free to build, {nCountBlocksRestricted} have specific restrictions");
        }

        protected override void UnloadData()
        {
            // TODO: restore blocks
        }
    }
}
