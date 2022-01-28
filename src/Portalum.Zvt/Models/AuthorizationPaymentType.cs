namespace Portalum.Zvt.Models
{
    public enum PaymentType : byte
    {
        ELV = 0x00,
        Geldkarte = 0x10,
        OLV = 0x20,
        PIN = 0x30,
        PTDecission = 0x50
    }
}
