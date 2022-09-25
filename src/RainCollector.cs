using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo("Rain Collector",
Authors = new[] { "Craluminum2413" })]

namespace RainCollector
{
    public class RainCollector : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockEntityBehaviorClass("RainCollector", typeof(BEBehaviorRainCollector));
            api.World.Logger.Event("started 'Rain Collector' mod");
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            foreach (var block in api.World.Blocks)
            {
                if (block is BlockLiquidContainerBase || block is BlockGroundStorage)
                {
                    var behavior = new BlockEntityBehaviorType()
                    {
                        Name = "RainCollector",
                        properties = null
                    };

                    block.BlockEntityBehaviors = block.BlockEntityBehaviors.Append(behavior);
                }
            }
        }
    }
}