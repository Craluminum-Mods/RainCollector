using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace RainCollector
{
    public class BEBehaviorRainCollector : BlockEntityBehavior
    {
        public BEBehaviorRainCollector(BlockEntity blockentity) : base(blockentity) { }

        WeatherSystemBase wsys;
        Vec3d tmpPos = new();
        long listenerId;

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            listenerId = api.Event.RegisterGameTickListener(UpdateEvery1000ms, 1000);
            wsys = api.ModLoader.GetModSystem<WeatherSystemBase>();

            base.Initialize(api, properties);
        }

        public override void OnBlockRemoved()
        {
            Blockentity.UnregisterGameTickListener(listenerId);
            base.OnBlockRemoved();
        }

        public void UpdateEvery1000ms(float dt)
        {
            float desiredLitres = 0.01f;
            var itemStack = new ItemStack(Api.World.GetItem(new AssetLocation("waterportion")));

            var pos = Blockentity.Pos;
            tmpPos.Set(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5);

            if (IsRaining(pos))
            {
                if (Blockentity is BlockEntityGroundStorage begs && !begs.Inventory.Empty)
                {
                    foreach (var slot in begs.Inventory)
                    {
                        if (!slot.Empty && slot.Itemstack.Collectible is BlockLiquidContainerBase blockCnt && blockCnt.IsTopOpened)
                        {
                            blockCnt.TryPutLiquid(slot.Itemstack, itemStack, desiredLitres);
                        }
                    }
                    begs.MarkDirty(true);
                }

                if (Blockentity.Block is BlockLiquidContainerBase blockCnt1)
                {
                    blockCnt1.TryPutLiquid(Blockentity.Pos, itemStack, desiredLitres);
                }
            }
        }

        private bool IsRaining(BlockPos pos)
        {
            return Api.Side == EnumAppSide.Server
                && Api.World.BlockAccessor.GetRainMapHeightAt(pos.X, pos.Z) <= pos.Y
                && wsys.GetPrecipitation(tmpPos) > 0.04;
        }
    }
}