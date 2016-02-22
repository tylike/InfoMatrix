using System;
using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Admiral.ERP.Module.BusinessObjects
{
    //[DefaultClassOptions]
    public class TestPropertyType:BaseObject
    {
        public TestPropertyType(Session s):base(s)
        {
            
        }
        [ImmediatePostData]
        public int Qty { get; set; }

        [ImmediatePostData]
        public decimal Price { get; set; }

        [PersistentAlias("Qty*Price")]
        public decimal Sum
        {
            get
            {
                
                var t = EvaluateAlias("Sum");
                if (t == null)
                    return 0;
                return (decimal) t;
            }
        }

        private Image _Imagge;
        [ValueConverter(typeof(ImageValueConverter))]
        public Image Imagge
        {
            get { return _Imagge; }
            set { SetPropertyValue("Imagge", ref _Imagge, value); }
        }

        private Color _Color;

        public Color Color
        {
            get { return _Color; }
            set { SetPropertyValue("Color", ref _Color, value); }

        }

        private string _String;

        public string String
        {
            get { return _String; }
            set { SetPropertyValue("String", ref _String, value); }

        }

        private bool _Boolean;

        public bool Boolean
        {
            get { return _Boolean; }
            set { SetPropertyValue("Boolean", ref _Boolean, value); }

        }

        private DateTime _DateTime;

        public DateTime DateTime
        {
            get { return _DateTime; }
            set { SetPropertyValue("DateTime", ref _DateTime, value); }

        }


        
    }
}