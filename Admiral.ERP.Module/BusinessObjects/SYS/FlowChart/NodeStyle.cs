using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public enum NodeStyle
    {
        //'ellipse'	The shape defines what the node looks like. 
        [XafDisplayName("椭圆")]
        ellipse = 0,
        //There are two types of nodes. One type has the label inside of it 
        //and the other type has the label underneath it. 
        //The types with the label inside of it are:
        //ellipse, circle, database, box, text. 
        [XafDisplayName("圆形")]
        circle = 1,
        [XafDisplayName("数据库")]
        database = 2,
        [XafDisplayName("正方形")]
        box = 3,
        [XafDisplayName("仅文字")]
        text = 4,
        //The ones with the label outside of it 
        //are: image, circularImage, diamond, dot, star, triangle, triangleDown, square and icon.
        [XafDisplayName("图片")]
        image = 5,
        circularImage = 6,
        [XafDisplayName("钻石")]
        diamond = 7,
        [XafDisplayName("圆点")]
        dot = 8,
        [XafDisplayName("五角星")]
        star = 9,
        [XafDisplayName("三角形")]
        triangle = 10,
        [XafDisplayName("倒三角形")]
        triangleDown=11,
        [XafDisplayName("方形")]
        square = 12,
        [XafDisplayName("字库图标")]
        icon = 13

    }
}