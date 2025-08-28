using System.Collections.Generic;
using ObjectBuilders.SafeZone;
using Sandbox.Common.ObjectBuilders;
using SpaceEngineers.ObjectBuilders.ObjectBuilders;
using VRage.Game;

namespace ZebraMonkeys.Scrapyard
{
    public partial class BuildRestrictionsSession
    {
        private List<BlockMapping> BlockRestrictions;

        private string DefaultScrapComponent = "ScrapConstructionFrame";
        private int LargeGridComponentMultiplier = 5;

        void InitBlockMappings()
        {
            BlockRestrictions = new List<BlockMapping>
            {
                // Specify if a block can be build, or which scrap component gets inserted as the placing requirement.
                // Blocks can be specified only by their major type (eg. MyObjectBuilder_CubeBlock), or with type+subtype.
                // Subtypes use a partial match, so that groups of blocks can be targeted - eg. MyObjectBuilder_Thrust, Subtype "Atmospheric" can match all atmo thrusters
                //
                // Mappings are evaluated in order, which means specific rules should be above generic rules, if so desired
                //
                // Parameters:
                // TypeId: Main type of the block, eg. typeof(MyObjectBuilder_CubeBlock)
                // Subtype: (partial) subtype of the block, optional
                // ScrapPart: Subtype of the scrap component, if any
                // NumComponents: Number of components required to build the block (1 if unspecified)
                // NumComponentsLargeGrid: Number of components for large grid (num * LargeGridComponentMultiplier if unspecified)
                //
                // - A block that is specified without a scrap component will be buildable freely
                // - A block with a scrap component will require this component to be placed
                // - A block that is not specified at all will get the default scrap component

                ////////////////////////////////////////////////////////////////////////////////////////////
                /// Allowed blocks

                // basic CubeBlocks (block without function, armor, decorative, etc)
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_CubeBlock) },

                // lighting
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_InteriorLight) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ReflectorLight) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_EmissiveBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Searchlight) },

                // ladders etc
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Ladder2) },

                // conveyors
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Conveyor) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ConveyorConnector) },

                // connector
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ShipConnector) },

                // mechanical blocks
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_PistonBase) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ExtendedPistonBase) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_PistonTop) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MotorRotor) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MotorStator) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MotorAdvancedRotor) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MotorAdvancedStator) },

                // merge block
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MergeBlock) },

                // wheels & suspensions
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MotorSuspension) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Wheel) },

                // Landing Gear
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_LandingGear) },

                // gyros
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Gyro) },

                // cockpits
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Cockpit) },

                // remote control
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_RemoteControl) },

                // doors
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Door) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_AirtightHangarDoor) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_AirtightSlideDoor) },

                // buttons
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ButtonPanel) },

                // terminal blocks (simple blocks with terminal access)
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_TerminalBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_LCDPanelsBlock) },

                // LCDs
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_TextPanel) },

                // Weapons
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_LargeMissileTurret) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_LargeGatlingTurret) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SmallMissileLauncher) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SmallMissileLauncherReload) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SmallGatlingGun) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_InteriorTurret) },

                // Weapon misc
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Warhead) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Decoy) },

                // Deco
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Kitchen) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Planter) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Jukebox) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ExhaustBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_HeatVentBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Passage) },

                // Misc blocks
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SensorBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_CameraBlock) },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SoundBlock) },

                ////////////////////////////////////////////////////////////////////////////////////////////
                /// Blocked
                
                // PB, Timer et al
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MyProgrammableBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_TimerBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_EventControllerBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_TurretControlBlock), ScrapPart = "ScrapConstructionFrame" },

                // Projector
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Projector), ScrapPart = "ScrapConstructionFrame" },

                // AI blocks
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_BasicMissionBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_FlightMovementBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_PathRecorderBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_DefensiveCombatBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_OffensiveCombatBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_EmotionControllerBlock), ScrapPart = "ScrapConstructionFrame" },

                // Beacon & Antenna
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Beacon), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_RadioAntenna), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_LaserAntenna), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_BroadcastController), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_TransponderBlock), ScrapPart = "ScrapConstructionFrame" },

                // Cargo
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_CargoContainer), ScrapPart = "ScrapConstructionFrame" },

                // Conveyor additions
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ConveyorSorter), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Collector), ScrapPart = "ScrapConstructionFrame" },

                // parachute
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Parachute), ScrapPart = "ScrapConstructionFrame" },

                // Ship Tools (Drill, etc)
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Drill), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ShipGrinder), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ShipWelder), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_OreDetector), ScrapPart = "ScrapConstructionFrame" },

                // Vents
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_AirVent), ScrapPart = "ScrapConstructionFrame" },

                // Power
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SolarPanel), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_WindTurbine), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Reactor), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_HydrogenEngine), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_BatteryBlock), ScrapPart = "ScrapConstructionFrame" },

                // H2/O2
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_OxygenTank), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_OxygenGenerator), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_OxygenFarm), ScrapPart = "ScrapConstructionFrame" },

                // Cryo & Medical
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_CryoChamber), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_MedicalRoom), ScrapPart = "ScrapConstructionFrame" },

                // GravGen
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_GravityGenerator), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_GravityGeneratorSphere), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_VirtualMass), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SpaceBall), ScrapPart = "ScrapConstructionFrame" },

                // Production blocks
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Assembler), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Refinery), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SurvivalKit), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_UpgradeModule), ScrapPart = "ScrapConstructionFrame" },

                // Atmos Thruster
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Thrust), Subtype = "Atmospheric", ScrapPart = "ScrapConstructionFrame" },

                // Hydrogen Thruster
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Thrust), Subtype = "Hydrogen", ScrapPart = "ScrapConstructionFrame" },

                // Remaining Thruster (Ion)
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_Thrust), ScrapPart = "ScrapConstructionFrame" },

                // Misc
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_VendingMachine), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_StoreBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_ContractBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_SafeZoneBlock), ScrapPart = "ScrapConstructionFrame" },
                new BlockMapping{ TypeId = typeof(MyObjectBuilder_JumpDrive), ScrapPart = "ScrapConstructionFrame" },
            };
        }
    }
}
