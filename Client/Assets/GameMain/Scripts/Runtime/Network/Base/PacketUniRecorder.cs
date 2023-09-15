namespace GameMain
{
    public static class PacketUniRecorder
    {
        private static int _lastUniId = 1;
        public static int LastUniId => _lastUniId++;
    }
}