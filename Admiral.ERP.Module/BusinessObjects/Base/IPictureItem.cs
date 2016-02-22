using System.Drawing;
using System.Security.Cryptography;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.BusinessObjects
{
    public interface IPictureItem
    {
        string ID { get; }
        Image Image { get; }
        string Text { get; }
    }
}