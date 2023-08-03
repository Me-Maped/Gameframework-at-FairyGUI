namespace GameMain
{
    public enum PacketType
    {
        /// <summary>
        /// 未定义。
        /// </summary>
        UNDEFINED = 0,

        /// <summary>
        /// 客户端发往服务器的包。
        /// </summary>
        C2S,

        /// <summary>
        /// 服务器发往客户端的包。
        /// </summary>
        S2C,
    }
}