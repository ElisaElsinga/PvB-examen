namespace SK.Libretro.Unity
{
    internal readonly struct LoadDefaultStateBridgeCommand : IBridgeCommand
    {
        public void Execute(Wrapper wrapper) => wrapper.SerializationHandler.LoadDefaultState();
    }
}

