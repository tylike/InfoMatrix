using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    //图标管理：
    //1.可以上传图标
    //2.上传图标时只上传48x48大小的图标
    //3.上传后程序将保存12x12,16x16,32x32,48x48的几种大小文件，文件名自动命名，可以批量上传N个图标
    //4.在使用图标处，选择的是48x48的，根据名称取得对应的图标名称

    //开发一个列表控件：
    //可以上传图片，上传后，保存bo信息
    [Domain]
    [NavigationItem("系统设置")]
    [XafDisplayName("图标管理")]
    public interface IICons : IName
    {
        //name为显示用的

        /// <summary>
        /// image name为系统使用，用户无需修改
        /// 上传图片后，生成此name
        /// 格式为custom:(png|jpeg|gif|bmp):ImageName
        /// 即 Custom:扩展名:图标名称:相对路径+文件名，无扩展名，无大小
        /// 
        /// </summary>
        [Browsable(false)]
        string ImageName { get; set; }

        [Browsable(false)]
        string FullName { get; set; }
    }
}