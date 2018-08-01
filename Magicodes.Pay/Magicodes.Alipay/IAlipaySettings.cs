namespace Magicodes.Alipay
{
    public interface IAlipaySettings
    {
        string AlipayPublicKey { get; set; }
        string AlipaySignPublicKey { get; set; }
        string AppId { get; set; }
        string CharSet { get; set; }
        string Gatewayurl { get; set; }
        bool IsKeyFromFile { get; set; }
        string Notify { get; set; }
        string PrivateKey { get; set; }
        string SignType { get; set; }
        string Uid { get; set; }
    }
}