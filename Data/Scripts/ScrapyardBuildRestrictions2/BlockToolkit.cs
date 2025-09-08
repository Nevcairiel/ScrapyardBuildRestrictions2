using Sandbox.Definitions;
using Sandbox.Game;

namespace ZebraMonkeys.Scrapyard
{
    // Based on the Toolkit provided by Arstraea
    internal class BlockToolkit
    {
        public static void Block_InsertInitialModComponent(MyCubeBlockDefinition targetBlock, MyComponentDefinition modComponent, int modComponent_Count = 1, MyPhysicalItemDefinition scrapDef = null)
        {
            // if no scrap is specified, then it deconstructs to itself
            if (scrapDef == null)
                scrapDef = modComponent;

            MyCubeBlockDefinition.Component[] new_Components = new MyCubeBlockDefinition.Component[targetBlock.Components.Length + 1];
            new_Components[0] = new MyCubeBlockDefinition.Component()
            {
                Definition = modComponent,
                Count = modComponent_Count,
                DeconstructItem = scrapDef
            };

            int new_Component_Index = 1;
            for (int index = 0; index < targetBlock.Components.Length; index++)
            {
                new_Components[new_Component_Index] = targetBlock.Components[index];
                new_Component_Index++;
            }
            targetBlock.Components = new_Components;
            targetBlock.CriticalGroup++;

            Block_SyncComponentChanges(targetBlock);
        }

        // Original Code : Sandbox.Game \ Sandbox \ Definitions \ MyCubeBlockDefinition.cs
        public static void Block_SyncComponentChanges(MyCubeBlockDefinition targetBlock)
        {
            if (targetBlock.Components != null && targetBlock.Components.Length != 0)
            {
                float block_Mass = 0.0f;
                float block_CriticalIntegrity = 0.0f;
                float block_OwnershipIntegrity = 0.0f;
                float block_MaxIntegrity = 0.0f;

                float original_MaxIntegrity = targetBlock.MaxIntegrity;
                float original_MaxIntegrityRatio = targetBlock.MaxIntegrityRatio;

                for (int index = 0; index < targetBlock.Components.Length; index++)
                {
                    MyCubeBlockDefinition.Component component = targetBlock.Components[index];

                    if (component.Definition.Id.SubtypeName == "Computer" && (double)block_OwnershipIntegrity == 0.0)
                        block_OwnershipIntegrity = block_MaxIntegrity + (float)component.Definition.MaxIntegrity;

                    block_MaxIntegrity += (float)(component.Count * component.Definition.MaxIntegrity);

                    if (index == targetBlock.CriticalGroup)
                        block_CriticalIntegrity = block_MaxIntegrity - (float)component.Definition.MaxIntegrity;

                    block_Mass += (float)component.Count * component.Definition.Mass;
                }

                targetBlock.MaxIntegrity = block_MaxIntegrity;
                targetBlock.IntegrityPointsPerSec = block_MaxIntegrity / (original_MaxIntegrity / targetBlock.IntegrityPointsPerSec);

                if (original_MaxIntegrityRatio != 1.0f)
                {
                    targetBlock.MaxIntegrityRatio = original_MaxIntegrityRatio * original_MaxIntegrity / targetBlock.MaxIntegrity;
                    targetBlock.DeformationRatio = targetBlock.DeformationRatio * original_MaxIntegrityRatio / targetBlock.MaxIntegrity;
                }

                targetBlock.Mass = block_Mass;

                targetBlock.CriticalIntegrityRatio = block_CriticalIntegrity / targetBlock.MaxIntegrity;
                targetBlock.OwnershipIntegrityRatio = block_OwnershipIntegrity / targetBlock.MaxIntegrity;
            }
        }
    }
}
