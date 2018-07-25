using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diego.WxHttpSdk;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Diego.WxHttpSdk.Extends;
namespace Diego.WxHttpSdk.Test
{
    public partial class Form1 : Form
    {
        public HttpWeChat httpWeChat;
        private TaskScheduler taskScheduler;
        public Form1()
        {
            httpWeChat = new HttpWeChat();
            httpWeChat.QRCodeChangeEvent += HttpWeChat_QRCodeChangeEvent;
            httpWeChat.QRCodeStatusChangeEvent += HttpWeChat_QRCodeStatusChangeEvent;
            httpWeChat.InitDatabeforeEvent += HttpWeChat_InitDatabeforeEvent;
            httpWeChat.InitDatacompleteEvent += HttpWeChat_InitDatacompleteEvent;
            httpWeChat.SyncCheckEvent += HttpWeChat_SyncCheckEvent;
            httpWeChat.WebwxsyncEvent += HttpWeChat_WebwxsyncEvent;
            CheckForIllegalCrossThreadCalls = false;
            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            InitializeComponent();
        }

        private void HttpWeChat_WebwxsyncEvent(object sender, SdkEventArgs.WebwxsyncEventArgs e)
        {
            Task.Run(() =>
            {
                foreach (var item in e.WeChatMessage.AddMsgList)
                {
                    var formName = httpWeChat.WeChatData.AllMember.Where(c => c.UserName == item.FromUserName).FirstOrDefault()?.NickName;
                    var toName = httpWeChat.WeChatData.AllMember.Where(c => c.UserName == item.ToUserName).FirstOrDefault()?.NickName;
                    WriteLog($"收到新的消息,是【{formName}】发送给【{toName}】,消息内容是:{item.Content}");
                }
            });
        }

        private void HttpWeChat_SyncCheckEvent(object sender, SdkEventArgs.SyncCheckEventArgs e)
        {
            Task.Run(() =>
            {
                WriteLog($"当前{nameof(e.Retcode)}值是:{e.Retcode},{nameof(e.Selector)}的值是:{e.Selector}");
            });
        }

        private void HttpWeChat_InitDatacompleteEvent(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                WriteLog("数据初始化完毕,开始显示当前登录的用户信息");
                lbl_current_NikeName.Text = httpWeChat.WeChatData.CurrentLoginMemberInfo.NickName;
                lbl_current_Sex.Text = httpWeChat.WeChatData.CurrentLoginMemberInfo.GetSex();
                lbl_current_QM.Text = httpWeChat.WeChatData.CurrentLoginMemberInfo.Signature;
                var stream = httpWeChat.HttpClient.GetStreamAsync("https://wx.qq.com" + httpWeChat.WeChatData.CurrentLoginMemberInfo.HeadImgUrl).Result;
                #region 这是一坨废代码
                var bytes = new List<byte>();
                var tempBytes = new byte[1024 * 20];
                var readLen = 0;
                while ((readLen = stream.Read(tempBytes, 0, tempBytes.Length)) > 0)
                {
                    for (int i = 0; i < readLen; i++)
                    {
                        bytes.Add(tempBytes[i]);
                    }
                }
                System.IO.MemoryStream memoryStream = new MemoryStream(tempBytes.ToArray());
                #endregion
                try
                {
                    this.pictureBox2.Image = Image.FromStream(memoryStream).GetThumbnailImage(72, 61, null, default(IntPtr));
                }
                catch (Exception)
                {
                    WriteLog("加载用户头像失败!");
                }
                //初始化最近联系人
                foreach (var item in httpWeChat.WeChatData.RecentContact)
                {
                    zuijinlianxiren.Items.Add(item.NickName ?? "");

                }
                //初始化好友列表
                foreach (var item in httpWeChat.WeChatData.Friends)
                {
                    haoyouliebiao.Items.Add(item.NickName ?? "");
                }
                foreach (var item in httpWeChat.WeChatData.PublicAccount)
                {
                    gongzhonghao.Items.Add(item.NickName ?? "");
                }
            });

        }

        private void HttpWeChat_InitDatabeforeEvent(object sender, EventArgs e)
        {
            WriteLog("开始初始化数据......");
        }

        private void HttpWeChat_QRCodeStatusChangeEvent(object sender, SdkEventArgs.QrCodeStatusChangEventArgs e)
        {
            WriteLog("当前二维码状态是:" + e.EQrCodeStatus.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void HttpWeChat_QRCodeChangeEvent(object sender, SdkEventArgs.QrCodeChangEventArgs e)
        {
            MemoryStream memoryStream = new MemoryStream(e.QrCodeBytes);
            Image img = Image.FromStream(memoryStream);
            this.pictureBox1.Image = img.GetThumbnailImage(270, 270, null, default(IntPtr)); ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                httpWeChat.Login();
            });
        }


        private void WriteLog(string msg)
        {
            richTextBox1.AppendText(msg + Environment.NewLine);     // 追加文本，并且使得光标定位到插入地方。
            richTextBox1.ScrollToCaret();
            this.richTextBox1.Focus();//获取焦点
            this.richTextBox1.Select(this.richTextBox1.TextLength, 0);//光标定位到文本最后
            this.richTextBox1.ScrollToCaret();//滚动到光标处
        }

        private void 发送消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = httpWeChat.SendMessage(new Model.SendMessage
            {
                ClientMsgId = DateTime.Now.ToUnix().ToString(),
                Content = "测试一下",
                ToUserName = httpWeChat.WeChatData.AllMember.FirstOrDefault(c => c.NickName == haoyouliebiao.Text)?.UserName,
                FromUserName = httpWeChat.WeChatData.CurrentLoginMemberInfo.UserName
            });
            if (result.Status)
            {
                MessageBox.Show("消息发送成功!消息Id是:" + result.MsgId);
            }
            else {
                MessageBox.Show("消息发送失败,错误消息是:"+result.ErrorMsg);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            httpWeChat.SignOut();
        }

        private void 修改备注ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            httpWeChat.UpdateRemarke(new Model.UpdateRemarke {
                UserName = httpWeChat.WeChatData.AllMember.FirstOrDefault(c => c.NickName == haoyouliebiao.Text)?.UserName,
                RemarkName = httpWeChat.WeChatData.AllMember.FirstOrDefault(c => c.NickName == haoyouliebiao.Text).RemarkName + "Sdk修改测试"
            });
            MessageBox.Show("修改成功!");
        }
    }
}
